using System.Diagnostics.CodeAnalysis;

namespace Lib.Models;

[ExcludeFromCodeCoverage]
public class UserLogin : BaseModel
{
    public Guid UserLoginId { get; set; }
    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }
    public string? ProviderDisplayName { get; set; }
}
