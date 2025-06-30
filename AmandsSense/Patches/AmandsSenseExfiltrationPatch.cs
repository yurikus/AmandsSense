using System.Reflection;
using AmandsSense.Sense;
using EFT.Interactive;
using SPT.Reflection.Patching;
using UnityEngine;

namespace AmandsSense.Patches;

public class AmandsSenseExfiltrationPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(ExfiltrationPoint).GetMethod(nameof(ExfiltrationPoint.Awake), BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPostfix]
    private static void PatchPostFix(ref ExfiltrationPoint __instance)
    {
        var amandsSenseExfiltrationGameObject = new GameObject("SenseExfil");
        AmandsSenseExfil amandsSenseExfil = amandsSenseExfiltrationGameObject.AddComponent<AmandsSenseExfil>();
        amandsSenseExfil.SetSense(__instance);
        amandsSenseExfil.Construct();
        amandsSenseExfil.ShowSense();
        AmandsSenseClass.SenseExfils.Add(amandsSenseExfil);
    }
}
