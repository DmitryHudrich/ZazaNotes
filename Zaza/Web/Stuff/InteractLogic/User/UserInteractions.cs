using Zaza.Web.DataBase.Entities;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.Stuff.InteractLogic.User;

internal class UserInteractions(ILogger<UserInteractions> logger, RepositoryContainer repositories) : InteractAbstract(logger, repositories) {
    public async Task<InteractResult<UserEntity?>> PullUserAsync(HttpContext context) => await PullUserAsync(context.GetId());
    public async Task<InteractResult<UserEntity?>> PullUserAsync(Guid id) {
        const InteractEvent INTERACT_EVENT = InteractEvent.RECEIVING;
        var user = await UserRepository.FindByFilterAsync(FindFilter.ID, id);
        var res = new Lazy<InteractResult<UserEntity?>>(() => new InteractResult<UserEntity?>(Success: true, Event: INTERACT_EVENT, Data: user));
        if (user == null) {
            var err = $"User {id} wasn't found in db.";
            logger.LogDebug($"{nameof(PullUserAsync)}: " + err);
            res = new Lazy<InteractResult<UserEntity?>>(() => new InteractResult<UserEntity?>(Success: false, Event: INTERACT_EVENT, Data: user, Error: err));
        }

        return res.Value;
    }

    public async Task<InteractResult> DeleteUserAsync(HttpContext context) => await DeleteUserAsync(context.GetId());
    public async Task<InteractResult> DeleteUserAsync(Guid id) {
        const InteractEvent INTERACT_EVENT = InteractEvent.DELETING;
        var userStatus = await UserRepository.DeleteByIdAsync(id);
        if (!userStatus) {
            var err = $"User {id} wasn't found";
            logger.LogDebug($"{nameof(DeleteUserAsync)}: {err}");
            return new InteractResult(Success: false, Event: INTERACT_EVENT, Error: err);
        }
        return new InteractResult(Success: true, Event: INTERACT_EVENT);
    }
}
