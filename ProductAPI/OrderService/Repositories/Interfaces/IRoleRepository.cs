using ProductDataAccess.Models;

namespace OrderService.Repositories.Interfaces
{
	public interface IRoleRepository:IRepository<Role>
	{
		Task<Role> GetRoleByName(string name);
	}
}
