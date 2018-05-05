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

        #endregion

        #region Constructors

        public MoviesRunner()
        {
            AddSetting(nameof(Folder), o => Folder = o, NoEmpty, string.Empty, SettingType.Folder);

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

            var files = Directory
                .EnumerateFiles(Folder, "*.*", SearchOption.AllDirectories)
                .Select(Path.GetFileNameWithoutExtension);

            var translitedGost = Transliterator.Convert(text, Transliterator.TranslateType.Gost);
            var translitedIso = Transliterator.Convert(text, Transliterator.TranslateType.Iso);
            Print($"TranslitedGost: {translitedGost}");
            Print($"TranslitedIso: {translitedIso}");
            foreach (var file in files)
            {
                Print($"File {file}. " +
                      $"Distance: {TextUtilities.LevenshteinDistance(file, text)}. " +
                      $"DistanceTGost: {TextUtilities.LevenshteinDistance(file, translitedGost)}. " +
                      $"DistanceTIso: {TextUtilities.LevenshteinDistance(file, translitedIso)}");
            }
        }

        #endregion
    }
}
