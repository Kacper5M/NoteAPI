using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using NoteAPI.Authorization;
using NoteAPI.Entities;
using NoteAPI.Exceptions;
using NoteAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace NoteAPI.Services
{
    public interface INoteService
    {
        NoteDto GetById(int id);
        IEnumerable<NoteDto> GetAll();
        IEnumerable<NoteDto> GetImportantNotes();
        int Create(CreateNoteDto dto);
        void Delete(int id);
        void Update(UpdateNoteDto dto, int id);
        void SetImportant(int id);
    }

    public class NoteService : INoteService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<NoteService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public NoteService(DatabaseContext dbContext, IMapper mapper, ILogger<NoteService> logger,
            IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public NoteDto GetById(int id)
        {
            var note = _dbContext
                       .Notes
                       .FirstOrDefault(r => r.Id == id);

            if (note is null)
            {
                throw new NotFoundException("Note not found");
            }
            
            if (!_userContextService.User.IsInRole("Admin"))
            {
                var authorizationResult = _authorizationService
                                      .AuthorizeAsync(_userContextService.User, note, new ResourceOperationRequirement(ResourceOperation.Read)).Result;

                if (!authorizationResult.Succeeded)
                {
                    throw new ForbidException();
                }
            }

            var result = _mapper.Map<NoteDto>(note);

            return result;
        }

        public IEnumerable<NoteDto> GetAll()
        {
            var notes = new List<Note>();

            if (_userContextService.User.IsInRole("Admin"))
            {
                 notes = _dbContext
                         .Notes
                         .ToList();
            }
            else
            {
                 notes = _dbContext
                         .Notes
                         .Where(x => x.UserId == _userContextService.GetUserID)
                         .ToList();
            }

            var notesDtos = _mapper.Map<List<NoteDto>>(notes);

            return notesDtos;
        }

        public IEnumerable<NoteDto> GetImportantNotes()
        {
            var notes = new List<Note>();

            if (_userContextService.User.IsInRole("Admin"))
            {
                notes = _dbContext
                        .Notes
                        .Where(x => x.IsImportant)
                        .ToList();
            }
            else
            {
                notes = _dbContext
                        .Notes
                        .Where(x => x.UserId == _userContextService.GetUserID & x.IsImportant)
                        .ToList();
            }

            var notesDtos = _mapper.Map<List<NoteDto>>(notes);

            return notesDtos;
        }

        public int Create(CreateNoteDto dto)
        {
            if (dto.NoteTitle == string.Empty)
            {
                string[] words = dto.NoteBody.Split();
                IEnumerable<string> firstThreeWords = words.Take(3);
                var noteTitle = string.Join(" ", firstThreeWords);

                dto.NoteTitle = noteTitle;
            }

            var note = _mapper.Map<Note>(dto);

            note.UserId  = _userContextService.GetUserID.Value;
            note.CreatedDate = DateTime.Now;
            note.ModifiedDate = DateTime.MinValue;

            _dbContext.Notes.Add(note);
            _dbContext.SaveChanges();

            return note.Id;
        }

        public void Delete(int id)
        {
            _logger.LogWarning($"[NoteService] DELETE action invoked - {id}");

            var note = _dbContext
                       .Notes
                       .FirstOrDefault(r => r.Id == id);

            if (note is null)
            {
                throw new NotFoundException("Note not found");
            }

            if (!_userContextService.User.IsInRole("Admin"))
            {
                var authorizationResult = _authorizationService
                              .AuthorizeAsync(_userContextService.User, note, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

                if (!authorizationResult.Succeeded)
                {
                    throw new ForbidException();
                }
            }

            _dbContext.Notes.Remove(note);
            _dbContext.SaveChanges();
        }

        public void Update(UpdateNoteDto dto, int noteID)
        {
            var note = _dbContext
                       .Notes
                       .FirstOrDefault(r => r.Id == noteID);
            
            if (note is null)
            {
                throw new NotFoundException("Note not found");
            }

            if (!_userContextService.User.IsInRole("Admin"))
            {
                var authorizationResult = _authorizationService
                                      .AuthorizeAsync(_userContextService.User, note, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

                if (!authorizationResult.Succeeded)
                {
                    throw new ForbidException();
                }
            }

            note = _mapper.Map(dto, note);
            note.ModifiedDate = DateTime.Now;

            _dbContext.Update(note);
            _dbContext.SaveChanges();

            _logger.LogWarning($"[NoteService] Note updated - {noteID}!");
        }

        public void SetImportant(int id)
        {
            var note = _dbContext
                       .Notes
                       .FirstOrDefault(r => r.Id == id);

            if (note is null)
            {
                throw new NotFoundException("Note not found");
            }

            if (!_userContextService.User.IsInRole("Admin"))
            {
                var authorizationResult = _authorizationService
                                      .AuthorizeAsync(_userContextService.User, note, new ResourceOperationRequirement(ResourceOperation.Read)).Result;

                if (!authorizationResult.Succeeded)
                {
                    throw new ForbidException();
                }
            }

            note.IsImportant = !note.IsImportant;
            note.ModifiedDate = DateTime.Now;

            _dbContext.Update(note);
            _dbContext.SaveChanges();

            _logger.LogWarning($"[NoteService] Set important - {id}!");
        }
    }
}