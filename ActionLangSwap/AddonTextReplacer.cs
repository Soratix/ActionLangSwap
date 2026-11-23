using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace ActionLangSwap;

/// <summary>
/// Parcourt les nœuds texte de certaines fenêtres du jeu juste avant leur rendu
/// et remplace ceux dont le contenu est exactement un nom d'action connu.
/// </summary>
public sealed unsafe class AddonTextReplacer : IDisposable
{
    // Fenêtres où apparaissent des noms d'actions.
    private static readonly string[] Addons =
    [
        "_CastBar",            // ta propre barre d'incantation
        "_TargetInfoCastBar",  // incantation de la cible (barre séparée)
        "_TargetInfo",         // incantation de la cible (barre intégrée)
        "_FocusTargetInfo",    // incantation de la cible focus
        "_EnemyList",          // liste des ennemis
        "_PartyList",          // incantations des membres du groupe
        "ActionDetail",        // infobulle d'une action
        "ActionMenu",          // menu des actions et des traits
        "_StatusCustom0", "_StatusCustom1", "_StatusCustom2", // (statuts, si activés)
    ];

    private readonly Plugin plugin;

    public AddonTextReplacer(Plugin plugin)
    {
        this.plugin = plugin;
        Plugin.AddonLifecycle.RegisterListener(AddonEvent.PreDraw, Addons, this.OnPreDraw);
    }

    public void Dispose()
        => Plugin.AddonLifecycle.UnregisterListener(AddonEvent.PreDraw, Addons, this.OnPreDraw);

    private void OnPreDraw(AddonEvent type, AddonArgs args)
    {
        var cfg = this.plugin.Configuration;
        if (!cfg.Enabled || !cfg.ReplaceInAddons || this.plugin.Translator.Count == 0)
            return;

        var addon = (AtkUnitBase*)args.Addon.Address;
        if (addon == null || !addon->IsVisible)
            return;

        this.Walk(&addon->UldManager, 0);
    }

    private void Walk(AtkUldManager* manager, int depth)
    {
        for (var i = 0; i < manager->NodeListCount; i++)
        {
            var node = manager->NodeList[i];
            if (node == null)
                continue;

            if (node->Type == NodeType.Text)
            {
                this.Replace((AtkTextNode*)node);
            }
            else if ((ushort)node->Type >= 1000 && depth < 4)
            {
                var component = ((AtkComponentNode*)node)->Component;
                if (component != null)
                    this.Walk(&component->UldManager, depth + 1);
            }
        }
    }

    private void Replace(AtkTextNode* node)
    {
        ref var text = ref node->NodeText;
        var byteLength = (int)text.Length;
        // Ignore les textes vides et ceux trop longs pour être un nom (descriptions...).
        if (byteLength < 2 || byteLength > this.plugin.Translator.MaxLength * 4)
            return;

        var current = text.ToString();
        // 0x02 = début d'une macro SeString (couleur, etc.) : on n'y touche pas.
        if (current.Contains('\u0002'))
            return;

        if (this.plugin.Translator.TryTranslateExact(current, out var translated))
            node->SetText(translated);
    }
}
