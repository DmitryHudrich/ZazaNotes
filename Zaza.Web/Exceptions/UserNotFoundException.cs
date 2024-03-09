namespace Zaza.Web;

public class UserNotFoundException : Exception {
    public UserNotFoundException(string? message) : base(message) {
    }
}
