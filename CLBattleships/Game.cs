using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace CLBattleships
{
    public class Game
    {
        private RichTextBox GameScreen;
        private Board PlayerBoard, ComputerBoard;
        public Cursor PlayerCursor;
        private void PrintData()
        {
            ViewData[,] playerData = PlayerBoard.GenerateViewData();
            ViewData[,] computerData = ComputerBoard.GenerateViewData();
            FlowDocument a = new FlowDocument(new Paragraph());
            a.Blocks.Add(new Paragraph());
            for(int y = 0; y < 21; y++)
            {
                Paragraph p = new Paragraph();
                Run r = new Run();
                r.Text = "       ";
                p.Foreground = Brushes.Green;
                for(int x = 0; x < 44; x++)
                {
                    p.Inlines.Add(""+(char)playerData[x, y]);
                    r.Text += "" + (char)computerData[x, y];
                }
                p.Inlines.Add(r);
                a.Blocks.Add(p);
            }

            GameScreen.Document = a;
        }
        public Game(RichTextBox gameScreen, Run playerCursor)
        {
            GameScreen = gameScreen;
            PlayerBoard = new Board();
            ComputerBoard = new Board();
            PlayerCursor = new Cursor(playerCursor);
            PrintData();
        }
        public void KeyPressed(Key key)
        {
            switch(key)
            {
                case Key.Left:
                    PlayerCursor.X = Math.Max(PlayerCursor.X - 1, 0);
                    break;
                case Key.Right:
                    PlayerCursor.X = Math.Min(PlayerCursor.X + 1, 9);
                    break;
                case Key.Up:
                    PlayerCursor.Y = Math.Max(PlayerCursor.Y - 1, 0);
                    break;
                case Key.Down:
                    PlayerCursor.Y = Math.Min(PlayerCursor.Y + 1, 9);
                    break;
            }
        }
    }
}
