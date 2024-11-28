using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataAccessLayer.DTOs;
using DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;
using DataAccessLayer.Interfaces;

namespace DataAccessLayer.Repositories
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
                user.RoleId = 2;
                await _dbSet.AddAsync(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> ForgotPassword(string email)
        {
            var newPassword = GenerateRandomPassword();
            var user = _dbSet.FirstOrDefault(u => u.Email == email);
            user.PasswordHash = user.PasswordHash = user.PasswordHash = _passwordHasher.HashPassword(user, newPassword); // Mã hóa mật khẩu
            _dbSet.Update(user);

            if (await _context.SaveChangesAsync() > 0)
                return newPassword;
            else
                return "";
        }



        private string GenerateRandomPassword(int length = 6)
        {
            // Định nghĩa các ký tự có thể sử dụng trong mật khẩu
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            // Tạo đối tượng Random để sinh số ngẫu nhiên
            Random random = new Random();

            // Xây dựng mật khẩu
            char[] password = new char[length];
            for (int i = 0; i < length; i++)
            {
                password[i] = validChars[random.Next(validChars.Length)];
            }

            // Trả về mật khẩu dưới dạng chuỗi
            return new string(password);
        }
    }
}
