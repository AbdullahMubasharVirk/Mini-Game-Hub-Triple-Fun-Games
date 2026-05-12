using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MiniGameHub
{
    public partial class ResultHistoryForm : Form
    {
        private readonly ResultHistory resultHistory;

        private Label titleLabel;
        private ListBox historyListBox;
        private Button refreshButton;
        private Button clearButton;
        private Button backButton;

        public ResultHistoryForm(ResultHistory resultHistory)
        {
            this.resultHistory = resultHistory;

            InitializeComponent();
            BuildUserInterface();
            LoadHistory();
        }

        private void BuildUserInterface()
        {
            Text = "Result History";
            Size = new Size(800, 650);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(20, 25, 35);

            titleLabel = new Label();
            titleLabel.Text = "RESULT HISTORY";
            titleLabel.Font = new Font("Segoe UI", 30, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = new Size(750, 70);
            titleLabel.Location = new Point(20, 25);
            Controls.Add(titleLabel);

            historyListBox = new ListBox();
            historyListBox.Size = new Size(680, 370);
            historyListBox.Location = new Point(55, 120);
            historyListBox.BackColor = Color.FromArgb(35, 40, 55);
            historyListBox.ForeColor = Color.White;
            historyListBox.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            historyListBox.BorderStyle = BorderStyle.None;
            Controls.Add(historyListBox);

            refreshButton = CreateButton(
                "Refresh",
                Color.FromArgb(52, 152, 219),
                new Point(75, 530)
            );
            refreshButton.Click += RefreshButton_Click;
            Controls.Add(refreshButton);

            clearButton = CreateButton(
                "Clear History",
                Color.FromArgb(241, 196, 15),
                new Point(305, 530)
            );
            clearButton.Click += ClearButton_Click;
            Controls.Add(clearButton);

            backButton = CreateButton(
                "Back to Menu",
                Color.FromArgb(231, 76, 60),
                new Point(535, 530)
            );
            backButton.Click += BackButton_Click;
            Controls.Add(backButton);
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

        private void LoadHistory()
        {
            historyListBox.Items.Clear();

            List<GameResult> results = resultHistory.GetResults();

            if (results.Count == 0)
            {
                historyListBox.Items.Add("No results available yet.");
                return;
            }

            for (int i = 0; i < results.Count; i++)
            {
                GameResult result = results[i];

                string line =
                    $"{i + 1}. {result.PlayedAt:g}   |   {result.GameName}   |   {result.ResultText}   |   Score: {result.Score}";

                historyListBox.Items.Add(line);
            }
        }

        private void RefreshButton_Click(object? sender, EventArgs e)
        {
            LoadHistory();
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Are you sure you want to clear all result history?",
                "Clear History",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    resultHistory.ClearHistory();
                    LoadHistory();

                    MessageBox.Show(
                        "Result history cleared successfully.",
                        "History Cleared",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Could not clear result history: " + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void BackButton_Click(object? sender, EventArgs e)
        {
            Close();
        }
    }
}
