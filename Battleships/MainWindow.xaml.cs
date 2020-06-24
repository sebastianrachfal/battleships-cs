using CLBattleships;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
	/// <summary>
	/// Logika interakcji dla klasy MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Game GameHandler;
		public MainWindow()
		{
			InitializeComponent();
            GameHandler = new Game(FindName("gameScreen") as RichTextBox, FindName("playerCursor") as Run, FindName("computerText") as Run, FindName("mainMenu") as RichTextBox, FindName("information") as RichTextBox);
		}
		private void WindowLoaded(object sender, RoutedEventArgs e)
		{
			this.KeyDown += new KeyEventHandler(MainWindowKeyDown);
		}
		void MainWindowKeyDown(object sender, KeyEventArgs e)
		{
			GameHandler.KeyPressed(e.Key);
		}
	}
}
