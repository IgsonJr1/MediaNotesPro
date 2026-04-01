using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using System.Net;

namespace MediaNotesPro.Api
{
    [ApiController]
    [Route("MediaNotes")]
    public class NotesController : ControllerBase
    {
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger<NotesController> _logger;

        public NotesController(ILibraryManager libraryManager, ILogger<NotesController> logger)
        {
            _libraryManager = libraryManager;
            _logger = logger;
        }

        private string GetSafePath(string userName, string mediaId)
        {
            var safeUser = string.Join("_", userName.Split(Path.GetInvalidFileNameChars()));
            var safeMedia = string.Join("_", mediaId.Split(Path.GetInvalidFileNameChars()));
            
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var path = Path.Combine(programData, "Jellyfin", "Server", "data", "MediaNotesData", safeUser);
            
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return Path.Combine(path, $"{safeMedia}.txt");
        }

        [HttpGet("{userName}/{mediaId}")]
        public ActionResult GetNotes([FromRoute] string userName, [FromRoute] string mediaId)
        {
            try
            {
                var filePath = GetSafePath(userName, mediaId);
                if (!System.IO.File.Exists(filePath)) return Ok(new { text = "" });
                return Ok(new { text = System.IO.File.ReadAllText(filePath) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{userName}/{mediaId}")]
        public ActionResult SaveNote([FromRoute] string userName, [FromRoute] string mediaId, [FromBody] NoteRequest request)
        {
            try
            {
                var filePath = GetSafePath(userName, mediaId);
                System.IO.File.WriteAllText(filePath, request.Text ?? "");
                return Ok(new { message = "Salvo com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class NoteRequest
    {
        public string Text { get; set; }
    }
}
