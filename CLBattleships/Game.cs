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
	enum GamePhase
	{
		MainMenu,
		BoardPlanning,
		Game,
		End
	}
	/// <summary>
	/// Main endpoint for app logic
	/// </summary>
	public class Game
	{
		private RichTextBox GameScreen;
		private Board PlayerBoard, ComputerBoard;
		private GamePhase CurrentPhase;
		private Cursor PlayerCursor;
		private ComputerLogic ComputerBrain;
		private Run ComputerText;

		// Board planning phase
		private int currentShipLength = 4;
		private bool currentShipOrientation = false;
		private bool currentPlacementViable = false;
		private int shipsLeftToPlace = 1;

		// Game phase
		private int playerScore = 0;
		private int computerScore = 0;

		/// <summary>
		/// Constructor for Game class, prepares board for gameplay
		/// </summary>
		/// <param name="gameScreen">RichTextBox for a playing board</param>
		/// <param name="playerCursor">Run for player cursor</param>
		/// <param name="computerText">Run for computer sayings</param>
		public Game(RichTextBox gameScreen, Run playerCursor, Run computerText)
		{
			GameScreen = gameScreen;
			PlayerBoard = new Board();
			ComputerBoard = new Board(true, true);
			PlayerCursor = new Cursor(playerCursor);
			ComputerBrain = new ComputerLogic();
			ComputerText = computerText;
			ChangePhase(GamePhase.BoardPlanning); // Temporary: development purposes
			UpdateScreen();
		}
		/// <summary>
		/// Reset the game after round ends
		/// </summary>
		private void ResetGame()
        {
			PlayerBoard = new Board();
			ComputerBoard = new Board(true, true);
			ComputerBrain = new ComputerLogic();
			PlayerCursor.X = PlayerCursor.Y = 0;
			ChangePhase(GamePhase.BoardPlanning);
			currentShipLength = 4;
			currentShipOrientation = false;
			currentPlacementViable = false;
			shipsLeftToPlace = 1;
			playerScore = 0;
			computerScore = 0;
			UpdateScreen();
			PostComputerMessage("No cóż, czas na kolejną rozgrywkę, ustawiaj statki!");
		}
		/// <summary>
		/// Change the phase of the game(look GamePhase enum)
		/// </summary>
		/// <param name="phase">One of the different game phases</param>
		private void ChangePhase(GamePhase phase)
		{
			CurrentPhase = phase;
			PlayerCursor.UpdatePhase(phase, 4, false);
		}
		/// <summary>
		/// Simple keyboard event handler
		/// </summary>
		/// <param name="key">Key pressed</param>
		public void KeyPressed(Key key)
		{
			switch(key)
			{
				case Key.Left:
					PlayerCursor.X = Math.Max(PlayerCursor.X - 1, 0);
					break;
				case Key.Right:
					PlayerCursor.X = Math.Min(PlayerCursor.X + 1, (CurrentPhase == GamePhase.BoardPlanning && !currentShipOrientation) ? 10-currentShipLength : 9);
					break;
				case Key.Up:
					PlayerCursor.Y = Math.Max(PlayerCursor.Y - 1, 0);
					break;
				case Key.Down:
					PlayerCursor.Y = Math.Min(PlayerCursor.Y + 1, (CurrentPhase == GamePhase.BoardPlanning && currentShipOrientation) ? 10 - currentShipLength : 9);
					break;
				case Key.E:
					if (CurrentPhase == GamePhase.Game && currentPlacementViable)
                    {
						ComputerBoard.MarkSpot(PlayerCursor.X, PlayerCursor.Y);
						UpdateScreen();
                    }
					break;
				case Key.R:
					if(CurrentPhase == GamePhase.BoardPlanning)
					{
						if (currentShipOrientation && PlayerCursor.X > 10-currentShipLength)
						{
							PlayerCursor.X = 10 - currentShipLength;
						} else if(!currentShipOrientation && PlayerCursor.Y > 10 - currentShipLength)
						{
							PlayerCursor.Y = 10 - currentShipLength;
						}
						currentShipOrientation = !currentShipOrientation;
					}
					break;
				case Key.Enter:
					if(CurrentPhase == GamePhase.End)
                    {
						Console.WriteLine("Game reset");
						ResetGame();
                    }
					break;
				case Key.Space:
					if(CurrentPhase == GamePhase.BoardPlanning && currentPlacementViable)
					{
						PlayerBoard.PlaceShipOnBoard(PlayerCursor.X, PlayerCursor.Y, currentShipLength, currentShipOrientation);
						UpdateScreen();
						if (--shipsLeftToPlace==0)
						{
							if (--currentShipLength<=0)
							{
								PlayerCursor.X = PlayerCursor.Y = 0;
								ChangePhase(GamePhase.Game);
								return;
							}
							shipsLeftToPlace = 5 - currentShipLength;
						}
					} else if(CurrentPhase == GamePhase.Game && currentPlacementViable)
					{
						Coordinate computerMove = ComputerBrain.GenerateNextMove(PlayerBoard.GetBoardValues());
						BoardValue playerResult = ComputerBoard.Shoot(PlayerCursor.X, PlayerCursor.Y);
						BoardValue computerResult = PlayerBoard.Shoot(computerMove.x, computerMove.y);

						if (playerResult == BoardValue.Hit || playerResult == BoardValue.Sank) playerScore++;
						if (computerResult == BoardValue.Hit || computerResult == BoardValue.Sank) computerScore++;

						if (playerScore == 20 || computerScore == 20)
						{
							Console.WriteLine("End of the game!!");
							string result = computerScore == playerScore ? "Remis" : playerScore == 20 ? "Wygrałeś" : "Wygrałem";
							PostComputerMessage($"{result}! Twoj wynik: {100 + 5 * (playerScore - computerScore)}([ENTER], aby zagrać ponownie)");
							ChangePhase(GamePhase.End);
						}
						else PostComputerMessage(ComputerBrain.GetComputerResponse(playerResult, computerResult));
						
						UpdateScreen();
					}
					break;
			}

			if (CurrentPhase == GamePhase.BoardPlanning)
			{
				currentPlacementViable = PlayerBoard.IsPlaceViable(PlayerCursor.X, PlayerCursor.Y, currentShipLength, currentShipOrientation);
				PlayerCursor.UpdatePhase(GamePhase.BoardPlanning, currentShipLength, currentShipOrientation, currentPlacementViable);
			} else if(CurrentPhase == GamePhase.Game)
			{
				currentPlacementViable = ComputerBoard.CanShoot(PlayerCursor.X, PlayerCursor.Y);
				PlayerCursor.UpdatePhase(GamePhase.Game, 1, false, currentPlacementViable);
			}
		}
		/// <summary>
		/// A function that updates most of the content on the screen
		/// </summary>
		private void UpdateScreen()
		{
			ViewData[,] playerData = PlayerBoard.GenerateViewData();
			ViewData[,] computerData = ComputerBoard.GenerateViewData();
			FlowDocument a = new FlowDocument(new Paragraph());
			a.Blocks.Add(new Paragraph());
			for (int y = 0; y < 21; y++)
			{
				Paragraph p = new Paragraph();
                Run r = new Run
                {
                    Text = "       "
                };
                p.Foreground = Brushes.DodgerBlue;
				r.Foreground = Brushes.DarkGoldenrod;
				for (int x = 0; x < 44; x++)
				{
					p.Inlines.Add("" + (char)playerData[x, y]);
					r.Text += "" + (char)computerData[x, y];
				}
				p.Inlines.Add(r);
				a.Blocks.Add(p);
			}

			GameScreen.Document = a;
		}
		/// <summary>
		/// Simple function to post computer's messages
		/// </summary>
		/// <param name="message">Your message</param>
		private void PostComputerMessage(string message)
        {
			ComputerText.Text = message;
        }
	}
}
