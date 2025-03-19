using System.Reflection;
using AmandsSense.Sense;
using EFT;
using SPT.Reflection.Patching;

namespace AmandsSense.Patches;

public class AmandsKillPatch : ModulePatch
{
    protected override MethodBase GetTargetMethod()
    {
        return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.Instance | BindingFlags.Public);
    }

    [PatchPostfix]
    private static void PatchPostFix(ref Player __instance, Player aggressor, DamageInfoStruct damageInfo, EBodyPart bodyPart, EDamageType lethalDamageType)
    {
        AmandsSenseClass.DeadPlayers.Add(new SenseDeadPlayerStruct(__instance, aggressor));
    }
}
