using System.IO;
using System.Media;

namespace HomeCenter.NET.Utilities
{
    public static class MediaUtilities
    {
        public static void Play(this byte[] bytes)
        {
            using (var stream = new MemoryStream())
            using (var player = new SoundPlayer(stream))
            {
                player.Play();
            }
        }
    }
}
