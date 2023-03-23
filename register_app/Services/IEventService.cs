using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using register_app.Data;
using register_app.Data.Models;
using register_app.Data.Roles;
using register_app.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace register_app.Services
{
    public interface IEventService
    {
        Task<EventViewModel> GetViewModelAsync(int id);
        Task<EventEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user);
        Task<EventDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal user);
        Task<List<EventViewModel>> GetIndexViewModelAsync();
        EventCreateViewModel GetCreateViewModel();
        Task CreateAsync(EventCreateViewModel model, ClaimsPrincipal user);
        Task EditAsync(EventEditViewModel model, ClaimsPrincipal user);
        Task DeleteAsync(EventDeleteViewModel model, ClaimsPrincipal user);
    }

    public class EventService : IEventService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private IFormService FormService { get; }

        public EventService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IFormService formService)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            FormService = formService;
        }

/*        public async Task<> UpdateEventFromForm(string formid) 
        {
            var event = Context.Events
                .Include()
        }*/

        public async Task<List<EventViewModel>> GetIndexViewModelAsync()
        {
            var events = await Context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Attendees) /*i am not sure if displaying everyone will be overkill for the index menu*/
                .ToListAsync();

            var model = Mapper.Map<List<EventViewModel>>(events);
            return model;
        }

        public async Task<EventViewModel> GetViewModelAsync(int id)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            return Mapper.Map<EventViewModel>(event_);
        }
        public EventCreateViewModel GetCreateViewModel()
        {
            return new EventCreateViewModel { };
        }

        public async Task<EventEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == event_.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            return Mapper.Map<EventEditViewModel>(event_);
        }
        public async Task<EventDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal user)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == event_.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            return Mapper.Map<EventDeleteViewModel>(event_);
        }

        public async Task CreateAsync(EventCreateViewModel model, ClaimsPrincipal User)
        {
            if (Context.Events.Any(x => x.Name.ToLower() == model.Name.ToLower()))
            {
                throw new ArgumentException($"Event with name {model.Name} already exists.");
            }

            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var newEvent = Mapper.Map<Event>(model);
            newEvent.Organiser = user;
            newEvent.FormId = await FormService.CreateFormAsync(model);

            Context.Events.Add(newEvent);
            await Context.SaveChangesAsync();
        }

        public async Task EditAsync(EventEditViewModel model, ClaimsPrincipal user)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == event_.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            var withSameName = await Context.Events.FirstOrDefaultAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (withSameName != null && event_.Id != withSameName.Id)
            {
                throw new ArgumentException($"An event with name {model.Name} already exists.");
            }

            event_.Name = model.Name;
            event_.Description = model.Description;
            event_.StartTime = model.StartTime;
            event_.EndTime = model.EndTime;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(EventDeleteViewModel model, ClaimsPrincipal user)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == event_.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }
            //add code here to also remove the form & maybe watch?
            Context.Events.Remove(event_);
            await Context.SaveChangesAsync();
        }
    }
}
