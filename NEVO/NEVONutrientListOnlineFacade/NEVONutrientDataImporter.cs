using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace NEVONutrientListOnlineFacade
{
    public class NEVONutrientDataImporter
    {
        public interface INEVOImporterRepository : NEVOProductNutrients.INEVOProductNutrientsRepository, NEVOProducts.INEVOProductsRepository
        {

        }
        private INEVOImporterRepository _repo;
        public NEVONutrientDataImporter(INEVOImporterRepository repo)
        {
            _repo = repo;
        }

        public static Encoding GetEncoding(byte[] bom)
        {
            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
        private async Task<string> loadFile(string fileName)
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + fileName));
            var inputStream = await file.OpenReadAsync();

            IBuffer buffer = await FileIO.ReadBufferAsync(file);
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] fileContent = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(fileContent);
            string text = GetEncoding(new byte[4] { fileContent[0], fileContent[1], fileContent[2], fileContent[3] }).GetString(fileContent);
            return text;
        }

        public async void Import(string fileName)
        {

            string text = await loadFile(fileName);
            Guid euAdditiveGuid = Guid.NewGuid();
            var reader = new StringReader(text);

            var nevoProducts = new NEVOProducts(_repo);
            var nevoProductNutrients = new NEVOProductNutrients(_repo);
            NEVOProduct nevoProduct = null;
            bool InProductProperties = false;
            bool InNutrientProperties = false;
            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (line == "----------")
                {
                    nevoProduct = nevoProducts.CreateNEVOProduct();
                    InProductProperties = true;
                    InNutrientProperties = false;
                    line = reader.ReadLine();
                }
                else if (line == "------------  -------  -----------  -------------  -----")
                {
                    InProductProperties = false;
                    InNutrientProperties = true;
                    line = reader.ReadLine();
                    line = reader.ReadLine();
                }
                if (InProductProperties)
                {
                    var label = line.Substring(0, 23).Trim();
                    var value = line.Substring(24).Trim();
                    switch (label)
                    {
                        case "Productgroep-oms":
                            break;
                        case "Productgroepcode":
                            nevoProduct.GroupId = int.Parse(value);
                            break;
                        case "Productcode":
                            nevoProduct.Id = int.Parse(value);
                            break;
                        case "Controlegetal":
                            nevoProduct.CheckId = int.Parse(value);
                            break;
                        case "Product_omschrijving":
                            nevoProduct.NLDescription = value;
                            break;
                        case "Product_description":
                            nevoProduct.ENDescription = value;
                            break;
                        case "Fabrikantnaam":
                            nevoProduct.ManufacturerName = value;
                            break;
                        case "Code_nonactief":
                            nevoProduct.InActive = value == "Y";
                            break;
                        case "Hoeveelheid":
                            nevoProduct.Quantity = int.Parse(value);
                            break;
                        case "Meeteenheid":
                            nevoProduct.Unit = value;
                            break;
                        case "Eetbaar_gedeelte":
                            nevoProduct.EdiblePart = value;
                            break;
                        case "Vertrouwelijk_code":
                            nevoProduct.Confidential = value == "Y";
                            break;
                        case "Commentaarregel":
                            nevoProduct.Comment = value;
                            break;
                    }
                }
                else if (InNutrientProperties)
                {
                    nevoProductNutrients.CreateNEVOProductNutrient(nevoProduct.Id, line.Substring(0, 12).Trim(), line.Substring(13, 12).Trim(), line.Substring(26, 20).Trim(), line.Substring(47, 9).Trim());
                }
            }
        }

    }
}
