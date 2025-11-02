using JsonVerifier;


if (args.Length != 1)
{
    Console.WriteLine("Please specify file path: program.exe <file-to-json>");
    return;
}

var success = JsonParser.CheckSchema(args[0], out IList<string> messages);
if (success)
{
    Console.WriteLine("The JSON file is valid according to the schema.");
}
else
{
    Console.WriteLine("The JSON file is invalid according to the schema. Errors:");
    foreach (var message in messages)
    {
        Console.WriteLine($"- {message}");
    }
}