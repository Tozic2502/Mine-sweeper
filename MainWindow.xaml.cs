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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        

        private void Easy_Click(object sender, RoutedEventArgs e) => StartGame(10, 20, 10);

        private void Normal_Click(object sender, RoutedEventArgs e) => StartGame(15, 30, 40);
        private void Hard_Click(object sender, RoutedEventArgs e) => StartGame(20, 40, 100);

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
            foreach (var tile in tiles)
                tile.AdjacentMines = CountAdjacentMines(tile);

            seconds = 0;
            TimerText.Text = "time: 0";
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { seconds++; TimerText.Text = "time: " + seconds; };
            timer.Start();

        }
        private int CountAdjacentMines(Tile tile)
        {
            int count = 0;
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    if (dy == 0 && dx == 0) continue;
                    int ny = tile.Row + dy;
                    int nx = tile.Col + dx;
                    if (ny >= 0 && ny < rows && nx >= 0 && nx < cols && tiles[ny, nx].IsMine)
                        count++;
                }
            }
            return count;
        }
        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile t = (Tile)sender;
            if (t.IsRevealed) return;

            if(t.IsMine)
            {
                t.Reveal();
                GameOver();
            }
            else
            {
                Reveal(t);
                CheckWin();
                   
            }
        }
        private void Reveal(Tile t)
        {
            if (t.IsRevealed) return;
            t.Reveal();

            if (t.AdjacentMines == 0)
            {
             foreach(var n in GetNeighbors(t))
             {
                    Reveal(n);
             }
            }
           
        }
        private void CheckWin()
        {
            foreach(var tile in tiles)
            {
                if(!tile.IsMine && !tile.IsRevealed)
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
                t.Reveal();
            MessageBox.Show("Game Over");
            ResetGame();
        }
        private void ResetGame()
        {
            GamePanel.Visibility = Visibility.Collapsed;
            StartMenu.Visibility = Visibility.Visible;
        }
        private Tile[] GetNeighbors(Tile tile)
        {
            var list = new System.Collections.Generic.List<Tile>();
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0) continue;
                    int nx = tile.X - x;
                    int ny = tile.Y - y;
                    if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                    {
                        list.Add(tiles[ny, nx]);
                    }
                }
            }
            return list.ToArray();
        }

    }
}