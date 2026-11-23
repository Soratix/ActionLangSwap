using Dalamud.Configuration;
using Dalamud.Game;

namespace ActionLangSwap;

[Serializable]
public sealed class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 1;

    public bool Enabled { get; set; } = true;

    /// <summary>Langue dans laquelle afficher les noms des actions.</summary>
    public ClientLanguage TargetLanguage { get; set; } = ClientLanguage.English;

    /// <summary>Traduire aussi les noms des statuts (buffs / debuffs).</summary>
    public bool IncludeStatuses { get; set; } = false;

    /// <summary>Remplacer dans l'interface (barres d'incantation, infobulles, menu des actions...).</summary>
    public bool ReplaceInAddons { get; set; } = true;

    /// <summary>Remplacer dans les textes volants (dégâts / soins au-dessus des personnages).</summary>
    public bool ReplaceInFlyText { get; set; } = true;

    /// <summary>Remplacer dans le journal de combat.</summary>
    public bool ReplaceInChat { get; set; } = true;
}
