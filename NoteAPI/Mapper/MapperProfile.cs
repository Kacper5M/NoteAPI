using AutoMapper;
using NoteAPI.Entities;
using NoteAPI.Models;
using System;

namespace NoteAPI.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Note, NoteDto>();

            CreateMap<CreateNoteDto, Note>()
                .ForMember(dest => dest.NoteTitle, opt => opt.Condition(src => !string.IsNullOrEmpty(src.NoteTitle)))
                .ForMember(dest => dest.NoteBody, opt => opt.Condition(src => !string.IsNullOrEmpty(src.NoteBody)))
                .ForMember(dest => dest.Color, opt => opt.Condition(src => src.Color != default))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.FirstName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.FirstName)))
                .ForMember(dest => dest.LastName, opt => opt.Condition(src => !string.IsNullOrEmpty(src.LastName)))
                .ForMember(dest => dest.Email, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Email)))
                .ForMember(dest => dest.DateOfBirth, opt => opt.Condition(src => src.DateOfBirth > DateTime.MinValue));

        }
    }
}
