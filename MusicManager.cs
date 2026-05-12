using System;
using System.IO;
using System.Media;
using System.Windows.Forms;

namespace MiniGameHub
{
    public static class MusicManager
    {
        private static SoundPlayer? player;
        private static bool isMuted = false;

        public static void StartMusic()
        {
            if (isMuted)
            {
                return;
            }

            try
            {
                string musicPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "background_music.wav"
                );

                if (!File.Exists(musicPath))
                {
                    MessageBox.Show(
                        "Music file not found: background_music.wav",
                        "Music Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                player = new SoundPlayer(musicPath);
                player.PlayLooping();
            }
            catch
            {
                MessageBox.Show(
                    "Music could not be played.",
                    "Music Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        public static void StopMusic()
        {
            try
            {
                player?.Stop();
            }
            catch
            {
              
            }
        }

        public static void ToggleMute()
        {
            isMuted = !isMuted;

            if (isMuted)
            {
                StopMusic();
            }
            else
            {
                StartMusic();
            }
        }

        public static bool IsMuted()
        {
            return isMuted;
        }
    }
}