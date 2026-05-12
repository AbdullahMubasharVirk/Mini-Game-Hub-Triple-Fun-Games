using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MiniGameHub
{
    public partial class TicTacToeForm : Form
    {
        private readonly ResultHistory resultHistory;

        private Button[,] boardButtons;
        private char[,] board;

        private int boardSize;
        private int winLength;
        private char currentPlayer;
        private int moves;
        private bool gameOver;
        private bool vsComputer;

        private int playerXWins;
        private int playerOWins;
        private int computerWins;
        private int draws;

        private Label titleLabel;
        private Label statusLabel;
        private Label statsLabel;

        private ComboBox boardSizeComboBox;
        private ComboBox modeComboBox;

        private Panel boardPanel;
        private ListBox matchHistoryListBox;

        private Button newMatchButton;
        private Button backButton;

        private readonly Random random = new Random();

        public TicTacToeForm(ResultHistory resultHistory)
        {
            this.resultHistory = resultHistory;

            boardSize = 3;
            winLength = 3;
            currentPlayer = 'X';
            moves = 0;
            gameOver = false;
            vsComputer = false;

            boardButtons = new Button[boardSize, boardSize];
            board = new char[boardSize, boardSize];

            InitializeComponent();
            BuildUserInterface();
            StartNewMatch();
        }

        private void BuildUserInterface()
        {
            Text = "Tic-Tac-Toe";
            Size = new Size(1450, 950);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(24, 24, 38);

            titleLabel = new Label();
            titleLabel.Text = "TIC-TAC-TOE";
            titleLabel.Font = new Font("Segoe UI Black", 30, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = new Size(1180, 60);
            titleLabel.Location = new Point(20, 20);
            Controls.Add(titleLabel);

            Label boardSizeLabel = CreateSmallLabel("Board:", new Point(60, 105));
            Controls.Add(boardSizeLabel);

            boardSizeComboBox = new ComboBox();
            boardSizeComboBox.Size = new Size(170, 35);
            boardSizeComboBox.Location = new Point(130, 102);
            boardSizeComboBox.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            boardSizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            boardSizeComboBox.Items.Add("3 x 3");
            boardSizeComboBox.Items.Add("5 x 5");
            boardSizeComboBox.SelectedIndex = 0;
            boardSizeComboBox.SelectedIndexChanged += BoardSizeComboBox_SelectedIndexChanged;
            Controls.Add(boardSizeComboBox);

            Label modeLabel = CreateSmallLabel("Mode:", new Point(360, 105));
            Controls.Add(modeLabel);

            modeComboBox = new ComboBox();
            modeComboBox.Size = new Size(260, 35);
            modeComboBox.Location = new Point(430, 102);
            modeComboBox.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            modeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            modeComboBox.Items.Add("Player vs Player");
            modeComboBox.Items.Add("Player vs Computer");
            modeComboBox.SelectedIndex = 0;
            modeComboBox.SelectedIndexChanged += ModeComboBox_SelectedIndexChanged;
            Controls.Add(modeComboBox);

            statusLabel = new Label();
            statusLabel.Text = "Player X's turn";
            statusLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            statusLabel.ForeColor = Color.White;
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Size = new Size(620, 40);
            statusLabel.Location = new Point(40, 150);
            Controls.Add(statusLabel);

            boardPanel = new Panel();
            boardPanel.Size = new Size(620, 620);
            boardPanel.Location = new Point(40, 190);
            boardPanel.BackColor = Color.FromArgb(35, 35, 55);
            Controls.Add(boardPanel);

            Panel sidePanel = new Panel();
            sidePanel.Size = new Size(500, 700);
            sidePanel.Location = new Point(800, 190);
            sidePanel.BackColor = Color.FromArgb(30, 30, 48);
            Controls.Add(sidePanel);

            Label statsTitleLabel = new Label();
            statsTitleLabel.Text = "Session Statistics";
            statsTitleLabel.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            statsTitleLabel.ForeColor = Color.White;
            statsTitleLabel.Size = new Size(430, 35);
            statsTitleLabel.Location = new Point(30, 20);
            sidePanel.Controls.Add(statsTitleLabel);

            statsLabel = new Label();
            statsLabel.Text = "";
            statsLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            statsLabel.ForeColor = Color.White;
            statsLabel.Size = new Size(430, 200);
            statsLabel.Location = new Point(30, 65);
            statsLabel.TextAlign = ContentAlignment.TopLeft;
            sidePanel.Controls.Add(statsLabel);

            Label historyTitle = new Label();
            historyTitle.Text = "Match History";
            historyTitle.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            historyTitle.ForeColor = Color.White;
            historyTitle.Size = new Size(430, 35);
            historyTitle.Location = new Point(30, 300);
            sidePanel.Controls.Add(historyTitle);

            matchHistoryListBox = new ListBox();
            matchHistoryListBox.Size = new Size(430, 210);
            matchHistoryListBox.Location = new Point(30, 350);
            matchHistoryListBox.BackColor = Color.FromArgb(40, 40, 60);
            matchHistoryListBox.ForeColor = Color.White;
            matchHistoryListBox.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            matchHistoryListBox.BorderStyle = BorderStyle.None;
            sidePanel.Controls.Add(matchHistoryListBox);

            newMatchButton = CreateButton(
                "New Match",
                Color.FromArgb(52, 152, 219),
                new Point(30, 600)
            );
            newMatchButton.Click += NewMatchButton_Click;
            sidePanel.Controls.Add(newMatchButton);

            backButton = CreateButton(
                "Back to Menu",
                Color.FromArgb(231, 76, 60),
                new Point(250, 600)
            );
            backButton.Click += BackButton_Click;
            sidePanel.Controls.Add(backButton);
        }

        private Label CreateSmallLabel(string text, Point location)
        {
            Label label = new Label();
            label.Text = text;
            label.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            label.ForeColor = Color.White;
            label.Size = new Size(70, 30);
            label.Location = location;
            return label;
        }

        private Button CreateButton(string text, Color color, Point location)
        {
            Button button = new Button();
            button.Text = text;
            button.Size = new Size(180, 55);
            button.Location = location;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            return button;
        }

        private void StartNewMatch()
        {
            board = new char[boardSize, boardSize];
            boardButtons = new Button[boardSize, boardSize];

            currentPlayer = 'X';
            moves = 0;
            gameOver = false;

            statusLabel.Text = "Player X's turn";
            boardPanel.Controls.Clear();

            int gap = 8;
            int buttonSize = (boardPanel.Width - gap * (boardSize + 1)) / boardSize;

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    Button button = new Button();

                    button.Size = new Size(buttonSize, buttonSize);
                    button.Location = new Point(
                        gap + col * (buttonSize + gap),
                        gap + row * (buttonSize + gap)
                    );

                    button.Font = new Font(
                        "Segoe UI Black",
                        boardSize == 3 ? 34 : 22,
                        FontStyle.Bold
                    );

                    button.BackColor = Color.FromArgb(45, 45, 70);
                    button.ForeColor = Color.White;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderSize = 0;
                    button.Cursor = Cursors.Hand;
                    button.Tag = new Point(row, col);
                    button.Click += BoardButton_Click;

                    boardButtons[row, col] = button;
                    boardPanel.Controls.Add(button);
                }
            }

            UpdateStatsLabel();
        }

        private void BoardButton_Click(object? sender, EventArgs e)
        {
            if (gameOver)
            {
                return;
            }

            Button clickedButton = (Button)sender!;
            Point position = (Point)clickedButton.Tag!;

            int row = position.X;
            int col = position.Y;

            if (!IsValidMove(row, col))
            {
                MessageBox.Show(
                    "This cell is already taken.",
                    "Invalid Move",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            MakeMove(row, col, currentPlayer);

            if (CheckGameEnd())
            {
                return;
            }

            SwitchPlayer();

            if (vsComputer && currentPlayer == 'O' && !gameOver)
            {
                statusLabel.Text = "Computer is thinking...";
                Application.DoEvents();

                ComputerMove();

                if (CheckGameEnd())
                {
                    return;
                }

                SwitchPlayer();
            }

            statusLabel.Text = GetTurnText();
        }

        private void MakeMove(int row, int col, char player)
        {
            board[row, col] = player;
            boardButtons[row, col].Text = player.ToString();

            if (player == 'X')
            {
                boardButtons[row, col].ForeColor = Color.FromArgb(52, 152, 219);
            }
            else
            {
                boardButtons[row, col].ForeColor = Color.FromArgb(241, 196, 15);
            }

            moves++;
        }

        private bool IsValidMove(int row, int col)
        {
            return board[row, col] == '\0';
        }

        private void SwitchPlayer()
        {
            currentPlayer = currentPlayer == 'X' ? 'O' : 'X';
        }

        private string GetTurnText()
        {
            if (vsComputer && currentPlayer == 'O')
            {
                return "Computer's turn";
            }

            return $"Player {currentPlayer}'s turn";
        }

        private void ComputerMove()
        {
            Point move;

            if (TryFindWinningMove('O', out move))
            {
                MakeMove(move.X, move.Y, 'O');
                return;
            }

            if (TryFindWinningMove('X', out move))
            {
                MakeMove(move.X, move.Y, 'O');
                return;
            }

            List<Point> emptyCells = GetEmptyCells();

            if (emptyCells.Count == 0)
            {
                return;
            }

            Point randomMove = emptyCells[random.Next(emptyCells.Count)];
            MakeMove(randomMove.X, randomMove.Y, 'O');
        }

        private bool TryFindWinningMove(char player, out Point winningMove)
        {
            List<Point> emptyCells = GetEmptyCells();

            foreach (Point cell in emptyCells)
            {
                board[cell.X, cell.Y] = player;

                bool wins = CheckWinner(player);

                board[cell.X, cell.Y] = '\0';

                if (wins)
                {
                    winningMove = cell;
                    return true;
                }
            }

            winningMove = new Point(-1, -1);
            return false;
        }

        private List<Point> GetEmptyCells()
        {
            List<Point> emptyCells = new List<Point>();

            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (board[row, col] == '\0')
                    {
                        emptyCells.Add(new Point(row, col));
                    }
                }
            }

            return emptyCells;
        }

        private bool CheckGameEnd()
        {
            if (CheckWinner(currentPlayer))
            {
                string message;

                if (vsComputer && currentPlayer == 'O')
                {
                    computerWins++;
                    message = "Computer wins!";
                }
                else
                {
                    if (currentPlayer == 'X')
                    {
                        playerXWins++;
                    }
                    else
                    {
                        playerOWins++;
                    }

                    message = $"Player {currentPlayer} wins!";
                }

                EndGame(message, 10);
                return true;
            }

            if (IsDraw())
            {
                draws++;
                EndGame("The game ended in a draw!", 5);
                return true;
            }

            return false;
        }

        public bool CheckWinner(char player)
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    if (CheckDirection(row, col, 0, 1, player) ||
                        CheckDirection(row, col, 1, 0, player) ||
                        CheckDirection(row, col, 1, 1, player) ||
                        CheckDirection(row, col, 1, -1, player))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckDirection(int startRow, int startCol, int rowDirection, int colDirection, char player)
        {
            for (int i = 0; i < winLength; i++)
            {
                int row = startRow + i * rowDirection;
                int col = startCol + i * colDirection;

                if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
                {
                    return false;
                }

                if (board[row, col] != player)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsDraw()
        {
            return moves == boardSize * boardSize;
        }

        private void EndGame(string message, int score)
        {
            gameOver = true;
            statusLabel.Text = message;

            DisableBoard();

            string modeText = vsComputer ? "Player vs Computer" : "Player vs Player";
            string boardText = $"{boardSize}x{boardSize}";
            string resultText = $"{message} | {boardText} | {modeText}";

            GameResult result = new GameResult(
                "Tic-Tac-Toe",
                resultText,
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

            matchHistoryListBox.Items.Insert(0, $"{DateTime.Now:t} - {resultText}");
            UpdateStatsLabel();

            MessageBox.Show(
                message + "\n\nResult saved to history.",
                "Match Finished",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void DisableBoard()
        {
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    boardButtons[row, col].Enabled = false;
                }
            }
        }

        private void UpdateStatsLabel()
        {
            statsLabel.Text =
                $"Player X wins: {playerXWins}\n" +
                $"Player O wins: {playerOWins}\n" +
                $"Computer wins: {computerWins}\n" +
                $"Draws: {draws}\n\n" +
                $"Current board: {boardSize}x{boardSize}\n" +
                $"Win condition: {winLength} in a row\n\n" +
                $"Mode: {(vsComputer ? "Player vs Computer" : "Player vs Player")}";
        }

        private void BoardSizeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (boardSizeComboBox.SelectedIndex == 0)
            {
                boardSize = 3;
                winLength = 3;
            }
            else
            {
                boardSize = 5;
                winLength = 4;
            }

            StartNewMatch();
        }

        private void ModeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
        {
            vsComputer = modeComboBox.SelectedIndex == 1;
            StartNewMatch();
        }

        private void NewMatchButton_Click(object? sender, EventArgs e)
        {
            StartNewMatch();
        }

        private void BackButton_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void TicTacToeForm_Load(object sender, EventArgs e)
        {

        }
    }
}