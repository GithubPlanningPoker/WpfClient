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
            get { return vote.VoteType.HasValue ? vote.VoteType.Value.ToAPIString() : "null"; }
        }
        public string HasVote
        {
            get { return vote.VoteType.HasValue ? "Voted" : ""; }
        }
        public VoteTypes? VoteType
        {
            get { return vote.VoteType; }
        }
    }
}
