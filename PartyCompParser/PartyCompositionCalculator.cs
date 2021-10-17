using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyCompParser
{
    internal class PartyCompositionCalculator
    {
        private static readonly List<PartyComposition> UniquePartyCompositions = new();

        public static IEnumerable<PartyComposition> GetPossibleCompositions(
            IEnumerable<RoleRequirement> roleRequirements,
            IEnumerable<JobSelection> jobSelections)
        {
            foreach (var roleRequirement in roleRequirements)
            {
                var remainingRoles = roleRequirements.ToList();
                remainingRoles.Remove(roleRequirement);
                GetPossibleAssignment(roleRequirement, jobSelections, new List<JobSelection>(), remainingRoles);
            }

            var uniquePlayerCount = jobSelections.Select(js => js.Name).Distinct().Count();
            return UniquePartyCompositions.Where(pc => pc.Assignments.Count() == uniquePlayerCount)
                                          //.Where(pc => roleRequirements.All(rr => IsRoleRequirementSatisfied(rr, pc)))
                                          .ToList();
        }

        private static bool IsRoleRequirementSatisfied(RoleRequirement roleRequirement, PartyComposition partyComposition)
        {
            var roleCount = partyComposition.Assignments.Count(a => a.Role == roleRequirement.Role);
            return roleCount >= roleRequirement.Min && roleCount <= roleRequirement.Max;
        }

        private static void GetPossibleAssignment(RoleRequirement requirement,
                                                  IEnumerable<JobSelection> available,
                                                  IEnumerable<JobSelection> assigned,
                                                  IEnumerable<RoleRequirement> remainingRoles)
        {
            var roleAvailable = available.Where(jp => jp.Role == requirement.Role)
                                         .OrderBy(jp => jp.Rank)
                                         .ToArray();

            var roleAssigned = assigned.Count(a => a.Role == requirement.Role);

            var maxAssigned = requirement.Max == roleAssigned;
            var noneAvailableForRole = !roleAvailable.Any();

            if (!maxAssigned && !noneAvailableForRole)
            {
                TryEachAvailableSelection(requirement, available, assigned, remainingRoles, roleAvailable);
            }
            else
            {
                ProcessRemainingRoles(available, assigned, remainingRoles);
            }
        }

        private static void TryEachAvailableSelection(RoleRequirement requirement,
                                                      IEnumerable<JobSelection> available,
                                                      IEnumerable<JobSelection> assigned,
                                                      IEnumerable<RoleRequirement> remainingRoles,
                                                      JobSelection[] roleAvailable)
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

                GetPossibleAssignment(requirement, remainingAvailable, newAssigned, remainingRoles);
            }
        }

        private static void ProcessRemainingRoles(IEnumerable<JobSelection> available,
                                                  IEnumerable<JobSelection> assigned,
                                                  IEnumerable<RoleRequirement> remainingRoles)
        {
            foreach (var roleRequirement in remainingRoles)
            {
                var newRemaining = remainingRoles.ToList();
                newRemaining.Remove(roleRequirement);
                GetPossibleAssignment(roleRequirement, available, assigned, newRemaining);
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