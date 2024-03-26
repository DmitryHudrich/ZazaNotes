namespace Zaza.Web.Stuff.InteractLogic;

internal record class InteractResult<T>(bool Success, InteractEvent Event, T? Data = default, string? Error = default) {
    public static explicit operator T?(InteractResult<T> res) => res.Data ?? default;
};
internal record class InteractResult(bool Success, InteractEvent Event, string? Error = default);

