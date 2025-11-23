namespace SolutionVerifier.Models;

internal class ScheduleCost
{
    internal double ImportanceCost { get; set; } = 0f;
    internal int UrgencyCost { get; set; } = 0;
    internal double DependencyCost { get; set; } = 0f;
    internal double LocationCost { get; set; } = 0f;
}
