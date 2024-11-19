using OrderService.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace OrderService.Repositories
{
	public class UserRoleRepository:Repository<UserRole>, IUserRoleRepository
	{
		public UserRoleRepository(ProductCategoryContext context) : base(context)
		{
		}
	}
}
