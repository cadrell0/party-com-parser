using System.Linq;

namespace PartyCompParser
{
    public class RoleRequirement
    {
        public Role Role { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public bool IsSatisfied(PartyComposition partyComposition)
        {
            var roleCount = partyComposition.Assignments.Count(a => a.Role == Role);
            return roleCount >= Min && roleCount <= Max;
        }
    }
}