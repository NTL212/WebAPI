using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;


namespace ProductDataAccess.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepoisitory
    {
        public UserRepository(ProductCategoryContext context) : base(context)
        {
        }
    }
}
