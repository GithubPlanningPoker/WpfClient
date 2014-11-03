using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlanning
{
    public class CardSelectedEventArgs : EventArgs
    {
        private string value;
        private VoteTypes voteType;

        public CardSelectedEventArgs(string value)
        {
            this.value = value;
            if (!VoteTypesExtension.TryParse(value, out this.voteType))
                throw new ArgumentException("Value " + value + " cannot be converted to a VoteTypes.");
        }

        public string Value
        {
            get { return value; }
        }
        public VoteTypes VoteType
        {
            get { return voteType; }
        }
    }
}
