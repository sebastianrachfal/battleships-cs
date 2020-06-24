using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLBattleships
{
	class ComputerLogic
	{
		// difficulty?

		Random random;
		/// <summary>
		/// Generates next move for computer
		/// </summary>
		/// <param name="boardData">BoardValue array with board data</param>
		/// <returns>Coordinate object</returns>
		public Coordinate GenerateNextMove(BoardValue[,] boardData)
		{
			random = new Random();
			int x, y;
			do
			{
				x = random.Next(0, 10);
				y = random.Next(0, 10);
			} while ((int)boardData[x,y] > 2);
			return new Coordinate(x, y);
		}
		/// <summary>
		/// Generates a "witty" response to a player move
		/// </summary>
		/// <param name="player">Players's BoardValue after shot</param>
		/// <param name="computer">Computer's BoardValue after shot</param>
		/// <returns></returns>
		public string GetComputerResponse(BoardValue player, BoardValue computer)
        {
			switch(player)
            {
				case BoardValue.Hit:
                    switch (computer)
                    {
						case BoardValue.Hit: return "Wygląda na to, że oboje trafiliśmy statek.";
						case BoardValue.Missed: return "Trafiłeś mój statek! To nie skończy się dobrze..";
						case BoardValue.Sank: return "Haha, trafiłeś mój statek, ale ja twój zatopiłem!";
                    }
					break;
				case BoardValue.Missed:
					switch (computer)
					{
						case BoardValue.Hit: return "Haha, ja trafłem! Spróbuj następnym razem!";
						case BoardValue.Missed: return "Wygląda na to, że oboje spudłowaliśmy.";
						case BoardValue.Sank: return "Trafiony zatopiony! Musisz się bardziej postarać.";
					}
					break;
				case BoardValue.Sank:
					switch (computer)
					{
						case BoardValue.Hit: return "Gdzie się podział mój statek?! Twój tylko lekko uszkodziłem.";
						case BoardValue.Missed: return "Trafiłeś, zatopiłeś.. Następnym razem cię pokonam!";
						case BoardValue.Sank: return "Wygląda na to, że oboje straciliśmy statki.";
					}
					break;
			}
			return "";
        }
	}
}
