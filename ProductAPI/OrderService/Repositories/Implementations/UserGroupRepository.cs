using OrderService.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace OrderService.Repositories.Implementations
{
    public class UserGroupRepository:Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
