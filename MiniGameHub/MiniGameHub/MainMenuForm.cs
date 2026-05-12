using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MiniGameHub
{
    public partial class MainMenuForm : Form
    {
        private readonly ResultHistory resultHistory;

        public MainMenuForm()
        {
            resultHistory = new ResultHistory();

            InitializeComponent();
            BuildUserInterface();

            MusicManager.StartMusic();

        }

        private void MusicButton_Click(object? sender, EventArgs e)
        {
            MusicManager.ToggleMute();
        }

        private void BuildUserInterface()
        {
            Text = "Mini Game Hub";
            Size = new Size(1400, 850);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(18, 28, 55);
            DoubleBuffered = true;

            Paint += MainMenuForm_Paint;

            Label appBadgeLabel = new Label();
            appBadgeLabel.Text = "MINI GAME HUB";
            appBadgeLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            appBadgeLabel.ForeColor = Color.FromArgb(255, 230, 120);
            appBadgeLabel.BackColor = Color.Transparent;
            appBadgeLabel.TextAlign = ContentAlignment.MiddleCenter;
            appBadgeLabel.Size = new Size(300, 35);
            appBadgeLabel.Location = new Point(550, 25);
            Controls.Add(appBadgeLabel);

            Label titleShadowLabel = new Label();
            titleShadowLabel.Text = "TRIPLE FUN GAMES";
            titleShadowLabel.Font = new Font("Segoe UI Black", 46, FontStyle.Bold);
            titleShadowLabel.ForeColor = Color.FromArgb(40, 45, 95);
            titleShadowLabel.BackColor = Color.Transparent;
            titleShadowLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleShadowLabel.Size = new Size(1300, 90);
            titleShadowLabel.Location = new Point(55, 65);
            Controls.Add(titleShadowLabel);

            Label titleLabel = new Label();
            titleLabel.Text = "TRIPLE FUN GAMES";
            titleLabel.Font = new Font("Segoe UI Black", 46, FontStyle.Bold);
            titleLabel.ForeColor = Color.White;
            titleLabel.BackColor = Color.Transparent;
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            titleLabel.Size = new Size(1300, 90);
            titleLabel.Location = new Point(50, 58);
            Controls.Add(titleLabel);

            Label subtitleLabel = new Label();
            subtitleLabel.Text = "Choose your challenge and start playing";
            subtitleLabel.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            subtitleLabel.ForeColor = Color.FromArgb(220, 230, 255);
            subtitleLabel.BackColor = Color.Transparent;
            subtitleLabel.TextAlign = ContentAlignment.MiddleCenter;
            subtitleLabel.Size = new Size(1300, 40);
            subtitleLabel.Location = new Point(50, 145);
            Controls.Add(subtitleLabel);

            GameCard quizCard = new GameCard(
                "❓",
                "QUIZ MASTER",
                "Answer questions,\ncollect points,\nand test your brain.",
                "PLAY QUIZ",
                Color.FromArgb(55, 220, 130),
                Color.FromArgb(15, 155, 95)
            );
            quizCard.Location = new Point(115, 240);
            quizCard.Click += QuizCard_Click;
            Controls.Add(quizCard);

            GameCard memoryCard = new GameCard(
                "🃏",
                "CARD MEMORY",
                "Unlock levels,\nmatch pairs,\nand beat your time.",
                "PLAY MEMORY",
                Color.FromArgb(255, 205, 55),
                Color.FromArgb(240, 125, 35)
            );
            memoryCard.Location = new Point(515, 240);
            memoryCard.Click += MemoryCard_Click;
            Controls.Add(memoryCard);

            GameCard ticTacToeCard = new GameCard(
                "✕○",
                "TIC-TAC-TOE",
                "Choose board size,\nplay with a friend,\nor beat the computer.",
                "PLAY TIC-TAC",
                Color.FromArgb(190, 95, 245),
                Color.FromArgb(105, 70, 220)
            );
            ticTacToeCard.Location = new Point(915, 240);
            ticTacToeCard.Click += TicTacToeCard_Click;
            Controls.Add(ticTacToeCard);

            Button historyButton = CreateBottomButton(
                "🏆  Result History",
                Color.FromArgb(52, 152, 219),
                new Point(300, 735),
                250
            );
            historyButton.Click += HistoryButton_Click;
            Controls.Add(historyButton);

            Button exitButton = CreateBottomButton(
                "Exit",
                Color.FromArgb(231, 76, 60),
                new Point(600, 735),
                250
            );
            exitButton.Click += ExitButton_Click;
            Controls.Add(exitButton);

            Button musicButton = CreateBottomButton(
                "🔊 On / Off",
                Color.FromArgb(155, 89, 182),
                new Point(900, 735),
                250
                );
            musicButton.Click += MusicButton_Click;
            Controls.Add(musicButton);
        }

        private Button CreateBottomButton(string text, Color color, Point location, int width)
        {
            Button button = new Button();

            button.Text = text;
            button.Size = new Size(width, 60);
            button.Location = location;
            button.BackColor = color;
            button.ForeColor = Color.White;
            button.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;

            button.MouseEnter += (sender, e) =>
            {
                button.BackColor = ControlPaint.Light(color);
            };

            button.MouseLeave += (sender, e) =>
            {
                button.BackColor = color;
            };

            return button;
        }

        private void MainMenuForm_Paint(object? sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (LinearGradientBrush backgroundBrush = new LinearGradientBrush(
                ClientRectangle,
                Color.FromArgb(20, 150, 230),
                Color.FromArgb(20, 45, 105),
                LinearGradientMode.Vertical))
            {
                graphics.FillRectangle(backgroundBrush, ClientRectangle);
            }

            DrawSoftCircle(graphics, new Rectangle(-120, -100, 360, 360), Color.FromArgb(90, 255, 190, 70));
            DrawSoftCircle(graphics, new Rectangle(1120, -120, 340, 340), Color.FromArgb(90, 180, 90, 245));
            DrawSoftCircle(graphics, new Rectangle(1080, 620, 360, 360), Color.FromArgb(80, 255, 175, 60));
            DrawSoftCircle(graphics, new Rectangle(-140, 600, 330, 330), Color.FromArgb(70, 80, 220, 255));

            DrawSmallStar(graphics, 120, 130, 22);
            DrawSmallStar(graphics, 310, 205, 14);
            DrawSmallStar(graphics, 1080, 135, 18);
            DrawSmallStar(graphics, 1245, 210, 24);
            DrawSmallStar(graphics, 215, 690, 16);
            DrawSmallStar(graphics, 1170, 700, 17);

            DrawBubble(graphics, 100, 245, 24);
            DrawBubble(graphics, 1230, 315, 30);
            DrawBubble(graphics, 1195, 110, 18);
            DrawBubble(graphics, 75, 545, 16);
            DrawBubble(graphics, 670, 205, 14);
            DrawBubble(graphics, 765, 690, 16);
        }

        private void DrawSoftCircle(Graphics graphics, Rectangle rectangle, Color color)
        {
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillEllipse(brush, rectangle);
            }
        }

        private void DrawBubble(Graphics graphics, int x, int y, int size)
        {
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(55, Color.White)))
            {
                graphics.FillEllipse(brush, x, y, size, size);
            }

            using (Pen pen = new Pen(Color.FromArgb(120, Color.White), 2))
            {
                graphics.DrawEllipse(pen, x, y, size, size);
            }
        }

        private void DrawSmallStar(Graphics graphics, int x, int y, int size)
        {
            Point[] points =
            {
                new Point(x, y - size),
                new Point(x + size / 3, y - size / 3),
                new Point(x + size, y - size / 4),
                new Point(x + size / 2, y + size / 4),
                new Point(x + size / 2, y + size),
                new Point(x, y + size / 2),
                new Point(x - size / 2, y + size),
                new Point(x - size / 2, y + size / 4),
                new Point(x - size, y - size / 4),
                new Point(x - size / 3, y - size / 3)
            };

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(255, 235, 95)))
            {
                graphics.FillPolygon(brush, points);
            }

            using (Pen pen = new Pen(Color.White, 2))
            {
                graphics.DrawPolygon(pen, points);
            }
        }

        private void QuizCard_Click(object? sender, EventArgs e)
        {
            QuizForm quizForm = new QuizForm(resultHistory);
            quizForm.ShowDialog();
        }

        private void MemoryCard_Click(object? sender, EventArgs e)
        {
            MemoryGameForm memoryGameForm = new MemoryGameForm(resultHistory);
            memoryGameForm.ShowDialog();
        }

        private void TicTacToeCard_Click(object? sender, EventArgs e)
        {
            TicTacToeForm ticTacToeForm = new TicTacToeForm(resultHistory);
            ticTacToeForm.ShowDialog();
        }

        private void HistoryButton_Click(object? sender, EventArgs e)
        {
            ResultHistoryForm resultHistoryForm = new ResultHistoryForm(resultHistory);
            resultHistoryForm.ShowDialog();
        }

        private void ExitButton_Click(object? sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Do you want to exit Mini Game Hub?",
                "Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {

        }

        private void MainMenuForm_Load_1(object sender, EventArgs e)
        {

        }
    }

    public class GameCard : Panel
    {
        private readonly string icon;
        private readonly string title;
        private readonly string description;
        private readonly string buttonText;
        private readonly Color topColor;
        private readonly Color bottomColor;

        private bool isHovering;

        public GameCard(
            string icon,
            string title,
            string description,
            string buttonText,
            Color topColor,
            Color bottomColor)
        {
            this.icon = icon;
            this.title = title;
            this.description = description;
            this.buttonText = buttonText;
            this.topColor = topColor;
            this.bottomColor = bottomColor;

            Size = new Size(340, 430);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
            BackColor = Color.Transparent;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovering = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovering = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle shadowRectangle = new Rectangle(22, 25, Width - 44, Height - 35);
            Rectangle cardRectangle = isHovering
                ? new Rectangle(10, 6, Width - 20, Height - 22)
                : new Rectangle(14, 14, Width - 28, Height - 28);

            using (GraphicsPath shadowPath = GetRoundedRectanglePath(shadowRectangle, 32))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(85, Color.Black)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }

            using (GraphicsPath cardPath = GetRoundedRectanglePath(cardRectangle, 32))
            using (LinearGradientBrush brush = new LinearGradientBrush(
                cardRectangle,
                isHovering ? ControlPaint.Light(topColor) : topColor,
                bottomColor,
                LinearGradientMode.Vertical))
            {
                graphics.FillPath(brush, cardPath);

                using (Pen borderPen = new Pen(Color.White, 5))
                {
                    graphics.DrawPath(borderPen, cardPath);
                }
            }

            Rectangle iconCircle = new Rectangle((Width - 115) / 2, 45, 115, 115);

            using (SolidBrush iconBackgroundBrush = new SolidBrush(Color.FromArgb(65, Color.White)))
            {
                graphics.FillEllipse(iconBackgroundBrush, iconCircle);
            }

            using (Pen iconBorderPen = new Pen(Color.FromArgb(180, Color.White), 4))
            {
                graphics.DrawEllipse(iconBorderPen, iconCircle);
            }

            using (Font iconFont = new Font("Segoe UI Emoji", 50, FontStyle.Bold))
            using (SolidBrush iconBrush = new SolidBrush(Color.White))
            {
                StringFormat iconFormat = new StringFormat();
                iconFormat.Alignment = StringAlignment.Center;
                iconFormat.LineAlignment = StringAlignment.Center;

                graphics.DrawString(icon, iconFont, iconBrush, iconCircle, iconFormat);
            }

            using (Font titleFont = new Font("Segoe UI Black", 23, FontStyle.Bold))
            using (SolidBrush titleShadowBrush = new SolidBrush(Color.FromArgb(95, Color.Black)))
            using (SolidBrush titleBrush = new SolidBrush(Color.White))
            {
                StringFormat titleFormat = new StringFormat();
                titleFormat.Alignment = StringAlignment.Center;

                Rectangle titleShadowRectangle = new Rectangle(3, 175, Width, 45);
                Rectangle titleRectangle = new Rectangle(0, 170, Width, 45);

                graphics.DrawString(title, titleFont, titleShadowBrush, titleShadowRectangle, titleFormat);
                graphics.DrawString(title, titleFont, titleBrush, titleRectangle, titleFormat);
            }

            using (Font descriptionFont = new Font("Segoe UI", 14, FontStyle.Bold))
            using (SolidBrush descriptionBrush = new SolidBrush(Color.FromArgb(245, 250, 255)))
            {
                StringFormat descriptionFormat = new StringFormat();
                descriptionFormat.Alignment = StringAlignment.Center;
                descriptionFormat.LineAlignment = StringAlignment.Center;

                Rectangle descriptionRectangle = new Rectangle(35, 235, Width - 70, 95);
                graphics.DrawString(description, descriptionFont, descriptionBrush, descriptionRectangle, descriptionFormat);
            }

            Rectangle playButtonRectangle = new Rectangle(70, Height - 90, Width - 140, 58);

            using (GraphicsPath buttonPath = GetRoundedRectanglePath(playButtonRectangle, 28))
            using (LinearGradientBrush playBrush = new LinearGradientBrush(
                playButtonRectangle,
                Color.White,
                Color.FromArgb(230, 235, 245),
                LinearGradientMode.Vertical))
            {
                graphics.FillPath(playBrush, buttonPath);

                using (Pen buttonBorder = new Pen(Color.FromArgb(90, Color.Black), 3))
                {
                    graphics.DrawPath(buttonBorder, buttonPath);
                }
            }

            using (Font playFont = new Font("Segoe UI Black", 16, FontStyle.Bold))
            using (SolidBrush playTextBrush = new SolidBrush(Color.FromArgb(35, 45, 65)))
            {
                StringFormat playFormat = new StringFormat();
                playFormat.Alignment = StringAlignment.Center;
                playFormat.LineAlignment = StringAlignment.Center;

                graphics.DrawString(buttonText, playFont, playTextBrush, playButtonRectangle, playFormat);
            }
        }

        private GraphicsPath GetRoundedRectanglePath(Rectangle rectangle, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(rectangle.X, rectangle.Y, diameter, diameter, 180, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Y, diameter, diameter, 270, 90);
            path.AddArc(rectangle.Right - diameter, rectangle.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rectangle.X, rectangle.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}