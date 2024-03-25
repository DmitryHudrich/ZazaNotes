using Zaza.Web.StorageInterfaces;

namespace Zaza.Web.Stuff.InteractLogic;

internal abstract class InteractAbstract(ILogger<InteractAbstract> logger, RepositoryContainer repositories) {
    protected ILogger<InteractAbstract> Logger => logger;
    protected IUserRepository UserRepository => repositories.ForUser;
    protected INoteRepository NoteRepository => repositories.ForNotes;
}

