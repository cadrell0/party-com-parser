using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PartyCompParser
{
    internal class Program
    {
        private const string OutputFile = @"C:\temp\party.csv";

        private static readonly RoleRequirement[] RoleRequirements = {
                                                                         new() {Role = Role.T, Min = 2, Max = 2},
                                                                         new() {Role = Role.H, Min = 1, Max = 1},
                                                                         new() {Role = Role.B, Min = 1, Max = 1},
                                                                         new() {Role = Role.M, Min = 1, Max = 2},
                                                                         new() {Role = Role.PR, Min = 1, Max = 2},
                                                                         new() {Role = Role.MR, Min = 1, Max = 2}
                                                                     };

        private static async Task Main(string[] args)
        {
            var sheetData = await GoogleSheetsProxy.GetSheetDataAsync();
            var jobSelections = JobSelectionBuilder.GetJobSelections(sheetData);
            var partyCompositions = PartyCompositionCalculator.GetPossibleCompositions(RoleRequirements, jobSelections);
            var playerNames = sheetData.players.Keys.Where(k => k != "undefined").ToList();
            await OutputResults(playerNames, partyCompositions);
        }

        private static async Task OutputResults(IEnumerable<string> playerNames, 
                                                IEnumerable<PartyComposition> partyCompositions)
        {
            await using var output = new StreamWriter(new FileStream(OutputFile, FileMode.Create, FileAccess.Write));
            var headers = playerNames.Append("Distance").ToArray();
            await output.WriteLineAsync($"{string.Join(",", headers)}");
            var body = new List<string[]>();

            foreach (var partyComposition in partyCompositions)
            {
                var distanceFromPreferred = partyComposition.GetDistanceFromPreferred().ToString();

                var jobs = playerNames.Select(p => FindJob(partyComposition, p))
                                      .Append(distanceFromPreferred)
                                      .ToArray();
                await output.WriteLineAsync($"{string.Join(",", jobs)}");
                body.Add(jobs);
            }

            var results = body.Prepend(headers).ToArray();

            await GoogleSheetsProxy.PostResultsAsync(results);
        }

        private static string FindJob(PartyComposition partyComposition, string p)
        {
            var assignment = partyComposition.Assignments.FirstOrDefault(a => a.Name == p);
            return assignment != null ? assignment.Job : "??";
        }
    }
}