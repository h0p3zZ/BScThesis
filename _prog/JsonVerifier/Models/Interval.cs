using System.ComponentModel.DataAnnotations;

namespace JsonVerifier.Models;

public class IntervalCost
{
    // Interval represented as [start, end] (end is excluded)
    [Length(2, 2)]
    public required List<int> Interval { get; set; }
    public int Cost { get; set; }
}
