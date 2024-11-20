using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductDataAccess.Repositories.Implementations
{
    public class UserGroupRepository:Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
