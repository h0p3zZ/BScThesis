using System.Text.Json;
using JsonVerifier;
using JsonVerifier.Models;
Console.OutputEncoding = System.Text.Encoding.Unicode;

if (args.Length != 2)
{
    Console.WriteLine("Please specify file path: program.exe <problem-file-path> <assignment_file_path>");
    return;
}

var success = JsonParser.TryParse(args[0], out ScheduleProblem? problem);
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

var assignment = JsonSerializer.Deserialize<ScheduleAssignment>(File.ReadAllText(args[1]));
if (assignment == null)
{
    Console.WriteLine("The file does not contain a valid assignment");
    return;
}

if (problem.Tasks.Any(x => !assignment.Assignments.ContainsKey(x.Key)))
{
    Console.WriteLine("Some tasks do not have an assignment.");
    return;
}


if (assignment.Assignments.Any(x => !problem.Tasks.ContainsKey(x.Key)))
{
    Console.WriteLine("Some non-existent tasks have an assignment.");
    return;
}

double importanceCost = 0;
double urgencyCost = 0;
double dependencyCost = 0;
double locationCost = 0;
foreach (var i1 in assignment.Assignments)
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

        foreach (var i2 in assignment.Assignments)
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
                    // TODO: handle multiple matching intervals
                    var interval = dependency.Intervals.FirstOrDefault(x =>
                        task2TimeSlot - task1TimeSlot + task1.Duration > x.Interval[0] &&
                        task2TimeSlot - task1TimeSlot + task1.Duration < x.Interval[1]
                    );
                    if (interval != null)
                        dependencyCost += interval.Cost;
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
Console.WriteLine($"Urgency cost of unassigned tasks: {urgencyCost}");
Console.WriteLine($"Dependency cost of unassigned tasks: {dependencyCost}");
Console.WriteLine($"Location cost of unassigned tasks: {locationCost}");