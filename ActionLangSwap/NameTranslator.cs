using System.Text.RegularExpressions;
using Dalamud.Game;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Lumina.Excel.Sheets;
using Action = Lumina.Excel.Sheets.Action;

namespace ActionLangSwap;

/// <summary>
/// Table de correspondance "nom dans la langue du client" -> "nom dans la langue cible",
/// construite à partir des feuilles Excel du jeu (le client global contient les 4 langues).
/// </summary>
public sealed class NameTranslator
{
    private Dictionary<string, string> map = new(StringComparer.Ordinal);
    private Regex? regex;

    public int Count => this.map.Count;
    public int MaxLength { get; private set; }

    public void Rebuild(ClientLanguage source, ClientLanguage target, bool includeStatuses)
    {
        var newMap = new Dictionary<string, string>(StringComparer.Ordinal);

        if (source != target)
        {
            var srcActions = Plugin.Data.GetExcelSheet<Action>(source);
            var dstActions = Plugin.Data.GetExcelSheet<Action>(target);

            // 1er passage : actions de joueur (prioritaires en cas d'homonymes), 2e passage : tout le reste.
            foreach (var playerOnly in new[] { true, false })
            {
                foreach (var row in srcActions)
                {
                    if (playerOnly && !row.IsPlayerAction)
                        continue;
                    var dst = dstActions.GetRowOrDefault(row.RowId);
                    if (dst is null)
                        continue;
                    Add(newMap, row.Name.ExtractText(), dst.Value.Name.ExtractText());
                }
            }

            if (includeStatuses)
            {
                var srcStatus = Plugin.Data.GetExcelSheet<Status>(source);
                var dstStatus = Plugin.Data.GetExcelSheet<Status>(target);
                foreach (var row in srcStatus)
                {
                    var dst = dstStatus.GetRowOrDefault(row.RowId);
                    if (dst is null)
                        continue;
                    Add(newMap, row.Name.ExtractText(), dst.Value.Name.ExtractText());
                }
            }
        }

        this.map = newMap;
        this.MaxLength = newMap.Count == 0 ? 0 : newMap.Keys.Max(k => k.Length);
        this.regex = BuildRegex(newMap.Keys, source);
    }

    private static void Add(Dictionary<string, string> map, string src, string dst)
    {
        src = src.Trim();
        dst = dst.Trim();
        if (src.Length < 2 || dst.Length == 0 || src == dst)
            return;
        map.TryAdd(src, dst);
    }

    private static Regex? BuildRegex(IEnumerable<string> keys, ClientLanguage source)
    {
        // Les noms les plus longs d'abord pour que "Feu III" passe avant "Feu".
        var alternation = string.Join("|", keys.OrderByDescending(k => k.Length).Select(Regex.Escape));
        if (alternation.Length == 0)
            return null;

        // Pas de notion de mot en japonais : on ne met des bornes que pour les langues à espaces.
        var pattern = source == ClientLanguage.Japanese
            ? $"(?:{alternation})"
            : $@"(?<![\p{{L}}\p{{N}}])(?:{alternation})(?![\p{{L}}\p{{N}}])";
        return new Regex(pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    /// <summary>Correspondance exacte (texte complet d'un nœud d'interface).</summary>
    public bool TryTranslateExact(string text, out string translated)
        => this.map.TryGetValue(text.Trim(), out translated!);

    /// <summary>Remplace tous les noms trouvés dans un texte libre.</summary>
    public string ReplaceAll(string text)
        => this.regex is null ? text : this.regex.Replace(text, m => this.map[m.Value]);

    /// <summary>Remplace les noms dans les segments de texte d'un SeString. Renvoie true si modifié.</summary>
    public bool ReplaceInSeString(SeString str)
    {
        if (this.regex is null)
            return false;
        var changed = false;
        foreach (var payload in str.Payloads)
        {
            if (payload is not TextPayload { Text: { Length: > 0 } text } textPayload)
                continue;
            var replaced = this.ReplaceAll(text);
            if (!ReferenceEquals(replaced, text) && replaced != text)
            {
                textPayload.Text = replaced;
                changed = true;
            }
        }
        return changed;
    }
}
