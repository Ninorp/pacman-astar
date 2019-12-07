using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pacman.Classes;

namespace Pacman
{
    public class Ghost
    {
        private const int GhostAmount = 4;

        public int Ghosts = GhostAmount;
        private ImageList GhostImages = new ImageList();
        public PictureBox[] GhostImage = new PictureBox[GhostAmount];
        public int[] State = new int[GhostAmount];
        private Timer timer = new Timer();
        private Timer killabletimer = new Timer();
        private Timer statetimer = new Timer();
        private Timer hometimer = new Timer();
        public int[] xCoordinate = new int[GhostAmount];
        public int[] yCoordinate = new int[GhostAmount];
        private int[] xStart = new int[GhostAmount];
        private int[] yStart = new int[GhostAmount];
        public int[] Direction = new int[GhostAmount];
        private Random ran = new Random();
        private bool GhostOn = false;
        private int[] contador = new int[GhostAmount];
        public int dificuldade;        
        public Ghost()
        {
            GhostImages.Images.Add(Properties.Resources.Ghost_0_1);
            GhostImages.Images.Add(Properties.Resources.Ghost_0_2);
            GhostImages.Images.Add(Properties.Resources.Ghost_0_3);
            GhostImages.Images.Add(Properties.Resources.Ghost_0_4);

            GhostImages.Images.Add(Properties.Resources.Ghost_1_1);
            GhostImages.Images.Add(Properties.Resources.Ghost_1_2);
            GhostImages.Images.Add(Properties.Resources.Ghost_1_3);
            GhostImages.Images.Add(Properties.Resources.Ghost_1_4);

            GhostImages.Images.Add(Properties.Resources.Ghost_2_1);
            GhostImages.Images.Add(Properties.Resources.Ghost_2_2);
            GhostImages.Images.Add(Properties.Resources.Ghost_2_3);
            GhostImages.Images.Add(Properties.Resources.Ghost_2_4);

            GhostImages.Images.Add(Properties.Resources.Ghost_3_1);
            GhostImages.Images.Add(Properties.Resources.Ghost_3_2);
            GhostImages.Images.Add(Properties.Resources.Ghost_3_3);
            GhostImages.Images.Add(Properties.Resources.Ghost_3_4);

            GhostImages.Images.Add(Properties.Resources.Ghost_4);
            GhostImages.Images.Add(Properties.Resources.Ghost_5);

            GhostImages.ImageSize = new Size(27, 28);

            timer.Interval = 100;
            timer.Enabled = true;
            timer.Tick += new EventHandler(timer_Tick);

            killabletimer.Interval = 200;
            killabletimer.Enabled = false;
            killabletimer.Tick += new EventHandler(killabletimer_Tick);

            statetimer.Interval = 10000;
            statetimer.Enabled = false;
            statetimer.Tick += new EventHandler(statetimer_Tick);

            hometimer.Interval = 150;
            hometimer.Enabled = false;
            hometimer.Tick += new EventHandler(hometimer_Tick);
        }

        public void CreateGhostImage(Form formInstance)
        {
            // Create Ghost Image
            for (int x=0; x<Ghosts; x++)
            {
                GhostImage[x] = new PictureBox();
                GhostImage[x].Name = "GhostImage" + x.ToString();
                GhostImage[x].SizeMode = PictureBoxSizeMode.AutoSize;
                formInstance.Controls.Add(GhostImage[x]);
                GhostImage[x].BringToFront();
            }
            Set_Ghosts();
            ResetGhosts();
        }

        public void Set_Ghosts()
        {
            // Find Ghost locations
            int Amount = -1;
            for (int y = 0; y < 30; y++)
            {
                for (int x = 0; x < 27; x++)
                {
                    if (Form1.gameboard.Matrix[y, x] == 15)
                    {
                        Amount++;
                        xStart[Amount] = x;
                        yStart[Amount] = y;
                    }
                }
            }
        }

        public void ResetGhosts()
        {
            // Reset Ghost States
            for (int x=0; x<GhostAmount; x++)
            {
                xCoordinate[x] = xStart[x];
                yCoordinate[x] = yStart[x];
                GhostImage[x].Location = new Point(xStart[x] * 16 - 3, yStart[x] * 16 + 43);
                GhostImage[x].Image = GhostImages.Images[x * 4];
                Direction[x] = 0;
                State[x] = 0;
            }
        }

        private void statetimer_Tick(object sender, EventArgs e)
        {
            // Turn Ghosts back
            for (int x=0; x<GhostAmount; x++)
            {
                State[x] = 0;
            }
            statetimer.Enabled = false;
            //killabletimer.Enabled = false;
        }

        private void hometimer_Tick(object sender, EventArgs e)
        {
            // Move ghosts to their home positions
            for (int x=0; x<GhostAmount; x++)
            {
                if (State[x] == 2)
                {
                    int xpos = xStart[x] * 16 - 3;
                    int ypos = yStart[x] * 16 + 43;                    
                    MoveToHomeGhost(x);                    
                    if (Util.ManhatthanDistance(xCoordinate[x], yCoordinate[x], xStart[x], yStart[x]) == 0)
                    {
                        State[x] = 0;
                        xCoordinate[x] = xStart[x];
                        yCoordinate[x] = yStart[x];
                        GhostImage[x].Left = xpos;
                        GhostImage[x].Top = ypos;
                    }
                } 
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            // Keep moving the ghosts
            for (int x = 0; x < Ghosts; x++)
            {
                if (State[x] > 0) { continue; }
                MoveGhosts(x);
            }
            GhostOn = !GhostOn;
            CheckForPacman();
        }

        private void killabletimer_Tick(object sender, EventArgs e)
        {
            // Keep moving the ghosts
            for (int x = 0; x < Ghosts; x++)
            {
                if (State[x] != 1) { continue; }
                MoveGhosts(x);
            }
        }        

        private void MoveToHomeGhost(int x)
        {           
            var caminho = Util.AStarAlg(new AStarLocation() { X = xCoordinate[x], Y = yCoordinate[x] }, new AStarLocation() { X = xStart[x], Y = yStart[x] });            
            caminho.RemoveAt(0);
            var next = caminho[0];            
            if (next != null)
            {               
                GhostImage[x].Left = next.X * 16 - 3;
                GhostImage[x].Top = next.Y * 16 + 43;               
                xCoordinate[x] = next.X;
                yCoordinate[x] = next.Y;                
                return;
            }                
            else 
                return;
        }

        private void MoveGhosts(int x)
        {
            if (contador[x] == int.MaxValue)
                contador[x] = 0;
            else
                contador[x]++;
            // Move the ghosts
            if (Direction[x] == 0)
            {
                if (ran.Next(0, 5) == 3) { Direction[x] = 1;}
            }
            else
            {
                var pacman = Form1.pacman;
                AStarLocation LocationPacMan = new AStarLocation();
                if(pacman.xHistory.Count > 0)
                {
                    switch (dificuldade)
                    {
                        case 0:
                            LocationPacMan.X = pacman.xHistory.Dequeue();
                            LocationPacMan.Y = pacman.yHistory.Dequeue();
                            break;
                        case 1:
                            LocationPacMan.X = pacman.xHistory.ElementAt(pacman.xHistory.Count / 2);
                            LocationPacMan.Y = pacman.yHistory.ElementAt(pacman.yHistory.Count / 2);
                            pacman.xHistory.Dequeue();
                            pacman.yHistory.Dequeue();
                            break;
                        case 2:
                            LocationPacMan.X = pacman.xCoordinate;
                            LocationPacMan.Y = pacman.yCoordinate;
                            break;
                    }
                } else
                {
                    LocationPacMan.X = 0;
                    LocationPacMan.Y = 0;
                }
                
                var caminho = Util.AStarAlg(new AStarLocation() { X = xCoordinate[x], Y = yCoordinate[x] }, LocationPacMan);                
                AStarLocation next = null;
                if(caminho != null && caminho.Count > 1)
                {
                    caminho.RemoveAt(0);
                    next = caminho[0];
                }                                
                if (next != null)
                {
                    int mod = Convert.ToInt16(Math.Pow(5.0, Convert.ToDouble(dificuldade)));
                    if (State[x] == 0 && contador[x] % mod != 0)
                    {                        
                        GhostImage[x].Left = next.X * 16 - 3;
                        GhostImage[x].Top = next.Y * 16 + 43;                        
                        xCoordinate[x] = next.X;
                        yCoordinate[x] = next.Y;                       
                    }
                    else
                    {
                        MoveRandom(x);
                    }
                                                                   
                } else
                {
                    MoveRandom(x);
                }
                RefreshImage(x);
            }
            
        }

        public void RefreshImage(int x)
        {
            switch (State[x])
            {
                case 0: GhostImage[x].Image = GhostImages.Images[x * 4 + (Direction[x] - 1)]; break;
                case 1:
                    if (GhostOn) { GhostImage[x].Image = GhostImages.Images[17]; } else { GhostImage[x].Image = GhostImages.Images[16]; };
                    break;
                case 2: GhostImage[x].Image = GhostImages.Images[18]; break;
            }
        }
        public void MoveRandom(int x)
        {
            bool CanMove = false;
            Other_Direction(Direction[x], x);

            while (!CanMove)
            {
                CanMove = check_direction(Direction[x], x);
                if (!CanMove) { Change_Direction(Direction[x], x); }

            }

            if (CanMove)
            {
                switch (Direction[x])
                {
                    case 1: GhostImage[x].Top -= 16; yCoordinate[x]--; break;
                    case 2: GhostImage[x].Left += 16; xCoordinate[x]++; break;
                    case 3: GhostImage[x].Top += 16; yCoordinate[x]++; break;
                    case 4: GhostImage[x].Left -= 16; xCoordinate[x]--; break;
                }
            }
        }
        private bool check_direction(int direction, int ghost)
        {
            // Check if ghost can move to space
            switch (direction)
            {
                case 1: return direction_ok(xCoordinate[ghost], yCoordinate[ghost] - 1, ghost);
                case 2: return direction_ok(xCoordinate[ghost] + 1, yCoordinate[ghost], ghost);
                case 3: return direction_ok(xCoordinate[ghost], yCoordinate[ghost] + 1, ghost);
                case 4: return direction_ok(xCoordinate[ghost] - 1, yCoordinate[ghost], ghost);
                default: return false;
            }
        }

        private bool direction_ok(int x, int y, int ghost)
        {
            // Check if board space can be used
            if (x < 0) {
                xCoordinate[ghost] = 27;
                GhostImage[ghost].Left = 429;
                return true;
            }
            if (x > 27) {
                xCoordinate[ghost] = 0;
                GhostImage[ghost].Left = -5;
                return true;
            }
            if (Form1.gameboard.Matrix[y, x] < 4 || Form1.gameboard.Matrix[y, x] > 10) {                
                return true;
            } else {                
                return false;
            }
        }

        private void Change_Direction(int direction, int ghost)
        {
            // Change the direction of the ghost
            int which = ran.Next(0, 2);
            switch (direction)
            {
                case 1: case 3: if (which == 1) { Direction[ghost] = 2; } else { Direction[ghost] = 4; }; break;
                case 2: case 4: if (which == 1) { Direction[ghost] = 1; } else { Direction[ghost] = 3; }; break;
            }
        }

        private void Other_Direction(int direction, int ghost)
        {
            // Check to see if the ghost can move a different direction
            if (Form1.gameboard.Matrix[yCoordinate[ghost], xCoordinate[ghost]] < 4)
            {
                bool[] directions = new bool[5];
                int x = xCoordinate[ghost];
                int y = yCoordinate[ghost];
                switch (direction)
                {
                    case 1: case 3: directions[2] = direction_ok(x + 1, y, ghost); directions[4] = direction_ok(x - 1, y, ghost); break;
                    case 2: case 4: directions[1] = direction_ok(x, y - 1, ghost); directions[3] = direction_ok(x, y + 1, ghost); break;
                }
                int which = ran.Next(0, 5);
                if (directions[which] == true) { Direction[ghost] = which; }
            }
        }

        public void ChangeGhostState()
        {
            // Change the state off all of the ghosts so that they can be eaten
            for (int x=0; x<GhostAmount; x++)
            {
                if (State[x] == 0)
                {
                    State[x] = 1;
                    GhostImage[x].Image = GhostImages.Images[16];
                }
            }
            killabletimer.Stop();
            killabletimer.Enabled = true;
            killabletimer.Start();
            statetimer.Stop();
            statetimer.Enabled = true;
            statetimer.Start();
        }

        public void CheckForPacman()
        {
            // Check to see if a ghost is on the same block as Pacman
            for (int x = 0; x < GhostAmount; x++)
            {
                var pacman = Form1.pacman;
                bool colidiu = Util.Colidiu(GhostImage[x].Location.X, GhostImage[x].Width, GhostImage[x].Location.Y, GhostImage[x].Height, pacman.PacmanImage.Location.X, pacman.PacmanImage.Width, pacman.PacmanImage.Location.Y, pacman.PacmanImage.Height);              
                if (colidiu)              
                {
                    switch (State[x])
                    {
                        case 0: Form1.player.LoseLife(); break;
                        case 1:
                            State[x] = 2;
                            hometimer.Enabled = true;
                            GhostImage[x].Image = Properties.Resources.eyes;
                            Form1.player.UpdateScore(300);
                            break;
                    }
                }
            }
        }
    }
}
