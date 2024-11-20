using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Interfaces
{
    public interface IUserRepoisitory:IRepository<User>
    {
        Task<bool> AssignGroupToUsersAsync(int[] userIds, int groupId);

        Task<bool> CreateUser(User user, string password);
    }
}
