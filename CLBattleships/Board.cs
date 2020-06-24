using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLBattleships
{
	enum BoardValue
	{
		Empty,
		Marked,
		Ship,
		Hit,
		Sank,
		Missed,
	}
	enum ViewData
	{
		Empty = ' ',
		ShipUpperLeft = '┏',
		ShipUpperRight = '┓',
		ShipLowerLeft = '┗',
		ShipLowerRight = '┛',
		ShipVertical = '┃',
		ShipVerticalLeft = '┫',
		ShipVerticalRight = '┣',
		ShipHorizontal = '━',
		ShipHorizontalUp = '┻',
		ShipHorizontalDown = '┳',
		Missed = '○',
		Hit = '●',
		Marked = '□'
	}
	/// <summary>
	/// Handles all board logic and calculations
	/// </summary>
	class Board
	{
		private BoardValue[,] BoardData;
		private bool[,] BoardMarkings;
		private bool isComputer;
		/// <summary>
		/// Standard constructor
		/// </summary>
		/// <param name="randomBoard">True, if you want the board to be randomized</param>
		/// <param name="computer">True, if it's computer's board</param>
		public Board(bool randomBoard = false, bool computer = false)
		{
			BoardData = new BoardValue[10, 10];
			BoardMarkings = new bool[10, 10];
			isComputer = computer;
			if(randomBoard)
				RandomizeBoard();
		}
		/// <summary>
		/// Simple "getter" function
		/// </summary>
		/// <returns>BoardData</returns>
		public BoardValue[,] GetBoardValues() => BoardData;
		/// <summary>
		/// Board shooting check
		/// </summary>
		/// <param name="x">X coordinate of the shot</param>
		/// <param name="y">Y coordinate of the shot</param>
		/// <returns>(true=can shoot; false=can't shoot)</returns>
		public bool CanShoot(int x, int y)
		{
			return (int)BoardData[x, y] < 3;
		}
		/// <summary>
		///	Mark a spot on the board
		/// </summary>
		/// <param name="x">X coordinate of the marking</param>
		/// <param name="y">Y coordinate of the marking</param>
		public void MarkSpot(int x, int y)
        {
			BoardMarkings[x, y] = !BoardMarkings[x, y];
        }
		/// <summary>
		/// Simple check to determine if ship at (x, y) is vertical
		/// </summary>
		/// <param name="x">X coordinate of the ship</param>
		/// <param name="y">Y coordinate of the ship</param>
		/// <returns>(true=vertical; false=horizontal)</returns>
		bool IsVertical(int x, int y)
		{
			// Can simplify, left for clarity
			if (y - 1 != -1 && (BoardData[x, y - 1] == BoardValue.Hit || BoardData[x, y - 1] == BoardValue.Ship))
				return true;
			if (y + 1 != 10 && (BoardData[x, y + 1] == BoardValue.Hit || BoardData[x, y + 1] == BoardValue.Ship))
				return true;
			return false;
		}
		/// <summary>
		/// Function for shooting on board, calculates sinking of a ship, and provides state after shot
		/// </summary>
		/// <param name="x">X coordinate of the shot</param>
		/// <param name="y">Y coordinate of the shot</param>
		/// <returns>State of a cell at (x,y) after shot</returns>
		public BoardValue Shoot(int x, int y)
		{
			if((BoardData[x, y] = BoardData[x, y] == BoardValue.Empty ? BoardValue.Missed : BoardValue.Hit) == BoardValue.Hit)
			{
				int i, j;
				BoardValue boardPointer;
                List<Coordinate> pastCoords = new List<Coordinate>
                {
                    new Coordinate(x, y)
                };
                bool vertical = IsVertical(x, y);
				bool wholeShip = true;
				if (vertical)
				{
					i = y - 1;
					j = y + 1;
				}
				else
				{
					i = x - 1;
					j = x + 1;
				}
				while (true)
				{
					if (i >= 0)
					{
						boardPointer = vertical ? BoardData[x, i] : BoardData[i, y];
						if (boardPointer == BoardValue.Ship)
						{
							wholeShip = false;
							break;
						}
						else if(boardPointer == BoardValue.Missed || boardPointer == BoardValue.Empty)
						{
							i = -1;
						} else
						{
							pastCoords.Add(vertical ? new Coordinate(x, i) : new Coordinate(i, y));
							i--;
						}
					}
					if (j <= 9)
					{
						boardPointer = vertical ? BoardData[x, j] : BoardData[j, y];
						if (boardPointer == BoardValue.Ship)
						{
							wholeShip = false;
							break;
						}
						else if (boardPointer == BoardValue.Missed || boardPointer == BoardValue.Empty)
						{
							j = 10;
						}
						else
						{
							pastCoords.Add(vertical ? new Coordinate(x, j) : new Coordinate(j, y));
							j++;
						}
					}
					if (i < 0 && j > 9) break;
				}
				
				if (wholeShip)
				{
					foreach (Coordinate c in pastCoords)
					{
						BoardData[c.x, c.y] = BoardValue.Sank;
					}
				}

			}
			
			return BoardData[x, y];
		}
		/// <summary>
		/// Checks if given placement of the ship is viable
		/// </summary>
		/// <param name="_x">X coordinate of the ship</param>
		/// <param name="_y">Y coordinate of the ship</param>
		/// <param name="l">Length of the ship(1<=l<=4)</param>
		/// <param name="o">Orientation of the ship(true=vertical; false=horizontal)</param>
		/// <returns>(true=viable; false=not viable)</returns>
		public bool IsPlaceViable(int _x, int _y, int l, bool o)
		{
			for (int x = Math.Max(0, _x - 1); x <= (o ? _x + 1 : _x + l); x++)
				for (int y = Math.Max(0, _y - 1); y <= (o ? _y + l : _y + 1); y++)
					if (x>10 || y>10 || (x < 10 && y < 10 && BoardData[x, y] == BoardValue.Ship))
						return false;
			return true;
		}
		/// <summary>
		/// Allows for a ship placement on the board given the specification
		/// </summary>
		/// <param name="_x">X coordinate of the ship</param>
		/// <param name="_y">Y coordinate of the ship</param>
		/// <param name="l">Length of the ship(1<=l<=4)</param>
		/// <param name="o">Orientation of the ship(true=vertical; false=horizontal)</param>
		public void PlaceShipOnBoard(int _x, int _y, int l, bool o)
		{
			for (int x = _x; x < (o ? _x + 1 : _x + l); x++)
				for (int y = _y; y < (o ? _y + l : _y + 1); y++)
					BoardData[x, y] = BoardValue.Ship;
		}
		/// <summary>
		/// Randomize current board
		/// </summary>
		private void RandomizeBoard()
		{
			var rand = new Random();
			for (int i = 4; i > 0; i--)
			{
				for(int j = 0; j <= 4-i; j++)
				{
					int x, y;
					bool o;
					do
					{
						x = rand.Next(0, 10);
						y = rand.Next(0, 10);
						o = rand.Next(0, 2) == 0;
					} while (!IsPlaceViable(x, y, i, o));
					PlaceShipOnBoard(x, y, i, o);
				}
			}
		}
		/// <summary>
		/// Checks if given (x, y) is a ship
		/// </summary>
		/// <param name="x">X coordinate of the check</param>
		/// <param name="y">Y coordinate of the check</param>
		/// <returns></returns>
		private bool IsShip(int x, int y)
		{
			// Ship=2,
			// Hit=3,
			// Sank=4,
			return x >= 0 && y >= 0 && x <= 9 && y <= 9 && (int)BoardData[x, y] > 1 && (int)BoardData[x, y] < 5;
		}
		/// <summary>
		/// Generate the view data to fill game view
		/// </summary>
		/// <returns>ViewData array with board characters</returns>
		public ViewData[,] GenerateViewData()
		{
			ViewData[,] data = new ViewData[44, 21];
			for (int x = 0; x < 10; x++)
			{
				for (int y = 0; y < 10; y++)
				{
					int xOffset = 3 + x * 4;
					int yOffset = y * 2;
					if ((!isComputer && (BoardData[x, y] == BoardValue.Ship || BoardData[x, y] == BoardValue.Hit)) || BoardData[x, y] == BoardValue.Sank)
					{
						data[xOffset, yOffset] = IsShip(x - 1, y) ? ViewData.ShipHorizontalDown : IsShip(x, y - 1) ? ViewData.ShipVerticalRight : ViewData.ShipUpperLeft;
						data[xOffset + 4, yOffset] = IsShip(x, y - 1) ? ViewData.ShipVerticalLeft : ViewData.ShipUpperRight;
						data[xOffset, yOffset + 2] = IsShip(x - 1, y) ? ViewData.ShipHorizontalUp : ViewData.ShipLowerLeft;
						data[xOffset + 4, yOffset + 2] = ViewData.ShipLowerRight;
						data[xOffset, yOffset + 1] = data[xOffset + 4, yOffset + 1] = ViewData.ShipVertical;
						data[xOffset + 1, yOffset] = data[xOffset + 2, yOffset] = data[xOffset + 3, yOffset] = data[xOffset + 1, yOffset + 2] = data[xOffset + 2, yOffset + 2] = data[xOffset + 3, yOffset + 2] = ViewData.ShipHorizontal;
					}

					if (BoardData[x, y] == BoardValue.Hit || BoardData[x, y] == BoardValue.Sank)
						data[xOffset + 2, yOffset + 1] = ViewData.Hit;
					else if (BoardData[x, y] == BoardValue.Missed)
						data[xOffset + 2, yOffset + 1] = ViewData.Missed;
					else if (BoardMarkings[x, y] && (int)BoardData[x, y] < 3)
						data[xOffset + 2, yOffset + 1] = ViewData.Marked;
				}
			}
			return data;
		}
	}
}
