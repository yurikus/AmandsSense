using System.Reflection;
using AmandsSense.Sense;
using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine.SceneManagement;
using static EFT.Player;

namespace AmandsSense.Patches;

public class AmandsPlayerPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player).GetMethod(nameof(Player.Init), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    [PatchPostfix]
    private static void PatchPostFix(ref Player __instance)
    {
        if (__instance == null || !__instance.IsYourPlayer)
            return;

        AmandsSenseClass.Player = __instance;
        AmandsSenseClass.inventoryControllerClass = Traverse.Create(__instance).Field("_inventoryController").GetValue<PlayerInventoryController>();
        AmandsSenseClass.Clear();
        AmandsSenseClass.scene = SceneManager.GetActiveScene().name;
        AmandsSenseClass.ReloadFiles();
    }
}
