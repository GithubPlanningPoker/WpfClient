using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for CardTable.xaml
    /// </summary>
    public partial class CardTable : UserControl
    {
        public CardTable()
        {
            InitializeComponent();

            scroller.MouseMove += scroller_MouseMove;
        }

        public event EventHandler<CardSelectedEventArgs> CardSelected;

        void scroller_MouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(fullgrid).X - scroller.Margin.Left - 30;
            double scale = p / scroller.ActualWidth;
            if (double.IsNaN(scale))
                return;

            double multBy = scroller.ExtentWidth - scroller.ActualWidth + 60;

            scroller.ScrollToHorizontalOffset(multBy * scale);
        }

        private void Card_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            var card = sender as Card;
            if (CardSelected != null)
                CardSelected(this, new CardSelectedEventArgs(card.APIValue));
        }
    }
}
