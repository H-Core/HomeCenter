using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core.Runners;
using H.NET.Core.Settings;

namespace H.NET.Runners
{
    public class MoviesRunner : Runner
    {
        #region Properties

        private string Folder { get; set; }
        private string Folder2 { get; set; }
        private string Folder3 { get; set; }
        private string MoviesExtensions { get; set; }
        private int MaximumDistance { get; set; }

        #endregion

        #region Constructors

        public MoviesRunner()
        {
            AddSetting(nameof(Folder), o => Folder = o, NoEmpty, string.Empty, SettingType.Folder);
            AddSetting(nameof(Folder2), o => Folder2 = o, NoEmpty, string.Empty, SettingType.Folder);
            AddSetting(nameof(Folder3), o => Folder3 = o, NoEmpty, string.Empty, SettingType.Folder);
            AddSetting(nameof(MoviesExtensions), o => MoviesExtensions = o, NoEmpty, "avi;mkv;mp4");
            AddSetting(nameof(MaximumDistance), o => MaximumDistance = o, Positive, 2);

            AddAsyncAction("find-movie", FindMovieCommand, "name");
        }

        #endregion

        #region Private methods

        private async Task FindMovieCommand(string text)
        {
            if (string.IsNullOrWhiteSpace(Folder) || 
                !Directory.Exists(Folder))
            {
                await SayAsync("Директория фильмов не найдена. Пожалуйста, укажите ее и попробуйте снова");

                ShowSettings();
                return;
            }

            await SayAsync($"Ищу фильм {text}");

            var files = GetFiles(Folder, Folder2, Folder3);
            if (!files.Any())
            {
                await SayAsync("Ничего не найдено");
                await CheckTorrent(text);
                return;
            }

            var translitedGost = Transliterator.Convert(text, Transliterator.TranslateType.Gost);
            var translitedIso = Transliterator.Convert(text, Transliterator.TranslateType.Iso);

            var distances = new List<Tuple<int, string>>();
            foreach (var path in files)
            {
                distances.Add(GetDistance(path, text));
                distances.Add(GetDistance(path, translitedGost));
                distances.Add(GetDistance(path, translitedIso));
            }

            distances = distances.OrderBy(i => i.Item1).ToList();

            var minimumItem = distances.FirstOrDefault();
            var minimumDistance = minimumItem?.Item1 ?? int.MaxValue;
            if (minimumDistance > MaximumDistance)
            {
                Print($"Ближайшее совпадение: дистанция {minimumDistance} и строка {minimumItem?.Item2}");

                await SayAsync("Ничего подходящего не найдено");
                await CheckTorrent(text);
                return;
            }

            var goodDistances = distances.Where(i => Math.Abs(i.Item1 - minimumDistance) <= 2).Distinct().ToList();
            if (goodDistances.Count == 1)
            {
                await StartMovie(goodDistances[0].Item2);
                return;
            }

            foreach (var distance in goodDistances)
            {
                Print($"File {distance.Item2}. Distance: {distance.Item1}");
            }

            await StartMovie(goodDistances[0].Item2);
        }

        private async Task StartMovie(string path)
        {
            await SayAsync("Нашла. Запускаю");
            Run($"explorer {path}");
        }

        private async Task CheckTorrent(string text)
        {
            await SayAsync("Скачать с торрента?");

            if (!await WaitAccept(3000, "скачай", "скачать"))
            {
                return;
            }

            await RunAsync($"torrent {text}");
        }

        private static Tuple<int, string> GetDistance(string path, string text)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            if (name == null)
            {
                return new Tuple<int, string>(int.MaxValue, path);
            }

            var distance = text.Length < name.Length
                ? TextUtilities.MinimalLevenshteinDistance(name, text)
                : TextUtilities.LevenshteinDistance(name, text);

            return new Tuple<int, string>(distance, path);
        }

        private bool IsGoodExtension(string path) => MoviesExtensions
            .Split(';')
            .Any(extension => string.Equals(Path.GetExtension(path), $".{extension}", StringComparison.OrdinalIgnoreCase));

        private List<string> GetFiles(params string[] folders) => folders
            .Where(folder => !string.IsNullOrWhiteSpace(folder) && Directory.Exists(folder))
            .SelectMany(folder => Directory
                .EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(IsGoodExtension))
            .ToList();

        #endregion
    }
}
