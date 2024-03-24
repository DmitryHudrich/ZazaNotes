using Zaza.Web.StorageInterfaces;

namespace Zaza.Web.Stuff;

internal class RepositoryContainer(IUserRepository userRepository, INoteRepository noteRepository) {
    public IUserRepository ForUser => userRepository;
    public INoteRepository ForNotes => noteRepository;
}
