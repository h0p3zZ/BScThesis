using System.Diagnostics.CodeAnalysis;
using JsonVerifier.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace JsonVerifier;

public class JsonParser
{
    private readonly string _filePath;

    public JsonParser(string filePath) => _filePath = filePath;

    public bool CheckSchema(out IList<string> messages)
    {
        messages = [];
        if (!File.Exists(_filePath))
        {
            messages.Add("File not found.");
            return false;
        }
        if (!Path.GetExtension(_filePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            messages.Add("The specified file is not a JSON file.");
            return false;
        }

        var jSchema = JSchema.Parse(File.ReadAllText("./schema.json"));
        using StreamReader fileReader = File.OpenText(_filePath);
        using JsonTextReader jsonReader = new(fileReader);
        using JSchemaValidatingReader validatingReader = new(jsonReader);
        validatingReader.Schema = jSchema;
        IList<string> errorMessages = [];
        validatingReader.ValidationEventHandler += (o, a) => errorMessages.Add(a.Message);
        while (validatingReader.Read()) { }
        if (errorMessages.Count == 0) return true;
        else
        {
            messages = errorMessages;
            return false;
        }
    }

    public bool TryParse([NotNullWhen(true)] out ScheduleProblem? schedule)
    {
        schedule = null;
        if (
            !File.Exists(_filePath)
            || !Path.GetExtension(_filePath).Equals(".json", StringComparison.OrdinalIgnoreCase)
        )
        {
            return false;
        }

        try
        {
            schedule = JsonConvert.DeserializeObject<ScheduleProblem>(File.ReadAllText(_filePath));
            return schedule != null;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
