using System;
using System.Collections.Generic;
using System.IO;

namespace MiniGameHub
{
    public class ResultHistory
    {
        private readonly List<GameResult> results;
        private readonly string filePath = "result_history.txt";

        public ResultHistory()
        {
            results = new List<GameResult>();
            LoadResultsFromFile();
        }

        public void AddResult(GameResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result), "Result cannot be null.");
            }

            results.Add(result);
            SaveResultToFile(result);
        }

        public List<GameResult> GetResults()
        {
            return results;
        }

        public void ClearHistory()
        {
            results.Clear();

            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (IOException)
            {
                throw new IOException("Could not clear the result history file.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("No permission to clear the result history file.");
            }
        }

        private void SaveResultToFile(GameResult result)
        {
            try
            {
                string line = $"{result.GameName}|{result.ResultText}|{result.Score}|{result.PlayedAt}";
                File.AppendAllText(filePath, line + Environment.NewLine);
            }
            catch (IOException)
            {
                throw new IOException("Could not save result to file.");
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException("No permission to save result file.");
            }
        }

        private void LoadResultsFromFile()
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return;
                }

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');

                    if (parts.Length == 4)
                    {
                        GameResult result = new GameResult
                        {
                            GameName = parts[0],
                            ResultText = parts[1],
                            Score = Convert.ToInt32(parts[2]),
                            PlayedAt = Convert.ToDateTime(parts[3])
                        };

                        results.Add(result);
                    }
                }
            }
            catch (FormatException)
            {
                // If the file has invalid data, ignore it so the app does not crash.
            }
            catch (IOException)
            {
                // If the file cannot be read, the app can still run with empty history.
            }
            catch (UnauthorizedAccessException)
            {
                // If there is no permission, the app can still run with empty history.
            }
        }
    }
}