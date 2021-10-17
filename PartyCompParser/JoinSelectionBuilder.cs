using System;
using System.Collections.Generic;
using System.Linq;

namespace PartyCompParser
{
    internal class JoinSelectionBuilder
    {
        public static List<JobSelection> GetJobSelections(SheetData sheetData)
        {
            var response = sheetData;
            var players = response.players;

            var jobSelections = new List<JobSelection>();
            foreach (var player in players.Where(p => p.Key != "undefined"))
            {
                for (var i = 0; i < player.Value.Length; i++)
                {
                    var job = player.Value[i];

                    if (string.IsNullOrEmpty(job))
                    {
                        continue;
                    }

                    var jobSelection = GetJobSelection(player.Key, job, i, response.jobs);
                    jobSelections.Add(jobSelection);
                }
            }

            return jobSelections;
        }

        private static JobSelection GetJobSelection(string name,
                                                    string job,
                                                    int rank,
                                                    Dictionary<string, string> jobs)
        {
            var jobRole = jobs[job];
            var role = Enum.Parse<Role>(jobRole);

            return new JobSelection
                   {
                       Name = name,
                       Rank = rank,
                       Job = job,
                       Role = role
                   };
        }
    }
}