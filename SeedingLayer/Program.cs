using BusinessLayer;
using DataLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SeedingLayer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IdentityOptions options = new IdentityOptions();
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 5;

                DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
                builder.UseSqlServer(
                    "Server=DESKTOP-RD8LV0K;Database=EventManager;Trusted_Connection=True;Encrypt=False"
                );

                EventManagerDbContext dbContext = new EventManagerDbContext(builder.Options);



                UserManager<User> userManager = new UserManager<User>(
                    new UserStore<User>(dbContext), Options.Create(options),
                    new PasswordHasher<User>(), new List<IUserValidator<User>>() { new UserValidator<User>() },
                    new List<IPasswordValidator<User>>() { new PasswordValidator<User>() }, new UpperInvariantLookupNormalizer(),
                    new IdentityErrorDescriber(), new ServiceCollection().BuildServiceProvider(),
                    new Logger<UserManager<User>>(new LoggerFactory())
                    );

                IdentityContext identityContext = new IdentityContext(dbContext, userManager);

                dbContext.Roles.Add(new IdentityRole(Role.ADMINISTRATOR.ToString()) { NormalizedName = "ADMINISTRATOR" });
                dbContext.Roles.Add(new IdentityRole(Role.USER.ToString()) { NormalizedName = "USER" });
                await dbContext.SaveChangesAsync();



                User user = new User("evmanageradmin", "evmanageradmin@email.bg");
                IdentityResult res = await userManager.CreateAsync(user, "evmanageradminpassword");
                await userManager.AddToRoleAsync(user, Role.ADMINISTRATOR.ToString());

                IdentityResultSet<User> result = new IdentityResultSet<User>(res, user);

                Console.WriteLine("Roles added successfully!");

                if (result.IdentityResult.Succeeded)
                {
                    Console.WriteLine("Admin account added successfully!");
                }
                else
                {
                    Console.WriteLine("Admin account failed to be created!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

            }
        }
    }
}
