using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EFT;
using EFT.InventoryLogic;
using SPT.Common.Utils;
using UnityEngine;
using UnityEngine.Networking;
using static EFT.Player;

namespace AmandsSense.Sense;

public class AmandsSenseClass : MonoBehaviour
{
    public static Player Player;
    public static PlayerInventoryController inventoryControllerClass;

    public static RaycastHit hit;
    public static LayerMask LowLayerMask;
    public static LayerMask HighLayerMask;
    public static LayerMask FoliageLayerMask;

    public static float CooldownTime = 0f;
    public static float AlwaysOnTime = 0f;
    public static float Radius = 0f;

    public static PrismEffects prismEffects;
    public static ItemsJsonClass itemsJsonClass;

    public static float lastDoubleClickTime = 0.0f;
    public static Dictionary<string, Sprite> LoadedSprites = [];

    public static Vector3[] SenseOverlapLocations = [
        Vector3.zero, Vector3.forward, Vector3.back, Vector3.left, Vector3.right,
        Vector3.forward + Vector3.left, Vector3.forward + Vector3.right,
        Vector3.back + Vector3.left, Vector3.back + Vector3.right
    ];

    public static int CurrentOverlapLocation = 9;

    public static LayerMask BoxInteractiveLayerMask;
    public static LayerMask BoxDeadbodyLayerMask;
    public static int[] CurrentOverlapCount = new int[9];
    public static Collider[] CurrentOverlapLocationColliders = new Collider[100];

    public static Dictionary<int, AmandsSenseWorld> SenseWorlds = [];
    public static List<SenseDeadPlayerStruct> DeadPlayers = [];

    public static List<AmandsSenseExfil> SenseExfils = [];
    public static AmandsSenseExfil ClosestAmandsSenseExfil = null;

    public static List<Item> SenseItems = [];
    public static Transform parent;
    public static string scene;
    public static bool isFactory = false;

    public void Awake()
    {
        LowLayerMask = LayerMask.GetMask("Terrain", "LowPolyCollider", "HitCollider");
        HighLayerMask = LayerMask.GetMask("Terrain", "HighPolyCollider", "HitCollider");
        FoliageLayerMask = LayerMask.GetMask("Terrain", "HighPolyCollider", "HitCollider", "Foliage");

        BoxInteractiveLayerMask = LayerMask.GetMask("Interactive");
        BoxDeadbodyLayerMask = LayerMask.GetMask("Deadbody");

        itemsJsonClass = ReadFromJsonFile<ItemsJsonClass>(AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/Sense/Items.json");

        if (itemsJsonClass != null)
            itemsJsonClass.Validate();
        else
            itemsJsonClass = new();

        ReloadFiles();
    }


    public void Update()
    {
        if (Plugin.EnableSense.Value == EEnableSense.Off || gameObject == null || Player == null)
            return;

        if (CurrentOverlapLocation <= 8)
        {
            int CurrentOverlapCountTest = Physics.OverlapBoxNonAlloc(
                Player.Position + SenseOverlapLocations[CurrentOverlapLocation] * (Plugin.Radius.Value * 2f / 3f),
                Vector3.one * (Plugin.Radius.Value * 2f / 3f),
                CurrentOverlapLocationColliders,
                Quaternion.Euler(0f, 0f, 0f),
                BoxInteractiveLayerMask,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < CurrentOverlapCountTest; i++)
            {
                if (!SenseWorlds.TryGetValue(CurrentOverlapLocationColliders[i].GetInstanceID(), out var sw))
                {
                    GameObject SenseWorldGameObject = new("SenseWorld");
                    sw = SenseWorldGameObject.AddComponent<AmandsSenseWorld>();
                    sw.OwnerCollider = CurrentOverlapLocationColliders[i];
                    sw.OwnerGameObject = sw.OwnerCollider.gameObject;
                    sw.Id = sw.OwnerCollider.GetInstanceID();
                    sw.Delay = Math.Min(0, Vector3.Distance(Player.Position, sw.OwnerCollider.transform.position) / Plugin.Speed.Value);
                    SenseWorlds.Add(sw.Id, sw);
                }

                sw.RestartSense();
            }

            CurrentOverlapLocation++;
        }
        else if (Plugin.SenseAlwaysOn.Value)
        {
            AlwaysOnTime += Time.deltaTime;
            if (AlwaysOnTime > Plugin.AlwaysOnFrequency.Value)
            {
                AlwaysOnTime = 0f;
                CurrentOverlapLocation = 0;
                SenseDeadBodies();
            }
        }

        if (CooldownTime < Plugin.Cooldown.Value)
            CooldownTime += Time.deltaTime;

        if (Input.GetKeyDown(Plugin.SenseKey.Value.MainKey))
        {
            if (Plugin.DoubleClick.Value)
            {
                float timeSinceLastClick = Time.time - lastDoubleClickTime;
                lastDoubleClickTime = Time.time;

                if (timeSinceLastClick <= 0.5f && CooldownTime >= Plugin.Cooldown.Value)
                {
                    CooldownTime = 0f;
                    CurrentOverlapLocation = 0;
                    SenseDeadBodies();
                    ShowSenseExfils();

                    if (prismEffects != null)
                    {
                        Radius = 0;
                        prismEffects.useDof = Plugin.useDof.Value;
                    }
                }
            }
            else
            {
                if (CooldownTime >= Plugin.Cooldown.Value)
                {
                    CooldownTime = 0f;
                    CurrentOverlapLocation = 0;
                    SenseDeadBodies();
                    ShowSenseExfils();

                    if (prismEffects != null)
                    {
                        Radius = 0;
                        prismEffects.useDof = Plugin.useDof.Value;
                    }
                }
            }
        }

        if (Radius < Mathf.Max(Plugin.Radius.Value, Plugin.DeadPlayerRadius.Value))
        {
            Radius += Plugin.Speed.Value * Time.deltaTime;
            if (prismEffects != null)
            {
                prismEffects.dofFocusPoint = Radius - prismEffects.dofFocusDistance;
                if (prismEffects.dofRadius < 0.5f)
                    prismEffects.dofRadius += 2f * Time.deltaTime;
            }
        }
        else if (prismEffects != null && prismEffects.dofRadius > 0.001f)
        {
            prismEffects.dofRadius -= 0.5f * Time.deltaTime;
            if (prismEffects.dofRadius < 0.001f)
                prismEffects.useDof = false;
        }
    }

    public void SenseDeadBodies()
    {
        foreach (var dp in DeadPlayers)
        {
            if (Vector3.Distance(Player.Position, dp.victim.Position) >= Plugin.DeadPlayerRadius.Value)
                continue;

            if (!SenseWorlds.TryGetValue(dp.victim.GetInstanceID(), out var sw))
            {
                var SenseWorldGameObject = new GameObject("SenseWorld");
                sw = SenseWorldGameObject.AddComponent<AmandsSenseWorld>();
                sw.OwnerGameObject = dp.victim.gameObject;
                sw.Id = dp.victim.GetInstanceID();
                sw.Delay = Math.Min(0, Vector3.Distance(Player.Position, dp.victim.Position) / Plugin.Speed.Value);
                sw.Lazy = false;
                sw.eSenseWorldType = ESenseWorldType.Deadbody;
                sw.SenseDeadPlayer = dp.victim as LocalPlayer;
                SenseWorlds.Add(sw.Id, sw);
            }

            sw.RestartSense();
        }
    }

    public void ShowSenseExfils()
    {
        if (!Plugin.EnableExfilSense.Value)
            return;

        if (scene.Contains("Factory_") && scene.Contains("Laboratory_"))
            return;

        float ClosestDistance = 10_000_000_000f;
        if (ClosestAmandsSenseExfil != null && ClosestAmandsSenseExfil.light != null)
            ClosestAmandsSenseExfil.light.shadows = LightShadows.None;

        foreach (AmandsSenseExfil senseExfil in SenseExfils)
        {
            if (Player != null && Vector3.Distance(senseExfil.transform.position, Player.gameObject.transform.position) < ClosestDistance)
            {
                ClosestAmandsSenseExfil = senseExfil;
                ClosestDistance = Vector3.Distance(senseExfil.transform.position, Player.gameObject.transform.position);
            }

            if (senseExfil.Intensity > 0.5f)
            {
                senseExfil.LifeSpan = 0f;
                senseExfil.UpdateSense();
            }
            else
                senseExfil.ShowSense();
        }

        if (Plugin.ExfilLightShadows.Value && ClosestAmandsSenseExfil != null && ClosestAmandsSenseExfil.light != null)
            ClosestAmandsSenseExfil.light.shadows = LightShadows.Hard;
    }

    public static void Clear()
    {
        foreach (var kv in SenseWorlds)
        {
            if (kv.Value is null)
                continue;

            kv.Value.RemoveSense();
        }

        SenseWorlds.Clear();

        ClosestAmandsSenseExfil = null;
        SenseExfils = SenseExfils.Where(x => x != null).ToList();

        DeadPlayers.Clear();
    }

    public static ESenseItemType SenseItemType(Type itemType)
    {
        if (TemplateIdToObjectMappingsClass.TypeTable["57864ada245977548638de91"].IsAssignableFrom(itemType))
            return ESenseItemType.BuildingMaterials;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864a66245977548f04a81f"].IsAssignableFrom(itemType))
            return ESenseItemType.Electronics;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864ee62459775490116fc1"].IsAssignableFrom(itemType))
            return ESenseItemType.EnergyElements;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864e4c24597754843f8723"].IsAssignableFrom(itemType))
            return ESenseItemType.FlammableMaterials;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864c322459775490116fbf"].IsAssignableFrom(itemType))
            return ESenseItemType.HouseholdMaterials;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864c8c245977548867e7f1"].IsAssignableFrom(itemType))
            return ESenseItemType.MedicalSupplies;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864bb7245977548b3b66c2"].IsAssignableFrom(itemType))
            return ESenseItemType.Tools;
        if (TemplateIdToObjectMappingsClass.TypeTable["57864a3d24597754843f8721"].IsAssignableFrom(itemType))
            return ESenseItemType.Valuables;
        if (TemplateIdToObjectMappingsClass.TypeTable["590c745b86f7743cc433c5f2"].IsAssignableFrom(itemType))
            return ESenseItemType.Others;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448e53e4bdc2d60728b4567"].IsAssignableFrom(itemType))
            return ESenseItemType.Backpacks;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448e54d4bdc2dcc718b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.BodyArmor;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448e5724bdc2ddf718b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.Eyewear;
        if (TemplateIdToObjectMappingsClass.TypeTable["5a341c4686f77469e155819e"].IsAssignableFrom(itemType))
            return ESenseItemType.Facecovers;
        if (TemplateIdToObjectMappingsClass.TypeTable["5a341c4086f77401f2541505"].IsAssignableFrom(itemType))
            return ESenseItemType.Headgear;
        if (TemplateIdToObjectMappingsClass.TypeTable["57bef4c42459772e8d35a53b"].IsAssignableFrom(itemType))
            return ESenseItemType.GearComponents;
        if (TemplateIdToObjectMappingsClass.TypeTable["5b3f15d486f77432d0509248"].IsAssignableFrom(itemType))
            return ESenseItemType.GearComponents;
        if (TemplateIdToObjectMappingsClass.TypeTable["5645bcb74bdc2ded0b8b4578"].IsAssignableFrom(itemType))
            return ESenseItemType.Headsets;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448bf274bdc2dfc2f8b456a"].IsAssignableFrom(itemType))
            return ESenseItemType.SecureContainers;
        if (TemplateIdToObjectMappingsClass.TypeTable["5795f317245977243854e041"].IsAssignableFrom(itemType))
            return ESenseItemType.StorageContainers;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448e5284bdc2dcb718b4567"].IsAssignableFrom(itemType))
            return ESenseItemType.TacticalRigs;
        if (TemplateIdToObjectMappingsClass.TypeTable["550aa4154bdc2dd8348b456b"].IsAssignableFrom(itemType))
            return ESenseItemType.FunctionalMods;
        if (TemplateIdToObjectMappingsClass.TypeTable["55802f3e4bdc2de7118b4584"].IsAssignableFrom(itemType))
            return ESenseItemType.GearMods;
        if (TemplateIdToObjectMappingsClass.TypeTable["5a74651486f7744e73386dd1"].IsAssignableFrom(itemType))
            return ESenseItemType.GearMods;
        if (TemplateIdToObjectMappingsClass.TypeTable["55802f4a4bdc2ddb688b4569"].IsAssignableFrom(itemType))
            return ESenseItemType.VitalParts;
        if (TemplateIdToObjectMappingsClass.TypeTable["5447e1d04bdc2dff2f8b4567"].IsAssignableFrom(itemType))
            return ESenseItemType.MeleeWeapons;
        if (TemplateIdToObjectMappingsClass.TypeTable["543be6564bdc2df4348b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.Throwables;
        if (TemplateIdToObjectMappingsClass.TypeTable["543be5cb4bdc2deb348b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.AmmoPacks;
        if (TemplateIdToObjectMappingsClass.TypeTable["5485a8684bdc2da71d8b4567"].IsAssignableFrom(itemType))
            return ESenseItemType.Rounds;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448e8d64bdc2dce718b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.Drinks;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448e8d04bdc2ddf718b4569"].IsAssignableFrom(itemType))
            return ESenseItemType.Food;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448f3a64bdc2d60728b456a"].IsAssignableFrom(itemType))
            return ESenseItemType.Injectors;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448f3ac4bdc2dce718b4569"].IsAssignableFrom(itemType))
            return ESenseItemType.InjuryTreatment;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448f39d4bdc2d0a728b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.Medkits;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448f3a14bdc2d27728b4569"].IsAssignableFrom(itemType))
            return ESenseItemType.Pills;
        if (TemplateIdToObjectMappingsClass.TypeTable["5c164d2286f774194c5e69fa"].IsAssignableFrom(itemType))
            return ESenseItemType.ElectronicKeys;
        if (TemplateIdToObjectMappingsClass.TypeTable["5c99f98d86f7745c314214b3"].IsAssignableFrom(itemType))
            return ESenseItemType.MechanicalKeys;
        if (TemplateIdToObjectMappingsClass.TypeTable["5448ecbe4bdc2d60728b4568"].IsAssignableFrom(itemType))
            return ESenseItemType.InfoItems;
        if (TemplateIdToObjectMappingsClass.TypeTable["5447e0e74bdc2d3c308b4567"].IsAssignableFrom(itemType))
            return ESenseItemType.SpecialEquipment;
        if (TemplateIdToObjectMappingsClass.TypeTable["616eb7aea207f41933308f46"].IsAssignableFrom(itemType))
            return ESenseItemType.SpecialEquipment;
        if (TemplateIdToObjectMappingsClass.TypeTable["61605ddea09d851a0a0c1bbc"].IsAssignableFrom(itemType))
            return ESenseItemType.SpecialEquipment;
        if (TemplateIdToObjectMappingsClass.TypeTable["5f4fbaaca5573a5ac31db429"].IsAssignableFrom(itemType))
            return ESenseItemType.SpecialEquipment;
        if (TemplateIdToObjectMappingsClass.TypeTable["567849dd4bdc2d150f8b456e"].IsAssignableFrom(itemType))
            return ESenseItemType.Maps;
        if (TemplateIdToObjectMappingsClass.TypeTable["543be5dd4bdc2deb348b4569"].IsAssignableFrom(itemType))
            return ESenseItemType.Money;

        return ESenseItemType.All;
    }

    public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
    {
        TextWriter writer = null;
        try
        {
            var contentsToWriteToFile = Json.Serialize(objectToWrite);
            writer = new StreamWriter(filePath, append);
            writer.Write(contentsToWriteToFile);
        }
        finally
        {
            writer?.Close();
        }
    }

    public static T ReadFromJsonFile<T>(string filePath) where T : new()
    {
        TextReader reader = null;
        try
        {
            reader = new StreamReader(filePath);
            var fileContents = reader.ReadToEnd();
            return Json.Deserialize<T>(fileContents);
        }
        finally
        {
            reader?.Close();
        }
    }

    public static void ReloadFiles()
    {
        var baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx", "plugins", "Sense");
        var imageDir = Path.Combine(baseDir, "images");

        var imageFiles = Directory.GetFiles(imageDir.CreateDirectory(), "*.png");
        foreach (string File in imageFiles)
            LoadSprite(File);
    }

    async static void LoadSprite(string path)
    {
        LoadedSprites[Path.GetFileName(path)] = await RequestSprite(path);
    }

    async static Task<Sprite> RequestSprite(string path)
    {
        var www = UnityWebRequestTexture.GetTexture(path);
        var SendWeb = www.SendWebRequest();

        while (!SendWeb.isDone)
            await Task.Yield();

        if (www.result != UnityWebRequest.Result.Success)
            return null;

        var texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}
