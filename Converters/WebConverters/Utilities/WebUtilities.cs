using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace H.NET.Converters.Utilities
{
    public static class WebUtilities
    {
        public class Information
        {
            public string Text { get; set; }
            public Exception Exception { get; set; }

            public Information(Exception exception)
            {
                Exception = exception;
            }

            public Information(string text)
            {
                Text = text;
            }
        }

        public static async Task<Information> GetResponseText(this HttpResponseMessage message)
        {
            try
            {
                if (!message.IsSuccessStatusCode)
                {
                    return new Information(new Exception($"Errored status code: {message.StatusCode}"));
                }

                var text = await message.Content.ReadAsStringAsync();
                return new Information(text);
            }
            catch (Exception exception)
            {
                return new Information(exception);
            }
        }
    }
}
