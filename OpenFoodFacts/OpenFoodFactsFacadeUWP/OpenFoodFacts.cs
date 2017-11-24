using Newtonsoft.Json;
using OpenFoodFactsContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace OpenFoodFactsFacade
{
    public class ProductSearchResultDTO
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Language { get; set; }
        public string FrontImageUrl { get; set; }
    }
    public class OpenFoodFacts
    {
        private CancellationTokenSource cts;
        public bool TestMode { get; set; } = false;
        public string BaseUrl { get { if (TestMode) { return "http://world.openfoodfacts.org"; } else { return "http://world.openfoodfacts.org"; } } }

        private async Task<string> GetJson(string url)
        {
            var handler = new HttpBaseProtocolFilter()
            {
                
            };
            handler.ServerCustomValidationRequested += (sender, args) =>
            {
                var cert = args.ServerCertificate;
                // Your custom cert validation code here.
                var errors = args.ServerCertificateErrors;
                Debug.WriteLine(errors.Count);
            };
            var httpClient = new HttpClient(handler);
            try
            {
                var response = httpClient.GetAsync(new Uri(url));

                //var response = await httpClient.GetStringAsync(new Uri(url));
                //return response;
                var result = response.GetResults();
                var resultString = await result.Content.ReadAsStringAsync();
                return resultString;
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
                throw;
            }
            return null;
        }

        public OpenFoodFactsResultDTO GetByBarCode(string barcode)
        {
            var barcodeString = System.Text.RegularExpressions.Regex.Unescape(barcode);
            var url = $"{BaseUrl}/api/v0/product/{barcode}.json";
            var json = GetJson(url).Result.ToString();
            OpenFoodFactsResultDTO result = null;
            try
            {
                result = JsonConvert.DeserializeObject<OpenFoodFactsResultDTO>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return result;
        }

        public OpenFoodFactsSearchResultDTO SearchByText(string text)
        {
            var url = $"{BaseUrl}/cgi/search.pl?search_terms={text}&search_simple=1&action=process&json=1";
            var json = GetJson(url).Result.ToString();
            OpenFoodFactsSearchResultDTO result;
            try
            {
                result = JsonConvert.DeserializeObject<OpenFoodFactsSearchResultDTO>(json);
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public async void UploadImage(string barcode, MemoryStream stream, string languageCode)
        {
            var jsonparams = $"{{code: '{barcode}', imagefield: 'front' }}";
            var jsonPost = $"{{ fileKey: 'imgupload_front', fileName: '{barcode}_front', params : {jsonparams} }}";
            Uri uri = new Uri($"http://world-{languageCode}.openfoodfacts.net/cgi/product_image_upload.pl");
            HttpClient client = new HttpClient();
            // todo: rewrite to system.net namespace?
            //var streamContent = new Windows.Web.Http.HttpStreamContent(stream.AsInputStream());
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            //request.Content = streamContent;
            
            //HttpResponseMessage response = await client.PostAsync(uri, streamContent).AsTask(cts.Token);
        }
    }
}
