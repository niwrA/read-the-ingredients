using System;
using System.Collections.Generic;
using System.Text;

namespace NEVOContractShared
{
    public class Nutrient
    {
        public string UID { get; set; }
        public string EUCode { get; set; }
        public string NLCode { get; set; }
        public string Unit { get; set; }
        public string NLName { get; set; }
        public string ENName { get; set; }
    }
    public class ProductGroup
    {
        public int Id { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int CheckId { get; set; }
        public string NLDescription { get; set; }
        public string ENDescription { get; set; }
        public string ManufacturerName { get; set; }
        public bool InActive { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public string EdiblePart { get; set; }
        public bool Confidential { get; set; }
        public string Comment { get; set; }
    }

    public class Source
    {
        public string UID { get; set; }
        public string Reference { get; set; }
    }

    public class ProductNutrient
    {
        public string NutrientUID { get; set; }
        public string Quantity { get; set; }
        public string SourceUID { get; set; }
        public string MutationDate { get; set; }
    }
}
