using ProductAPI.Repositories.Interfaces;
using ProductDataAccess.Models;

namespace ProductAPI.Repositories.Implementations
{
    public class UserRepository:Repository<User>, IUserRepoisitory 
    {
        public UserRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
