using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PartyCompParser
{
    public static class PartyCompositionCalculator
    {
        private static readonly ConcurrentDictionary<string, PartyComposition> UniquePartyCompositions = new();

        public static IEnumerable<PartyComposition> GetPossibleCompositions(
            IEnumerable<RoleRequirement> roleRequirements,
            IEnumerable<JobSelection> jobSelections)
        {
            ProcessRoles(jobSelections, roleRequirements);

            var uniquePlayerCount = jobSelections.Select(js => js.Name).Distinct().Count();
            return UniquePartyCompositions.Select(d => d.Value)
                                          .Where(pc => pc.Assignments.Count() == uniquePlayerCount)
                                          .Where(pc => roleRequirements.All(rr => rr.IsSatisfied(pc)))
                                          .ToList();
        }

        private static void ProcessRoles(IEnumerable<JobSelection> available,
                                         IEnumerable<RoleRequirement> remainingRoles)
        {
            
            foreach (var roleRequirement in remainingRoles)
            {
                var newRemaining = remainingRoles.ToList();
                newRemaining.Remove(roleRequirement);
                GetPossibleAssignments(roleRequirement, available, new PartyComposition(), newRemaining);
            }
        }

        private static void ProcessRemainingRoles(IEnumerable<JobSelection> available,
                                                  PartyComposition partyComposition,
                                                  IEnumerable<RoleRequirement> remainingRoles)
        {
            foreach (var roleRequirement in remainingRoles)
            {
                var newRemaining = remainingRoles.ToList();
                newRemaining.Remove(roleRequirement);
                GetPossibleAssignments(roleRequirement, available, partyComposition, newRemaining);
            }
        }

        private static void GetPossibleAssignments(RoleRequirement requirement,
                                                   IEnumerable<JobSelection> available,
                                                   PartyComposition partyComposition,
                                                   IEnumerable<RoleRequirement> remainingRoles)
        {
            var roleAssigned = partyComposition.Assignments.Count(a => a.Role == requirement.Role);

            var maxAssigned = requirement.Max == roleAssigned;

            var roleAvailable = available.Where(jp => jp.Role == requirement.Role)
                                         .OrderBy(jp => jp.Rank)
                                         .ToArray();
            var noneAvailableForRole = !roleAvailable.Any();

            if (maxAssigned || noneAvailableForRole)
            {
                ProcessRemainingRoles(available, partyComposition, remainingRoles);
            }
            else
            {
                TryEachAvailableSelection(roleAvailable, requirement, available, partyComposition, remainingRoles);
            }
        }

        private static void TryEachAvailableSelection(JobSelection[] roleAvailable,
                                                      RoleRequirement requirement,
                                                      IEnumerable<JobSelection> available,
                                                      PartyComposition partyComposition,
                                                      IEnumerable<RoleRequirement> remainingRoles)
        {
            foreach (var selection in roleAvailable)
            {
                var newComposition = partyComposition.Clone();
                newComposition.Add(selection);

                if (!IsCompositionUnique(newComposition))
                {
                    continue;
                }

                List<JobSelection> remainingAvailable;

                if (selection.Job.StartsWith("Any"))
                {
                    remainingAvailable = available.Where(a => a.Name != selection.Name)
                                                  .ToList();
                }
                else
                {
                    remainingAvailable = available.Where(a => a.Job != selection.Job
                                                              && a.Name != selection.Name)
                                                  .ToList();
                }

                GetPossibleAssignments(requirement, remainingAvailable, newComposition, remainingRoles);
            }
        }

        private static bool IsCompositionUnique(PartyComposition partyComposition)
        {
            return UniquePartyCompositions.TryAdd(partyComposition.Key, partyComposition);
            //foreach (var partyComposition in UniquePartyCompositions)
            //{
            //    if (assignments.All(partyComposition.Assignments.Contains)
            //        && assignments.Count() == partyComposition.Assignments.Count())
            //    {
            //        return false;
            //    }
            //}

            //UniquePartyCompositions.Add(composition);
            //return true;
        }
    }
}