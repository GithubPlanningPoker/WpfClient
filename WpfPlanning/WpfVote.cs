using Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlanning
{
    public class WpfVote
    {
        private string name;
        private VoteTypes votetype;
        private bool hasvoted;

        public WpfVote(Vote vote)
        {
            this.name = vote.Name;;
            this.votetype = vote.VoteType;
            this.hasvoted = vote.HasVoted;
        }

        public string UserName
        {
            get { return name; }
        }
        public string VoteAPI
        {
            get { return hasvoted ? votetype.ToAPIString() : "null"; }
        }
        public bool HasVoted
        {
            get { return hasvoted; }
        }
        public VoteTypes VoteType
        {
            get { return votetype; }
            set
            {
                votetype = value;
                hasvoted = true;
            }
        }

        public void ClearVote()
        {
            this.hasvoted = false;
            votetype = default(VoteTypes);
        }
    }
}
