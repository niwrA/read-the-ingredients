using EUAdditiveLanguageNamesShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace EUAdditivesLanguageNameImporterShared
{
    public class TaxonomyImporter
    {
        private EUAdditiveLanguageNames.IEUAdditiveLanguageNameRepository _repo;
        public TaxonomyImporter(EUAdditiveLanguageNames.IEUAdditiveLanguageNameRepository repo)
        {
            _repo = repo;
        }
        public async void Import(string fileName)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var uri = new Uri("ms-appx:///" + fileName);
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            string text = await Windows.Storage.FileIO.ReadTextAsync(file);
            var reader = new System.IO.StringReader(text);
            Guid ingredientGuid = Guid.NewGuid();

            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    ingredientGuid = Guid.NewGuid();
                }

                if (line.Contains(":"))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string language = parts[0];
                        if (language == "e_number")
                        {
                            // todo
                        }
                        else if (language == "wikidata")
                        {
                            // todo
                        }
                        else if (language == "colour_index")
                        {
                            // todo
                        }
                        else
                        {
                            var nameSection = parts[1];

                            var names = nameSection.Split(',');
                            var euNumber = names[0].Trim();
                            foreach (var name in names)
                            {
                                if (!string.IsNullOrWhiteSpace(name) && name != euNumber)
                                {
                                    AddTranslation(ingredientGuid, euNumber, language, name.ToString().Trim());
                                }
                            }
                        }
                    }
                }
            }
        }

        private void AddTranslation(Guid? ingredientGuid, string euNumber, string language, string name)
        {
            var state = _repo.CreateEUAdditiveLanguageNameState();
            state.EUAdditiveGuid = ingredientGuid.Value;
            state.Language = language;
            state.EUNumber = euNumber;
            state.Name = name;
            _repo.UpdateEUAdditiveNameState(state);
        }
    }
}
