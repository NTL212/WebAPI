using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderService.Repositories.Interfaces;
using ProductDataAccess.DTOs;
using ProductDataAccess.Models;

namespace OrderService.Repositories.Implementations
{
    public class UserRepository : Repository<User>, IUserRepoisitory
    {
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        public UserRepository(ProductCategoryContext context, PasswordHasher<User> passwordHasher) : base(context)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AssignGroupToUsersAsync(int[] userIds, int groupId)
        {
            var users = await _dbSet.Where(u => userIds.Contains(u.UserId)).ToListAsync();
            foreach (var user in users)
            {
                user.GroupId = groupId;
            }

            _dbSet.UpdateRange(users);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateUser(User user, string password)
        {
            try
            {
                user.PasswordHash = user.PasswordHash = _passwordHasher.HashPassword(user, password); // Mã hóa mật khẩu
                user.CreatedAt = DateTime.Now;
                user.RoleId = 1;
                await _dbSet.AddAsync(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
