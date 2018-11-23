using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace HomeCenter.NET.Utilities
{
    public static class MediaUtilities
    {
        public static void Play(this byte[] bytes)
        {
            bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0)
            {
                return;
            }


            using (var stream = new MemoryStream(bytes))
            using (var player = new SoundPlayer(stream))
            {
                player.PlaySync();
            }
        }

        public static async Task PlayAsync(this byte[] bytes) => await Task.Run(() => bytes?.Play());
    }
}
