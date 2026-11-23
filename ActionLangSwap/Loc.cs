using Dalamud.Game;

namespace ActionLangSwap;

/// <summary>
/// Textes de l'interface du plugin, dans la langue de l'interface de Dalamud.
/// Anglais par défaut si la langue n'est pas traduite.
/// </summary>
public static class Loc
{
    private const string Fallback = "en";

    private static readonly Dictionary<string, Dictionary<string, string>> Strings = new()
    {
        ["Enabled"] = new() { ["en"] = "Enabled", ["fr"] = "Activé", ["de"] = "Aktiviert", ["ja"] = "有効" },
        ["ActionLanguage"] = new()
        {
            ["en"] = "Action name language", ["fr"] = "Langue des actions",
            ["de"] = "Sprache der Aktionsnamen", ["ja"] = "アクション名の言語",
        },
        ["IncludeStatuses"] = new()
        {
            ["en"] = "Also translate statuses (buffs / debuffs)", ["fr"] = "Traduire aussi les statuts (buffs / debuffs)",
            ["de"] = "Auch Statuseffekte übersetzen (Buffs / Debuffs)", ["ja"] = "ステータス（バフ／デバフ）も翻訳する",
        },
        ["ReplaceIn"] = new() { ["en"] = "Replace in:", ["fr"] = "Où remplacer :", ["de"] = "Ersetzen in:", ["ja"] = "置き換え対象：" },
        ["ReplaceInAddons"] = new()
        {
            ["en"] = "Interface (cast bars, tooltips, actions menu)",
            ["fr"] = "Interface (incantations, infobulles, menu des actions)",
            ["de"] = "Oberfläche (Zauberleisten, Tooltips, Aktionsmenü)",
            ["ja"] = "UI（詠唱バー、ツールチップ、アクション一覧）",
        },
        ["ReplaceInFlyText"] = new()
        {
            ["en"] = "Flying text (damage / healing)", ["fr"] = "Textes volants (dégâts / soins)",
            ["de"] = "Schwebender Text (Schaden / Heilung)", ["ja"] = "フライテキスト（ダメージ／回復）",
        },
        ["ReplaceInChat"] = new() { ["en"] = "Battle log", ["fr"] = "Journal de combat", ["de"] = "Kampflog", ["ja"] = "バトルログ" },
        ["ClientLanguage"] = new()
        {
            ["en"] = "Client language: {0}", ["fr"] = "Langue du client : {0}",
            ["de"] = "Client-Sprache: {0}", ["ja"] = "クライアントの言語：{0}",
        },
        ["NamesLoaded"] = new()
        {
            ["en"] = "{0} names loaded", ["fr"] = "{0} noms chargés",
            ["de"] = "{0} Namen geladen", ["ja"] = "{0}件の名前を読み込みました",
        },
        ["SameLanguage"] = new()
        {
            ["en"] = "The selected language is already the client language.",
            ["fr"] = "La langue choisie est déjà celle du client.",
            ["de"] = "Die gewählte Sprache ist bereits die Client-Sprache.",
            ["ja"] = "選択した言語はすでにクライアントの言語です。",
        },
        ["CommandHelp"] = new()
        {
            ["en"] = "Opens the settings (action name language).",
            ["fr"] = "Ouvre la configuration (langue des noms d'actions).",
            ["de"] = "Öffnet die Einstellungen (Sprache der Aktionsnamen).",
            ["ja"] = "設定を開きます（アクション名の言語）。",
        },
        ["Lang.English"] = new() { ["en"] = "English", ["fr"] = "Anglais", ["de"] = "Englisch", ["ja"] = "英語" },
        ["Lang.French"] = new() { ["en"] = "French", ["fr"] = "Français", ["de"] = "Französisch", ["ja"] = "フランス語" },
        ["Lang.German"] = new() { ["en"] = "German", ["fr"] = "Allemand", ["de"] = "Deutsch", ["ja"] = "ドイツ語" },
        ["Lang.Japanese"] = new() { ["en"] = "Japanese", ["fr"] = "Japonais", ["de"] = "Japanisch", ["ja"] = "日本語" },
    };

    /// <summary>Code ISO à deux lettres de la langue utilisée (en, fr, de, ja...).</summary>
    public static string Language { get; private set; } = Fallback;

    public static void SetLanguage(string? langCode)
        => Language = string.IsNullOrWhiteSpace(langCode) ? Fallback : langCode.Trim().ToLowerInvariant();

    public static string Get(string key)
    {
        if (!Strings.TryGetValue(key, out var translations))
            return key;
        return translations.TryGetValue(Language, out var text) ? text : translations[Fallback];
    }

    public static string Get(string key, params object[] args) => string.Format(Get(key), args);

    public static string LanguageName(ClientLanguage language) => Get($"Lang.{language}");
}
