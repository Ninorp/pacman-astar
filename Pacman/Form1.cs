using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pacman
{
    public partial class Form1 : Form
    {
        public static GameBoard gameboard = new GameBoard();
        public static Food food = new Food();
        public static Pacman pacman = new Pacman();
        public static Ghost ghost = new Ghost();
        public static Player player = new Player();
        public static HighScore highscore = new HighScore();
        //public static Audio audio = new Audio();
        private static FormElements formelements = new FormElements();

        public Form1(int v)
        {
            InitializeComponent();
            SetupGame(1, v);
        }

        public void SetupGame(int Level, int dificuldade)
        {
            // Create Game Board
            gameboard.CreateBoardImage(this, Level);

            // Create Board Matrix
            Tuple<int, int> PacmanStartCoordinates = gameboard.InitialiseBoardMatrix(Level);

            // Create Player
            player.CreatePlayerDetails(this);
            player.CreateLives(this);

            // Create Form Elements
            formelements.CreateFormElements(this);

            // Create High Score
            highscore.CreateHighScore(this);

            // Create Food
            food.CreateFoodImages(this);

            // Create Ghosts
            ghost.CreateGhostImage(this);
            ghost.dificuldade = dificuldade;

            // Create Pacman
            pacman.CreatePacmanImage(this, PacmanStartCoordinates.Item1, PacmanStartCoordinates.Item2);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Up: pacman.nextDirection = 1; break;
                case Keys.Right: pacman.nextDirection = 2; break;
                case Keys.Down: pacman.nextDirection = 3; break;
                case Keys.Left: pacman.nextDirection = 4; break;
            }
        }

        public void CloseThis()
        {
            this.Close();
        }

    }
}
