


using AutoMapper;
using Microsoft.AspNetCore.Identity;
using register_app.Data;
using Google.Apis.Forms.v1;

namespace register_app.Services
{
    public interface IFormsService
    {
    }

    public class FormService : IFormsService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }


        public FormService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
        }


        public void CreateForm()
        {
            
        }
    }
}
