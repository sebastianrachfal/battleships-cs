using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace CLBattleships
{
    public class Cursor
    {
        private int x = 0, y= 0;
        public int X {
            get
            {
                return x;
            }
            set {
                x = value;
                UpdateCursor();
            } 
        }
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                UpdateCursor();
            }
        }
        Run playerCursor;
        public Cursor(Run cursor)
        {
            playerCursor = cursor;
            UpdateCursor();
        }
        public void UpdateCursor()
        {
            Console.WriteLine(x + " " + y);
            string topPad = string.Concat(Enumerable.Repeat("\x0a", y * 2));
            string leftPad = string.Concat(Enumerable.Repeat(" ", 54 + x * 4));
            playerCursor.Text = "\x0a\x0a" + topPad + leftPad + "┏━━━┓" + "\x0a" + leftPad + "┃   ┃" + "\x0a" + leftPad + "┗━━━┛";
        }
    }
}
