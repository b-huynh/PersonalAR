using System;
using System.IO;
using System.Text;
using UnityEngine;
using File = UnityEngine.Windows.File;

#if WINDOWS_UWP
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
#else
using System.Net.Http;
#endif

namespace Recaug
{
    public static class ConfigTools
    {
        // Fetch a .json config file from a web URL through HTTP.
        // on success: returns JSON file contents on success
        // on failure: returns ""
#if WINDOWS_UWP
        public static string GetJsonHTTP(string url)
        {
            // Set non-caching behavior
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.NoCache;
            HttpClient httpClient = new HttpClient(filter);

            Uri requestUri = new Uri(url);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                //Send GET request SYNCHRONOUSLY (will block)
                httpResponse = httpClient.GetAsync(requestUri).GetAwaiter().GetResult();
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.Log("Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
            }
            return httpResponseBody;
        }
#else
        public static string GetJsonHTTP(string url)
        {
            HttpClient httpClient = new HttpClient();
            string responseBody = "";
            try
            {
                HttpResponseMessage response = 
                    httpClient.GetAsync(url).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();
                responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.Log("Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
                Debug.Log(url);            
            }
            return responseBody;
        }
#endif
        // Reads a .json config file 
        // on success: returns JSON file contents on success
        // on failure: returns ""
        public static string GetJsonFile(string filepath)
        {
            return Encoding.UTF8.GetString(File.ReadAllBytes(filepath));
        }

        // Reads a .json config file from Application.persistantDataPath
        // on success: returns JSON file contents on success
        // on failure: returns ""
        public static string GetJsonPersistantFile(string filename)
        {
            string filepath = Path.Combine(Application.persistentDataPath, filename);
            return GetJsonFile(filepath);
        }
    } // ConfigTools
} // namespace Recaug