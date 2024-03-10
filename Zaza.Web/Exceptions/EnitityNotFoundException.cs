namespace Zaza.Web;

internal class EnitityNotFoundException : Exception {
    public EnitityNotFoundException(string? message) : base(message) {
    }
}
