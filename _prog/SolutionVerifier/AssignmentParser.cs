using JsonVerifier.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace SolutionVerifier;

public class AssignmentParser
{
    private readonly string _filePath;

    public AssignmentParser(string filePath)
    {
        _filePath = filePath;
    }

    public bool Prase([NotNullWhen(true)] out ScheduleAssignment? assignment)
    {
        if (!File.Exists(_filePath)){
            assignment = null;
            return false;
        }

        assignment = JsonSerializer.Deserialize<ScheduleAssignment>(File.ReadAllText(_filePath));
        return assignment != null;
    }
}
