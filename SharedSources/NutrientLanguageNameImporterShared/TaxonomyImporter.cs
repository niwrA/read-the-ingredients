
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using System.IO;
using NutrientLanguageNameShared;

namespace NutrientLanguageNameTaxonomyImporterShared
{
    public class TaxonomyImporter
    {
        private NutrientLanguageNames.INutrientLanguageNameRepository _repo;
        public TaxonomyImporter(NutrientLanguageNames.INutrientLanguageNameRepository repo)
        {
            _repo = repo;
        }
        private class DetectedNutrient
        {

            public DetectedNutrient(string name, string language)
            {
                Name = name;
                Language = language;
            }

            public Guid Guid { get; set; }
            public string Name { get; set; }
            public string Language { get; set; }
            public int Order { get; set; }
        }
        public async void Import(string fileName)
        {
            string text = await loadFile(fileName);
            Guid euAdditiveGuid = Guid.NewGuid();
            int order = 999;
            var reader = new StringReader(text);
            var buffer = new List<DetectedNutrient>();

            while (true)
            {
                var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (string.IsNullOrEmpty(line))
                {
                    if(buffer.Count>0)
                    {
                        foreach(var detectedNutrient in buffer)
                        {
                            detectedNutrient.Order = order;
                            detectedNutrient.Guid = euAdditiveGuid;
                            AddTranslation(detectedNutrient);
                        }
                    }
                    buffer.Clear();
                    euAdditiveGuid = Guid.NewGuid();
                    order = 999;
                }

                if (line.Contains(":"))
                {
                    var parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string language = parts[0].Trim();
                        string namePart = parts[1];

                        if (language == "order")
                        {
                            order = int.Parse(namePart);
                        }
                        else
                        {
                            var names = namePart.Split(',');
                            foreach(var name in names)
                            {
                                buffer.Add(new DetectedNutrient(name.Trim(), language));
                            }                     
                        }
                    }
                }
            }
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

        private static async Task<System.IO.StringReader> LoadFile(string fileName)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var uri = new Uri("ms-appx:///" + fileName);
            Windows.Storage.StorageFile sampleFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            var reader = new System.IO.StringReader(text);
            return reader;
        }

        private void AddTranslation(DetectedNutrient nutrient)
        {
            var state = _repo.CreateNutrientLanguageNameState();
            state.NutrientGuid = nutrient.Guid;
            state.Language = nutrient.Language.Trim();
            state.Name = nutrient.Name.Trim();
            state.Order = nutrient.Order;
            _repo.UpdateNutrientLanguageNameState(state);
        }
    }
}
