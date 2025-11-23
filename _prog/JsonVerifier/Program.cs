using JsonVerifier;


if (args.Length != 1)
{
    Console.WriteLine("Please specify file path: program.exe <file-to-json>");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine($"The JSON file ({args[0]}) does not exist.");
    return;
}

var jsonParser = new JsonParser(args[0]);

var success = jsonParser.CheckSchema(out IList<string> messages);
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