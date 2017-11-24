using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WikiAccess
{
    /// <summary>
    /// Class to create a cache of property labels, cutting down on Wikidata traffic.
    /// </summary>
    class WikidataCache
    {
        Dictionary<int, string> _Cache = new Dictionary<int, string>();
        private readonly string LABELCACHE = Path.GetTempPath() + "WikidataLabelCache";

        /// <summary>
        /// Constructor. Reads in existing cache from LABELCACHE
        /// TODO Error trap for dodgy cache.
        /// </summary>
        public WikidataCache()
        {
            //if (!File.Exists(LABELCACHE))
            //    File.Create(LABELCACHE).Close();

            //if (_Cache.Count == 0)
            //{
            //    using (StreamReader Sr = new StreamReader(LABELCACHE))
            //    {
            //        string PropertyAsString;
            //        int Property;
            //        while ((PropertyAsString = Sr.ReadLine()) != null)
            //        {
            //            Property = Convert.ToInt32(PropertyAsString);
            //            string Description = Sr.ReadLine();
            //            _Cache.Add(Property, Description);
            //        }
            //    }
            //}

        }

        public string RetrieveLabel(int qcode)
        {
            string Description = null;
            if (_Cache.TryGetValue(qcode, out Description))
            {
                return Description;
            }
            else
            {
                return LookupLabel(qcode);
            }
        }

        /// <summary>
        /// If its a new property, look up label on Wikidata and add to cache.
        /// </summary>
        /// <param name="qcode"></param>
        /// <returns></returns>
        private string LookupLabel(int qcode)
        {
            WikidataIO IO = new WikidataIO();
            IO.Action = "wbgetentities";
            IO.Format = "json";
            IO.Ids = qcode;
            IO.Props = "labels";
            IO.Languages = "en|en-gb|ro";
            WikidataFields Fields = new WikidataFields();

            Fields = IO.GetData();

            string Name = "";
            if (!Fields.Labels.TryGetValue("en-gb", out Name))
                if (!Fields.Labels.TryGetValue("en", out Name))
                    Fields.Labels.TryGetValue("en", out Name);

            //using (StreamWriter Sw = File.AppendText(LABELCACHE))
            //{
            //    Sw.WriteLine(qcode);
            //    Sw.WriteLine(Name);
            //}

            //_Cache.Add(qcode, Name);

            return Name;
        }

    }
}
