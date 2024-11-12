using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories
{
	public class UserRoleRepository:Repository<UserRole>, IUserRoleRepository
	{
		public UserRoleRepository(ProductCategoryContext context) : base(context)
		{
		}
	}
}
