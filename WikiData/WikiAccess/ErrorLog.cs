using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    public interface ErrorLog
    {
        string Module { get; }
        List<ErrorMessage> Errors { get; set; }
    }
}

/*
 * A = WikimediaAPI
 * B = WikidataBiography
 * C = Category (cf Wikipedia)
 * D = WikidataIO
 * E = WikidataExtract
 * G = WikipediaBiography
 * T = Template
 * W = WikipediaIO
 * Y = Category (cf Wikidata)
 */
