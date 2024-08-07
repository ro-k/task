using System.Diagnostics.CodeAnalysis;

namespace Lib.Models;

[ExcludeFromCodeCoverage]
public class BaseModel : IHasConcurrencyStamp
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public Guid UserId { get; set; }
    public Guid ConcurrencyStamp { get; set; } = Guid.NewGuid();
    public Status Status { get; set; } = Status.Active;
}