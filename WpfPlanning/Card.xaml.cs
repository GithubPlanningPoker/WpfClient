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

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("CardValue", typeof(string), typeof(Card), new PropertyMetadata("0"));
        public string DisplayedValue
        {
            get { return (string)this.GetValue(ValueProperty); }
            set
            {
                this.label.Content = value;
                this.SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty APIValueProperty = DependencyProperty.Register("CardAPIValue", typeof(string), typeof(Card), new PropertyMetadata((string)null));
        public string APIValue
        {
            get { return (string)this.GetValue(APIValueProperty); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value", "The APIValue property must be set.");

                Library.VoteTypes type;
                if (!Library.VoteTypesExtension.TryParse(value, out type))
                    throw new ArgumentException("Unable to parse " + value + " as a vote.");

                this.SetValue(APIValueProperty, value);
            }
        }
    }
}
