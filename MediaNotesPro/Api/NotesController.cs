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

        // Rota para Salvar: /MediaNotes/{itemId}
        [HttpPost("{itemId}")]
        public ActionResult SaveNote([FromRoute] string itemId, [FromBody] NoteRequest request)
        {
            try
            {
                // Caminho onde as notas serão salvas (Pasta do Plugin)
                var configPath = Path.Combine(AppContext.BaseDirectory, "plugins", "MediaNotesData");
                if (!Directory.Exists(configPath)) Directory.CreateDirectory(configPath);

                var filePath = Path.Combine(configPath, $"{itemId}.txt");

                // Adiciona a nova nota ao final do arquivo (Append)
                System.IO.File.AppendAllText(filePath, request.Text + Environment.NewLine);

                return Ok(new { message = "Nota salva com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao salvar nota: {0}", ex.Message);
                return StatusCode(500, "Erro interno ao salvar arquivo.");
            }
        }
    }

    public class NoteRequest
    {
        public string Text { get; set; }
    }
}
