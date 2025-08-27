using System.Text;
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

namespace Mine_sweeper
{
    public partial class MainWindow : Window
    {
        private int rows, cols, mines;
        private Tile[,] tiles;
        private DispatcherTimer timer;
        private int seconds;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Easy_Click(object sender, RoutedEventArgs e) => StartGame(8, 10, 10);
        private void Normal_Click(object sender, RoutedEventArgs e) => StartGame(14, 18, 20);
        private void Hard_Click(object sender, RoutedEventArgs e) => StartGame(20, 24, 100);

        private void StartGame(int r, int c, int m)
        {
            rows = r; cols = c; mines = m;
            StartMenu.Visibility = Visibility.Collapsed;
            GamePanel.Visibility = Visibility.Visible;
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Children.Clear();

            tiles = new Tile[rows, cols];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Tile tile = new Tile(y, x);
                    tile.Click += Tile_Click;
                    tiles[y, x] = tile;
                    GameGrid.Children.Add(tile);
                }
            }
            Random rand = new Random();
            int placedMines = 0;
            while (placedMines < mines)
            {
                int ry = rand.Next(rows);
                int rx = rand.Next(cols);
                if (!tiles[ry, rx].IsMine)
                {
                    tiles[ry, rx].IsMine = true;
                    placedMines++;
                }
            }
            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    tiles[y, x].AdjacentMines = CountAdjacentMines(y, x);

            seconds = 0;
            TimerText.Text = "time: 0";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { seconds++; TimerText.Text = "time: " + seconds; };
            timer.Start();
        }

        private int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dy == 0 && dx == 0) continue;
                    int ny = row + dy;
                    int nx = col + dx;
                    if (ny >= 0 && ny < rows && nx >= 0 && nx < cols)
                        if (tiles[ny, nx].IsMine) count++;
                }
            }
            return count;
        }

        private IEnumerable<Tile> GetNeighbors(Tile t)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dy == 0 && dx == 0) continue;
                    int ny = t.Y + dy;
                    int nx = t.X + dx;
                    if (ny >= 0 && ny < rows && nx >= 0 && nx < cols)
                        yield return tiles[ny, nx];
                }
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile t = (Tile)sender;
            if (t.IsRevealed) return;

            if (t.IsMine)
            {
                t.Reveal();
                GameOver();
            }
            else
            {
                RevealTile(t);
                CheckWin();
            }
        }

        private void RevealTile(Tile t)
        {
            if (t.IsRevealed) return;
            t.Reveal();

            if (!t.IsMine && t.AdjacentMines == 0)
            {
                foreach (var n in GetNeighbors(t))
                {
                    RevealTile(n);
                }
            }
        }

        private void CheckWin()
        {
            foreach (var tile in tiles)
            {
                if (!tile.IsMine && !tile.IsRevealed)
                    return;
            }
            timer.Stop();
            MessageBox.Show("You Win! Time: " + seconds + " seconds");
            ResetGame();
        }

        private void GameOver()
        {
            timer.Stop();
            foreach (var t in tiles)
                t.Reveal(); // No parameter needed
            MessageBox.Show("Game Over");
            ResetGame();
        }

        private void ResetGame()
        {
            GamePanel.Visibility = Visibility.Collapsed;
            StartMenu.Visibility = Visibility.Visible;
        }
    }
}