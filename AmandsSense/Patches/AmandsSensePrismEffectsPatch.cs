using System.Reflection;
using AmandsSense.Sense;
using SPT.Reflection.Patching;

namespace AmandsSense.Patches;

public class AmandsSensePrismEffectsPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(PrismEffects).GetMethod(nameof(PrismEffects.OnEnable), BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPostfix]
    private static void PatchPostFix(ref PrismEffects __instance)
    {
        if (__instance.gameObject.name != "FPS Camera")
            return;

        AmandsSenseClass.prismEffects = __instance;
        __instance.debugDofPass = false;
        __instance.dofForceEnableMedian = false;
        __instance.dofBokehFactor = 157f;
        __instance.dofFocusDistance = 2f;
        __instance.dofNearFocusDistance = 100f;
        __instance.dofRadius = 0f;
    }
}
