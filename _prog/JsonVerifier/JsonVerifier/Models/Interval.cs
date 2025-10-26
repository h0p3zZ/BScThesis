using System.ComponentModel.DataAnnotations;

namespace JsonVerifier.Models;

internal class IntervalCost
{
    [Length(2,2)]
    public required List<int> Interval { get; set; }
    public int Cost { get; set; }
}
