namespace Zaza.Web.Stuff.InteractLogic.Auth;

internal record class InteractResult<T>(bool Success, InteractEvent Event, T? Data = default, string? Error = default);
internal record class InteractResult(bool Success, InteractEvent Event, string? Error = default);

