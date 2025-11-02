using JsonVerifier;
using JsonVerifier.Models;

if (args.Length != 1)
{
    Console.WriteLine("Please specify file path: program.exe <file-to-verify>");
    return;
}

var success = JsonParser.TryParse(args[0], out ScheduleProblem? problem);

if (!success)
{
    Console.WriteLine("The file could not be parsed as a valid Schedule JSON. Please verify the file using the JsonVerifier.");
    return;
}

