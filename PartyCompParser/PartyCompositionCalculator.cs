using System.Collections.Generic;
using System.Linq;

namespace PartyCompParser
{
    public static class PartyCompositionCalculator
    {
        private static readonly List<PartyComposition> UniquePartyCompositions = new();

        public static IEnumerable<PartyComposition> GetPossibleCompositions(
            IEnumerable<RoleRequirement> roleRequirements,
            IEnumerable<JobSelection> jobSelections)
        {
            ProcessRemainingRoles(jobSelections, Enumerable.Empty<JobSelection>(), roleRequirements);

            var uniquePlayerCount = jobSelections.Select(js => js.Name).Distinct().Count();
            return UniquePartyCompositions.Where(pc => pc.Assignments.Count() == uniquePlayerCount)
                                          //.Where(pc => roleRequirements.All(rr => rr.IsSatisfied(pc)))
                                          .ToList();
        }

        private static void ProcessRemainingRoles(IEnumerable<JobSelection> available,
                                                  IEnumerable<JobSelection> assigned,
                                                  IEnumerable<RoleRequirement> remainingRoles)
        {
            foreach (var roleRequirement in remainingRoles)
            {
                var newRemaining = remainingRoles.ToList();
                newRemaining.Remove(roleRequirement);
                GetPossibleAssignments(roleRequirement, available, assigned, newRemaining);
            }
        }

        private static void GetPossibleAssignments(RoleRequirement requirement,
                                                   IEnumerable<JobSelection> available,
                                                   IEnumerable<JobSelection> assigned,
                                                   IEnumerable<RoleRequirement> remainingRoles)
        {
            var roleAssigned = assigned.Count(a => a.Role == requirement.Role);

            var maxAssigned = requirement.Max == roleAssigned;

            var roleAvailable = available.Where(jp => jp.Role == requirement.Role)
                                         .OrderBy(jp => jp.Rank)
                                         .ToArray();
            var noneAvailableForRole = !roleAvailable.Any();

            if (maxAssigned || noneAvailableForRole)
            {
                ProcessRemainingRoles(available, assigned, remainingRoles);
            }
            else
            {
                TryEachAvailableSelection(roleAvailable, requirement, available, assigned, remainingRoles);
            }
        }

        private static void TryEachAvailableSelection(JobSelection[] roleAvailable,
                                                      RoleRequirement requirement,
                                                      IEnumerable<JobSelection> available,
                                                      IEnumerable<JobSelection> assigned,
                                                      IEnumerable<RoleRequirement> remainingRoles)
        {
            foreach (var selection in roleAvailable)
            {
                var newAssigned = assigned.Append(selection).ToList();

                if (!IsCompositionUnique(newAssigned))
                {
                    continue;
                }

                var remainingAvailable = available.Where(a => a.Job != selection.Job
                                                              && a.Name != selection.Name)
                                                  .ToList();

                GetPossibleAssignments(requirement, remainingAvailable, newAssigned, remainingRoles);
            }
        }

        private static bool IsCompositionUnique(IEnumerable<JobSelection> assignments)
        {
            foreach (var partyComposition in UniquePartyCompositions)
            {
                if (assignments.All(partyComposition.Assignments.Contains))
                {
                    return false;
                }
            }

            UniquePartyCompositions.Add(new PartyComposition(assignments));
            return true;
        }
    }
}