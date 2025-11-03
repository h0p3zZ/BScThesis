using System.Text.Json;
using JsonVerifier;
using JsonVerifier.Models;
Console.OutputEncoding = System.Text.Encoding.Unicode;

if (args.Length != 2)
{
    Console.WriteLine("Please specify file path: program.exe <file-to-problem> <file-to-assignment>");
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

var importanceCost = assignment.Assignments.Sum((x) => (x.Value == null) ? problem.Tasks[x.Key].Importance : 0);
Console.WriteLine($"Importance cost of unassigned tasks: {importanceCost}");