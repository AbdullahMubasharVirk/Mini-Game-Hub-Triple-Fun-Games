using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MiniGameHub
{
    public partial class QuizForm : Form
    {
        private readonly ResultHistory resultHistory;

        private Panel menuPanel;
        private Panel quizPanel;

        private TextBox playerNameTextBox;
        private Label titleLabel;
        private Label playerStatsLabel;
        private Label categoryLabel;
        private Label questionNumberLabel;
        private Label questionLabel;
        private Label scoreLabel;

        private Button continueButton;
        private Button newPlayerButton;
        private Button backButton;
        private Button backToQuizMenuButton;

        private Button[] categoryButtons;
        private Button[] answerButtons;

        private List<Question> questions;
        private List<Question> selectedQuestions;

        private Dictionary<string, QuizPlayerProgress> playerProgress;

        private string playerName;
        private string selectedCategory;
        private int currentQuestionIndex;
        private int score;

        private readonly string progressFilePath = "quiz_player_progress.txt";

        public QuizForm(ResultHistory resultHistory)
        {
            this.resultHistory = resultHistory;

            questions = new List<Question>();
            selectedQuestions = new List<Question>();
            playerProgress = new Dictionary<string, QuizPlayerProgress>();

            playerName = "Player";
            selectedCategory = "Mixed";
            currentQuestionIndex = 0;
            score = 0;

            categoryButtons = new Button[4];
            answerButtons = new Button[4];

            InitializeComponent();
            LoadQuestions();
            LoadPlayerProgress();
            BuildUserInterface();
            ShowMenuPanel();
        }

        private void BuildUserInterface()
        {
            Text = "Quiz Master";
            Size = new Size(1000, 720);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(22, 28, 36);

            BuildMenuPanel();
            BuildQuizPanel();
        }

        private void BuildMenuPanel()
        {
            menuPanel = new Panel();
            menuPanel.Size = new Size(980, 680);
            menuPanel.Location = new Point(0, 0);
            menuPanel.BackColor = Color.FromArgb(22, 28, 36);
            Controls.Add(menuPanel);

            titleLabel = new Label();
            titleLabel.Text = "QUIZ MASTER";
            titleLabel.Font = new Font("Segoe UI Black", 34, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = new Size(940, 70);
            titleLabel.Location = new Point(20, 25);
            menuPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label();
            subtitleLabel.Text = "Choose a player and category. Your progress will be saved.";
            subtitleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            subtitleLabel.ForeColor = Color.LightGray;
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.Size = new Size(940, 35);
            subtitleLabel.Location = new Point(20, 95);
            menuPanel.Controls.Add(subtitleLabel);

            Label nameLabel = new Label();
            nameLabel.Text = "Player Name:";
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.ForeColor = Color.White;
            nameLabel.Size = new Size(130, 35);
            nameLabel.Location = new Point(220, 155);
            menuPanel.Controls.Add(nameLabel);

            playerNameTextBox = new TextBox();
            playerNameTextBox.Text = "Player";
            playerNameTextBox.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            playerNameTextBox.Size = new Size(250, 35);
            playerNameTextBox.Location = new Point(350, 152);
            menuPanel.Controls.Add(playerNameTextBox);

            continueButton = CreateButton(
                "Continue Player",
                Color.FromArgb(46, 204, 113),
                new Point(620, 145),
                170
            );
            continueButton.Click += ContinueButton_Click;
            menuPanel.Controls.Add(continueButton);

            newPlayerButton = CreateButton(
                "New Player / Reset",
                Color.FromArgb(241, 196, 15),
                new Point(400, 205),
                190
            );
            newPlayerButton.Click += NewPlayerButton_Click;
            menuPanel.Controls.Add(newPlayerButton);

            playerStatsLabel = new Label();
            playerStatsLabel.Text = "Enter player name and click Continue Player.";
            playerStatsLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            playerStatsLabel.ForeColor = Color.White;
            playerStatsLabel.TextAlign = ContentAlignment.MiddleCenter;
            playerStatsLabel.Size = new Size(850, 70);
            playerStatsLabel.Location = new Point(65, 270);
            menuPanel.Controls.Add(playerStatsLabel);

            Label categoryTitleLabel = new Label();
            categoryTitleLabel.Text = "Choose Category";
            categoryTitleLabel.Font = new Font("Segoe UI Black", 22, FontStyle.Bold);
            categoryTitleLabel.ForeColor = Color.White;
            categoryTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            categoryTitleLabel.Size = new Size(940, 50);
            categoryTitleLabel.Location = new Point(20, 350);
            menuPanel.Controls.Add(categoryTitleLabel);

            categoryButtons[0] = CreateCategoryButton("C# Programming", new Point(95, 430), Color.FromArgb(52, 152, 219));
            categoryButtons[1] = CreateCategoryButton("General Knowledge", new Point(300, 430), Color.FromArgb(155, 89, 182));
            categoryButtons[2] = CreateCategoryButton("Math", new Point(505, 430), Color.FromArgb(230, 126, 34));
            categoryButtons[3] = CreateCategoryButton("Mixed", new Point(710, 430), Color.FromArgb(46, 204, 113));

            for (int i = 0; i < categoryButtons.Length; i++)
            {
                categoryButtons[i].Click += CategoryButton_Click;
                menuPanel.Controls.Add(categoryButtons[i]);
            }

            backButton = CreateButton(
                "Back to Main Menu",
                Color.FromArgb(231, 76, 60),
                new Point(390, 590),
                220
            );
            backButton.Click += BackButton_Click;
            menuPanel.Controls.Add(backButton);
        }

        private void BuildQuizPanel()
        {
            quizPanel = new Panel();
            quizPanel.Size = new Size(980, 680);
            quizPanel.Location = new Point(0, 0);
            quizPanel.BackColor = Color.FromArgb(22, 28, 36);
            Controls.Add(quizPanel);

            Label quizTitleLabel = new Label();
            quizTitleLabel.Text = "QUIZ MASTER";
            quizTitleLabel.Font = new Font("Segoe UI Black", 32, FontStyle.Bold);
            quizTitleLabel.ForeColor = Color.White;
            quizTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            quizTitleLabel.Size = new Size(940, 65);
            quizTitleLabel.Location = new Point(20, 25);
            quizPanel.Controls.Add(quizTitleLabel);

            categoryLabel = new Label();
            categoryLabel.Text = "";
            categoryLabel.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            categoryLabel.ForeColor = Color.FromArgb(46, 204, 113);
            categoryLabel.TextAlign = ContentAlignment.MiddleCenter;
            categoryLabel.Size = new Size(940, 35);
            categoryLabel.Location = new Point(20, 95);
            quizPanel.Controls.Add(categoryLabel);

            questionNumberLabel = new Label();
            questionNumberLabel.Text = "";
            questionNumberLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            questionNumberLabel.ForeColor = Color.LightGray;
            questionNumberLabel.TextAlign = ContentAlignment.MiddleCenter;
            questionNumberLabel.Size = new Size(940, 30);
            questionNumberLabel.Location = new Point(20, 135);
            quizPanel.Controls.Add(questionNumberLabel);

            questionLabel = new Label();
            questionLabel.Text = "";
            questionLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            questionLabel.ForeColor = Color.White;
            questionLabel.TextAlign = ContentAlignment.MiddleCenter;
            questionLabel.Size = new Size(880, 100);
            questionLabel.Location = new Point(50, 185);
            quizPanel.Controls.Add(questionLabel);

            answerButtons[0] = CreateAnswerButton(new Point(95, 320));
            answerButtons[1] = CreateAnswerButton(new Point(505, 320));
            answerButtons[2] = CreateAnswerButton(new Point(95, 430));
            answerButtons[3] = CreateAnswerButton(new Point(505, 430));

            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].Tag = i;
                answerButtons[i].Click += AnswerButton_Click;
                quizPanel.Controls.Add(answerButtons[i]);
            }

            scoreLabel = new Label();
            scoreLabel.Text = "Score: 0";
            scoreLabel.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            scoreLabel.ForeColor = Color.White;
            scoreLabel.TextAlign = ContentAlignment.MiddleCenter;
            scoreLabel.Size = new Size(400, 35);
            scoreLabel.Location = new Point(290, 545);
            quizPanel.Controls.Add(scoreLabel);

            backToQuizMenuButton = CreateButton(
                "Quiz Menu",
                Color.FromArgb(231, 76, 60),
                new Point(390, 595),
                200
            );
            backToQuizMenuButton.Click += BackToQuizMenuButton_Click;
            quizPanel.Controls.Add(backToQuizMenuButton);
        }

        private Button CreateButton(string text, Color color, Point location, int width)
        {
            Button button = new Button();

            button.Text = text;
            button.Size = new Size(width, 55);
            button.Location = location;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            return button;
        }

        private Button CreateCategoryButton(string text, Point location, Color color)
        {
            Button button = new Button();

            button.Text = text;
            button.Size = new Size(180, 95);
            button.Location = location;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Tag = text;

            return button;
        }

        private Button CreateAnswerButton(Point location)
        {
            Button button = new Button();

            button.Size = new Size(380, 75);
            button.Location = location;
            button.BackColor = Color.FromArgb(46, 204, 113);
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.TextAlign = ContentAlignment.MiddleCenter;

            return button;
        }

        private void ShowMenuPanel()
        {
            UpdatePlayerStatsLabel();

            quizPanel.Visible = false;
            menuPanel.Visible = true;
            menuPanel.BringToFront();
        }

        private void ShowQuizPanel()
        {
            menuPanel.Visible = false;
            quizPanel.Visible = true;
            quizPanel.BringToFront();
        }

        private void ContinueButton_Click(object? sender, EventArgs e)
        {
            LoadProgressForCurrentPlayer();
        }

        private void NewPlayerButton_Click(object? sender, EventArgs e)
        {
            string enteredName = playerNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredName))
            {
                MessageBox.Show(
                    "Please enter a player name.",
                    "Missing Player Name",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DialogResult result = MessageBox.Show(
                $"Start/reset quiz progress for '{enteredName}'?\n\nThis will clear answered questions and score for this player.",
                "New Player / Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                playerName = enteredName;

                playerProgress[playerName] = new QuizPlayerProgress
                {
                    PlayerName = playerName,
                    TotalScore = 0,
                    HighScore = 0,
                    AnsweredQuestionIds = new List<int>()
                };

                SavePlayerProgress();
                UpdatePlayerStatsLabel();

                MessageBox.Show(
                    $"{playerName} is now reset for Quiz Master.",
                    "Progress Reset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void LoadProgressForCurrentPlayer()
        {
            string enteredName = playerNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredName))
            {
                MessageBox.Show(
                    "Please enter a player name.",
                    "Missing Player Name",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            playerName = enteredName;

            if (!playerProgress.ContainsKey(playerName))
            {
                playerProgress[playerName] = new QuizPlayerProgress
                {
                    PlayerName = playerName,
                    TotalScore = 0,
                    HighScore = 0,
                    AnsweredQuestionIds = new List<int>()
                };

                SavePlayerProgress();

                MessageBox.Show(
                    $"New quiz player created: {playerName}",
                    "New Player",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    $"Welcome back, {playerName}!\n\nYour quiz progress was loaded.",
                    "Progress Loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            UpdatePlayerStatsLabel();
        }

        private void CategoryButton_Click(object? sender, EventArgs e)
        {
            Button clickedButton = (Button)sender!;
            selectedCategory = clickedButton.Tag!.ToString()!;

            string enteredName = playerNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredName))
            {
                MessageBox.Show(
                    "Please enter a player name first.",
                    "Missing Player Name",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            playerName = enteredName;

            if (!playerProgress.ContainsKey(playerName))
            {
                playerProgress[playerName] = new QuizPlayerProgress
                {
                    PlayerName = playerName,
                    TotalScore = 0,
                    HighScore = 0,
                    AnsweredQuestionIds = new List<int>()
                };

                SavePlayerProgress();
            }

            StartQuiz(selectedCategory);
        }

        private void StartQuiz(string category)
        {
            QuizPlayerProgress progress = playerProgress[playerName];

            List<Question> categoryQuestions;

            if (category == "Mixed")
            {
                categoryQuestions = questions.ToList();
            }
            else
            {
                categoryQuestions = questions
                    .Where(q => q.Category == category)
                    .ToList();
            }

            selectedQuestions = categoryQuestions
                .Where(q => !progress.AnsweredQuestionIds.Contains(q.Id))
                .ToList();

            if (selectedQuestions.Count == 0)
            {
                MessageBox.Show(
                    $"No new questions left for {category}.\n\nUse New Player / Reset if you want to answer them again.",
                    "No Questions Left",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ShowMenuPanel();
                return;
            }

            currentQuestionIndex = 0;
            score = 0;

            MessageBox.Show(
                $"Starting quiz for {playerName}\n\n" +
                $"Category: {category}\n" +
                $"New questions available: {selectedQuestions.Count}",
                "Quiz Start",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ShowQuizPanel();
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            Question currentQuestion = selectedQuestions[currentQuestionIndex];

            categoryLabel.Text = $"Player: {playerName} | Category: {currentQuestion.Category} | Difficulty: {currentQuestion.Difficulty}";
            questionNumberLabel.Text = $"Question {currentQuestionIndex + 1} of {selectedQuestions.Count}";
            questionLabel.Text = currentQuestion.Text;
            scoreLabel.Text = $"Current Quiz Score: {score}";

            for (int i = 0; i < answerButtons.Length; i++)
            {
                answerButtons[i].Text = currentQuestion.Options[i];
                answerButtons[i].Tag = i;
                answerButtons[i].BackColor = Color.FromArgb(46, 204, 113);
                answerButtons[i].Enabled = true;
            }
        }

        private void AnswerButton_Click(object? sender, EventArgs e)
        {
            Button clickedButton = (Button)sender!;
            int selectedAnswerIndex = (int)clickedButton.Tag;

            Question currentQuestion = selectedQuestions[currentQuestionIndex];

            DisableAnswerButtons();

            if (selectedAnswerIndex == currentQuestion.CorrectOptionIndex)
            {
                score++;
                clickedButton.BackColor = Color.FromArgb(39, 174, 96);

                MessageBox.Show(
                    "Correct answer!",
                    "Correct",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                clickedButton.BackColor = Color.FromArgb(231, 76, 60);
                answerButtons[currentQuestion.CorrectOptionIndex].BackColor = Color.FromArgb(39, 174, 96);

                MessageBox.Show(
                    "Wrong answer!\n\nCorrect answer: " + currentQuestion.Options[currentQuestion.CorrectOptionIndex],
                    "Wrong",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            SaveAnsweredQuestion(currentQuestion.Id);

            currentQuestionIndex++;

            if (currentQuestionIndex >= selectedQuestions.Count)
            {
                FinishQuiz();
            }
            else
            {
                ShowQuestion();
            }
        }

        private void SaveAnsweredQuestion(int questionId)
        {
            QuizPlayerProgress progress = playerProgress[playerName];

            if (!progress.AnsweredQuestionIds.Contains(questionId))
            {
                progress.AnsweredQuestionIds.Add(questionId);
            }

            SavePlayerProgress();
        }

        private void DisableAnswerButtons()
        {
            foreach (Button button in answerButtons)
            {
                button.Enabled = false;
            }
        }

        private void FinishQuiz()
        {
            QuizPlayerProgress progress = playerProgress[playerName];

            progress.TotalScore += score;

            if (score > progress.HighScore)
            {
                progress.HighScore = score;
            }

            SavePlayerProgress();

            double percentage = ((double)score / selectedQuestions.Count) * 100;

            string resultText;

            if (percentage >= 80)
            {
                resultText = "Excellent";
            }
            else if (percentage >= 50)
            {
                resultText = "Good";
            }
            else
            {
                resultText = "Needs Practice";
            }

            GameResult result = new GameResult(
                "Quiz Game",
                $"{playerName} | {selectedCategory} | {resultText} - {score}/{selectedQuestions.Count}",
                score
            );

            try
            {
                resultHistory.AddResult(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Result could not be saved: " + ex.Message,
                    "File Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }

            MessageBox.Show(
                $"Quiz finished, {playerName}!\n\n" +
                $"Category: {selectedCategory}\n" +
                $"Score: {score}/{selectedQuestions.Count}\n" +
                $"Percentage: {percentage:0.00}%\n" +
                $"Result: {resultText}\n\n" +
                $"Total Saved Score: {progress.TotalScore}\n" +
                $"High Score: {progress.HighScore}",
                "Quiz Result",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ShowMenuPanel();
        }

        private void UpdatePlayerStatsLabel()
        {
            string enteredName = playerNameTextBox == null ? playerName : playerNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredName))
            {
                playerStatsLabel.Text = "Enter player name and click Continue Player.";
                return;
            }

            if (playerProgress.ContainsKey(enteredName))
            {
                QuizPlayerProgress progress = playerProgress[enteredName];

                playerStatsLabel.Text =
                    $"Player: {enteredName}\n" +
                    $"Total Saved Score: {progress.TotalScore} | High Score: {progress.HighScore} | Answered Questions: {progress.AnsweredQuestionIds.Count}/{questions.Count}";
            }
            else
            {
                playerStatsLabel.Text = $"Player '{enteredName}' has no saved quiz progress yet.";
            }
        }

        private void LoadPlayerProgress()
        {
            playerProgress.Clear();

            try
            {
                if (!File.Exists(progressFilePath))
                {
                    return;
                }

                string[] lines = File.ReadAllLines(progressFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');

                    if (parts.Length == 4)
                    {
                        string name = parts[0];

                        QuizPlayerProgress progress = new QuizPlayerProgress
                        {
                            PlayerName = name,
                            TotalScore = Convert.ToInt32(parts[1]),
                            HighScore = Convert.ToInt32(parts[2]),
                            AnsweredQuestionIds = ParseAnsweredQuestionIds(parts[3])
                        };

                        if (!playerProgress.ContainsKey(name))
                        {
                            playerProgress.Add(name, progress);
                        }
                    }
                }
            }
            catch
            {
                playerProgress.Clear();
            }
        }

        private List<int> ParseAnsweredQuestionIds(string value)
        {
            List<int> ids = new List<int>();

            if (string.IsNullOrWhiteSpace(value))
            {
                return ids;
            }

            string[] parts = value.Split(',');

            foreach (string part in parts)
            {
                if (int.TryParse(part, out int id))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        private void SavePlayerProgress()
        {
            try
            {
                List<string> lines = new List<string>();

                foreach (KeyValuePair<string, QuizPlayerProgress> item in playerProgress)
                {
                    QuizPlayerProgress progress = item.Value;

                    string answeredIds = string.Join(",", progress.AnsweredQuestionIds);

                    lines.Add($"{progress.PlayerName}|{progress.TotalScore}|{progress.HighScore}|{answeredIds}");
                }

                File.WriteAllLines(progressFilePath, lines);
            }
            catch
            {
                MessageBox.Show(
                    "Quiz progress could not be saved.",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        public int CalculateScore(List<int> userAnswers, List<Question> quizQuestions)
        {
            int calculatedScore = 0;

            for (int i = 0; i < userAnswers.Count && i < quizQuestions.Count; i++)
            {
                if (userAnswers[i] == quizQuestions[i].CorrectOptionIndex)
                {
                    calculatedScore++;
                }
            }

            return calculatedScore;
        }

        private void BackToQuizMenuButton_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Return to quiz menu? Current quiz progress will stop.",
                "Quiz Menu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ShowMenuPanel();
            }
        }

        private void BackButton_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void LoadQuestions()
        {
            questions.Add(new Question
            {
                Id = 1,
                Text = "Which keyword is used to create a class in C#?",
                Options = new List<string> { "class", "object", "method", "namespace" },
                CorrectOptionIndex = 0,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 2,
                Text = "Which method is the starting point of a C# application?",
                Options = new List<string> { "Start()", "Main()", "Run()", "Open()" },
                CorrectOptionIndex = 1,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 3,
                Text = "Which collection can store multiple items in C#?",
                Options = new List<string> { "int", "bool", "List", "char" },
                CorrectOptionIndex = 2,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 4,
                Text = "Which block is used to handle exceptions in C#?",
                Options = new List<string> { "if-else", "try-catch", "switch", "for" },
                CorrectOptionIndex = 1,
                Category = "C# Programming",
                Difficulty = "Medium"
            });

            questions.Add(new Question
            {
                Id = 5,
                Text = "What does OOP stand for?",
                Options = new List<string>
                {
                    "Object-Oriented Programming",
                    "Open Object Process",
                    "Only One Program",
                    "Object Output Program"
                },
                CorrectOptionIndex = 0,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 6,
                Text = "Which file normally starts a C# Windows Forms application?",
                Options = new List<string> { "Game.cs", "Program.cs", "Form.txt", "Start.html" },
                CorrectOptionIndex = 1,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 7,
                Text = "Which C# statement is used for decision making?",
                Options = new List<string> { "if", "print", "read", "open" },
                CorrectOptionIndex = 0,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 8,
                Text = "Which loop repeats while a condition is true?",
                Options = new List<string> { "while", "class", "using", "return" },
                CorrectOptionIndex = 0,
                Category = "C# Programming",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 9,
                Text = "What is the capital city of Poland?",
                Options = new List<string> { "Krakow", "Warsaw", "Gdansk", "Poznan" },
                CorrectOptionIndex = 1,
                Category = "General Knowledge",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 10,
                Text = "How many days are there in a leap year?",
                Options = new List<string> { "365", "366", "364", "360" },
                CorrectOptionIndex = 1,
                Category = "General Knowledge",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 11,
                Text = "Which planet is known as the Red Planet?",
                Options = new List<string> { "Earth", "Venus", "Mars", "Jupiter" },
                CorrectOptionIndex = 2,
                Category = "General Knowledge",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 12,
                Text = "Which ocean is the largest ocean on Earth?",
                Options = new List<string> { "Atlantic", "Indian", "Arctic", "Pacific" },
                CorrectOptionIndex = 3,
                Category = "General Knowledge",
                Difficulty = "Medium"
            });

            questions.Add(new Question
            {
                Id = 13,
                Text = "How many continents are there?",
                Options = new List<string> { "5", "6", "7", "8" },
                CorrectOptionIndex = 2,
                Category = "General Knowledge",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 14,
                Text = "Which gas do plants absorb from the atmosphere?",
                Options = new List<string> { "Oxygen", "Carbon Dioxide", "Hydrogen", "Nitrogen" },
                CorrectOptionIndex = 1,
                Category = "General Knowledge",
                Difficulty = "Medium"
            });

            questions.Add(new Question
            {
                Id = 15,
                Text = "What is 5 + 7?",
                Options = new List<string> { "10", "11", "12", "13" },
                CorrectOptionIndex = 2,
                Category = "Math",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 16,
                Text = "What is 9 x 6?",
                Options = new List<string> { "45", "54", "56", "63" },
                CorrectOptionIndex = 1,
                Category = "Math",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 17,
                Text = "What is the square root of 81?",
                Options = new List<string> { "7", "8", "9", "10" },
                CorrectOptionIndex = 2,
                Category = "Math",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 18,
                Text = "What is 100 divided by 4?",
                Options = new List<string> { "20", "25", "30", "40" },
                CorrectOptionIndex = 1,
                Category = "Math",
                Difficulty = "Easy"
            });

            questions.Add(new Question
            {
                Id = 19,
                Text = "What is 15 percent of 200?",
                Options = new List<string> { "15", "20", "30", "40" },
                CorrectOptionIndex = 2,
                Category = "Math",
                Difficulty = "Medium"
            });

            questions.Add(new Question
            {
                Id = 20,
                Text = "What is 12 x 12?",
                Options = new List<string> { "124", "144", "122", "132" },
                CorrectOptionIndex = 1,
                Category = "Math",
                Difficulty = "Easy"
            });
        }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
        public string Category { get; set; } = "";
        public string Difficulty { get; set; } = "";
    }

    public class QuizPlayerProgress
    {
        public string PlayerName { get; set; } = "";
        public int TotalScore { get; set; }
        public int HighScore { get; set; }
        public List<int> AnsweredQuestionIds { get; set; } = new List<int>();
    }
}