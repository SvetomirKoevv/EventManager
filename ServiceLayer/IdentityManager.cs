﻿using BusinessLayer;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public class IdentityManager
    {
        private readonly IdentityContext _identityContext;

        public IdentityManager(IdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        public async Task<UserResultSet> CreateUserAsync(string username, string password, string email, Role role)
        {
            IdentityResultSet<User> resultSet = await _identityContext.CreateUserAsync(username, password, email, role);
            return new UserResultSet(resultSet.IdentityResult, resultSet.Entity);
        }

        public async Task<User> LogInUserAsync(string username, string password)
        {
            return await _identityContext.LogInUserAsync(username, password);
        }

        public async Task<User> ReadUserAsync(string key, bool useNavigationalProperties = false)
        {
            return await _identityContext.ReadUserAsync(key, useNavigationalProperties);
        }

        public async Task<IEnumerable<User>> ReadAllUsersAsync(bool useNavigationalProperties = false)
        {
            return await _identityContext.ReadAllUsersAsync(useNavigationalProperties);
        }

        public async Task UpdateUserAsync(string id, string username, string email)
        {
            await _identityContext.UpdateUserAsync(id, username, email);
        }

        public async Task DeleteUserByNameAsync(string name)
        {
            await _identityContext.DeleteUserByNameAsync(name);
        }

        public async Task<User> FindUserByNameAsync(string name)
        {
            return await _identityContext.FindUserByNameAsync(name);
        }
    }
}
