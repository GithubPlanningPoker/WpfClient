using Library;
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
    /// Interaction logic for Card.xaml
    /// </summary>
    public partial class Card : UserControl
    {
        public Card()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty CardValueProperty = DependencyProperty.Register(
            "CardValue",
            typeof(VoteTypes),
            typeof(Card),
            new PropertyMetadata(VoteTypes.Zero));

        public VoteTypes DisplayedValue
        {
            get { return (VoteTypes)this.GetValue(CardValueProperty); }
            set
            {
                this.SetValue(CardValueProperty, value);
                this.label.Content = getVisual(value);
            }
        }

        private string getVisual(VoteTypes? votetype)
        {
            if (!votetype.HasValue)
                return "";

            switch (votetype.Value)
            {
                case VoteTypes.Zero: return "0";
                case VoteTypes.Half: return "½";
                case VoteTypes.One: return "1";
                case VoteTypes.Two: return "2";
                case VoteTypes.Three: return "3";
                case VoteTypes.Five: return "5";
                case VoteTypes.Eight: return "8";
                case VoteTypes.Thirteen: return "13";
                case VoteTypes.Twenty: return "20";
                case VoteTypes.Fourty: return "40";
                case VoteTypes.OneHundred: return "100";
                case VoteTypes.Infinite: return "∞";
                case VoteTypes.QuestionMark: return "??";
                case VoteTypes.Break: return "B";

                default:
                    return "";
            }
        }
    }
}
