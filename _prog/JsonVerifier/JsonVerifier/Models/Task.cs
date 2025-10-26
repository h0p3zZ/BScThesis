namespace JsonVerifier.Models;

internal class Task
{
    public string Name { get; set; }
    public int Duration { get; set; }
    public int Importance { get; set; }
    public int Urgency { get; set; }
    public required List<IntervalCost> TimeSlots { get; set; }
    public List<Dependency>? Dependencies { get; set; }
    public Location? Location { get; set; }
}
