using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLBattleships
{
	class Coordinate
	{
		public int x = 0, y = 0;
		/// <summary>
		/// Constructor for a Coordinate
		/// </summary>
		/// <param name="_x">X coordinate</param>
		/// <param name="_y">Y coordinate</param>
		public Coordinate(int _x, int _y)
		{
			x = _x;
			y = _y;
		}
	}
}
