using System;
using System.IO;
using System.Net;

namespace DuwMonitorWebJob
{
    public class DuwStorage: IDuwStorage
    {
        public string ReadDataByDate(DateTime targetDate)
        {
            var cookieData = "config[lang]=pol; AKIS=bnl4k9j1m16705kgn89qksgos1";
            var request = (HttpWebRequest)WebRequest.Create($"http://rezerwacja.duw.pl/reservation/pol/queues/25/7/{targetDate.ToString("yyyy-MM-dd")}");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36";
            request.Method = "GET";
            request.Headers.Add("Cookie", cookieData);

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                var pageData = reader.ReadToEnd();
                                if (AuthorizationRequired(pageData))
                                {
                                    throw new DuwStorageException("Athorization required.");
                                }
                                return pageData;
                            }
                        }
                        throw new DuwStorageException("The response stream is null.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new DuwStorageException("Cannot read data from DUW storage.", ex);
            }
            
        }

        private static bool AuthorizationRequired(string pageData)
        {
            return !string.IsNullOrEmpty(pageData) && pageData.Contains("Zaloguj");
        }
    }
}