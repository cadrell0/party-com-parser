using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PartyCompParser
{
    public class PartyComposition
    {
        private readonly List<JobSelection> _assignments;
        
        public PartyComposition()
        {
            _assignments = new ();
        }

        private PartyComposition(PartyComposition partyComposition)
        {
            _assignments = new List<JobSelection>(partyComposition.Assignments);
            Key = partyComposition.Key;
        }

        public void Add(JobSelection jobSelection)
        {
            _assignments.Add(jobSelection);

            Key = _assignments.OrderBy(a => a.Name)
                              .Select(a => a.Name + ":" + a.Job)
                              .Aggregate((a, b) => a + "|" + b);
        }

        public IEnumerable<JobSelection> Assignments => _assignments;
        public string Key { get; private set; }

        public int GetDistanceFromPreferred()
        {
            return Assignments.Sum(a => a.Rank);
        }

        public PartyComposition Clone()
        {
            return new(this);
        }
    }
}