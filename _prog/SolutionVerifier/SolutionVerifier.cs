using JsonVerifier.Models;
using SolutionVerifier.Models;
using System.Threading.Tasks;

namespace SolutionVerifier;

public class SolutionVerifier
{
    private readonly ScheduleProblem _problem;

    public SolutionVerifier(ScheduleProblem problem)
    {
        _problem = problem;
    }

    public ScheduleCost? CalcCost(ScheduleAssignment assignments, out List<string> messages)
    {
        messages = [];

        var scheduleCost = new ScheduleCost();

        foreach (var assign1 in assignments)
        {
            var task1 = _problem.Tasks[assign1.Key];

            if (assign1.Value is not { } task1TimeSlot) {
                scheduleCost.ImportanceCost += task1.Importance;
                if (task1.Importance == double.PositiveInfinity)
                    messages.Add($"Task {assign1.Key} is not scheduled, incurring infinite importance cost.");
                continue;
            }

            // Check whether the task has exceeded the scheduling horizon
            if (task1TimeSlot + task1.Duration >= _problem.Horizon)
                messages.Add($"Task {assign1.Key} exceeds the scheduling horizon.");

            var start = task1TimeSlot;
            var end = task1TimeSlot + task1.Duration;
            var intervals = task1.TimeSlots.Where(x =>
                start < x.Interval[1] && // end is exclusive
                end >= x.Interval[0]
            );

            // If the task is not fully covered by the provided intervals, log error message = mark invalid
            if (!IsRangeCoveredByIntervals(start, end, intervals))
            {
                scheduleCost.ImportanceCost += task1.Importance;
                messages.Add($"Task {assign1.Key} is only partially scheduled in appropriate intervals, there is some of the duration not in a provided interval.");
                continue;
            }

            scheduleCost.TimeSlotCost += CalculateWeightedIntervalCost(start, end, intervals);

            scheduleCost.UrgencyCost += (task1.Urgency + 1) * task1TimeSlot;

            foreach (var assign2 in assignments)
            {
                if (assign2.Value is not { } task2TimeSlot) continue;
                if (assign1.Key == assign2.Key) continue;

                var task2 = _problem.Tasks[assign2.Key];
                // Check whether tasks overlap with one another
                if (task1TimeSlot + task1.Duration > task2TimeSlot && task2TimeSlot + task2.Duration > task1TimeSlot)
                    messages.Add($"Tasks {assign1.Key} and {assign2.Key} overlap with one another.");

                // Add location cost if tasks are scheduled back-to-back and not at the same location
                if (
                    task1.Location != null && task2.Location != null
                    && task1TimeSlot + task1.Duration == task2TimeSlot
                    && task1.Location.Id != task2.Location.Id
                )
                {
                    scheduleCost.LocationCost += task2.Location.UnmetCost;

                    if (task2.Location.UnmetCost == double.PositiveInfinity)
                        messages.Add($"Tasks {assign1.Key} and {assign2.Key} are scheduled back-to-back at different locations with mandatory same location (infinite cost).");
                }

                // Check dependency cost
                foreach (var dependency in task2.Dependencies ?? [])
                {
                    if (dependency.Task != assign1.Key)
                        continue;

                    start = task2TimeSlot - task1TimeSlot;
                    end = task2TimeSlot - task1TimeSlot + task2.Duration;
                    intervals = dependency.Intervals.Where(x =>
                        start <= x.Interval[1] &&
                        end >= x.Interval[0]
                    );

                    if (!IsRangeCoveredByIntervals(start, end, intervals))
                    {
                        scheduleCost.DependencyCost += dependency.UnmetCost;
                        if (dependency.UnmetCost == double.PositiveInfinity)
                            messages.Add($"Dependency between tasks {assign1.Key} and {assign2.Key} is not met within any specified interval even though it is mandatory (infinite cost).");
                        continue;
                    }

                    scheduleCost.DependencyCost += CalculateWeightedIntervalCost(start, end, intervals);
                }
            }
        }

        // If error messages exist, return null indicating invalid solution
        return messages.Count == 0 ? scheduleCost : null;
    }

    /// <summary>
    /// Checks whether the range [a, b) is fully covered by the given intervals.
    /// </summary>
    /// <param name="start">Start of range included.</param>
    /// <param name="end">End of range excluded.</param>
    /// <param name="intervals">The intervals that should cover the initial range.</param>
    /// <returns>True if the <paramref name="intervals"/> cover the range between <paramref name="start"/> and <paramref name="end"/>.</returns>
    private static bool IsRangeCoveredByIntervals (int start, int end, IEnumerable<IntervalCost> intervals)
    {
        var values = Enumerable.Range(start, end - start).ToList();

        foreach (var interval in intervals)
        {
            values.RemoveAll(v => v >= interval.Interval[0] && v <= interval.Interval[1]);
        }

        return values.Count == 0;
    }

    /// <summary>
    /// Calculate the weighted average cost for the given range based on overlapping intervals.
    /// </summary>
    /// <param name="start">The start time slot of the range to check.</param>
    /// <param name="end">The end time slot of the range to check.</param>
    /// <param name="intervals">The intervals, that private the weighted cost over the range.</param>
    /// <returns>The weighted cost.</returns>
    private static double CalculateWeightedIntervalCost(int start, int end, IEnumerable<IntervalCost> intervals)
    {
        double weightedSum = 0;
        double totalOverlap = 0;

        foreach (var interval in intervals)
        {
            var intervalStart = interval.Interval[0];
            var intervalEnd = interval.Interval[1];

            // Calculating overlap to determine weighted cost
            var overlapStart = Math.Max(start, intervalStart);
            var overlapEnd = Math.Min(end, intervalEnd);
            var overlapLength = overlapEnd - overlapStart;

            if (overlapLength > 0)
            {
                weightedSum += overlapLength * interval.Cost;
                totalOverlap += overlapLength;
            }
        }

        return weightedSum / totalOverlap;
    }
}
