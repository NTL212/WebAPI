using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Interfaces
{
	public interface IRoleRepository:IRepository<Role>
	{
		Task<Role> GetRoleByName(string name);
	}
}
