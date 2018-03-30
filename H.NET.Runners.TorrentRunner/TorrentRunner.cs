using System.Collections.Generic;
using System.IO;
using System.Net;
using H.NET.Core.Managers;
using H.NET.Core.Runners;
using H.NET.Core.Settings;
using MonoTorrent;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;

namespace H.NET.Runners.TorrentRunner
{
    public class TorrentRunner : Runner
    {
        #region Properties

        private string SaveTo { get; set; }

        private ClientEngine Engine { get; set; }
        private List<TorrentManager> Managers { get; set; } = new List<TorrentManager>();
        private string TorrentsFolder => Path.Combine(SaveTo, "Torrents");
        private string DownloadsFolder => Path.Combine(SaveTo, "Downloads");

        #endregion

        #region Constructors

        public TorrentRunner()
        {
            AddSetting(nameof(SaveTo), o => SaveTo = o, NoEmpty, string.Empty, SettingType.Folder);
            //AddSetting(nameof(UserId), o => UserId = o, UsedIdIsValid, 0);

            AddAction("torrent", TorrentCommand, "text");

            Settings.PropertyChanged += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(SaveTo))
                {
                    return;
                }

                Directory.CreateDirectory(TorrentsFolder);
                Directory.CreateDirectory(DownloadsFolder);
            };
        }

        #endregion

        #region Private methods

        private ClientEngine CreateEngine()
        {
            // Torrents will be downloaded here by default when they are registered with the engine
            // Tell the engine to listen at port 6969 for incoming connections
            // If both encrypted and unencrypted connections are supported, an encrypted connection will be attempted
            // first if this is true. Otherwise an unencrypted connection will be attempted first.
            return new ClientEngine(new EngineSettings(TorrentsFolder, 6969)
            {
                AllowedEncryption = EncryptionTypes.All,
                PreferEncryption = true
            });
        }

        private void TorrentCommand(string text)
        {
            /*
            Engine?.Dispose();
            Engine = CreateEngine();

            var torrent = Torrent.Load(text);
            Print($"Created by: {torrent.CreatedBy}");
            Print($"Creation date: {torrent.CreationDate}");
            Print($"Comment: {torrent.Comment}");
            Print($"Publish URL: {torrent.PublisherUrl}");
            Print($"Size: {torrent.Size}");

            var manager = new TorrentManager(torrent, DownloadsFolder, new TorrentSettings(10, 10), SaveTo);
            Engine.Register(manager);

            manager.TorrentStateChanged += (sender, args) => Print($"New state: {args.NewState:G}");

            //manager.Start();

            Managers.Add(manager);

            Engine.StartAll();*/

            /* Generate the paths to the folder we will save .torrent files to and where we download files to */
            main.basePath = SaveTo;						// This is the directory we are currently in
            main.torrentsPath = Path.Combine(main.basePath, "Torrents");				// This is the directory we will save .torrents to
            main.downloadsPath = Path.Combine(main.basePath, "Downloads");			// This is the directory we will save downloads to
            main.fastResumeFile = Path.Combine(main.torrentsPath, "fastresume.data");
            main.dhtNodeFile = Path.Combine(main.basePath, "DhtNodes");
            main.torrents = new List<TorrentManager>();							// This is where we will store the torrentmanagers
            main.listener = new Top10Listener(10);

            // We need to cleanup correctly when the user closes the window by using ctrl-c
            // or an unhandled exception happens
            //Console.CancelKeyPress += delegate { shutdown(); };
            //AppDomain.CurrentDomain.ProcessExit += delegate { shutdown(); };
            //AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { Print(e.ExceptionObject.ToString()); shutdown(); };
            //Thread.GetDomain().UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e) { Print(e.ExceptionObject.ToString()); shutdown(); };

            main.PrintAction = Log;
            main.StartEngine(31337);
            //main.Main(SaveTo, 6969, Log);
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {
            base.Dispose();

            foreach (var manager in Managers)
            {
                manager?.Dispose();
            }
            Managers.Clear();

            Engine?.Dispose();
            Engine = null;
        }

        #endregion
    }
}
