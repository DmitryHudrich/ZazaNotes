namespace Zaza.Web.Exceptions;

public class UserNotFoundException(string? message) : Exception(message) {
}
