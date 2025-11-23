using System.Text.Json;
using JsonVerifier;
using JsonVerifier.Models;
using SolutionVerifier;
Console.OutputEncoding = System.Text.Encoding.Unicode;

if (args.Length != 2)
{
    Console.WriteLine("Please specify file path: program.exe <problem-file-path> <assignment_file_path>");
    return;
}

var jsonParser = new JsonParser(args[0]);
var success = jsonParser.TryParse(out ScheduleProblem? problem);
if (!success || problem == null)
{
    Console.WriteLine("The file could not be parsed as a valid Schedule JSON. Please verify the file using the JsonVerifier.");
    return;
}

if (!File.Exists(args[1]))
{
    Console.WriteLine("The assignment file does not exist.");
    return;
}

var assignmentParser = new AssignmentParser(args[1]);
success = assignmentParser.Prase(out ScheduleAssignment? assignments);
if (!success || assignments == null)
{
    Console.WriteLine("The file does not contain a valid assignment");
    return;
}

bool foundProblem = false;
foreach (var assignment in assignments)
{
    if (assignment.Value is not { } timeSlot) continue;
    if (timeSlot < 0)
    {
        Console.WriteLine($"Task {assignment.Key} has a negative time slot.");
        foundProblem = true;
    }
    if (!problem.Tasks.ContainsKey(assignment.Key))
    {
        Console.WriteLine($"Non-existent task {assignment.Key} has an assignment.");
        foundProblem = true;
    }
}

foreach (var task in problem.Tasks)
{
    if (!assignments.ContainsKey(task.Key))
    {
        Console.WriteLine($"Task {task.Key} does not have an assignment.");
        foundProblem = true;
    }
}

if (foundProblem)
    return;

double importanceCost = 0;
double urgencyCost = 0;
double dependencyCost = 0;
double locationCost = 0;
foreach (var i1 in assignments)
{
    var task1 = problem.Tasks[i1.Key];

    if (i1.Value is { } task1TimeSlot)
    {
        urgencyCost = (task1.Urgency + 1) * task1TimeSlot;
        // Check whether the task has exceeded the scheduling horizon
        if (task1TimeSlot + task1.Duration >= problem.Horizon)
        {
            Console.WriteLine($"Task {i1.Key} exceeds the scheduling horizon.");
            return;
        }

        foreach (var i2 in assignments)
        {
            if (i2.Value is not { } task2TimeSlot) continue;
            if (i1.Key == i2.Key) continue;

            var task2 = problem.Tasks[i2.Key];
            // Check whether tasks overlap with one another
            if (
                task1TimeSlot + task1.Duration > task2TimeSlot && task2TimeSlot + task2.Duration > task1TimeSlot
            )
            {
                Console.WriteLine($"Tasks {i1.Key} and {i2.Key} overlap with one another.");
                return;
            }

            // Check dependency cost
            foreach (var dependency in task2.Dependencies ?? [])
            {
                if (dependency.Task == i1.Key)
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
                        dependencyCost += weightedCost;
                    }
                    else
                        dependencyCost += dependency.UnmetCost;
                }
            }
        }
    }
    else
    {
        importanceCost += task1.Importance;
    }
}

Console.WriteLine($"Importance cost of unassigned tasks: {importanceCost}");
Console.WriteLine($"Urgency cost of all tasks: {urgencyCost}");
Console.WriteLine($"Dependency cost of all tasks: {dependencyCost}");
Console.WriteLine($"Location cost of all tasks: {locationCost}");