using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web.Script.Serialization;

namespace VF
{
    public enum URLShortenerService
    {
        Google,
        //TinyURL,
    }
    public class URLShortener
    {
        public static string GetFullURL(string _ShortenedURL, URLShortenerService _Service = URLShortenerService.Google)
        {
            if (_Service != URLShortenerService.Google)
                return "";

            if (_ShortenedURL.StartsWith("http://") == false)
                _ShortenedURL = "http://goo.gl/" + _ShortenedURL;
            try
            {
                string responseText = "";
                var webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create("https://www.googleapis.com/urlshortener/v1/url?shortUrl=" + _ShortenedURL);
                using (var webResponse = webRequest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(responseStream))
                        {
                            responseText = reader.ReadToEnd();
                        }
                    }
                }
                var jsonData = (new JavaScriptSerializer()).Deserialize<Dictionary<string, dynamic>>(responseText);
                if (jsonData.ContainsKey("status") == true)
                {
                    if (jsonData["status"] == "OK")
                    {
                        if (jsonData.ContainsKey("longUrl") == true)
                            return jsonData["longUrl"];
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }
        public static string GetFullURLAsync(string _ShortenedURL, URLShortenerService _Service = URLShortenerService.Google)
        {
            var retValue = "";
            var task = new System.Threading.Tasks.Task(() => {
                retValue = GetFullURL(_ShortenedURL, _Service);
            });
            task.Start();
            while (task.Wait(50))
            { }
            return retValue;
        }
        public static string CreateShortURL(string _FullURL, URLShortenerService _Service = URLShortenerService.Google, string _APIKey = null)
        {
            if (_Service != URLShortenerService.Google)
                return "";

            if (_APIKey == null)
                _APIKey = HiddenStrings.GoogleAPIKey;

            try
            {
                string responseText = "";
                var webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://www.googleapis.com/urlshortener/v1/url");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";

                byte[] byteData = UTF8Encoding.UTF8.GetBytes("{\"longUrl\": \"" + _FullURL + "\"}");
                webRequest.ContentLength = byteData.Length;

                var postStream = webRequest.GetRequestStream();
                postStream.Write(byteData, 0, byteData.Length);

                using (var webResponse = webRequest.GetResponse())
                {
                    using (var responseStream = webResponse.GetResponseStream())
                    {
                        using (System.IO.StreamReader reader = new System.IO.StreamReader(responseStream))
                        {
                            responseText = reader.ReadToEnd();
                        }
                    }
                }
                postStream.Close();

                var jsonData = (new JavaScriptSerializer()).Deserialize<Dictionary<string, dynamic>>(responseText);
                if (jsonData.ContainsKey("id") == true && jsonData.ContainsKey("longUrl") == true)
                {
                    if(Uri.Compare(new Uri(((string)jsonData["longUrl"])), new Uri(_FullURL), UriComponents.Query, UriFormat.Unescaped, StringComparison.InvariantCulture) == 0)
                        return jsonData["id"];
                }
            }
            catch (Exception)
            {
                return "";
            }
            return "";
        }
    }
}
