using System.Collections.Generic;
using System.Linq;

namespace PartyCompParser
{
    public class PartyComposition
    {
        public PartyComposition(IEnumerable<JobSelection> assignments)
        {
            Assignments = assignments;
        }

        public IEnumerable<JobSelection> Assignments { get; }

        public int GetDistanceFromPreferred()
        {
            return Assignments.Sum(a => a.Rank);
        }
    }
}