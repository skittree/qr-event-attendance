using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using register_app.Data.Models;
using register_app.ViewModels;


namespace register_app.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventViewModel>();
            CreateMap<Event, EventEditViewModel>();
            CreateMap<Event, EventDeleteViewModel>();
            CreateMap<EventCreateViewModel, Event>();
            CreateMap<EventEditViewModel, Event>();
            CreateMap<Event, AttendeeCreateViewModel>()
                .ForMember(x => x.Name, opt => opt.Ignore())
                .ForMember(x => x.Event, opt => opt.MapFrom(src => src))
                .ForMember(x => x.EventId, opt => opt.MapFrom(src => src.Id));


            CreateMap<Attendee, AttendeeViewModel>();
            CreateMap<Attendee, AttendeeEditViewModel>();
            CreateMap<Attendee, AttendeeDeleteViewModel>();
            CreateMap<AttendeeCreateViewModel, Attendee>();
            CreateMap<AttendeeEditViewModel, Attendee>();



        }
    }
}
