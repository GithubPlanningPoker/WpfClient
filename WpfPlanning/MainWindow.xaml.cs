using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private bool editingDescription = false;
        private bool editingTitle = false;

        private Brush initialBorder;
        private Thickness initialBorderThickness;

        private Game game = null;
        private Match newGameMatch = null;

        private VotesWorker worker;

        public MainWindow()
        {
            InitializeComponent();

            this.worker = new VotesWorker(this);
            this.Closing += (s, e) => worker.CancelAsync();

            this.initialBorder = description_border.BorderBrush;
            this.initialBorderThickness = description_border.BorderThickness;

            this.table.CardSelected += table_CardSelected;

            username.Text = Properties.Settings.Default.username;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Properties.Settings.Default.username = username.Text;
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }

        void table_CardSelected(object sender, CardSelectedEventArgs e)
        {
            game.Vote(e.VoteType);
        }

        void description_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            editingDescription = true;

            description_border.BorderBrush = Brushes.Orange;
            description_border.BorderThickness = new Thickness(2);
        }
        void description_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                description_border.BorderBrush = initialBorder;
                description_border.BorderThickness = initialBorderThickness;

                game.Description = description.Text;
                editingDescription = false;
                Keyboard.ClearFocus();
            }
            else if (e.Key == Key.Escape)
            {
                description_border.BorderBrush = initialBorder;
                description_border.BorderThickness = initialBorderThickness;

                description.Text = game.Description;
                editingDescription = false;
                Keyboard.ClearFocus();
            }
        }

        void issuetitle_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            editingTitle = true;

            issuetitle_border.BorderBrush = Brushes.Orange;
            issuetitle_border.BorderThickness = new Thickness(2);
        }
        void issuetitle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                issuetitle_border.BorderBrush = initialBorder;
                issuetitle_border.BorderThickness = initialBorderThickness;

                game.Title = issuetitle.Text;
                editingTitle = false;
                Keyboard.ClearFocus();
            }
            else if (e.Key == Key.Escape)
            {
                issuetitle_border.BorderBrush = initialBorder;
                issuetitle_border.BorderThickness = initialBorderThickness;

                issuetitle.Text = game.Description;
                editingTitle = false;
                Keyboard.ClearFocus();
            }
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
            base.OnMouseDown(e);
        }

        private void domain_TextInput(object sender, TextCompositionEventArgs e)
        {
            login_TextChanged(null, null);
        }
        private void domain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            login_TextChanged(null, null);
        }

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (creategame == null || domain == null || gameid == null)
                return;

            string usr = username.Text.Trim();
            if (usr.Length == 0)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                return;
            }

            var item = (domain.SelectedItem as ListBoxItem);
            string selectedDomain = item == null ? domain.Text as string : (item.Content as string);
            if (selectedDomain == null)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                return;
            }

            string gameURL = selectedDomain + gameid.Text.Trim();
            Match m = Regex.Match(gameURL, @"^(?<domain>http://.*/)game(/(?<id>[a-z0-9]{32}))?/?$");
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

            btnClear.Visibility = game.Host ? Visibility.Visible : Visibility.Collapsed;

            title.Content = "Planning @ " + domain;
            title.IsEnabled = true;

            login.Visibility = System.Windows.Visibility.Collapsed;
            gamegrid.IsEnabled = true;

            worker.RunWorkerAsync();
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(game.Id.Hash);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            game.ClearVotes();
        }

        private void github_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("GITHUB!");
            game.ResetGame();
        }

        private class VotesWorker : BackgroundWorker
        {
            private MainWindow main;

            public VotesWorker(MainWindow main)
                : base()
            {
                this.main = main;

                base.WorkerReportsProgress = true;
                base.WorkerSupportsCancellation = true;
            }

            protected override void OnDoWork(DoWorkEventArgs e)
            {
                while (!e.Cancel)
                {
                    var newVotes = main.game.Votes.ToArray();
                    ReportProgress(0, newVotes);
                    var desc = main.game.Description;
                    ReportProgress(1, desc);
                    var title = main.game.Title;
                    ReportProgress(2, title);
                    System.Threading.Thread.Sleep(1000);
                }
            }
            protected override void OnProgressChanged(ProgressChangedEventArgs e)
            {
                switch (e.ProgressPercentage)
                {
                    case 0:
                        var newVotes = e.UserState as Vote[];

                        main.votes.Items.Clear();

                        bool allvoted = true;
                        foreach (var v in newVotes)
                        {
                            if (v.Name == main.game.User.Name)
                                main.table.Visibility = v.HasVoted ? Visibility.Collapsed : Visibility.Visible;

                            if (!v.HasVoted)
                                allvoted = false;

                            main.votes.Items.Add(new WpfVote(v));
                        }

                        if (allvoted && main.game.Host)
                            main.github.Visibility = Visibility.Visible;
                        else
                            main.github.Visibility = Visibility.Collapsed;

                        break;

                    case 1:
                        if (!main.editingDescription)
                        {
                            main.description.Text = e.UserState as string;
                        }
                        break;

                    case 2:
                        if (!main.editingTitle)
                        {
                            main.issuetitle.Text = e.UserState as string;
                        }
                        break;
                }
            }
        }
    }
}
