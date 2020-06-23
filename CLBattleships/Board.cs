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
		Marked = '■'
	}
	class Board
	{
		private BoardValue[,] BoardData;
		private bool[,] BoardMarkings;
		private bool isComputer;
		public Board(bool randomBoard = false, bool computer = false)
		{
			BoardData = new BoardValue[10, 10];
			BoardMarkings = new bool[10, 10];
			isComputer = computer;
			if(randomBoard)
				RandomizeBoard();
		}
		public BoardValue[,] GetBoardValues() => BoardData;
		public bool CanShoot(int x, int y)
		{
			return (int)BoardData[x, y] < 3;
		}
		public void MarkSpot(int x, int y)
        {
			BoardMarkings[x, y] = !BoardMarkings[x, y];
        }
		bool IsVertical(int x, int y)
		{
			if (y - 1 != -1 && (BoardData[x, y - 1] == BoardValue.Hit || BoardData[x, y - 1] == BoardValue.Ship))
				return true;
			if (y + 1 != 10 && (BoardData[x, y + 1] == BoardValue.Hit || BoardData[x, y + 1] == BoardValue.Ship))
				return true;
			return false;
		}
		public BoardValue Shoot(int x, int y)
		{
			if((BoardData[x, y] = BoardData[x, y] == BoardValue.Empty ? BoardValue.Missed : BoardValue.Hit) == BoardValue.Hit)
			{
				int i = 0, j = 0;
				BoardValue boardPointer;
				List<Coordinate> pastCoords = new List<Coordinate>();
				pastCoords.Add(new Coordinate(x, y));
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
		//todo docs
		// (_x,_y) - coordinates
		// l - length of ship
		// o - orientation of ship(true=vertical; false=horizontal)
		public bool IsPlaceViable(int _x, int _y, int l, bool o)
		{
			for (int x = Math.Max(0, _x - 1); x <= (o ? _x + 1 : _x + l); x++)
				for (int y = Math.Max(0, _y - 1); y <= (o ? _y + l : _y + 1); y++)
					if (x>10 || y>10 || (x < 10 && y < 10 && BoardData[x, y] == BoardValue.Ship))
						return false;
			return true;
		}
		public void PlaceShipOnBoard(int _x, int _y, int l, bool o)
		{
			for (int x = _x; x < (o ? _x + 1 : _x + l); x++)
				for (int y = _y; y < (o ? _y + l : _y + 1); y++)
					BoardData[x, y] = BoardValue.Ship;
		}
		private void RandomizeBoard()
		{
			var rand = new Random();
			for (int i = 4; i > 0; i--)
			{
				for(int j = 0; j <= 4-i; j++)
				{
					int x = 0, y = 0;
					bool o = false;
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
		private bool IsShip(int x, int y)
		{
			// Ship=2,
			// Hit=3,
			// Sank=4,

			// WARNING: Only checking toward upper left corner for simplicity

			return x >= 0 && y >= 0 && x <= 9 && y <= 9 && (int)BoardData[x, y] > 1 && (int)BoardData[x, y] < 5;
		}
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
