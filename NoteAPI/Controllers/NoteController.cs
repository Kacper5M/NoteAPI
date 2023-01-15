using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteAPI.Models;
using NoteAPI.Services;

namespace NoteAPI.Controllers
{
    [Route("api/note")]
    [ApiController]
    [Authorize]
    public class NoteController : ControllerBase
    {
        private readonly INoteService _noteService;

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<NoteDto>> GetAll()
        {
            IEnumerable<NoteDto> notesDtos = null;

            notesDtos = _noteService.GetAll();

            return Ok(notesDtos);
        }

        [HttpGet("getImportant")]
        public ActionResult<IEnumerable<NoteDto>> GetImportantNotes()
        {
            IEnumerable<NoteDto> notesDtos = null;

            notesDtos = _noteService.GetImportantNotes();

            return Ok(notesDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<NoteDto> Get([FromRoute] int id)
        {
            var note = _noteService.GetById(id);

            return Ok(note);
        }

        [HttpPost]
        public ActionResult CreateNote([FromBody] CreateNoteDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var noteId = _noteService.Create(dto);

            return Created($"/api/note/{noteId}", null);
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _noteService.Delete(id);

            return NoContent();
        }

        [HttpPatch("{id}")]
        public ActionResult Update([FromBody] UpdateNoteDto dto, [FromRoute] int id)
        {
            if(!ModelState.IsValid || dto is null)
            {
                return BadRequest(ModelState);
            }

            _noteService.Update(dto, id);

            return Ok();
        }

        [HttpPatch("setImportant/{id}")]
        public ActionResult SetImportant([FromRoute] int id)
        {
            _noteService.SetImportant(id);

            return Ok();
        }
    }
}