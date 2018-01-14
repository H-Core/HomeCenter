using System;
using System.IO;
using System.Net;

namespace VoiceActions.NET.Converters.Core
{
    public static class WebUtilities
    {
        public static (string, Exception) GetResponseText(this WebRequest request)
        {
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return (null, new Exception($"Error: {response.StatusCode}"));
                }

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream == null)
                    {
                        return (null, new Exception("Error: GetResponseStream return null"));
                    }

                    using (var responseReader = new StreamReader(responseStream))
                    {
                        return (responseReader.ReadToEnd(), null);
                    }
                }
            }
            catch (Exception exception)
            {
                return (null, exception);
            }
        }
    }
}
