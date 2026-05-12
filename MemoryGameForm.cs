using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MiniGameHub
{
    public partial class MemoryGameForm : Form
    {
        private readonly ResultHistory resultHistory;

        private Panel levelMenuPanel;
        private Panel gamePanel;
        private Panel boardPanel;
        private Panel sidePanel;

        private TextBox playerNameTextBox;

        private Label titleLabel;
        private Label gameTitleLabel;
        private Label levelLabel;
        private Label playerLabel;
        private Label movesLabel;
        private Label timerLabel;
        private Label instructionLabel;

        private ListBox leaderboardListBox;

        private Button[] levelButtons;
        private Button[] cardButtons;

        private Button backToMenuButton;
        private Button restartLevelButton;
        private Button exitButton;

        private List<MemoryCard> cards;

        private int currentLevel;
        private int unlockedLevel;
        private int rows;
        private int columns;
        private int moves;
        private int firstSelectedIndex;
        private int secondSelectedIndex;

        private bool isChecking;

        private string playerName;

        private readonly string progressFilePath = "memory_player_progress.txt";
        private Dictionary<string, int> playerProgress;

        private Stopwatch stopwatch;
        private System.Windows.Forms.Timer gameTimer;

        public MemoryGameForm(ResultHistory resultHistory)
        {
            this.resultHistory = resultHistory;

            currentLevel = 1;
            unlockedLevel = 1;
            rows = 2;
            columns = 4;
            moves = 0;
            firstSelectedIndex = -1;
            secondSelectedIndex = -1;
            isChecking = false;
            playerName = "Player";

            cards = new List<MemoryCard>();
            levelButtons = new Button[10];
            cardButtons = Array.Empty<Button>();
            playerProgress = new Dictionary<string, int>();

            stopwatch = new Stopwatch();
            gameTimer = new System.Windows.Forms.Timer();

            InitializeComponent();
            LoadPlayerProgress();
            BuildUserInterface();
            ShowLevelMenu();
        }

        private void BuildUserInterface()
        {
            Text = "Card Memory";
            Size = new Size(1250, 860);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(24, 24, 38);

            BuildLevelMenuPanel();
            BuildGamePanel();

            gameTimer.Interval = 1000;
            gameTimer.Tick += GameTimer_Tick;
        }

        private void BuildLevelMenuPanel()
        {
            levelMenuPanel = new Panel();
            levelMenuPanel.Size = new Size(1230, 820);
            levelMenuPanel.Location = new Point(0, 0);
            levelMenuPanel.BackColor = Color.FromArgb(30, 27, 40);
            Controls.Add(levelMenuPanel);

            titleLabel = new Label();
            titleLabel.Text = "CARD MEMORY ROADMAP";
            titleLabel.Font = new Font("Segoe UI Black", 32, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = new Size(1150, 65);
            titleLabel.Location = new Point(40, 20);
            levelMenuPanel.Controls.Add(titleLabel);

            Label subtitleLabel = new Label();
            subtitleLabel.Text = "Complete each level to unlock the next challenge";
            subtitleLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            subtitleLabel.ForeColor = Color.LightGray;
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.Size = new Size(1150, 35);
            subtitleLabel.Location = new Point(40, 85);
            levelMenuPanel.Controls.Add(subtitleLabel);

            Label nameLabel = new Label();
            nameLabel.Text = "Player Name:";
            nameLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            nameLabel.ForeColor = Color.White;
            nameLabel.Size = new Size(130, 35);
            nameLabel.Location = new Point(280, 135);
            levelMenuPanel.Controls.Add(nameLabel);

            playerNameTextBox = new TextBox();
            playerNameTextBox.Text = "Player";
            playerNameTextBox.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            playerNameTextBox.Size = new Size(230, 35);
            playerNameTextBox.Location = new Point(410, 132);
            levelMenuPanel.Controls.Add(playerNameTextBox);

            Button continueButton = CreateNormalButton(
                "Continue Player",
                Color.FromArgb(46, 204, 113),
                new Point(660, 125),
                170
            );
            continueButton.Click += ContinueButton_Click;
            levelMenuPanel.Controls.Add(continueButton);

            Button newPlayerButton = CreateNormalButton(
                "New Player / Reset",
                Color.FromArgb(241, 196, 15),
                new Point(845, 125),
                190
            );
            newPlayerButton.Click += NewPlayerButton_Click;
            levelMenuPanel.Controls.Add(newPlayerButton);

            CreateRoadMapLevels();

            exitButton = CreateNormalButton(
                "Back to Main Menu",
                Color.FromArgb(231, 76, 60),
                new Point(500, 735),
                230
            );
            exitButton.Click += ExitButton_Click;
            levelMenuPanel.Controls.Add(exitButton);
        }

        private void CreateRoadMapLevels()
        {
            Point[] levelPositions =
            {
                new Point(120, 260),
                new Point(300, 210),
                new Point(480, 270),
                new Point(660, 220),
                new Point(840, 280),
                new Point(920, 450),
                new Point(720, 520),
                new Point(520, 460),
                new Point(320, 540),
                new Point(130, 470)
            };

            for (int i = 0; i < 10; i++)
            {
                int levelNumber = i + 1;

                Button levelButton = new Button();
                levelButton.Size = new Size(145, 95);
                levelButton.Location = levelPositions[i];
                levelButton.Tag = levelNumber;
                levelButton.Font = new Font("Segoe UI Black", 10, FontStyle.Bold);
                levelButton.FlatStyle = FlatStyle.Flat;
                levelButton.FlatAppearance.BorderSize = 3;
                levelButton.FlatAppearance.BorderColor = Color.White;
                levelButton.Cursor = Cursors.Hand;
                levelButton.Click += LevelButton_Click;

                levelButtons[i] = levelButton;
                levelMenuPanel.Controls.Add(levelButton);

                if (i < 9)
                {
                    Label arrowLabel = new Label();
                    arrowLabel.Text = GetArrowSymbol(levelPositions[i], levelPositions[i + 1]);
                    arrowLabel.Font = new Font("Segoe UI Black", 28, FontStyle.Bold);
                    arrowLabel.ForeColor = Color.White;
                    arrowLabel.BackColor = Color.Transparent;
                    arrowLabel.Size = new Size(70, 55);
                    arrowLabel.Location = GetArrowPosition(levelPositions[i], levelPositions[i + 1]);
                    arrowLabel.TextAlign = ContentAlignment.MiddleCenter;

                    levelMenuPanel.Controls.Add(arrowLabel);
                    arrowLabel.BringToFront();
                }
            }

            UpdateLevelButtons();
        }

        private Point GetArrowPosition(Point first, Point second)
        {
            int x = (first.X + second.X) / 2 + 45;
            int y = (first.Y + second.Y) / 2 + 25;

            return new Point(x, y);
        }

        private string GetArrowSymbol(Point first, Point second)
        {
            int dx = second.X - first.X;
            int dy = second.Y - first.Y;

            if (dx > 0 && dy > 20)
            {
                return "↘";
            }

            if (dx > 0 && dy < -20)
            {
                return "↗";
            }

            if (dx < 0 && dy > 20)
            {
                return "↙";
            }

            if (dx < 0 && dy < -20)
            {
                return "↖";
            }

            if (dx > 0)
            {
                return "→";
            }

            if (dx < 0)
            {
                return "←";
            }

            if (dy > 0)
            {
                return "↓";
            }

            return "↑";
        }

        private void UpdateLevelButtons()
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int level = i + 1;
                Button button = levelButtons[i];

                if (level <= unlockedLevel)
                {
                    button.Text = $"LEVEL {level}\n{GetLevelName(level)}";
                    button.BackColor = GetLevelColor(level);
                    button.ForeColor = Color.White;
                    button.Enabled = true;
                }
                else
                {
                    button.Text = $"LEVEL {level}\nLOCKED 🔒";
                    button.BackColor = Color.FromArgb(90, 90, 100);
                    button.ForeColor = Color.LightGray;
                    button.Enabled = true;
                }
            }
        }

        private void BuildGamePanel()
        {
            gamePanel = new Panel();
            gamePanel.Size = new Size(1230, 820);
            gamePanel.Location = new Point(0, 0);
            gamePanel.BackColor = Color.FromArgb(30, 27, 40);
            Controls.Add(gamePanel);

            gameTitleLabel = new Label();
            gameTitleLabel.Text = "CARD MEMORY";
            gameTitleLabel.Font = new Font("Segoe UI Black", 30, FontStyle.Bold);
            gameTitleLabel.ForeColor = Color.White;
            gameTitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            gameTitleLabel.Size = new Size(1180, 60);
            gameTitleLabel.Location = new Point(25, 20);
            gamePanel.Controls.Add(gameTitleLabel);

            instructionLabel = new Label();
            instructionLabel.Text = "Find all matching pairs.";
            instructionLabel.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            instructionLabel.ForeColor = Color.LightGray;
            instructionLabel.TextAlign = ContentAlignment.MiddleCenter;
            instructionLabel.Size = new Size(900, 35);
            instructionLabel.Location = new Point(40, 90);
            gamePanel.Controls.Add(instructionLabel);

            levelLabel = CreateGameInfoLabel("Level 1", new Point(50, 145), Color.FromArgb(241, 196, 15), 250);
            playerLabel = CreateGameInfoLabel("Player: Player", new Point(310, 145), Color.White, 240);
            movesLabel = CreateGameInfoLabel("Moves: 0", new Point(560, 145), Color.White, 160);
            timerLabel = CreateGameInfoLabel("Time: 00:00", new Point(730, 145), Color.White, 180);

            gamePanel.Controls.Add(levelLabel);
            gamePanel.Controls.Add(playerLabel);
            gamePanel.Controls.Add(movesLabel);
            gamePanel.Controls.Add(timerLabel);

            boardPanel = new Panel();
            boardPanel.Size = new Size(700, 560);
            boardPanel.Location = new Point(50, 210);
            boardPanel.BackColor = Color.FromArgb(42, 38, 58);
            gamePanel.Controls.Add(boardPanel);

            sidePanel = new Panel();
            sidePanel.Size = new Size(400, 560);
            sidePanel.Location = new Point(790, 210);
            sidePanel.BackColor = Color.FromArgb(38, 34, 52);
            gamePanel.Controls.Add(sidePanel);

            Label leaderboardTitle = new Label();
            leaderboardTitle.Text = "Leaderboard";
            leaderboardTitle.Font = new Font("Segoe UI Black", 20, FontStyle.Bold);
            leaderboardTitle.ForeColor = Color.White;
            leaderboardTitle.TextAlign = ContentAlignment.MiddleCenter;
            leaderboardTitle.Size = new Size(350, 45);
            leaderboardTitle.Location = new Point(25, 20);
            sidePanel.Controls.Add(leaderboardTitle);

            leaderboardListBox = new ListBox();
            leaderboardListBox.Size = new Size(350, 315);
            leaderboardListBox.Location = new Point(25, 85);
            leaderboardListBox.BackColor = Color.FromArgb(50, 45, 68);
            leaderboardListBox.ForeColor = Color.White;
            leaderboardListBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            leaderboardListBox.BorderStyle = BorderStyle.None;
            sidePanel.Controls.Add(leaderboardListBox);

            restartLevelButton = CreateNormalButton(
                "Restart Level",
                Color.FromArgb(52, 152, 219),
                new Point(25, 435),
                160
            );
            restartLevelButton.Click += RestartLevelButton_Click;
            sidePanel.Controls.Add(restartLevelButton);

            backToMenuButton = CreateNormalButton(
                "Roadmap",
                Color.FromArgb(231, 76, 60),
                new Point(210, 435),
                160
            );
            backToMenuButton.Click += BackToMenuButton_Click;
            sidePanel.Controls.Add(backToMenuButton);
        }

        private Label CreateGameInfoLabel(string text, Point location, Color color, int width)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            label.ForeColor = color;
            label.Size = new Size(width, 35);
            label.Location = location;

            return label;
        }

        private Button CreateNormalButton(string text, Color color, Point location, int width)
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

        private void ShowLevelMenu()
        {
            gameTimer.Stop();
            stopwatch.Stop();

            UpdateLevelButtons();
            LoadLeaderboard();

            gamePanel.Visible = false;
            levelMenuPanel.Visible = true;
            levelMenuPanel.BringToFront();
        }

        private void ShowGameScreen()
        {
            levelMenuPanel.Visible = false;
            gamePanel.Visible = true;
            gamePanel.BringToFront();
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
                $"Start/reset progress for player '{enteredName}'?\n\nThis will set this player back to Level 1.",
                "New Player / Reset",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                playerName = enteredName;
                unlockedLevel = 1;
                currentLevel = 1;

                if (playerProgress.ContainsKey(playerName))
                {
                    playerProgress[playerName] = 1;
                }
                else
                {
                    playerProgress.Add(playerName, 1);
                }

                SavePlayerProgress();
                UpdateLevelButtons();

                MessageBox.Show(
                    $"{playerName} is now set to Level 1.",
                    "Progress Reset",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void LevelButton_Click(object? sender, EventArgs e)
        {
            Button clickedButton = (Button)sender!;
            int selectedLevel = (int)clickedButton.Tag!;

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

            if (playerProgress.ContainsKey(playerName))
            {
                unlockedLevel = playerProgress[playerName];
            }
            else
            {
                unlockedLevel = 1;
                playerProgress[playerName] = unlockedLevel;
                SavePlayerProgress();
            }

            if (selectedLevel > unlockedLevel)
            {
                MessageBox.Show(
                    $"Level {selectedLevel} is locked for {playerName}.\nComplete Level {unlockedLevel} first.",
                    "Level Locked",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                UpdateLevelButtons();
                return;
            }

            currentLevel = selectedLevel;
            SetBoardSizeForLevel();

            MessageBox.Show(
                $"LEVEL {currentLevel}: {GetLevelName(currentLevel)}\n\n" +
                $"Board: {rows} x {columns}\n" +
                $"Theme: {GetLevelName(currentLevel)}\n\n" +
                "Match all pairs to complete this level.",
                "Level Start",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ShowGameScreen();
            SetupGame();
        }

        private void SetBoardSizeForLevel()
        {
            if (currentLevel <= 2)
            {
                rows = 2;
                columns = 4;
            }
            else if (currentLevel <= 6)
            {
                rows = 4;
                columns = 4;
            }
            else
            {
                rows = 4;
                columns = 5;
            }
        }

        private void SetupGame()
        {
            cards.Clear();
            boardPanel.Controls.Clear();

            int totalCards = rows * columns;
            cardButtons = new Button[totalCards];

            List<string> values = GenerateCardValues(totalCards);
            Random random = new Random();

            values = values.OrderBy(value => random.Next()).ToList();

            for (int i = 0; i < values.Count; i++)
            {
                cards.Add(new MemoryCard
                {
                    Id = i,
                    Value = values[i],
                    IsFlipped = false,
                    IsMatched = false
                });
            }

            int gap = 10;
            int buttonWidth = (boardPanel.Width - gap * (columns + 1)) / columns;
            int buttonHeight = (boardPanel.Height - gap * (rows + 1)) / rows;

            for (int i = 0; i < totalCards; i++)
            {
                int row = i / columns;
                int col = i % columns;

                Button button = new Button();

                button.Size = new Size(buttonWidth, buttonHeight);
                button.Location = new Point(
                    gap + col * (buttonWidth + gap),
                    gap + row * (buttonHeight + gap)
                );

                button.Text = "?";
                button.Font = new Font("Segoe UI Emoji", rows == 2 ? 30 : 24, FontStyle.Bold);
                button.BackColor = Color.FromArgb(241, 196, 15);
                button.ForeColor = Color.White;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderSize = 0;
                button.Cursor = Cursors.Hand;
                button.Tag = i;
                button.Click += CardButton_Click;

                cardButtons[i] = button;
                boardPanel.Controls.Add(button);
            }

            firstSelectedIndex = -1;
            secondSelectedIndex = -1;
            moves = 0;
            isChecking = false;

            stopwatch.Reset();
            stopwatch.Start();
            gameTimer.Start();

            instructionLabel.Text = $"Match all {GetLevelName(currentLevel)} pairs.";
            levelLabel.Text = $"Level {currentLevel}: {GetLevelName(currentLevel)}";
            playerLabel.Text = $"Player: {playerName}";

            UpdateLabels();
            LoadLeaderboard();
        }

        private List<string> GenerateCardValues(int totalCards)
        {
            List<string> symbols = GetSymbolsForLevel(currentLevel);
            List<string> values = new List<string>();

            int pairs = totalCards / 2;

            for (int i = 0; i < pairs; i++)
            {
                values.Add(symbols[i]);
                values.Add(symbols[i]);
            }

            return values;
        }

        private List<string> GetSymbolsForLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

                case 2:
                    return new List<string> { "🗼", "🗽", "🏰", "⛩️", "🕌", "🏛️", "🌉", "🎡", "🛕", "🌍" };

                case 3:
                    return new List<string> { "▲", "■", "●", "◆", "★", "♥", "♣", "♠", "☀", "☂" };

                case 4:
                    return new List<string> { "🍕", "🍔", "🍟", "🌮", "🍩", "🍎", "🍓", "🍉", "🍪", "🍦" };

                case 5:
                    return new List<string> { "🐱", "🐶", "🦁", "🐼", "🐸", "🐵", "🦊", "🐯", "🐨", "🐮" };

                case 6:
                    return new List<string> { "⚽", "🏀", "🏈", "🎾", "🏐", "🏓", "🥊", "🏏", "⛳", "🏆" };

                case 7:
                    return new List<string> { "🎵", "🎸", "🎹", "🥁", "🎤", "🎧", "🎻", "🎺", "📀", "🔊" };

                case 8:
                    return new List<string> { "☀️", "🌙", "⭐", "☁️", "🌧️", "⛈️", "❄️", "🌈", "🔥", "💧" };

                case 9:
                    return new List<string> { "🚗", "🚕", "🚌", "🚓", "🚑", "🚲", "✈️", "🚀", "🚢", "🚆" };

                default:
                    return new List<string> { "A", "🗼", "▲", "🍕", "🐱", "⚽", "🎵", "☀️", "🚗", "⭐" };
            }
        }

        private string GetLevelName(int level)
        {
            switch (level)
            {
                case 1:
                    return "Alphabets";

                case 2:
                    return "Landmarks";

                case 3:
                    return "Shapes";

                case 4:
                    return "Food";

                case 5:
                    return "Animals";

                case 6:
                    return "Sports";

                case 7:
                    return "Music";

                case 8:
                    return "Weather";

                case 9:
                    return "Vehicles";

                case 10:
                    return "Mixed Challenge";

                default:
                    return "Unknown";
            }
        }

        private Color GetLevelColor(int level)
        {
            Color[] colors =
            {
                Color.FromArgb(46, 204, 113),
                Color.FromArgb(52, 152, 219),
                Color.FromArgb(155, 89, 182),
                Color.FromArgb(230, 126, 34),
                Color.FromArgb(241, 196, 15),
                Color.FromArgb(26, 188, 156),
                Color.FromArgb(231, 76, 60),
                Color.FromArgb(41, 128, 185),
                Color.FromArgb(142, 68, 173),
                Color.FromArgb(243, 156, 18)
            };

            return colors[level - 1];
        }

        private void CardButton_Click(object? sender, EventArgs e)
        {
            if (isChecking || cards.Count == 0)
            {
                return;
            }

            Button clickedButton = (Button)sender!;
            int selectedIndex = (int)clickedButton.Tag!;

            if (cards[selectedIndex].IsMatched || cards[selectedIndex].IsFlipped)
            {
                return;
            }

            FlipCard(selectedIndex);

            if (firstSelectedIndex == -1)
            {
                firstSelectedIndex = selectedIndex;
                return;
            }

            secondSelectedIndex = selectedIndex;
            moves++;
            UpdateLabels();

            CheckSelectedCards();
        }

        private void FlipCard(int index)
        {
            cards[index].IsFlipped = true;
            cardButtons[index].Text = cards[index].Value;
            cardButtons[index].BackColor = Color.White;
            cardButtons[index].ForeColor = Color.Black;
        }

        private void HideCard(int index)
        {
            cards[index].IsFlipped = false;
            cardButtons[index].Text = "?";
            cardButtons[index].BackColor = Color.FromArgb(241, 196, 15);
            cardButtons[index].ForeColor = Color.White;
        }

        private async void CheckSelectedCards()
        {
            isChecking = true;

            await System.Threading.Tasks.Task.Delay(650);

            if (IsMatch(firstSelectedIndex, secondSelectedIndex))
            {
                cards[firstSelectedIndex].IsMatched = true;
                cards[secondSelectedIndex].IsMatched = true;

                cardButtons[firstSelectedIndex].BackColor = Color.FromArgb(46, 204, 113);
                cardButtons[secondSelectedIndex].BackColor = Color.FromArgb(46, 204, 113);

                cardButtons[firstSelectedIndex].Enabled = false;
                cardButtons[secondSelectedIndex].Enabled = false;
            }
            else
            {
                HideCard(firstSelectedIndex);
                HideCard(secondSelectedIndex);
            }

            firstSelectedIndex = -1;
            secondSelectedIndex = -1;
            isChecking = false;

            if (AllCardsMatched())
            {
                FinishLevel();
            }
        }

        public bool IsMatch(int firstIndex, int secondIndex)
        {
            if (firstIndex < 0 || secondIndex < 0)
            {
                return false;
            }

            return cards[firstIndex].Value == cards[secondIndex].Value;
        }

        public bool AllCardsMatched()
        {
            foreach (MemoryCard card in cards)
            {
                if (!card.IsMatched)
                {
                    return false;
                }
            }

            return true;
        }

        private void FinishLevel()
        {
            stopwatch.Stop();
            gameTimer.Stop();

            int totalSeconds = (int)stopwatch.Elapsed.TotalSeconds;
            int score = CalculateScore(moves, totalSeconds, rows * columns, currentLevel);

            string boardText = $"{rows}x{columns}";
            string levelText = $"Level {currentLevel}: {GetLevelName(currentLevel)}";

            GameResult result = new GameResult(
                "Card Memory",
                $"{playerName} completed {levelText} ({boardText}) in {moves} moves and {stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}",
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

            if (currentLevel == unlockedLevel && unlockedLevel < 10)
            {
                unlockedLevel++;

                if (playerProgress.ContainsKey(playerName))
                {
                    playerProgress[playerName] = unlockedLevel;
                }
                else
                {
                    playerProgress.Add(playerName, unlockedLevel);
                }

                SavePlayerProgress();

                MessageBox.Show(
                    $"Congratulations, {playerName}!\n\n" +
                    $"You completed {levelText}.\n" +
                    $"Score: {score}\n\n" +
                    $"Level {unlockedLevel}: {GetLevelName(unlockedLevel)} unlocked!\n\n" +
                    "Returning to roadmap.",
                    "Level Completed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else if (currentLevel == 10)
            {
                if (playerProgress.ContainsKey(playerName))
                {
                    playerProgress[playerName] = 10;
                }
                else
                {
                    playerProgress.Add(playerName, 10);
                }

                SavePlayerProgress();

                MessageBox.Show(
                    $"Amazing, {playerName}!\n\n" +
                    "You completed all 10 levels!\n\n" +
                    $"Score: {score}\n\n" +
                    "Returning to roadmap.",
                    "All Levels Completed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                MessageBox.Show(
                    $"Congratulations, {playerName}!\n\n" +
                    $"You completed {levelText}.\n" +
                    $"Score: {score}\n\n" +
                    "Returning to roadmap.",
                    "Level Completed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            ShowLevelMenu();
        }

        public int CalculateScore(int totalMoves, int totalSeconds, int totalCards, int level)
        {
            int score = 100 + totalCards * 5 + level * 10;

            score -= totalMoves * 2;
            score -= totalSeconds;

            if (score < 0)
            {
                score = 0;
            }

            return score;
        }

        private void LoadLeaderboard()
        {
            if (leaderboardListBox == null)
            {
                return;
            }

            leaderboardListBox.Items.Clear();

            List<GameResult> memoryResults = resultHistory
                .GetResults()
                .Where(result => result.GameName == "Card Memory")
                .OrderByDescending(result => result.Score)
                .Take(10)
                .ToList();

            if (memoryResults.Count == 0)
            {
                leaderboardListBox.Items.Add("No memory game results yet.");
                return;
            }

            for (int i = 0; i < memoryResults.Count; i++)
            {
                GameResult result = memoryResults[i];

                leaderboardListBox.Items.Add(
                    $"{i + 1}. Score {result.Score} - {result.ResultText}"
                );
            }
        }

        private void LoadPlayerProgress()
        {
            playerProgress.Clear();

            try
            {
                if (!System.IO.File.Exists(progressFilePath))
                {
                    return;
                }

                string[] lines = System.IO.File.ReadAllLines(progressFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('|');

                    if (parts.Length == 2)
                    {
                        string name = parts[0];
                        int savedLevel = Convert.ToInt32(parts[1]);

                        if (!playerProgress.ContainsKey(name))
                        {
                            playerProgress.Add(name, savedLevel);
                        }
                    }
                }
            }
            catch
            {
                playerProgress.Clear();
            }
        }

        private void SavePlayerProgress()
        {
            try
            {
                List<string> lines = new List<string>();

                foreach (KeyValuePair<string, int> progress in playerProgress)
                {
                    lines.Add($"{progress.Key}|{progress.Value}");
                }

                System.IO.File.WriteAllLines(progressFilePath, lines);
            }
            catch
            {
                MessageBox.Show(
                    "Progress could not be saved.",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
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

            if (playerProgress.ContainsKey(playerName))
            {
                unlockedLevel = playerProgress[playerName];

                MessageBox.Show(
                    $"Welcome back, {playerName}!\n\n" +
                    $"Your progress was loaded.\n" +
                    $"Unlocked Level: {unlockedLevel}",
                    "Progress Loaded",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            else
            {
                unlockedLevel = 1;
                playerProgress[playerName] = unlockedLevel;
                SavePlayerProgress();

                MessageBox.Show(
                    $"New player created: {playerName}\n\nStarting from Level 1.",
                    "New Player",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            currentLevel = unlockedLevel;
            UpdateLevelButtons();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            UpdateLabels();
        }

        private void UpdateLabels()
        {
            levelLabel.Text = $"Level {currentLevel}: {GetLevelName(currentLevel)}";
            playerLabel.Text = $"Player: {playerName}";
            movesLabel.Text = $"Moves: {moves}";
            timerLabel.Text = $"Time: {stopwatch.Elapsed.Minutes:D2}:{stopwatch.Elapsed.Seconds:D2}";
        }

        private void RestartLevelButton_Click(object? sender, EventArgs e)
        {
            SetupGame();
        }

        private void BackToMenuButton_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Return to roadmap? Current level progress will be lost.",
                "Return to Roadmap",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                ShowLevelMenu();
            }
        }

        private void ExitButton_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }

    public class MemoryCard
    {
        public int Id { get; set; }
        public string Value { get; set; } = "";
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
    }
}