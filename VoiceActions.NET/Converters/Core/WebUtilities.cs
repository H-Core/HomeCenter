using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VoiceActions.NET.Converters.Core
{
    public static class WebUtilities
    {
        public static async Task<(string, Exception)> GetResponseText(this HttpResponseMessage message)
        {
            try
            {
                if (!message.IsSuccessStatusCode)
                {
                    return (null, new Exception($"Errored status code: {message.StatusCode}"));
                }

                var text = await message.Content.ReadAsStringAsync();
                return (text, null);
            }
            catch (Exception exception)
            {
                return (null, exception);
            }
        }
    }
}
