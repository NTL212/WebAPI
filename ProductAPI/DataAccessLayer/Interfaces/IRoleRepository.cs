using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetRoleByName(string name);
    }
}
