using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SolutionVerifier;

internal class AssignmentParser
{
    private readonly string _filePath;

    internal AssignmentParser(string filePath)
    {
        _filePath = filePath;
    }

    internal bool Prase([NotNullWhen(true)] out ScheduleAssignment? assignment)
    {
        assignment = null;
        if (!File.Exists(_filePath))
        {
            return false;
        }

        var jsonString = File.ReadAllText(_filePath);

        try
        {
            assignment = JsonSerializer.Deserialize<ScheduleAssignment>(jsonString);
        }
        catch (JsonException ex)
        {
            Console.WriteLine("JSON parse error:");
            Console.WriteLine($"Message: {ex.InnerException.Message}");
            Console.WriteLine($"Path: {ex.Path}");
            Console.WriteLine($"LineNumber: {ex.LineNumber}");
            Console.WriteLine($"BytePositionInLine: {ex.BytePositionInLine}");
        }
        return assignment != null;
    }
}
