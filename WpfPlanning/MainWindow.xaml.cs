using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfPlanning
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game game = null;
        private Match newGameMatch = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
            base.OnMouseDown(e);
        }

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {
            string usr = username.Text.Trim();
            if (usr.Length == 0)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                return;
            }

            string gameURL = url.Text.Trim();
            Match m = Regex.Match(gameURL, @"^(?<domain>http://.*/)game(/(?<id>[a-z0-9]{64}))?/?$");
            if (!m.Success)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                return;
            }
            else if (!m.Groups["id"].Success)
            {
                creategame.Content = "Create";
                creategame.IsEnabled = true;
                newGameMatch = m;
                return;
            }
            else
            {
                creategame.Content = "Join";
                creategame.IsEnabled = true;
                newGameMatch = m;
                return;
            }
        }
        private void login_KeyUp(object sender, KeyEventArgs e)
        {
            if (creategame.IsEnabled)
                if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    creategame_Click(null, null);
        }

        private void creategame_Click(object sender, RoutedEventArgs e)
        {
            string domain = newGameMatch.Groups["domain"].Value;
            string id = newGameMatch.Groups["id"].Value;
            string usr = username.Text.Trim();

            if (id.Length == 0)
                game = Game.CreateGame(domain, usr);
            else
                game = Game.JoinGame(domain, new Id(id), usr);

            title.Content = "Planning @ " + domain;
            title.IsEnabled = true;

            login.Visibility = System.Windows.Visibility.Collapsed;
            gamegrid.IsEnabled = true;
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(game.Id.Hash);
        }
    }
}
