﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLBattleships
{
    enum BoardValue
    {
        Empty,
        Ship
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
        ShipHorizontalDown = '┳'
    }
    class Board
    {
        private BoardValue[,] BoardData;
        public Board()
        {
            BoardData = new BoardValue[10, 10];
            RandomizeBoard();
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
        private void PlaceShipOnBoard(int _x, int _y, int l, bool o)
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
        public ViewData[,] GenerateViewData()
        {
            ViewData[,] data = new ViewData[44, 21];
            for (int x = 0; x < 10; x++)
            {
                for(int y = 0; y < 10; y++)
                {
                    if(BoardData[x, y] == BoardValue.Ship)
                    {
                        int xOffset = 3 + x * 4;
                        int yOffset = y * 2;

                        data[xOffset, yOffset] = (x != 0 && BoardData[x - 1, y] == BoardValue.Ship) ? ViewData.ShipHorizontalDown : (y != 0 && BoardData[x, y-1] == BoardValue.Ship) ? ViewData.ShipVerticalRight :  ViewData.ShipUpperLeft;
                        data[xOffset + 4, yOffset] = (y != 0 && BoardData[x, y - 1] == BoardValue.Ship) ? ViewData.ShipVerticalLeft : ViewData.ShipUpperRight;
                        data[xOffset, yOffset + 2] = (x != 0 && BoardData[x - 1, y] == BoardValue.Ship) ? ViewData.ShipHorizontalUp : ViewData.ShipLowerLeft;
                        data[xOffset + 4, yOffset + 2] = ViewData.ShipLowerRight;
                        data[xOffset, yOffset + 1] = data[xOffset + 4, yOffset + 1] = ViewData.ShipVertical;
                        data[xOffset + 1, yOffset] = data[xOffset + 2, yOffset] = data[xOffset + 3, yOffset] = data[xOffset + 1, yOffset + 2] = data[xOffset + 2, yOffset + 2] = data[xOffset + 3, yOffset + 2] = ViewData.ShipHorizontal;
                    }
                }
            }
            return data;
        }
    }
}