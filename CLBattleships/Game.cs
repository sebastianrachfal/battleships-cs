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
        Game
    }
    public class Game
    {
        private RichTextBox GameScreen;
        private Board PlayerBoard, ComputerBoard;
        private GamePhase CurrentPhase;
        private Cursor PlayerCursor;

        // Board planning phase
        private int currentShipLength = 4;
        private bool currentShipOrientation = false;
        private bool currentPlacementViable = false;
        private int shipsLeftToPlace = 1;
        public Game(RichTextBox gameScreen, Run playerCursor)
        {
            GameScreen = gameScreen;
            PlayerBoard = new Board();
            ComputerBoard = new Board(true);
            PlayerCursor = new Cursor(playerCursor);
            ChangePhase(GamePhase.BoardPlanning); // Temporary: development purposes
            UpdateScreen();
        }
        private void ChangePhase(GamePhase phase)
        {
            CurrentPhase = phase;
            PlayerCursor.UpdatePhase(phase, 4, false);
        }
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
                        BoardValue result = ComputerBoard.Shoot(PlayerCursor.X, PlayerCursor.Y);
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
        private void UpdateScreen()
        {
            ViewData[,] playerData = PlayerBoard.GenerateViewData();
            ViewData[,] computerData = ComputerBoard.GenerateViewData();
            FlowDocument a = new FlowDocument(new Paragraph());
            a.Blocks.Add(new Paragraph());
            for (int y = 0; y < 21; y++)
            {
                Paragraph p = new Paragraph();
                Run r = new Run();
                r.Text = "       ";
                p.Foreground = Brushes.DodgerBlue;
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
    }
}
