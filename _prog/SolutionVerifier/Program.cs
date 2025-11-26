using JsonVerifier;
using JsonVerifier.Models;
using SolutionVerifier;
Console.OutputEncoding = System.Text.Encoding.Unicode;

#region ---------- Argument Check ----------

if (args.Length != 2)
{
    Console.WriteLine("Please specify file path: program.exe <problem-file-path> <assignment_file_path>");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine($"The problem file ({args[1]}) does not exist.");
    return;
}

var jsonParser = new JsonParser(args[0]);
var success = jsonParser.TryParse(out ScheduleProblem? problem);
if (!success || problem == null)
{
    Console.WriteLine($"The file ({args[0]}) could not be parsed as a valid Schedule JSON. Please verify the file using the JsonVerifier.");
    return;
}

if (!File.Exists(args[1]))
{
    Console.WriteLine($"The assignment file ({args[1]}) does not exist.");
    return;
}

var assignmentParser = new AssignmentParser(args[1]);
success = assignmentParser.Prase(out ScheduleAssignment? _assignments);
if (!success || _assignments == null)
{
    Console.WriteLine("The file does not contain a valid assignment");
    return;
}

bool foundProblem = false;
foreach (var assignment in _assignments)
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
    if (!_assignments.ContainsKey(task.Key))
    {
        Console.WriteLine($"Task {task.Key} does not have an assignment.");
        foundProblem = true;
    }
}

if (foundProblem)
    return;

#endregion Argument Check

var verifier = new SolutionVerifier.SolutionVerifier(problem);
var scheduleCost = verifier.CalcCost(_assignments, out var messages);

if (scheduleCost == null)
{
    Console.WriteLine($"The solution ({args[1]}) is invalid:");
    foreach (var message in messages)
    {
        Console.WriteLine($"- {message}");
    }
    return;
}

Console.WriteLine($"The solution ({args[1]}) is valid.");
Console.WriteLine($"Importance cost of unassigned tasks: {scheduleCost.ImportanceCost}");
Console.WriteLine($"Urgency cost of all tasks: {scheduleCost.UrgencyCost}");
Console.WriteLine($"Dependency cost of all tasks: {scheduleCost.DependencyCost}");
Console.WriteLine($"Location cost of all tasks: {scheduleCost.LocationCost}");