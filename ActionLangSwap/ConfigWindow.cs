using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Interface.Windowing;

namespace ActionLangSwap;

public sealed class ConfigWindow : Window
{
    private static readonly ClientLanguage[] Languages =
        [ClientLanguage.English, ClientLanguage.French, ClientLanguage.German, ClientLanguage.Japanese];


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
        if (ImGui.Checkbox($"{Loc.Get("Enabled")}###enabled", ref enabled))
        {
            cfg.Enabled = enabled;
            changed = true;
        }

        var index = Array.IndexOf(Languages, cfg.TargetLanguage);
        if (index < 0) index = 0;
        ImGui.SetNextItemWidth(180);
        var labels = Languages.Select(Loc.LanguageName).ToArray();
        if (ImGui.Combo($"{Loc.Get("ActionLanguage")}###language", ref index, labels, labels.Length))
        {
            cfg.TargetLanguage = Languages[index];
            needsRebuild = true;
        }

        var statuses = cfg.IncludeStatuses;
        if (ImGui.Checkbox($"{Loc.Get("IncludeStatuses")}###statuses", ref statuses))
        {
            cfg.IncludeStatuses = statuses;
            needsRebuild = true;
        }

        ImGui.Separator();
        ImGui.TextUnformatted(Loc.Get("ReplaceIn"));

        var addons = cfg.ReplaceInAddons;
        if (ImGui.Checkbox($"{Loc.Get("ReplaceInAddons")}###addons", ref addons))
        {
            cfg.ReplaceInAddons = addons;
            changed = true;
        }

        var fly = cfg.ReplaceInFlyText;
        if (ImGui.Checkbox($"{Loc.Get("ReplaceInFlyText")}###flytext", ref fly))
        {
            cfg.ReplaceInFlyText = fly;
            changed = true;
        }

        var chat = cfg.ReplaceInChat;
        if (ImGui.Checkbox($"{Loc.Get("ReplaceInChat")}###chat", ref chat))
        {
            cfg.ReplaceInChat = chat;
            changed = true;
        }

        ImGui.Separator();
        ImGui.TextDisabled(Loc.Get("ClientLanguage", Loc.LanguageName(Plugin.ClientState.ClientLanguage)));
        ImGui.TextDisabled(Loc.Get("NamesLoaded", this.plugin.Translator.Count));
        if (cfg.TargetLanguage == Plugin.ClientState.ClientLanguage)
            ImGui.TextColored(new Vector4(1f, 0.7f, 0.3f, 1f), Loc.Get("SameLanguage"));

        if (needsRebuild)
        {
            this.plugin.RebuildTranslator();
            changed = true;
        }

        if (changed)
            this.plugin.SaveConfiguration();
    }
}
