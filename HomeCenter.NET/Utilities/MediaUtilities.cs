using System;
using System.IO;
using System.Media;

namespace HomeCenter.NET.Utilities
{
    public static class MediaUtilities
    {
        public static void Play(this byte[] bytes)
        {
            bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));

            using (var stream = new MemoryStream(bytes))
            using (var player = new SoundPlayer(stream))
            {
                player.Play();
            }
        }
    }
}
