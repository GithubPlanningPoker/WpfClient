using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlanning
{
    public class WpfVote
    {
        private Vote vote;

        public WpfVote(Vote vote)
        {
            this.vote = vote;
        }

        public string UserName
        {
            get { return vote.Name; }
        }
        public string VoteAPI
        {
            get { return vote.HasVoted ? vote.VoteType.ToAPIString() : "null"; }
        }
        public bool HasVoted
        {
            get { return vote.HasVoted; }
        }
        public VoteTypes VoteType
        {
            get { return vote.VoteType; }
        }
    }
}
