﻿using DataAccessLayer.Models;

namespace DataAccessLayer.Interfaces
{
    public interface IUserRepoisitory : IRepository<User>
    {
        Task<bool> AssignGroupToUsersAsync(int[] userIds, int groupId);

        Task<bool> CreateUser(User user, string password);


        Task<string> ForgotPassword(string email);
    }
}
