using JsonVerifier.Models;
using SolutionVerifier.Models;

namespace SolutionVerifier;

public class SolutionVerifier
{
    private readonly ScheduleProblem _problem;

    public SolutionVerifier(ScheduleProblem problem)
    {
        _problem = problem;
    }

    public ScheduleCost CalcCost(ScheduleAssignment assignments)
    {
        var scheduleCost = new ScheduleCost();

        foreach (var assign1 in assignments)
        {
            var task1 = _problem.Tasks[assign1.Key];

            if (assign1.Value is not { } task1TimeSlot) {
                scheduleCost.ImportanceCost += task1.Importance;
                continue;
            }

            scheduleCost.UrgencyCost += (task1.Urgency + 1) * task1TimeSlot;
            // Check whether the task has exceeded the scheduling horizon
            if (task1TimeSlot + task1.Duration >= _problem.Horizon)
            {
                Console.WriteLine($"Task {assign1.Key} exceeds the scheduling horizon.");
                return scheduleCost;
            }

            foreach (var assign2 in assignments)
            {
                if (assign2.Value is not { } task2TimeSlot) continue;
                if (assign1.Key == assign2.Key) continue;

                var task2 = _problem.Tasks[assign2.Key];
                // Check whether tasks overlap with one another
                if (
                    task1TimeSlot + task1.Duration > task2TimeSlot && task2TimeSlot + task2.Duration > task1TimeSlot
                )
                {
                    Console.WriteLine($"Tasks {assign1.Key} and {assign2.Key} overlap with one another.");
                    return scheduleCost;
                }

                // Add location cost if tasks are scheduled back-to-back and not at the same location
                if (
                    task1.Location != null && task2.Location != null
                    && task1TimeSlot + task1.Duration == task2TimeSlot
                    && task1.Location.Id != task2.Location.Id
                )
                {
                    scheduleCost.LocationCost += task2.Location.UnmetCost;
                }

                // Check dependency cost
                foreach (var dependency in task2.Dependencies ?? [])
                {
                    if (dependency.Task != assign1.Key)
                        continue;

                    var intervals = dependency.Intervals.Where(x =>
                        task2TimeSlot - task1TimeSlot + task1.Duration > x.Interval[0] &&
                        task2TimeSlot - task1TimeSlot + task1.Duration < x.Interval[1]
                    );

                    if (!intervals.Any())
                    {
                        scheduleCost.DependencyCost += dependency.UnmetCost;
                        continue;
                    }

                    double weightedSum = 0;
                    double totalOverlap = 0;

                    foreach (var interval in intervals)
                    {
                        var intervalStart = interval.Interval[0];
                        var intervalEnd = interval.Interval[1];

                        // Calculating overlap to determine weighted cost
                        var overlapStart = Math.Max(task2TimeSlot, intervalStart);
                        var overlapEnd = Math.Min(task2TimeSlot + task2.Duration, intervalEnd);
                        var overlapLength = overlapEnd - overlapStart;

                        if (overlapLength > 0)
                        {
                            weightedSum += overlapLength * interval.Cost;
                            totalOverlap += overlapLength;
                        }
                    }

                    var weightedCost = weightedSum / totalOverlap;
                    scheduleCost.DependencyCost += weightedCost;
                }
            }
        }

        // Check if any cost is infinite, mark the schedule as valid or invalid
        // If this part is not reached, the schedule is automatically invalid
        scheduleCost.Valid = 
            scheduleCost.ImportanceCost != double.PositiveInfinity
            && scheduleCost.LocationCost != double.PositiveInfinity
            && scheduleCost.DependencyCost != double.PositiveInfinity;
        return scheduleCost;
    }
}
