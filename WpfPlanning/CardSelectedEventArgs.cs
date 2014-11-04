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
        private VoteTypes voteType;

        public CardSelectedEventArgs(VoteTypes voteType)
        {
            this.voteType = voteType;
        }

        public VoteTypes VoteType
        {
            get { return voteType; }
        }
    }
}
