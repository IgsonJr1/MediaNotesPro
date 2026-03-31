using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;
using MediaBrowser.Model.Serialization;

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

        // Rota para Salvar: POST /MediaNotes/{itemId}
        [HttpPost("{itemId}")]
        public ActionResult SaveNote([FromRoute] string itemId, [FromBody] NoteRequest request)
        {
            try
            {
                // Define o caminho em C:\ProgramData\Jellyfin\Server\data\MediaNotesData
                // Este é o local mais seguro para evitar Erro 500 de permissão no Windows
                var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                var configPath = Path.Combine(programData, "Jellyfin", "Server", "data", "MediaNotesData");

                if (!Directory.Exists(configPath))
                {
                    Directory.CreateDirectory(configPath);
                }

                var filePath = Path.Combine(configPath, $"{itemId}.txt");

                // Formata a linha com quebra de linha no final para o próximo "Append"
                var contentToSave = request.Text + Environment.NewLine;

                // AppendAllText cria o arquivo se não existir e adiciona ao final se já existir
                System.IO.File.AppendAllText(filePath, contentToSave);

                _logger.LogInformation("Nota salva com sucesso para o item {0}", itemId);
                
                return Ok(new { message = "Nota registrada!" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao salvar nota: {0}", ex.ToString());
                // Retornamos o erro detalhado para ajudar no diagnóstico se o 500 persistir
                return StatusCode(500, $"Erro no Servidor: {ex.Message}");
            }
        }

        // Rota opcional para verificar se o plugin responde (Teste de URL)
        [HttpGet("ping")]
        public ActionResult Ping()
        {
            return Ok("Plugin MediaNotesPro está Online!");
        }
    }

    public class NoteRequest
    {
        public string Text { get; set; }
    }
}
