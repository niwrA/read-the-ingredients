using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IngredientsParserShared
{
    public class IngredientsParser
    {
        public class ParsedIngredient
        {
            public string Text { get; set; }
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public List<ParsedIngredient> Ingredients { get; set; }
            public List<ParsedAnnotation> Annotations { get; set; }
            public string PercentageText { get; set; }
            public List<string> Allergens { get; set; }
            public int WikiDataId { get; set; }
        }

        public class ParsedAnnotation
        {
            public string Identifier { get; set; }
            public string Text { get; set; }
        }

        public class ParsedIngredients
        {
            public ParsedIngredients()
            {
                Ingredients = new List<ParsedIngredient>();
                Annotations = new List<ParsedAnnotation>();
            }
            public List<ParsedIngredient> Ingredients { get; set; }
            public List<ParsedAnnotation> Annotations { get; set; }
            public IEnumerable<string> EUNumbers { get; set; }
        }

        public ParsedIngredients Parse(string ingredientsInput, List<ParsedAnnotation> parsedAnnotations)
        {

            if (string.IsNullOrEmpty(ingredientsInput))
            {
                return new ParsedIngredients();
            }

            ingredientsInput = Normalise(ingredientsInput);

            var sb = new StringBuilder();
            var ingredientParts = new List<string>();
            int accoladeDepth = 0;
            var inIngredients = true;
            var ingredientFound = false;
            var isLastIngredient = false;
            var isLastChar = false;
            var commaIndex = 0;
            char? prevChar = null;
            int countAll = 0;
            int countCurrent = 0;

            bool inParenthesis = false;
            bool inSquareBrackets = false;

            foreach (var ch in ingredientsInput)
            {
                ProcessIngredientsText(ingredientsInput, ref sb, ingredientParts, ref accoladeDepth, ref inIngredients, ref ingredientFound, ref isLastIngredient, ref isLastChar, ref commaIndex, ref prevChar, ref countAll, ref countCurrent, ref inParenthesis, ref inSquareBrackets, ch);
            }

            var annotations = sb.ToString().Trim();
            var annotationsList = annotations.Split(Environment.NewLine.ToCharArray());
            var warnings = new List<string>();

            parsedAnnotations.AddRange(ParseAnnotations(annotationsList));

            var parsedIngredients = ParseIngredients(ingredientParts, parsedAnnotations);

            var result = new ParsedIngredients { Ingredients = parsedIngredients, Annotations = parsedAnnotations };
            return result;
        }

        private void ProcessIngredientsText(string ingredientsInput, ref StringBuilder sb, List<string> ingredientParts, ref int accoladeDepth, ref bool inIngredients, ref bool ingredientFound, ref bool isLastIngredient, ref bool isLastChar, ref int commaIndex, ref char? prevChar, ref int countAll, ref int countCurrent, ref bool inParenthesis, ref bool inSquareBrackets, char ch)
        {
            countAll++;
            countCurrent++;
            sb.Append(ch);

            ProcessSpecialCharacters(ref accoladeDepth, inIngredients, ref ingredientFound, ref isLastIngredient, ref commaIndex, prevChar, countCurrent, ref inParenthesis, ref inSquareBrackets, ch);

            DetectIngredientTransition(ingredientsInput, ref ingredientFound, ref isLastIngredient, ref isLastChar, countAll);

            HandleIngredientFound(ref sb, ingredientParts, ref inIngredients, ref ingredientFound, isLastIngredient, isLastChar, ref commaIndex, ref countCurrent);

            prevChar = ch;
        }

        private static void DetectIngredientTransition(string ingredientsInput, ref bool ingredientFound, ref bool isLastIngredient, ref bool isLastChar, int countAll)
        {
            if (countAll == ingredientsInput.Length) { ingredientFound = true; isLastIngredient = true; isLastChar = true; }
        }

        private static void ProcessSpecialCharacters(ref int accoladeDepth, bool inIngredients, ref bool ingredientFound, ref bool isLastIngredient, ref int commaIndex, char? prevChar, int countCurrent, ref bool inParenthesis, ref bool inSquareBrackets, char ch)
        {
            switch (ch)
            {
                case '(': accoladeDepth++; inParenthesis = true; inSquareBrackets = false; break;
                case ')':
                    accoladeDepth--;
                    if (inSquareBrackets)
                    {
                        throw new Exception("] expected.");
                    };
                    break;
                case '[': accoladeDepth++; inParenthesis = false; inSquareBrackets = true; break;
                case ']':
                    accoladeDepth--;
                    if (inParenthesis)
                    {
                        throw new Exception(") expected.");
                    }
                    break;
                case ',': commaIndex = countCurrent; break;
                case '.': if (!char.IsDigit(prevChar.Value) && inIngredients && accoladeDepth == 0) { ingredientFound = true; isLastIngredient = true; }; break;
                case ' ': if (prevChar.HasValue && prevChar.Value == ',' && accoladeDepth == 0) { ingredientFound = true; }; break;
            }
        }

        private void HandleIngredientFound(ref StringBuilder sb, List<string> ingredientParts, ref bool inIngredients, ref bool ingredientFound, bool isLastIngredient, bool isLastChar, ref int commaIndex, ref int countCurrent)
        {
            if (ingredientFound)
            {
                if (inIngredients)
                {
                    AddIngredientListItem(ref sb, ingredientParts, isLastIngredient, ref commaIndex, ref countCurrent, isLastChar);
                    ingredientFound = false;
                }
                if (isLastIngredient)
                {
                    inIngredients = false;
                }
            }
        }

        private string Normalise(string ingredientsInput)
        {
            ingredientsInput = NormalisePercentages(ingredientsInput);
            return ingredientsInput;
        }

        private void AddIngredientListItem(ref StringBuilder sb, List<string> ingredientParts, bool isLastIngredient, ref int commaIndex, ref int count, bool isLastChar)
        {
            if (!isLastChar)
            {
                if (!isLastIngredient)
                {
                    sb = sb.Remove(commaIndex - 1, count - (commaIndex - 1)); // remove trailing ,
                }
                else
                {
                    sb = sb.Remove(count - 1, 1); // remove trailing .
                }
            }
            var ingredientPart = sb.ToString().Trim();
            if(ingredientPart.LastIndexOf('.') == ingredientPart.Length-1)
            {
                sb = sb.Remove(count - 1, 1); // remove trailing .
                ingredientPart = sb.ToString().Trim();
            }
            ingredientParts.Add(ingredientPart);
            sb.Clear();
            count = 0;
            commaIndex = 0;
        }

        public List<ParsedIngredient> ParseIngredients(List<string> ingredientParts, List<ParsedAnnotation> parsedAnnotations)
        {
            var parsedIngredients = new List<ParsedIngredient>();
            foreach (var ingredient in ingredientParts)
            {
                var parsedIngredient = new ParsedIngredient { Text = ingredient.Trim() };
                var name = ingredient.Trim();
                parsedIngredient.Ingredients = ParseDetails(ref name, parsedAnnotations).Ingredients;
                parsedIngredient.PercentageText = ParsePercentage(ref name);
                parsedIngredient.Annotations = ParseAnnotations(ref name, parsedAnnotations);
                parsedIngredient.Allergens = ParseAllergents(ref name);

                parsedIngredient.Name = name;
                parsedIngredients.Add(parsedIngredient);

                // todo
                // - ingredients with a percentage can be prefixed by 'at least' or 'at most'
            }
            return parsedIngredients;
        }

        public List<string> ParseAllergents(ref string text)
        {
            var allergants = new List<string>();
            var underscores = text.Trim().Split('_');
            var count = 0;
            foreach (var underscore in underscores)
            {
                if (count % 2 != 0)
                {
                    allergants.Add(underscore);
                }
                count++;
            }
            text = text.Replace("_", "");
            return allergants;
        }

        private List<ParsedAnnotation> ParseAnnotations(string[] annotationsList)
        {
            var parsedAnnotations = new List<ParsedAnnotation>();
            foreach (var annotation in annotationsList)
            {
                var parsedAnnotation = new ParsedAnnotation();
                var regex = new Regex(@"[\*]+");
                var result = regex.Match(annotation.Trim());
                var index = 0;
                if (result.Success)
                {
                    index = result.Length;
                }
                if (index > 0)
                {
                    parsedAnnotation.Identifier = result.Value.Trim();
                    parsedAnnotation.Text = annotation.Substring(index).Trim();
                    parsedAnnotations.Add(parsedAnnotation);
                }
            }
            return parsedAnnotations;
        }

        private string NormalizeSpaces(string text)
        {
            return Regex.Replace(text, @"\s+", " ");
        }

        public string NormalisePercentages(string text)
        {
            var percentageMatch = @"\(\d+(?:[\.\,]\d+)?\s?%\)";
            var match = GetRegExMatch(text, percentageMatch);
            while (match.Length > 0)
            {
                var cleaned = match.Substring(1, match.Length - 2);
                text = text.Replace(match, cleaned);
                match = GetRegExMatch(text, percentageMatch);
            };
            text = NormalizeSpaces(text);
            return text;
        }

        public string ParsePercentage(ref string text)
        {
            var percentageMatch = @"\d+(?:[\.\,]\d+)?\s?%";
            var match = GetRegExMatch(text, percentageMatch);
            if (match.Length > 0)
            {
                text = text.Replace(match, "").Trim();
                text = NormalizeSpaces(text);
            }
            return match;
        }

        public string GetRegExMatch(string text, string pattern)
        {
            var regex = new Regex(pattern);
            var result = regex.Match(text.Trim());
            var index = 0;
            if (result.Success)
            {
                index = result.Length;
            }
            if (index > 0)
            {
                var match = result.Value.Trim();
                return match;
            }
            return "";
        }

        public ParsedIngredients ParseDetails(ref string text, List<ParsedAnnotation> parsedAnnotations)
        {
            var indexOfOpenParenthesis = text.IndexOf('(');
            var indexOfOpenSquareBrackets = text.IndexOf('[');
            var indexOfColon = text.IndexOf(':');
            if (indexOfColon > 0)
            {
                var detailSection = text.Substring(indexOfColon + 1);
                text = text.Substring(0, indexOfColon - 1).Trim();
                return Parse(detailSection, parsedAnnotations);
                // todo : convert colon notation to bracket notation, replace colon by open parenth and first dot after colon by close parenth and comma?)
                // todo : and do so recursively?
            }
            else
            {
                var openChar = '(';
                var closeChar = ')';
                if (indexOfOpenSquareBrackets != -1)
                {
                    if (indexOfOpenParenthesis > indexOfOpenSquareBrackets || indexOfOpenParenthesis == -1)
                    {
                        openChar = '[';
                        closeChar = ']';
                    }
                }


                if (text.Contains(openChar.ToString()))
                {
                    var starting = text.IndexOf(openChar);
                    var closing = text.LastIndexOf(closeChar);
                    if (starting < closing)
                    {
                        var detailSection = text.Substring(starting + 1, closing - (starting + 1));
                        text = text.Replace(openChar + detailSection + closeChar, "").Trim();
                        if (detailSection.Substring(0, 1) != "=")
                        {
                            return Parse(detailSection, parsedAnnotations);
                        }
                    }
                }
            }

            return new ParsedIngredients();
        }
        public List<ParsedAnnotation> ParseAnnotations(ref string text, List<ParsedAnnotation> annotations)
        {
            var detectedAnnotations = new List<ParsedAnnotation>();
            foreach (var annotation in annotations)
            {
                var match = GetRegExMatch(text, @"[\*]+");
                if (match == annotation.Identifier)
                {
                    detectedAnnotations.Add(annotation);
                    text = text.Replace(match, "").Trim();
                }
                match = GetRegExMatch(text, @"[\#]+");
                if (match == annotation.Identifier)
                {
                    detectedAnnotations.Add(annotation);
                    text = text.Replace(match, "").Trim();
                }
            }
            return detectedAnnotations;
        }
    }
}
