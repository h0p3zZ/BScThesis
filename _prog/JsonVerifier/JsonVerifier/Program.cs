using JsonVerifier.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

if (args.Length != 1)
{
    Console.WriteLine("Please specify file path: program.exe <file-to-json>");
    return;
}

if (!File.Exists(args[0]))
{
    Console.WriteLine("File not found.");
    return;
}

if (!Path.GetExtension(args[0]).Equals(".json", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine("The specified file is not a JSON file.");
    return;
}

var jSchema = JSchema.Parse(File.ReadAllText("./schema.json"));

using StreamReader fileReader = File.OpenText(args[0]);
using JsonTextReader jsonReader = new(fileReader);
using JSchemaValidatingReader validatingReader = new(jsonReader);
validatingReader.Schema = jSchema;

IList<string> messages = [];
validatingReader.ValidationEventHandler += (o, a) => messages.Add(a.Message);

JsonSerializer serializer = new();
var token = JToken.ReadFrom(validatingReader);

if (messages.Count == 0)
{
    Console.WriteLine("JSON is valid!");

    var schedule = token.ToObject<Schedule>(serializer)!;
    Console.WriteLine($"Deserialized schedule with horizon of {schedule.Horizon} and {schedule.Tasks.Count} tasks.");
}
else
{
    Console.WriteLine("Validation errors:");
    foreach (string message in messages)
        Console.WriteLine($"\t{message}");
}