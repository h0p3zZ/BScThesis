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
        if (!File.Exists(_filePath)){
            assignment = null;
            return false;
        }

        assignment = JsonSerializer.Deserialize<ScheduleAssignment>(File.ReadAllText(_filePath));
        return assignment != null;
    }
}
