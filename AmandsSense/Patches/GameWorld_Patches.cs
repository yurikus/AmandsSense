using System.Reflection;
using AmandsSense.Sense;
using EFT;
using SPT.Reflection.Patching;
using UnityEngine.SceneManagement;
using static EFT.Player;

namespace AmandsSense.Patches;

public class GameWorldAwakePrefixPatch : ModulePatch
{
    public static bool IsHideout;

    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.Awake));
    }

    [PatchPrefix]
    public static void Prefix(GameWorld __instance)
    {
        IsHideout = __instance is HideoutGameWorld;
        Plugin.Log.LogInfo($"Game world hideout flag: {IsHideout}");

        if (IsHideout)
            return;

        //Singleton<LitMaterialRegistry>.Create(new LitMaterialRegistry());
        //Singleton<PlayerDamageRegistry>.Create(new PlayerDamageRegistry());
    }
}

public class GameWorldStartedPostfixPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.OnGameStarted));
    }

    [PatchPostfix]
    public static void Postfix(GameWorld __instance)
    {
        if (GameWorldAwakePrefixPatch.IsHideout)
            return;

        Plugin.Log.LogInfo($"Found local player: {__instance.MainPlayer.ProfileId}");

        AmandsSenseClass.Player = __instance.MainPlayer;
        AmandsSenseClass.inventoryControllerClass = (PlayerInventoryController) AmandsSenseClass.Player.InventoryController;
        AmandsSenseClass.Clear();
        AmandsSenseClass.scene = SceneManager.GetActiveScene().name;
        AmandsSenseClass.ReloadFiles();

        if (__instance.LocationId.Contains("factory"))
        {
            Plugin.Log.LogInfo("Factory location detected");
        }
        else
        {
            __instance.gameObject.AddComponent<AmandsSenseClass>();
        }

        //if (MegamodPlugin.LodOverrideEnabled.Value)
        //    QualitySettings.lodBias = MegamodPlugin.LodBias.Value;
    }
}

public class GameWorldDisposePostfixPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(GameWorld).GetMethod(nameof(GameWorld.Dispose));
    }

    [PatchPostfix]
    public static void Postfix()
    {
        Plugin.Log.LogInfo("Disposing of static & long lived objects.");

        AmandsSenseClass.inventoryControllerClass = null;
        AmandsSenseClass.Player = null;

        //Singleton<DecalPainter>.Release(Singleton<DecalPainter>.Instance);
        //Singleton<ImpactController>.Release(Singleton<ImpactController>.Instance);
        //Singleton<EmissionController>.Release(Singleton<EmissionController>.Instance);
        //Singleton<BloodEffects>.Release(Singleton<BloodEffects>.Instance);
        //Singleton<PlayerDamageRegistry>.Release(Singleton<PlayerDamageRegistry>.Instance);
        //Singleton<LitMaterialRegistry>.Release(Singleton<LitMaterialRegistry>.Instance);
        //
        //Singleton<LocalPlayerMuzzleEffects>.Release(Singleton<LocalPlayerMuzzleEffects>.Instance);
        //Singleton<MuzzleEffects>.Release(Singleton<MuzzleEffects>.Instance);
        //Singleton<FirearmsEffectsCache>.Release(Singleton<FirearmsEffectsCache>.Instance);
        //Singleton<MuzzleStatic>.Release(Singleton<MuzzleStatic>.Instance);
        //
        //ImpactStatic.Kinetics = new ImpactKinetics();
        //ImpactStatic.LocalPlayer = null;
        //ImpactStatic.PlayerHitInfo = null;

        Plugin.Log.LogInfo("Disposing complete.");
    }
}