namespace SolutionVerifier.Models;

public class ScheduleCost
{
    public double TimeSlotCost { get; set; } = 0f;
    public double ImportanceCost { get; set; } = 0f;
    public int UrgencyCost { get; set; } = 0;
    public double DependencyCost { get; set; } = 0f;
    public double LocationCost { get; set; } = 0f;
}
