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

        private Game game = null;
        private Match newGameMatch = null;
        private string newGameDomain;

        private bool GameStarted
        {
            get { return game != null; }
        }

        private VotesWorker worker;

        public MainWindow()
        {
            InitializeComponent();

            this.worker = new VotesWorker(this);
            this.Closing += (s, e) => worker.CancelAsync();

            this.table.CardSelected += table_CardSelected;

            this.newGameDomain = domain.Content as string;
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

            description_hint.Visibility = System.Windows.Visibility.Visible;
        }
        void description_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {

                game.Description = description.Text;
                editingDescription = false;
                Keyboard.ClearFocus();
            }
            else if (e.Key == Key.Escape)
            {

                description.Text = game.Description;
                editingDescription = false;
                Keyboard.ClearFocus();
            }
        }

        void issuetitle_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            editingTitle = true;

        }
        void issuetitle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {

                game.Title = issuetitle.Text;
                editingTitle = false;
                Keyboard.ClearFocus();
            }
            else if (e.Key == Key.Escape)
            {

                issuetitle.Text = game.Title;
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

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (creategame == null || gameid == null)
                return;

            string usr = username.Text.Trim();
            if (usr.Length == 0)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                game_errormessage.Content = "You must supply a username...";
                return;
            }

            if (newGameDomain == null)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                game_errormessage.Content = "You must select a game domain...";
                return;
            }

            string gameURL = newGameDomain + gameid.Text.Trim();
            Match m = Regex.Match(gameURL, @"^(?<domain>http://.*/)game(/(?<id>[a-z0-9]{32}))?/?$");
            if (!m.Success)
            {
                creategame.Content = "";
                creategame.IsEnabled = false;
                game_errormessage.Content = "You must supply a valid game ID...";
                return;
            }
            else if (!m.Groups["id"].Success)
            {
                creategame.Content = "Create";
                creategame.IsEnabled = true;
                newGameMatch = m;
                game_errormessage.Content = "";
                return;
            }
            else
            {
                if (Game.Exists(m.Groups["domain"].Value, new Id(m.Groups["id"].Value)))
                {
                    creategame.Content = "Join";
                    creategame.IsEnabled = true;
                    newGameMatch = m;
                    game_errormessage.Content = "";
                    return;
                }
                else
                {
                    creategame.Content = "";
                    creategame.IsEnabled = false;
                    game_errormessage.Content = "You must supply a valid game ID...";
                    return;
                }
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

            userinfo_grid.Visibility = System.Windows.Visibility.Collapsed;
            this.domain.Content = "Planning @ " + domain;
            this.domain_explanation.Content = "Click to copy the game ID to clipboard...";

            gamegrid.Visibility = System.Windows.Visibility.Visible;

            worker.RunWorkerAsync();
        }

        private void domain_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender != domain)
            {
                newGameDomain = (sender as Label).Content as string;
                domain.Content = newGameDomain;

                login_TextChanged(null, null);

                domain_display.Visibility = System.Windows.Visibility.Visible;
                userinfo_grid.Visibility = System.Windows.Visibility.Visible;
                domain_list.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }

            if (!GameStarted)
            {
                domain_display.Visibility = System.Windows.Visibility.Collapsed;
                userinfo_grid.Visibility = System.Windows.Visibility.Collapsed;
                domain_list.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Clipboard.SetText(game.Id.Hash);
            }
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
                        bool somethingChanged = false;
                        var newVotes = e.UserState as Vote[];
                        Array.Sort(newVotes, (x, y) => x.Name.CompareTo(y.Name));

                        int n = 0, o = 0;
                        while (n < newVotes.Length || o < main.votes.Items.Count)
                        {
                            if (n >= newVotes.Length)
                            {
                                somethingChanged = true;
                                while (o < main.votes.Items.Count)
                                    main.votes.Items.RemoveAt(o);
                                break;
                            }
                            else if (o >= main.votes.Items.Count)
                            {
                                somethingChanged = true;
                                for (; n < newVotes.Length; n++)
                                    main.votes.Items.Add(new WpfVote(newVotes[n]));
                                break;
                            }
                            else
                            {
                                var nV = newVotes[n];
                                var oV = main.votes.Items[o] as WpfVote;

                                int cmp = nV.Name.CompareTo(oV.UserName);
                                if (cmp < 0)
                                {
                                    main.votes.Items.Insert(o, new WpfVote(newVotes[n]));
                                    n++; o++;
                                }
                                else if (cmp > 0)
                                {
                                    main.votes.Items.RemoveAt(o);
                                }
                                else
                                {
                                    if (!nV.HasVoted && oV.HasVoted)
                                    {
                                        oV.ClearVote();
                                        somethingChanged = true;
                                    }
                                    else if (nV.HasVoted && (!oV.HasVoted || nV.VoteType != oV.VoteType))
                                    {
                                        oV.VoteType = nV.VoteType;
                                        somethingChanged = true;
                                    }

                                    n++; o++;
                                }
                            }
                        }

                        if (somethingChanged)
                            main.votes.ItemTemplateSelector = new VoteDataTemplateSelector();

                        var me = (from WpfVote v in main.votes.Items where v.UserName == main.game.User.Name select v).First();
                        main.table.Visibility = me.HasVoted ? Visibility.Collapsed : Visibility.Visible;

                        var allvoted = (from WpfVote v in main.votes.Items where !v.HasVoted select v).Any();

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

        private void votes_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (votes.SelectedItem == null)
            {
                e.Handled = true;
            }
            else
            {
                var user = votes.SelectedItem as WpfVote;
                kick_menuitem.Header = "Kick " + user.UserName;
                kick_menuitem.Tag = user;

                if (user.UserName == game.User.Name)
                    e.Handled = true;
            }
        }

        private void kick_Click(object sender, RoutedEventArgs e)
        {
            var vote = kick_menuitem.Tag as WpfVote;
            if (vote != null)
                game.Kick(vote.UserName);
        }
    }
}
