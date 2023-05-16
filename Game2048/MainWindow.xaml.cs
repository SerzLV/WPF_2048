using Game2048.GameLogic;
using System.Windows;
using System.Windows.Input;

namespace Game2048
{
    public partial class MainWindow : Window
    {
        private Game _gameLogic = new Game();

        public MainWindow()
        {
            InitializeComponent();
            _gameLogic.gameBoard = this.gameBoard;
            _gameLogic.scoreText = this.scoreText;
            _gameLogic.timerText = this.timerText;
            _gameLogic.StartNewGame();
        }
        
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            _gameLogic.TileMove(e);
        }
    }
}
