using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mine_sweeper
{
    class Tile : Button
    {
        public int Y { get; }
        public int X { get; }
        public bool IsMine { get; set; }
        public int AdjacentMines { get; set; }
        public bool IsRevealed { get; private set; }

        public Tile(int y, int x)
        {
            Y = y; X = x;
            Margin = new Thickness(1);
            FontSize = 16;
        }

        public void Reveal()
        {
            if (IsRevealed) return;
            IsRevealed = true;

            if (IsMine)
            {
                Content = "💣";
                Background = Brushes.Red;
            }
            else if (AdjacentMines > 0)
            {
                Content = AdjacentMines.ToString();
                Background = Brushes.LightGray;
            }
            else
            {
                Content = "";
                Background = Brushes.White;
            }
            IsEnabled = false;
        }
    }
}
