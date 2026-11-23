using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Windowing;

namespace ActionLangSwap;

public sealed class ConfigWindow : Window
{
    private static readonly ClientLanguage[] Languages =
        [ClientLanguage.English, ClientLanguage.French, ClientLanguage.German, ClientLanguage.Japanese];

    private static readonly string[] LanguageLabels = ["Anglais", "Français", "Allemand", "Japonais"];

    private readonly Plugin plugin;

    public ConfigWindow(Plugin plugin)
        : base("Action Language Swap###ActionLangSwapConfig", ImGuiWindowFlags.AlwaysAutoResize)
    {
        this.plugin = plugin;
        this.SizeConstraints = new WindowSizeConstraints { MinimumSize = new Vector2(320, 0) };
    }

    public override void Draw()
    {
        var cfg = this.plugin.Configuration;
        var needsRebuild = false;
        var changed = false;

        var enabled = cfg.Enabled;
        if (ImGui.Checkbox("Activé", ref enabled))
        {
            cfg.Enabled = enabled;
            changed = true;
        }

        var index = Array.IndexOf(Languages, cfg.TargetLanguage);
        if (index < 0) index = 0;
        ImGui.SetNextItemWidth(180);
        if (ImGui.Combo("Langue des actions", ref index, LanguageLabels, LanguageLabels.Length))
        {
            cfg.TargetLanguage = Languages[index];
            needsRebuild = true;
        }

        var statuses = cfg.IncludeStatuses;
        if (ImGui.Checkbox("Traduire aussi les statuts (buffs / debuffs)", ref statuses))
        {
            cfg.IncludeStatuses = statuses;
            needsRebuild = true;
        }

        ImGui.Separator();
        ImGui.TextUnformatted("Où remplacer :");

        var addons = cfg.ReplaceInAddons;
        if (ImGui.Checkbox("Interface (incantations, infobulles, menu des actions)", ref addons))
        {
            cfg.ReplaceInAddons = addons;
            changed = true;
        }

        var fly = cfg.ReplaceInFlyText;
        if (ImGui.Checkbox("Textes volants (dégâts / soins)", ref fly))
        {
            cfg.ReplaceInFlyText = fly;
            changed = true;
        }

        var chat = cfg.ReplaceInChat;
        if (ImGui.Checkbox("Journal de combat", ref chat))
        {
            cfg.ReplaceInChat = chat;
            changed = true;
        }

        ImGui.Separator();
        ImGui.TextDisabled($"Langue du client : {Plugin.ClientState.ClientLanguage}");
        ImGui.TextDisabled($"{this.plugin.Translator.Count} noms chargés");
        if (cfg.TargetLanguage == Plugin.ClientState.ClientLanguage)
            ImGui.TextColored(new Vector4(1f, 0.7f, 0.3f, 1f), "La langue choisie est déjà celle du client.");

        if (needsRebuild)
        {
            this.plugin.RebuildTranslator();
            changed = true;
        }

        if (changed)
            this.plugin.SaveConfiguration();
    }
}
