using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Mine_sweeper
{
    class Tile : Button
    {
        public int X { get; }
        public int Y { get; }
        public bool IsMine { get; set; }
        public int AdjacentMines { get; set; }
        public bool IsRevealed { get; private set; }

        public Tile(int x, int y)
        {
            X = x; Y = y;
            Margin = new Thickness(1);
            FontSize = 16;
        }
        public void Reveal()
        {
            if (IsRevealed) return;
            IsRevealed = true;

            if (IsMine)
            {
                Content = "";
                Background = System.Windows.Media.Brushes.Red;

            }
            else if (AdjacentMines > 0)
            {
                Content = AdjacentMines.ToString();
                Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                Content = "";
                Background = System.Windows.Media.Brushes.Green;
            }
        }   
    }
}
