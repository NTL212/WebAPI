using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Implementations
{
    public class UserGroupRepository:Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
