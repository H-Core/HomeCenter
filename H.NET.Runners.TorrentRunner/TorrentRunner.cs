using System.IO;
using H.NET.Core.Runners;
using H.NET.Core.Settings;
using MonoTorrent.Client;
using MonoTorrent.Common;

namespace H.NET.Runners.TorrentRunner
{
    public class TorrentRunner : Runner
    {
        #region Properties

        private string SaveTo { get; set; }

        private ClientEngine Engine { get; set; }
        private TorrentManager Manager { get; set; }

        #endregion

        #region Constructors

        public TorrentRunner()
        {
            AddSetting(nameof(SaveTo), o => SaveTo = o, NoEmpty, string.Empty, SettingType.Folder);
            //AddSetting(nameof(UserId), o => UserId = o, UsedIdIsValid, 0);

            AddAction("torrent", TorrentCommand, "text");
        }

        #endregion

        #region Private methods

        private void TorrentCommand(string text)
        {
            Engine = new ClientEngine(new EngineSettings(Path.GetDirectoryName(SaveTo), 31337));

            var torrent = Torrent.Load(text);
            Manager = new TorrentManager(torrent, SaveTo, new TorrentSettings());
            Engine.Register(Manager);

            Manager.Start();
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            Manager?.Dispose();
            Manager = null;

            Engine?.Dispose();
            Engine = null;
        }

        #endregion
    }
}
