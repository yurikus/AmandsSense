using System;
using System.Linq;
using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using UnityEngine;

namespace AmandsSense.Sense;

public class AmandsSenseItem : AmandsSenseConstructor
{
    public ObservedLootItem observedLootItem;
    public string ItemId;
    public string type;

    public ESenseItemType eSenseItemType = ESenseItemType.All;

    public override void SetSense(ObservedLootItem ObservedLootItem)
    {
        Console.WriteLine("sense: AmandsSenseItem");

        eSenseItemType = ESenseItemType.All;
        color = Plugin.ObservedLootItemColor.Value;

        observedLootItem = ObservedLootItem;
        if (observedLootItem == null || !observedLootItem.gameObject.activeSelf || observedLootItem.Item == null)
        {
            if (amandsSenseWorld != null)
                amandsSenseWorld.CancelSense();

            return;
        }

        AmandsSenseClass.SenseItems.Add(observedLootItem.Item);

        ItemId = observedLootItem.ItemId;

        // Weapon SenseItem Color, Sprite and Type
        Weapon weapon = observedLootItem.Item as Weapon;
        if (weapon != null)
        {
            switch (weapon.WeapClass)
            {
                case "assaultCarbine":
                    eSenseItemType = ESenseItemType.AssaultCarbines;
                    color = Plugin.AssaultCarbinesColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_carbines.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_carbines.png"];
                    type = AmandsSenseHelper.Localized("5b5f78e986f77447ed5636b1", EStringCase.None);
                    break;
                case "assaultRifle":
                    eSenseItemType = ESenseItemType.AssaultRifles;
                    color = Plugin.AssaultRiflesColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_assaultrifles.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_assaultrifles.png"];
                    type = AmandsSenseHelper.Localized("5b5f78fc86f77409407a7f90", EStringCase.None);
                    break;
                case "sniperRifle":
                    eSenseItemType = ESenseItemType.BoltActionRifles;
                    color = Plugin.BoltActionRiflesColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_botaction.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_botaction.png"];
                    type = AmandsSenseHelper.Localized("5b5f798886f77447ed5636b5", EStringCase.None);
                    break;
                case "grenadeLauncher":
                    eSenseItemType = ESenseItemType.GrenadeLaunchers;
                    color = Plugin.GrenadeLaunchersColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_gl.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_gl.png"];
                    type = AmandsSenseHelper.Localized("5b5f79d186f774093f2ed3c2", EStringCase.None);
                    break;
                case "machinegun":
                    eSenseItemType = ESenseItemType.MachineGuns;
                    color = Plugin.MachineGunsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_mg.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_mg.png"];
                    type = AmandsSenseHelper.Localized("5b5f79a486f77409407a7f94", EStringCase.None);
                    break;
                case "marksmanRifle":
                    eSenseItemType = ESenseItemType.MarksmanRifles;
                    color = Plugin.MarksmanRiflesColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_dmr.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_dmr.png"];
                    type = AmandsSenseHelper.Localized("5b5f791486f774093f2ed3be", EStringCase.None);
                    break;
                case "pistol":
                    eSenseItemType = ESenseItemType.Pistols;
                    color = Plugin.PistolsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_pistols.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_pistols.png"];
                    type = AmandsSenseHelper.Localized("5b5f792486f77447ed5636b3", EStringCase.None);
                    break;
                case "smg":
                    eSenseItemType = ESenseItemType.SMGs;
                    color = Plugin.SMGsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_smg.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_smg.png"];
                    type = AmandsSenseHelper.Localized("5b5f796a86f774093f2ed3c0", EStringCase.None);
                    break;
                case "shotgun":
                    eSenseItemType = ESenseItemType.Shotguns;
                    color = Plugin.ShotgunsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_shotguns.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_shotguns.png"];
                    type = AmandsSenseHelper.Localized("5b5f794b86f77409407a7f92", EStringCase.None);
                    break;
                case "specialWeapon":
                    eSenseItemType = ESenseItemType.SpecialWeapons;
                    color = Plugin.SpecialWeaponsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_special.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_weapons_special.png"];
                    type = AmandsSenseHelper.Localized("5b5f79eb86f77447ed5636b7", EStringCase.None);
                    break;
                default:
                    eSenseItemType = AmandsSenseClass.SenseItemType(observedLootItem.Item.GetType());
                    break;
            }
        }
        else
            eSenseItemType = AmandsSenseClass.SenseItemType(observedLootItem.Item.GetType());

        // SenseItem Color, Sprite and Type
        switch (eSenseItemType)
        {
            case ESenseItemType.All:
                color = Plugin.ObservedLootItemColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("ObservedLootItem.png"))
                    sprite = AmandsSenseClass.LoadedSprites["ObservedLootItem.png"];
                type = "ObservedLootItem";
                break;
            case ESenseItemType.Others:
                color = Plugin.OthersColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_others.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_others.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2f4", EStringCase.None);
                break;
            case ESenseItemType.BuildingMaterials:
                color = Plugin.BuildingMaterialsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_building.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_building.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2ee", EStringCase.None);
                break;
            case ESenseItemType.Electronics:
                color = Plugin.ElectronicsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_electronics.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_electronics.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2ef", EStringCase.None);
                break;
            case ESenseItemType.EnergyElements:
                color = Plugin.EnergyElementsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_energy.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_energy.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2ed", EStringCase.None);
                break;
            case ESenseItemType.FlammableMaterials:
                color = Plugin.FlammableMaterialsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_flammable.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_flammable.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2f2", EStringCase.None);
                break;
            case ESenseItemType.HouseholdMaterials:
                color = Plugin.HouseholdMaterialsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_household.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_household.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2f0", EStringCase.None);
                break;
            case ESenseItemType.MedicalSupplies:
                color = Plugin.MedicalSuppliesColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_medical.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_medical.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2f3", EStringCase.None);
                break;
            case ESenseItemType.Tools:
                color = Plugin.ToolsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_tools.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_tools.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2f6", EStringCase.None);
                break;
            case ESenseItemType.Valuables:
                color = Plugin.ValuablesColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter_valuables.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_barter_valuables.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b2f1", EStringCase.None);
                break;
            case ESenseItemType.Backpacks:
                color = Plugin.BackpacksColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_backpacks.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_backpacks.png"];
                type = AmandsSenseHelper.Localized("5b5f6f6c86f774093f2ecf0b", EStringCase.None);
                break;
            case ESenseItemType.BodyArmor:
                color = Plugin.BodyArmorColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_armor.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_armor.png"];
                type = AmandsSenseHelper.Localized("5b5f701386f774093f2ecf0f", EStringCase.None);
                break;
            case ESenseItemType.Eyewear:
                color = Plugin.EyewearColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_visors.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_visors.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b331", EStringCase.None);
                break;
            case ESenseItemType.Facecovers:
                color = Plugin.FacecoversColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_facecovers.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_facecovers.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b32f", EStringCase.None);
                break;
            case ESenseItemType.GearComponents:
                color = Plugin.GearComponentsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_components.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_components.png"];
                type = AmandsSenseHelper.Localized("5b5f704686f77447ec5d76d7", EStringCase.None);
                break;
            case ESenseItemType.Headgear:
                color = Plugin.HeadgearColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_headwear.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_headwear.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b330", EStringCase.None);
                break;
            case ESenseItemType.Headsets:
                color = Plugin.HeadsetsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_headsets.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_headsets.png"];
                type = AmandsSenseHelper.Localized("5b5f6f3c86f774094242ef87", EStringCase.None);
                break;
            case ESenseItemType.SecureContainers:
                color = Plugin.SecureContainersColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_secured.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_secured.png"];
                type = AmandsSenseHelper.Localized("5b5f6fd286f774093f2ecf0d", EStringCase.None);
                break;
            case ESenseItemType.StorageContainers:
                color = Plugin.StorageContainersColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_cases.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_cases.png"];
                type = AmandsSenseHelper.Localized("5b5f6fa186f77409407a7eb7", EStringCase.None);
                break;
            case ESenseItemType.TacticalRigs:
                color = Plugin.TacticalRigsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_gear_rigs.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_gear_rigs.png"];
                type = AmandsSenseHelper.Localized("5b5f6f8786f77447ed563642", EStringCase.None);
                break;
            case ESenseItemType.FunctionalMods:
                color = Plugin.FunctionalModsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_mods_functional.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_mods_functional.png"];
                type = AmandsSenseHelper.Localized("5b5f71b386f774093f2ecf11", EStringCase.None);
                break;
            case ESenseItemType.GearMods:
                color = Plugin.GearModsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_mods_gear.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_mods_gear.png"];
                type = AmandsSenseHelper.Localized("5b5f750686f774093e6cb503", EStringCase.None);
                break;
            case ESenseItemType.VitalParts:
                color = Plugin.VitalPartsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_mods_vital.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_mods_vital.png"];
                type = AmandsSenseHelper.Localized("5b5f75b986f77447ec5d7710", EStringCase.None);
                break;
            case ESenseItemType.MeleeWeapons:
                color = Plugin.MeleeWeaponsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_melee.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_weapons_melee.png"];
                type = AmandsSenseHelper.Localized("5b5f7a0886f77409407a7f96", EStringCase.None);
                break;
            case ESenseItemType.Throwables:
                color = Plugin.ThrowablesColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_weapons_throw.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_weapons_throw.png"];
                type = AmandsSenseHelper.Localized("5b5f7a2386f774093f2ed3c4", EStringCase.None);
                break;
            case ESenseItemType.AmmoPacks:
                color = Plugin.AmmoPacksColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_ammo_boxes.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_ammo_boxes.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b33c", EStringCase.None);
                break;
            case ESenseItemType.Rounds:
                color = Plugin.RoundsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_ammo_rounds.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_ammo_rounds.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b33b", EStringCase.None);
                break;
            case ESenseItemType.Drinks:
                color = Plugin.DrinksColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_provisions_drinks.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_provisions_drinks.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b335", EStringCase.None);
                break;
            case ESenseItemType.Food:
                color = Plugin.FoodColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_provisions_food.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_provisions_food.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b336", EStringCase.None);
                break;
            case ESenseItemType.Injectors:
                color = Plugin.InjectorsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_medical_injectors.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_medical_injectors.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b33a", EStringCase.None);
                break;
            case ESenseItemType.InjuryTreatment:
                color = Plugin.InjuryTreatmentColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_medical_injury.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_medical_injury.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b339", EStringCase.None);
                break;
            case ESenseItemType.Medkits:
                color = Plugin.MedkitsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_medical_medkits.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_medical_medkits.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b338", EStringCase.None);
                break;
            case ESenseItemType.Pills:
                color = Plugin.PillsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_medical_pills.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_medical_pills.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b337", EStringCase.None);
                break;
            case ESenseItemType.ElectronicKeys:
                color = Plugin.ElectronicKeysColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_keys_electronic.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_keys_electronic.png"];
                type = AmandsSenseHelper.Localized("5c518ed586f774119a772aee", EStringCase.None);
                break;
            case ESenseItemType.MechanicalKeys:
                color = Plugin.MechanicalKeysColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_keys_mechanic.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_keys_mechanic.png"];
                type = AmandsSenseHelper.Localized("5c518ec986f7743b68682ce2", EStringCase.None);
                break;
            case ESenseItemType.InfoItems:
                color = Plugin.InfoItemsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_info.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_info.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b341", EStringCase.None);
                break;
            case ESenseItemType.SpecialEquipment:
                color = Plugin.SpecialEquipmentColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_spec.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_spec.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b345", EStringCase.None);
                break;
            case ESenseItemType.Maps:
                color = Plugin.MapsColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_maps.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_maps.png"];
                type = AmandsSenseHelper.Localized("5b47574386f77428ca22b343", EStringCase.None);
                break;
            case ESenseItemType.Money:
                color = Plugin.MoneyColor.Value;
                if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_money.png"))
                    sprite = AmandsSenseClass.LoadedSprites["icon_money.png"];
                type = AmandsSenseHelper.Localized("5b5f78b786f77447ed5636af", EStringCase.None);
                break;
        }

        // Quest SenseItem Color
        if (observedLootItem.Item.QuestItem)
            color = Plugin.QuestItemsColor.Value;

        // JSON SenseItem Color
        if (AmandsSenseClass.itemsJsonClass != null)
        {
            if (AmandsSenseClass.itemsJsonClass.KappaItems != null)
                if (AmandsSenseClass.itemsJsonClass.KappaItems.Contains(observedLootItem.Item.TemplateId))
                    color = Plugin.KappaItemsColor.Value;

            if (Plugin.EnableFlea.Value && !observedLootItem.Item.CanSellOnRagfair && !AmandsSenseClass.itemsJsonClass.NonFleaExclude.Contains(observedLootItem.Item.TemplateId))
                color = Plugin.NonFleaItemsColor.Value;

            if (AmandsSenseClass.Player != null
                && AmandsSenseClass.Player.Profile != null
                && AmandsSenseClass.Player.Profile.WishlistManager != null)
            {
                var wm = AmandsSenseClass.Player.Profile.WishlistManager;
                if (wm.IsInWishlist(observedLootItem.Item.TemplateId, includeQol: true, out var wmGroup))
                {
                    color = Plugin.WishListItemsColor.Value;
                }
            }

            if (AmandsSenseClass.itemsJsonClass.RareItems != null)
                if (AmandsSenseClass.itemsJsonClass.RareItems.Contains(observedLootItem.Item.TemplateId))
                    color = Plugin.RareItemsColor.Value;
        }

        if (Plugin.UseBackgroundColor.Value)
            color = AmandsSenseHelper.ToColor(observedLootItem.Item.BackgroundColor);

        // SenseItem Sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
        }

        // SenseItem Light
        if (light != null)
        {
            light.color = new Color(color.r, color.g, color.b, 1f);
            light.intensity = 0f;
            light.range = Plugin.LightRange.Value;
        }

        // SenseItem Type
        if (typeText != null)
        {
            typeText.fontSize = 0.5f;
            typeText.text = type;
            typeText.color = new Color(color.r, color.g, color.b, 0f);
        }

        if (AmandsSenseClass.inventoryControllerClass != null && !AmandsSenseClass.inventoryControllerClass.Examined(observedLootItem.Item))
        {
            // SenseItem Unexamined Name
            if (nameText != null)
            {
                nameText.fontSize = 1f;
                nameText.text = "<b>???</b>";
                nameText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }
            // SenseItem Unexamined Description
            if (descriptionText != null)
            {
                descriptionText.text = "";
                descriptionText.fontSize = 0.75f;
                descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }
        }
        else
        {
            // SenseItem Name
            if (nameText != null)
            {
                nameText.fontSize = 1f;
                string Name = "<b>" + AmandsSenseHelper.Localized(observedLootItem.Item.Name, 0) + "</b>";
                if (Name.Count() > 16)
                    Name = "<b>" + AmandsSenseHelper.Localized(observedLootItem.Item.ShortName, 0) + "</b>";
                if (observedLootItem.Item.StackObjectsCount > 1)
                    Name = Name + " (" + observedLootItem.Item.StackObjectsCount + ")";
                nameText.text = Name + "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + "<size=50%><voffset=0.5em> " + observedLootItem.Item.Weight + "kg";
                nameText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }

            // SenseItem Description
            if (descriptionText != null)
            {
                FoodDrinkComponent foodDrinkComponent;
                if (observedLootItem.Item.TryGetItemComponent(out foodDrinkComponent) && (int) foodDrinkComponent.MaxResource > 1)
                    descriptionText.text = (int) foodDrinkComponent.HpPercent + "/" + (int) foodDrinkComponent.MaxResource;
                KeyComponent keyComponent;
                if (observedLootItem.Item.TryGetItemComponent(out keyComponent))
                {
                    int MaximumNumberOfUsage = Traverse.Create(Traverse.Create(keyComponent).Field("Template").GetValue<object>()).Field("MaximumNumberOfUsage").GetValue<int>();
                    descriptionText.text = MaximumNumberOfUsage - keyComponent.NumberOfUsages + "/" + MaximumNumberOfUsage;
                }
                MedKitComponent medKitComponent;
                if (observedLootItem.Item.TryGetItemComponent(out medKitComponent) && medKitComponent.MaxHpResource > 1)
                    descriptionText.text = (int) medKitComponent.HpResource + "/" + medKitComponent.MaxHpResource;
                RepairableComponent repairableComponent;
                if (observedLootItem.Item.TryGetItemComponent(out repairableComponent))
                    descriptionText.text = (int) repairableComponent.Durability + "/" + (int) repairableComponent.MaxDurability;
                MagazineItemClass magazineClass = observedLootItem.Item as MagazineItemClass;
                if (magazineClass != null)
                    descriptionText.text = magazineClass.Count + "/" + magazineClass.MaxCount;
                descriptionText.fontSize = 0.75f;
                descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }
        }
    }

    public override void UpdateIntensity(float Intensity)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(color.r, color.g, color.b, color.a * Intensity);
        if (light != null)
            light.intensity = Plugin.LightIntensity.Value * Intensity;
        if (typeText != null)
            typeText.color = new Color(color.r, color.g, color.b, Intensity);
        if (nameText != null)
            nameText.color = new Color(textColor.r, textColor.g, textColor.b, Intensity);
        if (descriptionText != null)
            descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, Intensity);
    }

    public override void RemoveSense()
    {
        if (observedLootItem != null && observedLootItem.gameObject.activeSelf && observedLootItem.Item != null)
            AmandsSenseClass.SenseItems.Remove(observedLootItem.Item);

        //Destroy(gameObject);
    }
}
