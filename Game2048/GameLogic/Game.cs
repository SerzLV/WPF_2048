using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Game2048.GameLogic
{
    public class Game
    {
        static int[,] boardSize = new int[6, 6];
        private int[,] board = boardSize;
        private int score = 0;
        private int winScore = 64;

        private DispatcherTimer timer;
        private int timeElapsed;

        public System.Windows.Controls.Primitives.UniformGrid gameBoard { get; set; }
        public TextBlock scoreText { get; set; }
        public TextBlock timerText { get; set; }

        private readonly Brush[] brushList =
        {
            Brushes.LightGray,
            Brushes.Orange,
            Brushes.GreenYellow,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Purple,
            Brushes.Red,
            Brushes.Brown,
            Brushes.Lime,
            Brushes.Magenta,
            Brushes.Orchid,
            Brushes.DarkGoldenrod
        };

        public void StartNewGame()
        {
            // Clear the board
            gameBoard.Children.Clear();
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    board[row, col] = 0;
                    gameBoard.Children.Add(new Border() { Background = Brushes.LightGray });
                }
            }

            // Add two random tiles
            AddRandomTile();
            AddRandomTile();

            // Reset the score
            score = 0;
            UpdateScore();
            TimerStart();
        }

        public void TimerStart()
        {
            // Create a new timer with a tick interval of 1 second
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timeElapsed = 0;

            // Start the timer
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Increment the timeElapsed variable and update the timer display
            timeElapsed++;
            timerText.Text = $"Time: {timeElapsed}";
        }

        private static readonly Random rand = new Random();
        public void AddRandomTile()
        {
            // Find all empty tiles
            var emptyTiles = new List<(int, int)>();
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == 0)
                    {
                        emptyTiles.Add((i, j));
                    }
                }
            }

            // If there are no empty tiles, return
            if (emptyTiles.Count == 0)
            {
                return;
            }

            // Choose a random empty tile
            var index = rand.Next(emptyTiles.Count);
            var (row, col) = emptyTiles[index];

            // Add a new tile with value 2 or 4
            var value = rand.NextDouble() < 0.9 ? 2 : 4;
            board[row, col] = value;

            var tile = new Border
            {
                Background = brushList[(int)Math.Log2(value)],
                BorderThickness = new Thickness(2),
                BorderBrush = Brushes.Black,
                CornerRadius = new CornerRadius(10),
                Child = new TextBlock
                {
                    Text = value.ToString(),
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            var cellIndex = row * board.GetLength(1) + col;
            if (gameBoard.Children.Count == board.GetLength(0) * board.GetLength(1))
            {
                gameBoard.Children.RemoveAt(cellIndex);
            }
            gameBoard.Children.Insert(cellIndex, tile);
        }

        public void UpdateScore()
        {
            scoreText.Text = "Score: " + score;
        }

        public bool MoveLeft()
        {
            bool moved = false;

            // Loop through each row on the board
            for (int row = 0; row < board.GetLength(0); row++)
            {
                // Loop through each column starting from the second one
                for (int col = 1; col < board.GetLength(1); col++)
                {
                    // Get the value at the current position
                    int currentValue = board[row, col];

                    // If the value is 0, skip it and move to the next column
                    if (currentValue == 0)
                    {
                        continue;
                    }

                    // Set a variable to the left of the current column
                    int left = col - 1;

                    // Move left until the left-most position or a non-zero value is found
                    while (left >= 0 && board[row, left] == 0)
                    {
                        left--;
                    }

                    // If the left value is non-zero and equal to the current value,
                    // merge them and update the score
                    if (left >= 0 && board[row, left] == currentValue)
                    {
                        board[row, left] *= 2;
                        score += board[row, left];
                        board[row, col] = 0;
                        moved = true;
                    }
                    // If the left value is non-zero and different from the current value,
                    // move the current value to the left-most empty position
                    else if (left < col - 1)
                    {
                        board[row, left + 1] = currentValue;
                        board[row, col] = 0;
                        moved = true;
                    }
                }
            }

            return moved;
        }

        public bool MoveRight()
        {
            bool moved = false;

            // Loop through each row on the board
            for (int row = 0; row < board.GetLength(0); row++)
            {
                // Loop through each column starting from the fifth one
                for (int col = 4; col >= 0; col--)
                {
                    // Get the value at the current position
                    int currentValue = board[row, col];

                    // If the value is 0, skip it and move to the next column
                    if (currentValue == 0)
                    {
                        continue;
                    }

                    // Set a variable to the right of the current column
                    int right = col + 1;

                    // Move right until the right-most position or a non-zero value is found
                    while (right <= 5 && board[row, right] == 0)
                    {
                        right++;
                    }

                    // If the right value is non-zero and equal to the current value,
                    // merge them and update the score
                    if (right <= 5 && board[row, right] == currentValue)
                    {
                        board[row, right] *= 2;
                        score += board[row, right];
                        board[row, col] = 0;
                        moved = true;
                    }
                    // If the right value is non-zero and different from the current value,
                    // move the current value to the right-most empty position
                    else if (right > col + 1)
                    {
                        board[row, right - 1] = currentValue;
                        board[row, col] = 0;
                        moved = true;
                    }
                }
            }

            return moved;
        }

        public bool MoveUp()
        {
            bool moved = false;

            // Loop through each column on the board
            for (int col = 0; col < board.GetLength(1); col++)
            {
                // Loop through each row starting from the fourth one
                for (int row = 4; row < board.GetLength(0); row++)
                {
                    // Get the value at the current position
                    int currentValue = board[row, col];

                    // If the value is 0, skip it and move to the next row
                    if (currentValue == 0)
                    {
                        continue;
                    }

                    // Set a variable to the row above the current position
                    int above = row - 1;

                    // Move up until the top row or a non-zero value is found
                    while (above >= 0 && board[above, col] == 0)
                    {
                        above--;
                    }

                    // If the value above is non-zero and equal to the current value,
                    // merge them and update the score
                    if (above >= 0 && board[above, col] == currentValue)
                    {
                        board[above, col] *= 2;
                        score += board[above, col];
                        board[row, col] = 0;
                        moved = true;
                    }
                    // If the value above is non-zero and different from the current value,
                    // move the current value to the top-most empty position
                    else if (above < row - 1)
                    {
                        board[above + 1, col] = currentValue;
                        board[row, col] = 0;
                        moved = true;
                    }
                }
            }

            return moved;
        }

        public bool MoveDown()
        {
            bool moved = false;

            // loop through columns from left to right
            for (int col = 0; col < board.GetLength(1); col++)
            {
                // loop through rows from bottom to top
                for (int row = 4; row >= 0; row--)
                {
                    int currentValue = board[row, col];

                    if (currentValue == 0)
                    {
                        continue;
                    }

                    int k = row + 1;

                    // move currentValue down as far as possible
                    while (k <= 5 && board[k, col] == 0)
                    {
                        k++;
                    }

                    // combine currentValue with the tile below it, if possible
                    if (k <= 5 && board[k, col] == currentValue)
                    {
                        board[k, col] *= 2;
                        score += board[k, col];
                        board[row, col] = 0;
                        moved = true;
                    }
                    // otherwise, move currentValue down to the next available space
                    else if (k > row + 1)
                    {
                        board[k - 1, col] = currentValue;
                        board[row, col] = 0;
                        moved = true;
                    }
                }
            }

            return moved;
        }

        public void GameOver()
        {
            timer.Stop();
            MessageBox.Show($"Game over! Your score is: {score}, time spent: {timeElapsed} sec.", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            Restart();
        }

        private void Restart()
        {
            TimerStart();
            score = 0;
            UpdateScore();
            InitializeBoard();
            AddRandomTile();
            AddRandomTile();
        }

        private void InitializeBoard()
        {
            board = boardSize;
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    board[row, col] = 0;
                }
            }
            UpdateBoard();
        }

        public void UpdateBoard()
        {
            gameBoard.Children.Clear();

            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    Border tile = new Border
                    {
                        BorderThickness = new Thickness(2),
                        BorderBrush = Brushes.Black,
                        CornerRadius = new CornerRadius(10),
                        Background = GetBrushForValue(board[row, col]),
                        Child = board[row, col] != 0 ? GetTextBlockForValue(board[row, col]) : null
                    };

                    Grid.SetRow(tile, row);
                    Grid.SetColumn(tile, col);
                    gameBoard.Children.Add(tile);
                }
            }
        }

        public void CheckForWin()
        {
            // Check for win condition
            if (board.Cast<int>().Any(tile => tile == winScore))
            {
                timer.Stop();
                MessageBox.Show($"Congratulations! You won with a score of {score}, time spent: {timeElapsed} sec.", "Game Over", MessageBoxButton.OK);
                StartNewGame();
            }
        }

        private Brush GetBrushForValue(int value)
        {
            int index = (int)Math.Log2(value);
            return index >= 0 && index < brushList.Length ? brushList[index] : Brushes.White;
        }

        private TextBlock GetTextBlockForValue(int value)
        {
            return new TextBlock
            {
                Text = value.ToString(),
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        public bool IsGameOver()
        {
            for (int row = 0; row < board.GetLength(0); row++)
            {
                for (int col = 0; col < board.GetLength(1); col++)
                {
                    if (board[row, col] == 0)
                    {
                        // There's an empty cell, so the game is not over.
                        return false;
                    }
                    if (row < 3 && board[row, col] == board[row + 1, col] || col < 3 && board[row, col] == board[row, col + 1])
                    {
                        // There's a cell with a matching neighbor, so the game is not over.
                        return false;
                    }
                }
            }
            // If we got here, there are no empty cells or matching neighbors, so the game is over.
            return true;
        }

        public void TileMove(KeyEventArgs e)
        {
            bool moved = false;
            switch (e.Key)
            {
                case Key.Left:
                    moved = MoveLeft();
                    break;
                case Key.Right:
                    moved = MoveRight();
                    break;
                case Key.Up:
                    moved = MoveUp();
                    break;
                case Key.Down:
                    moved = MoveDown();
                    break;
            }
            if (moved)
            {
                AddRandomTile();
                UpdateBoard();
                UpdateScore();
                CheckForWin();
                if (IsGameOver())
                {
                    GameOver();
                }
            }
        }
    }
}
