using EFT;
using JsonType;
using UnityEngine;

namespace AmandsSense;

public class AmandsSenseHelper
{
    public static string Localized(string id, EStringCase @case)
        => GClass2175.Localized(id, @case);

    public static bool IsBoss(WildSpawnType role)
        => role.IsBoss();

    public static bool IsFollower(WildSpawnType role)
        => role.IsFollower();

    public static bool CountAsBossForStatistics(WildSpawnType role)
        => role.CountAsBossForStatistics();

    public static string GetScavRoleKey(WildSpawnType role)
        => role.GetScavRoleKey();

    public static string Transliterate(string text)
        => GClass950.Transliterate(text);

    public static Color ToColor(TaxonomyColor taxonomyColor)
        => GClass1403.ToColor(taxonomyColor);
}
