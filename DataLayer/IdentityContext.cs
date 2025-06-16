using BusinessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class IdentityContext
    {
        UserManager<User> userManager;
        EventManagerDbContext context;

        public IdentityContext(EventManagerDbContext context_, UserManager<User> userManager)
        {
            this.context = context_;
            this.userManager = userManager;
        }

        public async Task<IdentityResultSet<User>> CreateUserAsync(string username, string password, string email, Role role)
        {
            try
            {
                User user = new User(username, email);
                IdentityResult result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    return new IdentityResultSet<User>(result, user);
                }

                if (role == Role.ADMINISTRATOR)
                {
                    await userManager.AddToRoleAsync(user, Role.ADMINISTRATOR.ToString());
                }
                else
                {
                    await userManager.AddToRoleAsync(user, Role.USER.ToString());
                }

                return new IdentityResultSet<User>(IdentityResult.Success, user);
            }
            catch (Exception ex)
            {
                IdentityResult identityResult = IdentityResult.Failed(new IdentityError()
                { Code = "Registration", Description = ex.Message });

                return new IdentityResultSet<User>(identityResult, null);
            }
        }

        public async Task<User> LogInUserAsync(string username, string password)
        {
            try
            {
                User user = await userManager.FindByNameAsync(username);

                if (user == null)
                {
                    return null;
                }

                IdentityResult result = await userManager.PasswordValidators[0].ValidateAsync(userManager, user, password);

                if (result.Succeeded)
                {
                    return await context.Users.FindAsync(user.Id);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> ReadUserAsync(string key, bool useNavigationalProperties = false)
        {
            try
            {
                if (!useNavigationalProperties)
                {
                    return await userManager.FindByIdAsync(key);
                }
                else
                {

                    User user = await context.Users
                                                .Include(u => u.Events)
                                                .FirstOrDefaultAsync(x => x.Id == key);

                    return user;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<User>> ReadAllUsersAsync(bool useNavigationalProperties = false)
        {
            try
            {
                if (!useNavigationalProperties)
                {
                    return await context.Users.ToListAsync();
                }

                return await context.Users.Include(u => u.Events).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateUserAsync(string id, string username, string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(username))
                {
                    User user = await context.Users.FindAsync(id);
                    user.UserName = username;
                    user.Email = email;
                    await userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                User userFromDb = await context.Users.FindAsync(user.Id);
                if (userFromDb != null)
                {
                    userFromDb.Events = user.Events;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task DeleteUserByNameAsync(string name)
        {
            try
            {
                User user = await FindUserByNameAsync(name);

                if (user == null)
                {
                    throw new InvalidOperationException("User not found for deletion!");
                }

                await userManager.DeleteAsync(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<User> FindUserByNameAsync(string name, bool useNavigationalProperties = false)
        {
            try
            {
                var User = await userManager.FindByNameAsync(name);
                if (!useNavigationalProperties)
                {
                    return await userManager.FindByIdAsync(User.Id);
                }
                else
                {
                    return await context.Users.Include(u => u.Events).FirstOrDefaultAsync(x => x.Id == User.Id);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
