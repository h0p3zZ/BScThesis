using JsonVerifier.Models;
using SolutionVerifier.Models;

namespace SolutionVerifier;

internal class SolutionVerifier
{
    private readonly ScheduleProblem _problem;
    private readonly ScheduleAssignment _assignments;

    internal SolutionVerifier(
        ScheduleProblem problem,
        ScheduleAssignment assignments
    )
    {
        _problem = problem;
        _assignments = assignments;
    }

    internal ScheduleCost CalcCost()
    {
        var scheduleCost = new ScheduleCost();

        foreach (var assign1 in _assignments)
        {
            var task1 = _problem.Tasks[assign1.Key];

            if (assign1.Value is { } task1TimeSlot)
            {
                scheduleCost.UrgencyCost += (task1.Urgency + 1) * task1TimeSlot;
                // Check whether the task has exceeded the scheduling horizon
                if (task1TimeSlot + task1.Duration >= _problem.Horizon)
                {
                    Console.WriteLine($"Task {assign1.Key} exceeds the scheduling horizon.");
                    return scheduleCost;
                }

                foreach (var assign2 in _assignments)
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

                    // Check dependency cost
                    foreach (var dependency in task2.Dependencies ?? [])
                    {
                        if (dependency.Task == assign1.Key)
                        {
                            var intervals = dependency.Intervals.Where(x =>
                                task2TimeSlot - task1TimeSlot + task1.Duration > x.Interval[0] &&
                                task2TimeSlot - task1TimeSlot + task1.Duration < x.Interval[1]
                            );
                            if (intervals.Any())
                            {
                                double weightedSum = 0;
                                double totalOverlap = 0;

                                foreach (var interval in intervals)
                                {
                                    var intervalStart = interval.Interval[0];
                                    var intervalEnd = interval.Interval[1];

                                    // actual overlap between the task window and the interval
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
                            else
                                scheduleCost.DependencyCost += dependency.UnmetCost;
                        }
                    }
                }
            }
            else
            {
                scheduleCost.ImportanceCost += task1.Importance;
            }
        }

        return scheduleCost;
    }
}
