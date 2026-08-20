using System.Security.Claims;

namespace Api.Auth.Utils;

public interface ICurrentUserId
{
    public Guid? Id { get; }
}

public class CurrentUserId : ICurrentUserId
{
    public Guid? Id { get; init; }
    public CurrentUserId(IHttpContextAccessor ctx)
    {
        if (ctx.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) is { } userIdStr && Guid.TryParse(userIdStr, out Guid userId))
        {
            Id = userId;
        }
    }
}