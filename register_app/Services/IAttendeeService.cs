using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using register_app.Data;
using register_app.Data.Models;
using register_app.Data.Roles;
using register_app.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace register_app.Services
{
    public interface IAttendeeService
    {
        Task<AttendeeEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user);
        Task<AttendeeDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal user);
        Task<List<AttendeeViewModel>> GetIndexViewModelAsync(int id);
        Task<AttendeeCreateViewModel> GetCreateViewModelAsync(int? id);
        Task CreateAsync(AttendeeCreateViewModel model, ClaimsPrincipal User);

        Task CreateAutomaticAsync(AttendeeCreateViewModel model);
        Task EditAsync(AttendeeEditViewModel model, ClaimsPrincipal user);
        Task DeleteAsync(AttendeeDeleteViewModel model, ClaimsPrincipal user);

        Task<AttendeeViewModel> AuthenticateAttendeeAsync(string key);

        Task RefreshAttendeesAsync(string form_id);
    }

    public class AttendeeService : IAttendeeService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }

        private IEmailService EmailService { get; }

        private IFormService FormService { get; }

        public AttendeeService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            IEmailService emailService,
            IFormService formService)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            EmailService = emailService;
            FormService = formService;
        }

        public async Task<List<AttendeeViewModel>> GetIndexViewModelAsync(int id)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            var model = Mapper.Map<List<AttendeeViewModel>>(event_.Attendees);
            return model;
        }

        public async Task<AttendeeCreateViewModel> GetCreateViewModelAsync(int? id)
        {
            var event_ = await Context.Events
                .FirstOrDefaultAsync(x => x.Id == id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }

            var createViewModel = Mapper.Map<AttendeeCreateViewModel>(event_);

            return createViewModel;
        }

        public async Task<AttendeeEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user)
        {
            var attendee = await Context.Attendees
                .Include(x => x.Event)
                .ThenInclude(y => y.Organiser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (attendee == null)
            {
                throw new ArgumentNullException(nameof(attendee));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == attendee.Event.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            return Mapper.Map<AttendeeEditViewModel>(attendee);
        }
        public async Task<AttendeeDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal user)
        {
            var attendee = await Context.Attendees
                .Include(x => x.Event)
                .ThenInclude(y => y.Organiser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (attendee == null)
            {
                throw new ArgumentNullException(nameof(attendee));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == attendee.Event.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            return Mapper.Map<AttendeeDeleteViewModel>(attendee);
        }

        public async Task CreateAsync(AttendeeCreateViewModel model, ClaimsPrincipal User)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == model.EventId);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }
            if (!User.IsInRole(Roles.Admin) || !User.IsInRole(Roles.Organiser))
            {
                
                throw new ArgumentException("User did not create this event");
                
            }
            var newAttendee = Mapper.Map<Attendee>(model);
            newAttendee.Event = event_;

            newAttendee.Key = Guid.NewGuid().ToString() + DateTime.Now.ToString();

            Context.Attendees.Add(newAttendee);
            await Context.SaveChangesAsync();

            await EmailService.SendEmail(newAttendee.Email, newAttendee.Name, newAttendee.Key, event_.Name, event_.StartTime);


        }
        public async Task CreateAutomaticAsync(AttendeeCreateViewModel model)
        {
            var event_ = await Context.Events
                .Include(x => x.Organiser)
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.Id == model.EventId);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }
            
            var newAttendee = Mapper.Map<Attendee>(model);
            newAttendee.Event = event_;

            newAttendee.Key = Guid.NewGuid().ToString() + DateTime.Now.ToString();

            Context.Attendees.Add(newAttendee);
            await Context.SaveChangesAsync();

            await EmailService.SendEmail(newAttendee.Email, newAttendee.Name, newAttendee.Key, event_.Name, event_.StartTime);


        }


        public async Task EditAsync(AttendeeEditViewModel model, ClaimsPrincipal user)
        {
            var attendee = await Context.Attendees
                .Include(x => x.Event)
                .ThenInclude(x => x.Organiser)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (attendee == null)
            {
                throw new ArgumentNullException(nameof(attendee));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == attendee.Event.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            attendee.Name = model.Name;
            attendee.Email = model.Email;
            attendee.TimeArrived = model.TimeArrived;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AttendeeDeleteViewModel model, ClaimsPrincipal user)
        {
            var attendee = await Context.Attendees
                .Include(x => x.Event)
                .ThenInclude(x => x.Organiser)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (attendee == null)
            {
                throw new ArgumentNullException(nameof(attendee));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!(user.Identity.Name == attendee.Event.Organiser.UserName))
                {
                    throw new ArgumentNullException("User did not create this event");
                }
            }

            Context.Attendees.Remove(attendee);
            await Context.SaveChangesAsync();
        }

        public async Task<AttendeeViewModel> AuthenticateAttendeeAsync(string key)
        {
            var attendee_ = await Context.Attendees
                .Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.Key == key);

            if (attendee_ == null)
            {
                throw new ArgumentNullException(nameof(attendee_));
            }

            var model = Mapper.Map<AttendeeViewModel>(attendee_);
            return model;
        }

        public async Task RefreshAttendeesAsync(string form_id)
        {
            var attendees = await FormService.GetFormResponsesAsync(form_id);
            var event_ = await Context.Events
                .Include(x => x.Attendees)
                .FirstOrDefaultAsync(x => x.FormId == form_id);

            if (event_ == null)
            {
                throw new ArgumentNullException(nameof(event_));
            }


            foreach (var attendee in attendees)
            {
                var find_attendee = event_.Attendees.FirstOrDefault(x => x.Email == attendee.Email);
                if (find_attendee == null)
                {
                    attendee.EventId = event_.Id;
                    if (new EmailAddressAttribute().IsValid(attendee.Email))
                        await CreateAutomaticAsync(attendee);
                }
            }

            return;

        }
    }
}
