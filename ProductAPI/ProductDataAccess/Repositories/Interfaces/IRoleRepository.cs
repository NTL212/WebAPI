using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Interfaces
{
	public interface IRoleRepository:IRepository<Role>
	{
		Task<Role> GetRoleByName(string name);
	}
}
