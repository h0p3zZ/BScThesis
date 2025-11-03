using System.Diagnostics.CodeAnalysis;
using JsonVerifier.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace JsonVerifier;

public static class JsonParser
{
    public static bool CheckSchema(string filePath, out IList<string> messages)
    {
        messages = [];
        if (!File.Exists(filePath))
        {
            messages.Add("File not found.");
            return false;
        }
        if (!Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            messages.Add("The specified file is not a JSON file.");
            return false;
        }

        var jSchema = JSchema.Parse(File.ReadAllText("./schema.json"));
        using StreamReader fileReader = File.OpenText(filePath);
        using JsonTextReader jsonReader = new(fileReader);
        using JSchemaValidatingReader validatingReader = new(jsonReader);
        validatingReader.Schema = jSchema;
        IList<string> errorMessages = [];
        validatingReader.ValidationEventHandler += (o, a) => errorMessages.Add(a.Message);
        while (validatingReader.Read()) { }
        if (messages.Count == 0) return true;
        else
        {
            messages = errorMessages;
            return false;
        }
    }

    public static bool TryParse(
        string filePath,
        [NotNullWhen(true)] out ScheduleProblem? schedule
    )
    {
        schedule = null;
        if (
            !File.Exists(filePath)
            || !Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase)
        )
        {
            return false;
        }

        try
        {
            schedule = JsonConvert.DeserializeObject<ScheduleProblem>(File.ReadAllText(filePath));
            return schedule != null;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
