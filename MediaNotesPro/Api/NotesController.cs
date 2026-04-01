using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

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

        [HttpGet("{userName}/{mediaName}")]
        public ActionResult GetNotes([FromRoute] string userName, [FromRoute] string mediaName)
        {
            try
            {
                var safeUserName = string.IsNullOrWhiteSpace(userName) || userName == "undefined" ? "Geral" : userName;
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var userPath = Path.Combine(programData, "Jellyfin", "Server", "data", "MediaNotesData", safeUserName);
                var filePath = Path.Combine(userPath, $"{mediaName}.txt");

                if (!System.IO.File.Exists(filePath)) return Ok(new { text = "" });
                return Ok(new { text = System.IO.File.ReadAllText(filePath) });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("{userName}/{mediaName}")]
        public ActionResult SaveNote([FromRoute] string userName, [FromRoute] string mediaName, [FromBody] NoteRequest request)
        {
            try
            {
                var safeUserName = string.IsNullOrWhiteSpace(userName) || userName == "undefined" ? "Geral" : userName;
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var userPath = Path.Combine(programData, "Jellyfin", "Server", "data", "MediaNotesData", safeUserName);

                if (!Directory.Exists(userPath)) Directory.CreateDirectory(userPath);
                var filePath = Path.Combine(userPath, $"{mediaName}.txt");

                System.IO.File.WriteAllText(filePath, request.Text);
                return Ok(new { message = "Sincronizado!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao salvar: {0}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class NoteRequest
    {
        public string Text { get; set; }
    }
}
