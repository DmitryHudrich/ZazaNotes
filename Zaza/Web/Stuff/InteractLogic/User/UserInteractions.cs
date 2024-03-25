using Zaza.Web.DataBase.Entities;
using Zaza.Web.DataBase.Repository;

namespace Zaza.Web.Stuff.InteractLogic.User;

internal class UserInteractions(ILogger<UserInteractions> logger, RepositoryContainer repositories) : InteractAbstract(logger, repositories) {
    public async Task<InteractResult<UserEntity?>> PullUser(HttpContext context) => await PullUser(context.GetId());
    public async Task<InteractResult<UserEntity?>> PullUser(Guid id) {
        const InteractEvent INTERACT_EVENT = InteractEvent.RECEIVING;
        var user = await UserRepository.FindByFilterAsync(FindFilter.ID, id);
        var res = new Lazy<InteractResult<UserEntity?>>(() => new InteractResult<UserEntity?>(Success: true, Event: INTERACT_EVENT, Data: user));
        if (user == null) {
            var err = $"User {id} wasn't found in db.";
            logger.LogDebug($"{nameof(PullUser)}: " + err);
            res = new Lazy<InteractResult<UserEntity?>>(() => new InteractResult<UserEntity?>(Success: false, Event: INTERACT_EVENT, Data: user, Error: err));
        }

        return res.Value;
    }
}
