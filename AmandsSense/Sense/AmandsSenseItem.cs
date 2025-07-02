using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
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
        if (observedLootItem.Item is Weapon weapon)
        {
            string spriteName = null;
            string itmTplId = null;

            switch (weapon.WeapClass)
            {
                case "assaultCarbine":
                    eSenseItemType = ESenseItemType.AssaultCarbines;
                    color = Plugin.AssaultCarbinesColor.Value;
                    spriteName = "icon_weapons_carbines.png";
                    itmTplId = "5b5f78e986f77447ed5636b1";
                    break;

                case "assaultRifle":
                    eSenseItemType = ESenseItemType.AssaultRifles;
                    color = Plugin.AssaultRiflesColor.Value;
                    spriteName = "icon_weapons_assaultrifles.png";
                    itmTplId = "5b5f78fc86f77409407a7f90";
                    break;

                case "sniperRifle":
                    eSenseItemType = ESenseItemType.BoltActionRifles;
                    color = Plugin.BoltActionRiflesColor.Value;
                    spriteName = "icon_weapons_botaction.png";
                    itmTplId = "5b5f798886f77447ed5636b5";
                    break;

                case "grenadeLauncher":
                    eSenseItemType = ESenseItemType.GrenadeLaunchers;
                    color = Plugin.GrenadeLaunchersColor.Value;
                    spriteName = "icon_weapons_gl.png";
                    itmTplId = "5b5f79d186f774093f2ed3c2";
                    break;

                case "machinegun":
                    eSenseItemType = ESenseItemType.MachineGuns;
                    color = Plugin.MachineGunsColor.Value;
                    spriteName = "icon_weapons_mg.png";
                    itmTplId = "5b5f79a486f77409407a7f94";
                    break;

                case "marksmanRifle":
                    eSenseItemType = ESenseItemType.MarksmanRifles;
                    color = Plugin.MarksmanRiflesColor.Value;
                    spriteName = "icon_weapons_dmr.png";
                    itmTplId = "5b5f791486f774093f2ed3be";
                    break;

                case "pistol":
                    eSenseItemType = ESenseItemType.Pistols;
                    color = Plugin.PistolsColor.Value;
                    spriteName = "icon_weapons_pistols.png";
                    itmTplId = "5b5f792486f77447ed5636b3";
                    break;

                case "smg":
                    eSenseItemType = ESenseItemType.SMGs;
                    color = Plugin.SMGsColor.Value;
                    spriteName = "icon_weapons_smg.png";
                    itmTplId = "5b5f796a86f774093f2ed3c0";
                    break;

                case "shotgun":
                    eSenseItemType = ESenseItemType.Shotguns;
                    color = Plugin.ShotgunsColor.Value;
                    spriteName = "icon_weapons_shotguns.png";
                    itmTplId = "5b5f794b86f77409407a7f92";
                    break;

                case "specialWeapon":
                    eSenseItemType = ESenseItemType.SpecialWeapons;
                    color = Plugin.SpecialWeaponsColor.Value;
                    spriteName = "icon_weapons_special.png";
                    itmTplId = "5b5f79eb86f77447ed5636b7";
                    break;

                default:
                    eSenseItemType = AmandsSenseClass.SenseItemType(observedLootItem.Item.GetType());
                    break;
            }

            if (spriteName != null && itmTplId != null)
            {
                type = AmandsSenseHelper.Localized(itmTplId, EStringCase.None);
                if (AmandsSenseClass.LoadedSprites.TryGetValue(spriteName, out var tmpSprite))
                    sprite = tmpSprite;
            }
        }
        else
            eSenseItemType = AmandsSenseClass.SenseItemType(observedLootItem.Item.GetType());

        {
            string spriteName = null;
            string itmTplId = null;

            // SenseItem Color, Sprite and Type
            switch (eSenseItemType)
            {
                case ESenseItemType.All:
                    color = Plugin.ObservedLootItemColor.Value;
                    spriteName = "ObservedLootItem.png";
                    itmTplId = "ObservedLootItem";
                    break;

                case ESenseItemType.Others:
                    color = Plugin.OthersColor.Value;
                    spriteName = "icon_barter_others.png";
                    itmTplId = "5b47574386f77428ca22b2f4";
                    break;

                case ESenseItemType.BuildingMaterials:
                    color = Plugin.BuildingMaterialsColor.Value;
                    spriteName = "icon_barter_building.png";
                    itmTplId = "5b47574386f77428ca22b2ee";
                    break;

                case ESenseItemType.Electronics:
                    color = Plugin.ElectronicsColor.Value;
                    spriteName = "icon_barter_electronics.png";
                    itmTplId = "5b47574386f77428ca22b2ef";
                    break;

                case ESenseItemType.EnergyElements:
                    color = Plugin.EnergyElementsColor.Value;
                    spriteName = "icon_barter_energy.png";
                    itmTplId = "5b47574386f77428ca22b2ed";
                    break;

                case ESenseItemType.FlammableMaterials:
                    color = Plugin.FlammableMaterialsColor.Value;
                    spriteName = "icon_barter_flammable.png";
                    itmTplId = "5b47574386f77428ca22b2f2";
                    break;

                case ESenseItemType.HouseholdMaterials:
                    color = Plugin.HouseholdMaterialsColor.Value;
                    spriteName = "icon_barter_household.png";
                    itmTplId = "5b47574386f77428ca22b2f0";
                    break;

                case ESenseItemType.MedicalSupplies:
                    color = Plugin.MedicalSuppliesColor.Value;
                    spriteName = "icon_barter_medical.png";
                    itmTplId = "5b47574386f77428ca22b2f3";
                    break;

                case ESenseItemType.Tools:
                    color = Plugin.ToolsColor.Value;
                    spriteName = "icon_barter_tools.png";
                    itmTplId = "5b47574386f77428ca22b2f6";
                    break;

                case ESenseItemType.Valuables:
                    color = Plugin.ValuablesColor.Value;
                    spriteName = "icon_barter_valuables.png";
                    itmTplId = "5b47574386f77428ca22b2f1";
                    break;

                case ESenseItemType.Backpacks:
                    color = Plugin.BackpacksColor.Value;
                    spriteName = "icon_gear_backpacks.png";
                    itmTplId = "5b5f6f6c86f774093f2ecf0b";
                    break;

                case ESenseItemType.BodyArmor:
                    color = Plugin.BodyArmorColor.Value;
                    spriteName = "icon_gear_armor.png";
                    itmTplId = "5b5f701386f774093f2ecf0f";
                    break;

                case ESenseItemType.Eyewear:
                    color = Plugin.EyewearColor.Value;
                    spriteName = "icon_gear_visors.png";
                    itmTplId = "5b47574386f77428ca22b331";
                    break;

                case ESenseItemType.Facecovers:
                    color = Plugin.FacecoversColor.Value;
                    spriteName = "icon_gear_facecovers.png";
                    itmTplId = "5b47574386f77428ca22b32f";
                    break;

                case ESenseItemType.GearComponents:
                    color = Plugin.GearComponentsColor.Value;
                    spriteName = "icon_gear_components.png";
                    itmTplId = "5b5f704686f77447ec5d76d7";
                    break;

                case ESenseItemType.Headgear:
                    color = Plugin.HeadgearColor.Value;
                    spriteName = "icon_gear_headwear.png";
                    itmTplId = "5b47574386f77428ca22b330";
                    break;

                case ESenseItemType.Headsets:
                    color = Plugin.HeadsetsColor.Value;
                    spriteName = "icon_gear_headsets.png";
                    itmTplId = "5b5f6f3c86f774094242ef87";
                    break;

                case ESenseItemType.SecureContainers:
                    color = Plugin.SecureContainersColor.Value;
                    spriteName = "icon_gear_secured.png";
                    itmTplId = "5b5f6fd286f774093f2ecf0d";
                    break;

                case ESenseItemType.StorageContainers:
                    color = Plugin.StorageContainersColor.Value;
                    spriteName = "icon_gear_cases.png";
                    itmTplId = "5b5f6fa186f77409407a7eb7";
                    break;

                case ESenseItemType.TacticalRigs:
                    color = Plugin.TacticalRigsColor.Value;
                    spriteName = "icon_gear_rigs.png";
                    itmTplId = "5b5f6f8786f77447ed563642";
                    break;

                case ESenseItemType.FunctionalMods:
                    color = Plugin.FunctionalModsColor.Value;
                    spriteName = "icon_mods_functional.png";
                    itmTplId = "5b5f71b386f774093f2ecf11";
                    break;

                case ESenseItemType.GearMods:
                    color = Plugin.GearModsColor.Value;
                    spriteName = "icon_mods_gear.png";
                    itmTplId = "5b5f750686f774093e6cb503";
                    break;

                case ESenseItemType.VitalParts:
                    color = Plugin.VitalPartsColor.Value;
                    spriteName = "icon_mods_vital.png";
                    itmTplId = "5b5f75b986f77447ec5d7710";
                    break;

                case ESenseItemType.MeleeWeapons:
                    color = Plugin.MeleeWeaponsColor.Value;
                    spriteName = "icon_weapons_melee.png";
                    itmTplId = "5b5f7a0886f77409407a7f96";
                    break;

                case ESenseItemType.Throwables:
                    color = Plugin.ThrowablesColor.Value;
                    spriteName = "icon_weapons_throw.png";
                    itmTplId = "5b5f7a2386f774093f2ed3c4";
                    break;

                case ESenseItemType.AmmoPacks:
                    color = Plugin.AmmoPacksColor.Value;
                    spriteName = "icon_ammo_boxes.png";
                    itmTplId = "5b47574386f77428ca22b33c";
                    break;

                case ESenseItemType.Rounds:
                    color = Plugin.RoundsColor.Value;
                    spriteName = "icon_ammo_rounds.png";
                    itmTplId = "5b47574386f77428ca22b33b";
                    break;

                case ESenseItemType.Drinks:
                    color = Plugin.DrinksColor.Value;
                    spriteName = "icon_provisions_drinks.png";
                    itmTplId = "5b47574386f77428ca22b335";
                    break;

                case ESenseItemType.Food:
                    color = Plugin.FoodColor.Value;
                    spriteName = "icon_provisions_food.png";
                    itmTplId = "5b47574386f77428ca22b336";
                    break;

                case ESenseItemType.Injectors:
                    color = Plugin.InjectorsColor.Value;
                    spriteName = "icon_medical_injectors.png";
                    itmTplId = "5b47574386f77428ca22b33a";
                    break;

                case ESenseItemType.InjuryTreatment:
                    color = Plugin.InjuryTreatmentColor.Value;
                    spriteName = "icon_medical_injury.png";
                    itmTplId = "5b47574386f77428ca22b339";
                    break;

                case ESenseItemType.Medkits:
                    color = Plugin.MedkitsColor.Value;
                    spriteName = "icon_medical_medkits.png";
                    itmTplId = "5b47574386f77428ca22b338";
                    break;

                case ESenseItemType.Pills:
                    color = Plugin.PillsColor.Value;
                    spriteName = "icon_medical_pills.png";
                    itmTplId = "5b47574386f77428ca22b337";
                    break;

                case ESenseItemType.ElectronicKeys:
                    color = Plugin.ElectronicKeysColor.Value;
                    spriteName = "icon_keys_electronic.png";
                    itmTplId = "5c518ed586f774119a772aee";
                    break;

                case ESenseItemType.MechanicalKeys:
                    color = Plugin.MechanicalKeysColor.Value;
                    spriteName = "icon_keys_mechanic.png";
                    itmTplId = "5c518ec986f7743b68682ce2";
                    break;

                case ESenseItemType.InfoItems:
                    color = Plugin.InfoItemsColor.Value;
                    spriteName = "icon_info.png";
                    itmTplId = "5b47574386f77428ca22b341";
                    break;

                case ESenseItemType.SpecialEquipment:
                    color = Plugin.SpecialEquipmentColor.Value;
                    spriteName = "icon_spec.png";
                    itmTplId = "5b47574386f77428ca22b345";
                    break;

                case ESenseItemType.Maps:
                    color = Plugin.MapsColor.Value;
                    spriteName = "icon_maps.png";
                    itmTplId = "5b47574386f77428ca22b343";
                    break;

                case ESenseItemType.Money:
                    color = Plugin.MoneyColor.Value;
                    spriteName = "icon_money.png";
                    itmTplId = "5b5f78b786f77447ed5636af";
                    break;
            }

            if (spriteName != null && itmTplId != null)
            {
                type = AmandsSenseHelper.Localized(itmTplId, EStringCase.None);
                if (AmandsSenseClass.LoadedSprites.TryGetValue(spriteName, out var tmpSprite))
                    sprite = tmpSprite;
            }
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

        if (AmandsSenseClass.inventoryControllerClass != null 
            && !AmandsSenseClass.inventoryControllerClass.Examined(observedLootItem.Item))
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
                string Name = "<b>" + AmandsSenseHelper.Localized(observedLootItem.Item.Name, 0) + "</b>";

                if (Name.Length > 16)
                    Name = "<b>" + AmandsSenseHelper.Localized(observedLootItem.Item.ShortName, 0) + "</b>";

                if (observedLootItem.Item.StackObjectsCount > 1)
                    Name = Name + " (" + observedLootItem.Item.StackObjectsCount + ")";

                nameText.fontSize = 1f;
                nameText.text = Name + "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + "<size=50%><voffset=0.5em> " + observedLootItem.Item.Weight + "kg";
                nameText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }

            // SenseItem Description
            if (descriptionText != null)
            {
                if (observedLootItem.Item.TryGetItemComponent(out FoodDrinkComponent foodDrinkComponent) && (int) foodDrinkComponent.MaxResource > 1)
                    descriptionText.text = (int) foodDrinkComponent.HpPercent + "/" + (int) foodDrinkComponent.MaxResource;

                if (observedLootItem.Item.TryGetItemComponent(out KeyComponent keyComponent) && keyComponent.Template.MaximumNumberOfUsage > 0)
                {
                    int maxUses = keyComponent.Template.MaximumNumberOfUsage;
                    descriptionText.text = maxUses - keyComponent.NumberOfUsages + "/" + maxUses;
                }

                if (observedLootItem.Item.TryGetItemComponent(out MedKitComponent medKitComponent) && medKitComponent.MaxHpResource > 1)
                    descriptionText.text = (int) medKitComponent.HpResource + "/" + medKitComponent.MaxHpResource;

                if (observedLootItem.Item.TryGetItemComponent(out RepairableComponent repairableComponent))
                    descriptionText.text = (int) repairableComponent.Durability + "/" + (int) repairableComponent.MaxDurability;

                if (observedLootItem.Item is MagazineItemClass magazineClass)
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
