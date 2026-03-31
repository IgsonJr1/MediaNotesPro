using System;
using System.IO;
using System.Collections.Generic;
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

        // Carregar Notas: GET /MediaNotes/{userId}/{mediaName}
        [HttpGet("{userId}/{mediaName}")]
        public ActionResult GetNotes([FromRoute] string userId, [FromRoute] string mediaName)
        {
            try
            {
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var userPath = Path.Combine(programData, "Jellyfin", "Server", "data", "MediaNotesData", userId);
                var filePath = Path.Combine(userPath, $"{mediaName}.txt");

                if (!System.IO.File.Exists(filePath)) return Ok(new { text = "" });

                var content = System.IO.File.ReadAllText(filePath);
                return Ok(new { text = content });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Salvar Notas: POST /MediaNotes/{userId}/{mediaName}
        [HttpPost("{userId}/{mediaName}")]
        public ActionResult SaveNote([FromRoute] string userId, [FromRoute] string mediaName, [FromBody] NoteRequest request)
        {
            try
            {
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var userPath = Path.Combine(programData, "Jellyfin", "Server", "data", "MediaNotesData", userId);

                if (!Directory.Exists(userPath)) Directory.CreateDirectory(userPath);

                var filePath = Path.Combine(userPath, $"{mediaName}.txt");

                // Sobrescreve o arquivo com o conteúdo total (edição em tempo real)
                System.IO.File.WriteAllText(filePath, request.Text);

                return Ok(new { message = "Sincronizado!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro: {0}", ex.Message);
                return StatusCode(500, ex.Message);
            }
        }
    }

    public class NoteRequest
    {
        public string Text { get; set; }
    }
}
