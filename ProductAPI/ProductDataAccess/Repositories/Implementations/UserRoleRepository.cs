using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories
{
	public class UserRoleRepository:Repository<UserRole>, IUserRoleRepository
	{
		public UserRoleRepository(ProductCategoryContext context) : base(context)
		{
		}
	}
}
