using System;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using H.NET.Core.Settings;
using H.NET.Notifiers;

namespace H.Notifiers
{
    // ReSharper disable once UnusedMember.Global
    public class RssNotifier : TimerNotifier
    {
        #region Properties

        private string Url { get; set; }

        private string LastTitle { get; set; }

        #endregion

        #region Contructors

        public RssNotifier()
        {
            AddSetting(nameof(Url), o => Url = o, o => true, string.Empty, SettingType.Path);

            AddVariable("$rss_last_title$", () => LastTitle);
        }

        #endregion

        #region Protected methods

        protected override bool OnResult()
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return false;
            }

            SyndicationFeed feed;
            try
            {
                using (var reader = XmlReader.Create(Url))
                {
                    feed = SyndicationFeed.Load(reader);
                }
            }
            catch (WebException exception)
            {
                Print($"Rss Web Exception: {exception.Message}");

                return false;
            }

            var firstItem = feed.Items.FirstOrDefault();
            var title = firstItem?.Title.Text;

            if (title == null ||
                string.Equals(title, LastTitle, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var isFirstFeed = LastTitle == null;

            LastTitle = title;

            return !isFirstFeed;
        }

        #endregion

    }
}
