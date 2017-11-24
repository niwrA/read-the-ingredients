using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace WikiAccess
{
    public enum ClaimType { @null, @string, @int, @DateTime }

    /// <summary>
    /// Container to hold a Wikidata claim. 
    /// </summary>
    public class WikidataClaim
    {
        private string _ValueAsString;
        private int _ValueAsInt;
        private Wikidate _ValueAsDateTime;

        public int Qcode { get; set; }
        public int Pcode { get; set; }
        public ClaimType Type { private set; get; }
        public Wikidate ValueAsDateTime
        {
            get
            {
                return _ValueAsDateTime;
            }
            set
            {
                _ValueAsDateTime = value;
                Type = ClaimType.DateTime;
            }
        }

        public string ValueAsString
        {
            get
            {
                return _ValueAsString;
            }
            set
            {
                _ValueAsString = value;
                Type = ClaimType.@string;
            }
        }

        public int ValueAsInt
        {
            get
            {
                return _ValueAsInt;
            }
            set
            {
                _ValueAsInt = value;
                Type = ClaimType.@int;
            }
        }

        public WikidataClaim()
        {
            _ValueAsDateTime = new Wikidate();
            Type = new ClaimType();
            Qcode = 0;
        }
        public override string ToString()
        {
            switch (Type)
            {
                case ClaimType.DateTime:
                    return ValueAsDateTime.ToString();
                case ClaimType.@int:
                    return ValueAsInt.ToString();
                default:
                    return ValueAsString;
            }
        }
    }
}


