using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;    // https://github.com/JamesNK/Newtonsoft.Json

namespace WikiAccess
{
    /// <summary>
    /// Class to extract data from downloaded Wikidata
    /// This could have been part of WikidataIO.cs, but split out as it is large and cumbersome
    /// </summary>
    class WikidataExtract
    {
        public WikidataFields Fields { get; set; }
        private string Content { get; set; }
        public string[] ClaimsRequired { get; set; }
        private WikidataCache Cache = new WikidataCache();
        public WikidataExtractErrorLog WikidataExtractErrors { get; set; }
        public bool Success { get; set; }

        public WikidataExtract(string content, string[] claimsrequired)
        {
            WikidataExtractErrors = new WikidataExtractErrorLog();
            ClaimsRequired = claimsrequired;
            Fields = new WikidataFields();
            Content = content;
            Success = ExtractJSON();
        }

        /// <summary>
        /// Note: This method requires Newtonsoft Json to be installed
        /// </summary>
        /// <returns></returns>
        private bool ExtractJSON()
        {
            //Interpret the JSON - Basically read in a level at a time.
            var DataFromWiki = JObject.Parse(Content);
            var Entities = (JObject)DataFromWiki["entities"];

            var Entity = Entities.Properties().First();   // Name is variable, so grab data by using first method
            string EntityKey = Entity.Name;

            var EntityData = (JObject)Entity.Value;

            if (EntityKey == "-1")
            {
                WikidataExtractErrors.NotWikidata();
                return false;
            }

            string Qcode = (string)EntityData["id"];
            Fields.Id = Convert.ToInt32(Qcode.Substring(1));
            string EntityType = (string)EntityData["type"];

            if (EntityType == null)
            {
                WikidataExtractErrors.QcodeNotExist(EntityKey);
                return false;
            }

            var Descriptions = (JObject)EntityData["descriptions"];
            var Labels = (JObject)EntityData["labels"];
            var WikipediaLinks = (JObject)EntityData["sitelinks"];

            if (Labels != null)
            {
                foreach (var Label in Labels.Properties())
                {
                    var LabelData = (JObject)Label.Value;
                    Fields.Labels.Add((string)LabelData["language"], (string)LabelData["value"]);
                }
            }

            if (Descriptions != null)
            {
                foreach (var Description in Descriptions.Properties())
                {
                    var DescriptionData = (JObject)Description.Value;
                    string l = (string)DescriptionData["language"];
                    Fields.Description.Add((string)DescriptionData["language"], (string)DescriptionData["value"]);
                }
            }

            if (WikipediaLinks != null)
            {
                foreach (var WikipediaLink in WikipediaLinks.Properties())
                {
                    var WikipediaLinkData = (JObject)WikipediaLink.Value;
                    Fields.WikipediaLinks.Add((string)WikipediaLinkData["site"], (string)WikipediaLinkData["title"]);
                }
            }

            var Claims = (JObject)EntityData["claims"];
            if (Claims != null)
            {
                //Now we get to loop through each claim property for that article
                foreach (var Claim in Claims.Properties())
                {
                    string ClaimKey = Claim.Name;

                    if (ClaimsRequired != null && Array.IndexOf(ClaimsRequired, ClaimKey) == -1) continue;

                    var ClaimData = (JArray)Claim.Value;

                    for (int ThisClaim = 0; ThisClaim < ClaimData.Count(); ThisClaim++)
                    {
                        //claimData is an array - another loop

                        WikidataClaim ThisClaimData = new WikidataClaim();

                        var MainSnak = (JObject)ClaimData[ThisClaim]["mainsnak"];
                        string SnakType = (string)MainSnak["snaktype"];
                        string SnakDataType = (string)MainSnak["datatype"];
                        var SnakDataValue = (JObject)MainSnak["datavalue"];

                        if (SnakType == "novalue" || SnakType == "somevalue")
                        {
                            ThisClaimData.ValueAsString = SnakType;
                        }
                        else
                        {
                            if (SnakDataType == "string" || SnakDataType == "commonsMedia" || SnakDataType == "url")
                            {
                                ThisClaimData.ValueAsString = (string)SnakDataValue["value"];
                            }
                            else if (SnakDataType == "wikibase-item")
                            {
                                var ObjectValue = (JObject)SnakDataValue["value"];
                                ThisClaimData.Qcode = (int)ObjectValue["numeric-id"];
                                ThisClaimData.ValueAsString = Cache.RetrieveLabel(ThisClaimData.Qcode);

                            }
                            else if (SnakDataType == "time")
                            {
                                var ObjectValue = (JObject)SnakDataValue["value"];

                                string ValueTime = (string)ObjectValue["time"];


                                string ValueTimePrecision = (string)ObjectValue["precision"];
                                string ValueTimeCalendarModel = (string)ObjectValue["calendarmodel"];

                                bool Julian = false;
                                bool Gregorian = false;

                                if (ValueTimeCalendarModel != "http://www.Wikidata.org/entity/Q1985727")
                                    Gregorian = true;
                                if (ValueTimeCalendarModel == "http://www.Wikidata.org/entity/Q1985786")
                                    Julian = true;

                                if (ValueTimePrecision == "11" || ValueTimePrecision == "10" || ValueTimePrecision == "9"
                                                               || ValueTimePrecision == "8" || ValueTimePrecision == "7" || ValueTimePrecision == "6")
                                {
                                    int DateStart = ValueTime.IndexOf("-", 2) - 4;

                                    string ThisDateString = (ValueTime.Substring(DateStart, 10));
                                    ThisDateString = ThisDateString.Replace("-00", "-01");  // Occasionally get 1901-00-00 ?

                                    bool ValidDate = true;
                                    DateTime thisDate;
                                    try
                                    {
                                        thisDate = DateTime.Parse(ThisDateString, null, DateTimeStyles.RoundtripKind);
                                    }
                                    catch
                                    {
                                        thisDate = DateTime.MinValue;
                                        ValidDate = false;
                                    }
                                    if (Julian == true && ValueTimePrecision == "11")
                                    {
                                        // All dates will be Gregorian
                                        // Julian flag tells us to display Julian date.
                                        // JulianCalendar JulCal = new JulianCalendar();
                                        // DateTime dta = JulCal.ToDateTime(thisDate.Year, thisDate.Month, thisDate.Day, 0, 0, 0, 0);
                                        // thisDate = dta;
                                    }

                                    DatePrecision Precision = DatePrecision.Null;


                                    if (ValidDate == false)
                                    {
                                        Precision = DatePrecision.Invalid;
                                    }
                                    else if (ValueTime.Substring(0, 1) == "+")
                                    {
                                        switch (ValueTimePrecision)
                                        {
                                            case "11":
                                                Precision = DatePrecision.Day;
                                                break;
                                            case "10":
                                                Precision = DatePrecision.Month;
                                                break;
                                            case "9":
                                                Precision = DatePrecision.Year;
                                                break;
                                            case "8":
                                                Precision = DatePrecision.Decade;
                                                break;
                                            case "7":
                                                Precision = DatePrecision.Century;
                                                break;
                                            case "6":
                                                Precision = DatePrecision.Millenium;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        Precision = DatePrecision.BCE;
                                    }

                                    ThisClaimData.ValueAsDateTime.thisDate = thisDate;
                                    ThisClaimData.ValueAsDateTime.thisPrecision = Precision;
                                }
                            }
                            else if (SnakDataType == "monolingualtext")
                            {
                                var ObjectValue = (JObject)SnakDataValue["value"];
                                string ValueText = (string)ObjectValue["text"];
                                string ValueLanguage = (string)ObjectValue["language"];
                                // TODO Multi language handling
                                ThisClaimData.ValueAsString = ValueText + "(" + ValueLanguage + ")";
                            }
                            else if (SnakDataType == "quantity")
                            {
                                var ObjectValue = (JObject)SnakDataValue["value"];
                                string ValueAmount = (string)ObjectValue["amount"];
                                string ValueUnit = (string)ObjectValue["unit"];
                                string ValueUpper = (string)ObjectValue["upperBound"];
                                string ValueLower = (string)ObjectValue["lowerBound"];

                                ThisClaimData.ValueAsString = "(" + ValueLower + " to " + ValueUpper + ") Unit " + ValueUnit;
                            }
                        }
                        Fields.Claims.Add(new KeyValuePair<int, WikidataClaim>(Convert.ToInt32(ClaimKey.Substring(1)), ThisClaimData));
                    }
                }
            }
            return true;
        }
    }
}
