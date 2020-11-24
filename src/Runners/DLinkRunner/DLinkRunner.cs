using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using H.NET.Core.Runners;

namespace H.Runners
{
    // ReSharper disable once UnusedMember.Global
    public class DLinkRunner : Runner
    {
        #region Properties

        public string Url { get; set; } = "http://192.168.0.1/";
        public string Login { get; set; } = "admin";
        public string Password { get; set; } = string.Empty;

        #endregion

        #region Constructors

        public DLinkRunner()
        {
            AddSetting(nameof(Url), o => Url = o, null, Url);
            AddSetting(nameof(Login), o => Login = o, null, Login);
            AddSetting(nameof(Password), o => Password = o, null, Password);

            AddAsyncAction("reload-router", ReloadRouter);
        }

        #endregion

        #region Private methods

        private async Task ReloadRouter(string text)
        {
            var uri = new Uri(Url);

            var cookieContainer = new CookieContainer();
            cookieContainer.Add(uri, new Cookie("user_ip", "192.168.0.94"));
            cookieContainer.Add(uri, new Cookie("cookie_lang", "rus"));
            cookieContainer.Add(uri, new Cookie("url_hash", ""));
            cookieContainer.Add(uri, new Cookie("client_login", Login));
            cookieContainer.Add(uri, new Cookie("client_password", Password));

            using var client = new HttpClient(new HttpClientHandler
            {
                CookieContainer = cookieContainer
            })
            {
                BaseAddress = uri
            };
            using var request = new HttpRequestMessage
            {
                RequestUri = new Uri("index.cgi?res_cmd=6&res_buf=null&res_cmd_type=nbl&v2=y&rq=y&proxy=y&_=1542876693187", UriKind.Relative),
                Method = HttpMethod.Get
            };
            using var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Print($"Bad Response: {response}");
                return;
            }

            Print("Reloading in process");
        }

        #endregion
    }
}
