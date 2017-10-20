using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace SnakeGame
{
    public partial class Form1 : Form
    {

        private int cWidth = 300;
        private int cHeight = 300;
        private int size = 10;
        private DIR direction = DIR.RIGHT;
        private int score = 10;
        private Point berry; 
        private List<Point> snakePart; 
        private int speed = 100;
        bool gameOver = false;
        Font font = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular);

        public Form1()
        {
            startGame();
            InitializeComponent();
            content.Paint += new PaintEventHandler(panel_Paint);
            content.Size = new System.Drawing.Size(cWidth, cHeight);

            Thread piThread = new Thread(new ThreadStart(CalcPiThreadStart));
            piThread.Start();
        }

        void CalcPiThreadStart()
        {
            while (!gameOver)
            {
                try
                {
                    Thread.Sleep(speed);
                    content.Invoke(new MethodInvoker(delegate { content.Refresh(); }));
                }
                catch (Exception)
                {

                    gameOver = true;
                }
              
            }
        }

        bool  iniGame = false;
        void panel_Paint(object sender, PaintEventArgs e)
        {
            if (!iniGame)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black), 0, 0, cWidth, cHeight);
                e.Graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, cWidth, cHeight);
                iniGame = true;
            }
            var headX = snakePart[0].X;
            var headY = snakePart[0].Y;
            
            //Movement of the snake depending on the position
            if(direction == DIR.RIGHT ) headX++;
            else if(direction == DIR.LEFT ) headX--;
            else if(direction == DIR.UP ) headY--;
            else if(direction == DIR.DOWN ) headY++;
            
           //Determine possible collisions of the snake to end the game
            if(headX == -1 || //If the head hits the wall on the left
            headX == cWidth/size || //If the head hits the wall on the right
            headY == -1 || //If the head hits the top wall
            headY == cHeight/size || //If the head hits the bottom wall
            checkCollision(headX, headY, snakePart)) //If the head collides with the tail
            {   
                //Game over
                var gameOver_text = "Game Over Score: " + score;
                e.Graphics.DrawString(gameOver_text, font, Brushes.Black, new PointF((cWidth - 125) / 2, cHeight / 2));
                gameOver = true;
                return; 
            }

            Point tail = new Point();;
            //Determine if the head of the snake hit a berry
            if(headX == berry.X && headY == berry.Y)
            {
                //New part of the snake
                tail.X = headX;
                tail.Y = headY;
                score++;
                //Create new food
                createBerry();
            }
            else
            {
                //Get las Part of the snake
                snakePart.RemoveAt(snakePart.Count - 1);
                tail.X = headX;
                tail.Y = headY;
            }

            snakePart.Insert(0, tail);
            
            for(int i = 0; i < snakePart.Count; i++)
            {
                Point Part = snakePart[i];
                //Lets paint 10px wide cells
                if(i == 0) paintObject(Part.X, Part.Y, Color.Magenta, e.Graphics);
                else
                    paintObject(Part.X, Part.Y, Color.Green, e.Graphics);
            }
            
            //Lets paint the berry
            paintObject(berry.X, berry.Y, Color.Yellow, e.Graphics);
            
            //Lets paint the score
            var score_text = "Score: " + score;

            e.Graphics.DrawString(score_text, font, Brushes.Black, new PointF(5, cHeight - 25));
        }

        //Lets first create a generic function to paint cells
        public void paintObject(int x, int y, Color color, Graphics g)
        {  
            //Object backgroundColor
            g.FillRectangle(new SolidBrush(color), x * size, y * size, size, size);
            //BoderColor
            g.DrawRectangle(new Pen(Color.White), x * size, y * size, size, size);
        }

        //Check if the snake's head collided with the tail
        public bool checkCollision(int x, int y, List<Point> array)
        {
            foreach (var item in array)
            {
                if (item.X == x && item.Y == y)
                    return true;
            }
            return false;
        }

        public void startGame()
        {
            direction = DIR.RIGHT;
            createSnake();
            createBerry();
            score = 0;
        }

        public void createSnake()
        {
            int length = 5; //Initial Length of the snake
            snakePart = new List<Point>(); //Empty array to start with
            for (int i = length - 1; i >= 0; i--)
            {
                var part = new Point { X= i , Y = 0};
                //This will create a horizontal snake starting from the top left
                snakePart.Add(part);
            }
        }

        public void createBerry(){
            berry = new Point();
            Random r = new Random();
            berry.X = (int) (r.NextDouble() * (cWidth - size) / size);
            berry.Y = (int)(r.NextDouble() * (cWidth - size) / size);  
        }

        enum DIR { RIGHT, DOWN, UP, LEFT };

        public class Point
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Right && direction != DIR.LEFT)
                direction = DIR.RIGHT;
            else if (e.KeyData == Keys.Left && direction != DIR.RIGHT)
                direction = DIR.LEFT;
            else if (e.KeyData == Keys.Down && direction != DIR.UP)
                direction = DIR.DOWN;
            else if (e.KeyData == Keys.Up && direction != DIR.DOWN)
                direction = DIR.UP;
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            gameOver = true;

        }

    }
}
