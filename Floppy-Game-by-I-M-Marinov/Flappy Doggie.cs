using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Floppy_Game_by_I_M_Marinov
{

    public partial class Form1 : Form
    {
        private int initialObstacleBottomX;
        private int initialObstacleTopX;
        private int initialObstacleBottom2X;
        private int initialObstacleTop2X;
        private int initialDoggieY;


        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.KeyPreview = true;
            gameOverLabel.Visible = false;
            retryButton.Visible = false;
            levelNumber.Visible = false;
            quitButton.Visible = false;
            saveScoresLabel.Visible = false;
            nameLabel.Visible = false;
            scoresTextBox.Visible = false;
            submitScoresButton.Visible = false;
            resetAllScoresButton.Visible = false;
            statusTextLabel.Visible = false;
            statusTextLabel.Text = "";

            /* save the initial positions of the doggie and obstacles */
            initialObstacleBottomX = obstacleBottom.Left;
            initialObstacleTopX = obstacleTop.Left;
            initialObstacleBottom2X = obstacleBottom2.Left;
            initialObstacleTop2X = obstacleTop2.Left;
            initialDoggieY = doggie.Top;

        }

        // public variables
        int obstacleSpeed = 3; // movement speed of the obstacles
        int level = 1;
        int gravity = 3; // movement of doggie 
        int score = 0; // scores ... ofc
        private bool speedIncreasedAlready = false;
        int lastCheckedScore = 0;
        readonly DateTime date = DateTime.Now;
        static readonly string path = "HighScores.txt";


        private void timer_Tick(object sender, EventArgs e)
        {
            doggie.Top += gravity;

            MoveObstacle(obstacleBottom);
            MoveObstacle(obstacleTop);
            MoveObstacle(obstacleBottom2);
            MoveObstacle(obstacleTop2);

            scoreText.Text = score.ToString();
            levelNumber.Text = level.ToString();

            CheckAndResetObstacle(obstacleBottom);
            CheckAndResetObstacle(obstacleTop);
            CheckAndResetObstacle(obstacleBottom2);
            CheckAndResetObstacle(obstacleTop2);

            if (CheckCollision())
            {
                gameOver();
            }

            if (score % 20 == 0 && score > 0 && !speedIncreasedAlready)
            {
                IncreaseGameSpeed(score);
            }
            else if (score % 20 != 0)
            {
                speedIncreasedAlready = false;
            }
        }

        private void IncreaseGameSpeed(int score)
        {
            if (score % 20 == 0 && score > lastCheckedScore)
            {
                obstacleSpeed += 1;
                lastCheckedScore = score;
                level++;
            }
        }

        private void MoveObstacle(PictureBox obstacle)
        {
            obstacle.Left -= obstacleSpeed;
        }

        private bool IsOffScreen(PictureBox obstacle)
        {
            return obstacle.Left < -obstacle.Width;
        }

        private void ResetObstaclePosition(PictureBox obstacle)
        {
            obstacle.Left = 950;
        }

        private void CheckAndResetObstacle(PictureBox obstacle)
        {
            if (IsOffScreen(obstacle))
            {
                ResetObstaclePosition(obstacle);
                score++;
            }
        }

        private bool CheckCollision()
        {
            return doggie.Bounds.IntersectsWith(obstacleBottom.Bounds) || doggie.Bounds.IntersectsWith(obstacleBottom2.Bounds)
                || doggie.Bounds.IntersectsWith(obstacleTop.Bounds) || doggie.Bounds.IntersectsWith(obstacleTop2.Bounds) || doggie.Bounds.IntersectsWith(grass.Bounds) || doggie.Top < -15;
        }

        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                gravity = 3;
            }
        }

        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                gravity = -3;
            }
        }

        private void gameOver()
        {
            timer.Stop();
            gameOverLabel.Text = "Game Over !";
            gameOverLabel.Visible = true;
            retryButton.Visible = true;
            quitButton.Visible = true;
            HideAllObstacles();
            HighScoresShowAndHide();

        }

        private void StartGame()
        {
            timer.Start();
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            StartGame();
            startButton.Visible = false;
            levelNumber.Visible = true;
            Focus(); // Ensure the form has focus to capture key events
        }

        private void retryButton_Click(object sender, EventArgs e)
        {
            retryGame();
        }

        private void retryGame()
        {
            score = 0;
            gravity = 3;
            obstacleSpeed = 3;
            /* call the doggy and obstacles's initial positions */
            ShowAllObstacles();
            doggie.Top = initialDoggieY;
            obstacleBottom.Left = initialObstacleBottomX;
            obstacleTop.Left = initialObstacleTopX;
            obstacleBottom2.Left = initialObstacleBottom2X;
            obstacleTop2.Left = initialObstacleTop2X;
            gameOverLabel.Visible = false;
            retryButton.Visible = false; // Hide the retry button before restarting the game
            levelNumber.Visible = true;
            quitButton.Visible = false;
            HighScoresShowAndHide();
            statusTextLabel.Text = "";
            timer.Start();
        }

        private void quitButton_Click(object sender, EventArgs e)
        {
            Environment.Exit(500);
        }

        private void SaveScore(string name, int score, int level, DateTime date)
        {
            string formattedScore = $"Name: {name} ----- Score: {score} ----- Level: {level} ----- Saved on: {date}";

            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(formattedScore);
            }
        }

        private void DeleteScores()
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                statusTextLabel.Text = "All scores deleted successfully !";
            }
            else
            {
                statusTextLabel.Text = "There are no scores saved as of now !";
            }
        }

        private void submitScoresButton_Click(object sender, EventArgs e)
        {
            string playerName = scoresTextBox.Text;
            if (playerName == "")
            {
                statusTextLabel.Text = "You need to write a name in to save your score! ";
            }
            else
            {
                SaveScore(playerName, score, level, date);
                scoresTextBox.Text = "";
                statusTextLabel.Text = $"{playerName} your score has been saved successfully !";
            }
        }

        private void HighScoresShowAndHide()
        {
            if (!saveScoresLabel.Visible && !nameLabel.Visible  &&
                !scoresTextBox.Visible && !submitScoresButton.Visible && 
                !resetAllScoresButton.Visible && !statusTextLabel.Visible )
            {
                saveScoresLabel.Visible = true;
                nameLabel.Visible = true;
                scoresTextBox.Visible = true;
                submitScoresButton.Visible = true;
                resetAllScoresButton.Visible = true;
                statusTextLabel.Visible = true;
            }
            else
            {
                saveScoresLabel.Visible = false;
                nameLabel.Visible = false;
                scoresTextBox.Visible = false;
                submitScoresButton.Visible = false;
                resetAllScoresButton.Visible = false;
                statusTextLabel.Visible = false;
            }
        }

        private void resetAllScores_Click(object sender, EventArgs e)
        {
            DeleteScores();
        }

        private void HideAllObstacles()
        {
            obstacleBottom.Visible = false;
            obstacleTop.Visible = false;
            obstacleBottom2.Visible = false;
            obstacleTop2.Visible = false;
        }

        private void ShowAllObstacles()
        {
            obstacleBottom.Visible = true;
            obstacleTop.Visible = true;
            obstacleBottom2.Visible = true;
            obstacleTop2.Visible = true;
        }
    }
}
