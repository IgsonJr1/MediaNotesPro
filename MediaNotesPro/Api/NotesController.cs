using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace MediaNotesPro.Api;

[ApiController]
[Route("MediaNotes")]
[Authorize]
public class NotesController : ControllerBase
{
    private string GetNotePath(string itemId) 
    {
        // Pega o ID do usuário logado
        var userId = User.Claims.FirstOrDefault(c => c.Type == "InternalId")?.Value ?? "default";
        
        // Cria uma pasta chamada MediaNotesData dentro da pasta do Jellyfin
        var folder = Path.Combine("plugins", "MediaNotesData");
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        
        return Path.Combine(folder, $"{userId}_{itemId}.md");
    }

    [HttpGet("{itemId}")]
    public IActionResult GetNote([FromRoute] string itemId)
    {
        var path = GetNotePath(itemId);
        
        // Usamos System.IO.File para evitar conflito com o controlador
        if (!System.IO.File.Exists(path)) 
        {
            return Ok(new { text = "" });
        }
        
        return Ok(new { text = System.IO.File.ReadAllText(path) });
    }

    [HttpPost("{itemId}")]
    public IActionResult SaveNote([FromRoute] string itemId, [FromBody] NoteRequest request)
    {
        var path = GetNotePath(itemId);
        System.IO.File.WriteAllText(path, request.Text);
        return NoContent();
    }
}

public class NoteRequest 
{ 
    public string Text { get; set; } = ""; 
}
