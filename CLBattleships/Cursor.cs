using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace CLBattleships
{
	/// <summary>
	/// Class for handling the player interaction
	/// </summary>
	class Cursor
	{
		private int x = 0, y = 0, shipLength = 2;
		private bool shipOrientation = false;
		private GamePhase currentPhase;
		private Brush greenBrush;
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
		/// <summary>
		/// Constructor for a Cursor
		/// </summary>
		/// <param name="cursor">Run object of a player's cursor</param>
		public Cursor(Run cursor)
		{
			playerCursor = cursor;
			greenBrush = new SolidColorBrush(Color.FromArgb(255, 144, 187, 70));
			UpdateCursor();
		}
		/// <summary>
		/// Updates the cursor based on current phase, coordinates and availability of placement
		/// </summary>
		public void UpdateCursor()
		{
			string topPad = string.Concat(Enumerable.Repeat("\x0a", 2 + y * 2)), leftPad;
			switch (currentPhase)
			{
				case GamePhase.Game:
					leftPad = string.Concat(Enumerable.Repeat(" ", 54 + x * 4));
					playerCursor.Text = topPad + leftPad + "┏━━━┓" + "\x0a" + leftPad + "┃   ┃" + "\x0a" + leftPad + "┗━━━┛";
					break;
				case GamePhase.BoardPlanning:
					leftPad = string.Concat(Enumerable.Repeat(" ", 3 + x * 4));
					string newCursor = leftPad;
					int endY = (shipOrientation ? 1 + shipLength * 2 : 3);
					int endX = (shipOrientation ? 5 : 1 + shipLength * 4);
					for (int i = 0; i < endY; i++) // y dimension
					{
						for (int j = 0; j < endX; j++) // x dimension
						{
							if(i==0 && j==0)
							{
								newCursor += (char)ViewData.ShipUpperLeft;
								continue;
							}
							if(i==0 && j == endX-1)
							{
								newCursor += (char)ViewData.ShipUpperRight;
								continue;
							}
							if(i==endY-1 && j == 0)
							{
								newCursor += (char)ViewData.ShipLowerLeft;
								continue;
							}
							if(i == endY - 1 && j == endX-1)
							{
								newCursor += (char)ViewData.ShipLowerRight;
								continue;
							}
							if(i%2==0)
							{
								if (j == 0)
									newCursor += (char)ViewData.ShipVerticalRight;
								else if (j == endX - 1)
									newCursor += (char)ViewData.ShipVerticalLeft;
								else if(i == 0 && j % 4 == 0)
									newCursor += (char)ViewData.ShipHorizontalDown;
								else if (i == endY - 1 && j % 4 == 0)
									newCursor += (char)ViewData.ShipHorizontalUp;
								else
									newCursor += (char)ViewData.ShipHorizontal;
								continue;
							}
							else if(j%4==0)
							{
									newCursor += (char)ViewData.ShipVertical;
								continue;
							}
							newCursor += " ";
						}
						newCursor += "\x0a" + leftPad;
					}
					playerCursor.Text = topPad + newCursor;
					break;
				default:
					playerCursor.Text = "";
					break;
			}
		}
		// o - orientation of ship(true=vertical; false=horizontal)
		/// <summary>
		/// Updates game phase and parameters for Cursor object
		/// </summary>
		/// <param name="phase">Current GamePhase</param>
		/// <param name="length">Length of a current ship</param>
		/// <param name="orientation">Orientation of a current ship</param>
		/// <param name="viablePlace">Is the place on board viable</param>
		public void UpdatePhase(GamePhase phase, int length = 1, bool orientation = false, bool viablePlace = true)
		{
			currentPhase = phase;
			shipLength = length;
		   
			shipOrientation = orientation;
			playerCursor.Foreground = viablePlace ? greenBrush : Brushes.Salmon;
			UpdateCursor();
		}
	}
}
