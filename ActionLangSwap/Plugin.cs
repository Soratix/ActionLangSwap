using Dalamud.Game.Chat;
using Dalamud.Game.Command;
using Dalamud.Game.Gui.FlyText;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ActionLangSwap;

public sealed class Plugin : IDalamudPlugin
{
    private const string Command = "/actionlang";

    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IDataManager Data { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;
    [PluginService] internal static IFlyTextGui FlyTextGui { get; private set; } = null!;
    [PluginService] internal static IAddonLifecycle AddonLifecycle { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    public Configuration Configuration { get; }
    public NameTranslator Translator { get; } = new();

    private readonly WindowSystem windowSystem = new("ActionLangSwap");
    private readonly ConfigWindow configWindow;
    private readonly AddonTextReplacer addonReplacer;

    public Plugin()
    {
        this.Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        this.RebuildTranslator();

        this.configWindow = new ConfigWindow(this);
        this.windowSystem.AddWindow(this.configWindow);

        this.addonReplacer = new AddonTextReplacer(this);
        ChatGui.CheckMessageHandled += this.OnChatMessage;
        FlyTextGui.FlyTextCreated += this.OnFlyText;

        Loc.SetLanguage(PluginInterface.UiLanguage);
        PluginInterface.LanguageChanged += this.OnLanguageChanged;
        this.RegisterCommand();

        PluginInterface.UiBuilder.Draw += this.windowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi += this.configWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi += this.configWindow.Toggle;
    }

    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= this.windowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= this.configWindow.Toggle;
        PluginInterface.UiBuilder.OpenMainUi -= this.configWindow.Toggle;
        PluginInterface.LanguageChanged -= this.OnLanguageChanged;
        CommandManager.RemoveHandler(Command);
        FlyTextGui.FlyTextCreated -= this.OnFlyText;
        ChatGui.CheckMessageHandled -= this.OnChatMessage;
        this.addonReplacer.Dispose();
        this.windowSystem.RemoveAllWindows();
    }

    public void SaveConfiguration() => PluginInterface.SavePluginConfig(this.Configuration);

    private void RegisterCommand()
        => CommandManager.AddHandler(Command, new CommandInfo((_, _) => this.configWindow.Toggle())
        {
            HelpMessage = Loc.Get("CommandHelp"),
        });

    // La langue de l'interface Dalamud a changé : on met à jour les textes (y compris l'aide de la commande).
    private void OnLanguageChanged(string langCode)
    {
        Loc.SetLanguage(langCode);
        CommandManager.RemoveHandler(Command);
        this.RegisterCommand();
    }

    public void RebuildTranslator()
    {
        var source = ClientState.ClientLanguage;
        this.Translator.Rebuild(source, this.Configuration.TargetLanguage, this.Configuration.IncludeStatuses);
        Log.Info($"{this.Translator.Count} noms chargÃ©s ({source} -> {this.Configuration.TargetLanguage}).");
    }

    // Journal de combat : les canaux 41 Ã  55 (0x29-0x37) correspondent aux messages de combat.
    private static bool IsBattleLog(XivChatType type)
    {
        var channel = (int)type & 0x7F;
        return channel is >= 0x29 and <= 0x37;
    }

    private void OnChatMessage(IHandleableChatMessage chat)
    {
        if (!this.Configuration.Enabled || !this.Configuration.ReplaceInChat || !IsBattleLog(chat.LogKind))
            return;
        var message = chat.Message;
        if (this.Translator.ReplaceInSeString(message))
            chat.Message = message;
    }

    private void OnFlyText(
        ref FlyTextKind kind, ref int val1, ref int val2, ref SeString text1, ref SeString text2,
        ref uint color, ref uint icon, ref uint damageTypeIcon, ref float yOffset, ref bool handled)
    {
        if (!this.Configuration.Enabled || !this.Configuration.ReplaceInFlyText)
            return;
        this.Translator.ReplaceInSeString(text1);
        this.Translator.ReplaceInSeString(text2);
    }
}
