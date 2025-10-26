namespace JsonVerifier.Models;

internal class Dependency
{
    public required string Task { get; set; }
    public required List<IntervalCost> Intervals { get; set; }
    public required int UntmetCost { get; set; }
}
