using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using System.Collections.Generic;

namespace MediaNotesPro;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    public override string Name => "Media Notes Pro";

    public override Guid Id => Guid.Parse("a5e2f1b4-8c9a-4d2e-9b3c-1a2b3c4d5e6f");

    public override string Description => "Crie anotações vinculadas ao tempo da mídia.";

    public static Plugin? Instance { get; private set; }

    public IEnumerable<PluginPageInfo> GetPages()
{
    return new[]
    {
        new PluginPageInfo
        {
            Name = "medianotes",
            EmbeddedResourcePath = "MediaNotesPro.Pages.notespage.html",
            EnableInMainMenu = true,
            MenuSection = "userinfo", // Mudamos para ser mais visível no Mobile
            MenuIcon = "edit_note",
            DisplayName = "Media Notes Pro"
        }
    };
}
