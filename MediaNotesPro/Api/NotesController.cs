using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace MediaNotesPro.Api;

[ApiController]
[Route("MediaNotes")]
[Authorize]
public class NotesController : ControllerBase
{
    // Caminho onde as notas serão salvas (dentro da pasta de dados do Jellyfin)
    private string GetNotePath(string itemId) 
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "InternalId")?.Value ?? "default";
        var folder = Path.Combine("plugins", "MediaNotesData");
        Directory.CreateDirectory(folder);
        return Path.Combine(folder, $"{userId}_{itemId}.md");
    }

    [HttpGet("{itemId}")]
    public IActionResult GetNote([FromRoute] string itemId)
    {
        var path = GetNotePath(itemId);
        if (!File.Exists(path)) return Ok(new { text = "" });
        return Ok(new { text = File.ReadAllText(path) });
    }

    [HttpPost("{itemId}")]
    public IActionResult SaveNote([FromRoute] string itemId, [FromBody] NoteRequest request)
    {
        System.IO.File.WriteAllText(GetNotePath(itemId), request.Text);
        return NoContent();
    }
}

public class NoteRequest { public string Text { get; set; } = ""; }
