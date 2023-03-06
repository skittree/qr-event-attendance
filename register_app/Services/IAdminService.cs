using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using register_app.ViewModels;
using register_app.Data;
using register_app.Areas.Identity.Pages.Account;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace register_app.Services
{
    public interface IAdminService
    {


        public RegisterViewModel GetCreateModelAsync();

        Task AddUserRoleOrganiser(string username);
        Task AddUserRoleAdmin(string username);
        Task AddUserRoleSecurity(string username);
        Task RemoveRoleOrganiser(string username);
        Task RemoveRoleAdmin(string username);
        Task RemoveRoleSecurity(string username);

        Task RegisterAsync(RegisterViewModel model);
        Task<List<AccountViewModel>> GetIndexViewModelAsync();


    }

    public class AdminService : IAdminService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private SignInManager<IdentityUser> SignInManager { get; }

        private IEmailSender EmailSender { get; }

        private ILogger<RegisterModel> Logger { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        public AdminService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment appEnvironment)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            SignInManager = signInManager;
            AppEnvironment = appEnvironment;
        }

        public RegisterViewModel GetCreateModelAsync()
        {
            return new RegisterViewModel { };
        }
        public async Task RegisterAsync(RegisterViewModel model)
        {
            var withSameEmail = await Context.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == model.Email.ToLower());
            if (withSameEmail != null)
            {
                throw new ArgumentException($"User with {model.Email} is already registered.");
            }

            var identityResult = await UserManager.CreateAsync(
                new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    UserName = model.UserName
                },
                model.Password);

            if (identityResult.Succeeded)
            {
                return;
            }

            throw new Exception(identityResult.Errors.First().Description);
        }


        public async Task<List<AccountViewModel>> GetIndexViewModelAsync()
        {
            var users = await Context.Users.ToListAsync();
            var model = Mapper.Map<List<AccountViewModel>>(users);
            
            return model;
        }
        public async Task AddUserRoleOrganiser(string user)
        {
            var userAcc = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == user);
            if (userAcc == null)
            {
                throw new ArgumentNullException(nameof(userAcc));
            }

            await UserManager.AddToRoleAsync(userAcc, "Organiser");
        }

        public async Task AddUserRoleSecurity(string user)
        {
            var userAcc = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == user);
            if (userAcc == null)
            {
                throw new ArgumentNullException(nameof(userAcc));
            }

            await UserManager.AddToRoleAsync(userAcc, "Security");
        }

        public async Task AddUserRoleAdmin(string user)
        {
            var userAcc = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == user);
            if (userAcc == null)
            {
                throw new ArgumentNullException(nameof(userAcc));
            }

            await UserManager.AddToRoleAsync(userAcc, "Admin");
        }




        public async Task RemoveRoleOrganiser(string username)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }


            var userEvents = await Context.Events.Where(x => x.Organiser.UserName == username).ToListAsync();

            if (userEvents != null)
            {
                foreach (var userEvent in userEvents)
                {
                    userEvent.Organiser = null;
                }
                await Context.SaveChangesAsync();
            }



            await UserManager.RemoveFromRoleAsync(user, "Organiser");
        }

        public async Task RemoveRoleSecurity(string username)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await UserManager.RemoveFromRoleAsync(user, "Security");
        }
        public async Task RemoveRoleAdmin(string username)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await UserManager.RemoveFromRoleAsync(user, "Admin");
        }

    }
}