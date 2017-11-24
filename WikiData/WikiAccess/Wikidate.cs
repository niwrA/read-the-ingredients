﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WikiAccess
{
    public enum DatePrecision { Null, Day, Month, Year, Decade, Century, Unknown, NotEntered, NoProperty, BCE, Invalid, Millenium };

    /// <summary>
    /// Class to hold a date, which includes a precision
    /// </summary>
    public class Wikidate
    {
        public Wikidate() { }
        public DateTime thisDate { get; set; }
        public DatePrecision thisPrecision { get; set; }
        public int Year
        {
            get
            {
                if (isCalculable(thisPrecision))
                    return thisDate.Year;
                else
                    return 0;
            }
        }


        public override string ToString()
        {
            string FormattedDate = "Invalid";

            switch (thisPrecision)
            {
                case DatePrecision.Null:
                case DatePrecision.Day:
                    FormattedDate = thisDate.ToString("d MMMM yyyy");
                    break;
                case DatePrecision.Month:
                    FormattedDate = thisDate.ToString("MMMM yyyy");
                    break;
                case DatePrecision.Year:
                    FormattedDate = thisDate.ToString("yyyy");
                    break;
                case DatePrecision.Decade:
                    FormattedDate = thisDate.ToString("yyyy").Substring(0, 3) + "0s";
                    break;
                case DatePrecision.Century:
                    int Century = Convert.ToInt32(thisDate.ToString("yyyy").Substring(0, 2));
                    FormattedDate = (Century + 1).ToString() + "th century";
                    break;
                case DatePrecision.Millenium:
                    int Millenium = Convert.ToInt32(thisDate.ToString("yyyy").Substring(0, 1));
                    FormattedDate = (Millenium + 1).ToString() + " millenium";
                    break;
                case DatePrecision.Unknown:
                    FormattedDate = "Unknown";
                    break;
                case DatePrecision.NotEntered:
                case DatePrecision.NoProperty:
                    FormattedDate = "No value";
                    break;
            }
            return FormattedDate;
        }

        public static bool isCalculable(DatePrecision thisPrecision)
        {
            switch (thisPrecision)
            {
                case DatePrecision.Day:
                case DatePrecision.Decade:
                case DatePrecision.Month:
                case DatePrecision.Year:
                    return true;

                default:
                    return false;

            }
        }
    }
}