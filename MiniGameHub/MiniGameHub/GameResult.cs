using System;

namespace MiniGameHub
{
    public class GameResult
    {
        public string GameName { get; set; } = "";
        public string ResultText { get; set; } = "";
        public int Score { get; set; }
        public DateTime PlayedAt { get; set; }

        public GameResult()
        {
            PlayedAt = DateTime.Now;
        }

        public GameResult(string gameName, string resultText, int score)
        {
            GameName = gameName;
            ResultText = resultText;
            Score = score;
            PlayedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{PlayedAt:g} | {GameName} | {ResultText} | Score: {Score}";
        }
    }
}