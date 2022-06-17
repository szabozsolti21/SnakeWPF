using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vector = Snake.Models.Vector;


namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const int GameWidth = 25;
        private const int GameHeight = 25;
        private const int FoodCount = 100;
        private const double RefreshTimeSec = 0.2;

        private readonly List<Vector> _foodPositions = new List<Vector>();

        private readonly List<Vector> _snakePositions = new List<Vector>();

        private readonly Random _random = new Random();

        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Send);
        
        private Vector _direction;

        private bool _isRunning = false;


        private int _score;

        public int Score {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                tbScore.Text = _score.ToString(); 
            }
        }




        public MainWindow()
        {
            InitializeComponent();

            _timer.Interval = TimeSpan.FromSeconds(RefreshTimeSec);
            _timer.Tick += TimerTick;
            _timer.Start();
            _isRunning = true;

            InitGame();

            RenderState();

        }

        private void GameOver()
        {
            InitGame();
        }

        private void InitGame()
        {

            Score = 0;

            _direction = new Vector(0, 0);

            _snakePositions.Clear();
            _snakePositions.Add(new Vector(GameHeight / 2, GameHeight / 2));

            _foodPositions.Clear();
            for (int i = 0; i < FoodCount; i++)
            {
                _foodPositions.Add(GenerateFreeRandomPosition());
            }
        }

        private void RenderState()
        {
            canvas.Children.Clear();

            RenderSnake();
            RenderFood();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if(!CalculateState())
            {
                GameOver();
            }

            RenderState();
        }

        private void RenderSnake()
        {
            foreach (Vector snakePart in _snakePositions)
            {
                DrawPoint(snakePart, Brushes.Coral);
            }
        }

        private void RenderFood()
        {
            foreach (Vector food in _foodPositions)
            {
                DrawPoint(food, Brushes.Green);
            }
        }

        private Vector GenerateFreeRandomPosition()
        {
            var newPos = new Vector();
            do
            {
                newPos = new Vector(_random.Next(GameWidth), _random.Next(GameHeight));
            } while (_foodPositions.Any(s => s == newPos) || (_snakePositions.Any(s => s == newPos)));


            return newPos;
        }


        private void DrawPoint(Vector position, Brush brush)
        {
            var shape = new Rectangle();
            shape.Fill = brush;

            var unitX = canvas.Width / GameWidth;
            var unitY = canvas.Height / GameHeight;

            shape.Width = unitX;
            shape.Height = unitY;

            Canvas.SetTop(shape, unitY * position.Y);

            Canvas.SetLeft(shape, unitX * position.X);

            canvas.Children.Add(shape);
        }

        private void ApplyInputKey(Key pressedKey)
        {
            if(pressedKey == Key.Up)
            {
                _direction = Vector.Up;
            }
            else if(pressedKey == Key.Down)
            {
                _direction = Vector.Down;
            }
            else if(pressedKey == Key.Left)
            {
                _direction = Vector.Left;
            }
            else if(pressedKey == Key.Right)
            {
                _direction = Vector.Right;
            }
            else if (pressedKey == Key.P)
            {
                if(_isRunning == true)
                {
                    _timer.Stop();
                }
                else
                {
                    _timer.Start();
                    
                }
                _isRunning = !_isRunning;
            }

        }

        private bool CalculateState()
        {
            Vector lastPosition = _snakePositions[0];
            _snakePositions[0] = _snakePositions[0] + _direction;

            //Out of area
            
            Vector head = _snakePositions.First();

            if(head.X < 0 || head.X >= GameWidth ||
               head.Y < 0 || head.Y >= GameHeight)
            {
                return false;
            }


            // Body movement

            for (int i = 1; i < _snakePositions.Count; i++)
            {
                var temp = _snakePositions[i];
                _snakePositions[i] = lastPosition;
                lastPosition = temp;
            }

            // Eating food

            int index = _foodPositions.IndexOf(head);

            if(index > 0)
            {
                Score++;
                _snakePositions.Add(lastPosition);
                _foodPositions.RemoveAt(index);
                _foodPositions.Add(GenerateFreeRandomPosition());
            
            
            }

            // Snake bitesitself

            if(_snakePositions.Skip(1).Any(s => s == head))
            {
                return false;
            }


            return true;
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ApplyInputKey(e.Key);
        }
    }
}
