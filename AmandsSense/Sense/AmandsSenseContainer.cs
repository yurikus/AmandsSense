using UnityEngine;
using System.Collections.Generic;
using HarmonyLib;
using EFT;
using EFT.InventoryLogic;
using Comfort.Common;
using EFT.Interactive;

namespace AmandsSense.Sense;

public class AmandsSenseContainer : AmandsSenseConstructor
{
    public LootableContainer lootableContainer;
    public bool emptyLootableContainer = false;
    public int itemCount = 0;
    public string ContainerId;
    public bool Drawer;

    public override void SetSense(LootableContainer LootableContainer)
    {
        lootableContainer = LootableContainer;
        if (lootableContainer != null && lootableContainer.gameObject.activeSelf)
        {
            Drawer = amandsSenseWorld.eSenseWorldType == ESenseWorldType.Drawer;
            // SenseContainer Defaults
            emptyLootableContainer = false;
            itemCount = 0;

            ContainerId = lootableContainer.Id;

            // SenseContainer Items
            ESenseItemColor eSenseItemColor = ESenseItemColor.Default;
            if (lootableContainer.ItemOwner != null && AmandsSenseClass.Player.Profile != null)// && AmandsSenseClass.Player.Profile.WishList != null)
            {
                CompoundItem lootItemClass = lootableContainer.ItemOwner.RootItem as CompoundItem;
                if (lootItemClass != null)
                {
                    object[] Grids = Traverse.Create(lootItemClass).Field("Grids").GetValue<object[]>();
                    if (Grids != null)
                        foreach (object grid in Grids)
                        {
                            IEnumerable<Item> Items = Traverse.Create(grid).Property("Items").GetValue<IEnumerable<Item>>();
                            if (Items != null)
                                foreach (Item item in Items)
                                {
                                    itemCount += 1;
                                    if (AmandsSenseClass.itemsJsonClass.RareItems.Contains(item.TemplateId))
                                        eSenseItemColor = ESenseItemColor.Rare;
                                    /*else if (AmandsSenseClass.Player.Profile.WishList.Contains(item.TemplateId) && eSenseItemColor != ESenseItemColor.Rare)
                                    {
                                        eSenseItemColor = ESenseItemColor.WishList;
                                    }*/
                                    else if (item.Template != null && !item.Template.CanSellOnRagfair && !AmandsSenseClass.itemsJsonClass.NonFleaExclude.Contains(item.TemplateId) && eSenseItemColor != ESenseItemColor.Rare && eSenseItemColor != ESenseItemColor.WishList)
                                        if (!AmandsSensePlugin.FleaIncludeAmmo.Value && TemplateIdToObjectMappingsClass.TypeTable["5485a8684bdc2da71d8b4567"].IsAssignableFrom(item.GetType()))
                                            continue;
                                        else if (AmandsSensePlugin.EnableFlea.Value)
                                            eSenseItemColor = ESenseItemColor.NonFlea;
                                        else if (AmandsSenseClass.itemsJsonClass.KappaItems.Contains(item.TemplateId) && eSenseItemColor == ESenseItemColor.Default)
                                            eSenseItemColor = ESenseItemColor.Kappa;
                                }
                        }
                }
            }
            if (itemCount == 0)
            {
                amandsSenseWorld.CancelSense();
                return;
            }

            // SenseContainer Color and Sprite
            if (AmandsSenseClass.LoadedSprites.ContainsKey("LootableContainer.png"))
                sprite = AmandsSenseClass.LoadedSprites["LootableContainer.png"];
            switch (eSenseItemColor)
            {
                case ESenseItemColor.Default:
                    color = AmandsSensePlugin.ObservedLootItemColor.Value;
                    break;
                case ESenseItemColor.Kappa:
                    color = AmandsSensePlugin.KappaItemsColor.Value;
                    break;
                case ESenseItemColor.NonFlea:
                    color = AmandsSensePlugin.NonFleaItemsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_barter.png"];
                    break;
                case ESenseItemColor.WishList:
                    color = AmandsSensePlugin.WishListItemsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_fav_checked.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_fav_checked.png"];
                    break;
                case ESenseItemColor.Rare:
                    color = AmandsSensePlugin.RareItemsColor.Value;
                    break;
            }

            // SenseContainer Sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);
            }

            // SenseContainer Light
            if (light != null)
            {
                light.color = new Color(color.r, color.g, color.b, 1f);
                light.intensity = 0f;
                light.range = AmandsSensePlugin.LightRange.Value;
            }

            // SenseContainer Type
            if (typeText != null)
            {
                typeText.fontSize = 0.5f;
                typeText.text = AmandsSenseHelper.Localized("container", EStringCase.None);
                typeText.color = new Color(color.r, color.g, color.b, 0f);
            }

            // SenseContainer Name
            if (nameText != null)
            {
                nameText.fontSize = 1f;
                //nameText.text = "Name";
                nameText.text = "<b>" + lootableContainer.ItemOwner.ContainerName + "</b>";
                nameText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }

            // SenseContainer Description
            if (descriptionText != null)
            {
                descriptionText.fontSize = 0.75f;
                if (AmandsSensePlugin.ContainerLootcount.Value)
                    descriptionText.text = AmandsSenseHelper.Localized("loot", EStringCase.None) + " " + itemCount;
                else
                    descriptionText.text = "";
                descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
            }

            // SenseContainer Sound
            if (AmandsSensePlugin.SenseRareSound.Value && AmandsSenseClass.LoadedAudioClips.ContainsKey("SenseRare.wav"))
                if (!AmandsSensePlugin.SenseAlwaysOn.Value)
                    Singleton<BetterAudio>.Instance.PlayAtPoint(transform.position, AmandsSenseClass.LoadedAudioClips["SenseRare.wav"], AmandsSensePlugin.AudioDistance.Value, BetterAudio.AudioSourceGroupType.Environment, AmandsSensePlugin.AudioRolloff.Value, AmandsSensePlugin.ContainerAudioVolume.Value, EOcclusionTest.Fast);
                else
                if (!AmandsSensePlugin.SenseAlwaysOn.Value && !Drawer && lootableContainer.OpenSound.Length > 0)
                {
                    AudioClip OpenSound = lootableContainer.OpenSound[0];
                    if (OpenSound != null)
                        Singleton<BetterAudio>.Instance.PlayAtPoint(transform.position, OpenSound, AmandsSensePlugin.AudioDistance.Value, BetterAudio.AudioSourceGroupType.Environment, AmandsSensePlugin.AudioRolloff.Value, AmandsSensePlugin.ContainerAudioVolume.Value, EOcclusionTest.Fast);
                }
        }
        else if (amandsSenseWorld != null)
            amandsSenseWorld.CancelSense();
    }
    public override void UpdateSense()
    {
        if (lootableContainer != null && lootableContainer.gameObject.activeSelf)
        {
            // SenseContainer Defaults
            emptyLootableContainer = false;
            itemCount = 0;

            ContainerId = lootableContainer.Id;

            // SenseContainer Items
            ESenseItemColor eSenseItemColor = ESenseItemColor.Default;
            if (lootableContainer.ItemOwner != null && AmandsSenseClass.Player.Profile != null)// && AmandsSenseClass.Player.Profile.WishList != null)
            {
                CompoundItem lootItemClass = lootableContainer.ItemOwner.RootItem as CompoundItem;
                if (lootItemClass != null)
                {
                    object[] Grids = Traverse.Create(lootItemClass).Field("Grids").GetValue<object[]>();
                    if (Grids != null)
                    {
                        foreach (object grid in Grids)
                        {
                            IEnumerable<Item> Items = Traverse.Create(grid).Property("Items").GetValue<IEnumerable<Item>>();
                            if (Items != null)
                                foreach (Item item in Items)
                                {
                                    itemCount += 1;
                                    if (AmandsSenseClass.itemsJsonClass.RareItems.Contains(item.TemplateId))
                                        eSenseItemColor = ESenseItemColor.Rare;

                                    /*else if (AmandsSenseClass.Player.Profile.WishList.Contains(item.TemplateId) && eSenseItemColor != ESenseItemColor.Rare)
                                    {
                                        eSenseItemColor = ESenseItemColor.WishList;
                                    }*/

                                    else if (item.Template != null &&
                                             !item.Template.CanSellOnRagfair &&
                                             !AmandsSenseClass.itemsJsonClass.NonFleaExclude.Contains(item.TemplateId) &&
                                             eSenseItemColor != ESenseItemColor.Rare &&
                                             eSenseItemColor != ESenseItemColor.WishList)
                                    {
                                        if (!AmandsSensePlugin.FleaIncludeAmmo.Value && TemplateIdToObjectMappingsClass.TypeTable["5485a8684bdc2da71d8b4567"].IsAssignableFrom(item.GetType()))
                                            continue;
                                        else if (AmandsSensePlugin.EnableFlea.Value)
                                            eSenseItemColor = ESenseItemColor.NonFlea;
                                        else if (AmandsSenseClass.itemsJsonClass.KappaItems.Contains(item.TemplateId) && eSenseItemColor == ESenseItemColor.Default)
                                            eSenseItemColor = ESenseItemColor.Kappa;
                                    }
                                }
                        }
                    }
                }
            }

            if (itemCount == 0)
            {
                amandsSenseWorld.CancelSense();
                return;
            }

            // SenseContainer Color and Sprite
            if (AmandsSenseClass.LoadedSprites.ContainsKey("LootableContainer.png"))
                sprite = AmandsSenseClass.LoadedSprites["LootableContainer.png"];

            switch (eSenseItemColor)
            {
                case ESenseItemColor.Default:
                    color = AmandsSensePlugin.ObservedLootItemColor.Value;
                    break;
                case ESenseItemColor.Kappa:
                    color = AmandsSensePlugin.KappaItemsColor.Value;
                    break;
                case ESenseItemColor.NonFlea:
                    color = AmandsSensePlugin.NonFleaItemsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_barter.png"];
                    break;
                case ESenseItemColor.WishList:
                    color = AmandsSensePlugin.WishListItemsColor.Value;
                    if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_fav_checked.png"))
                        sprite = AmandsSenseClass.LoadedSprites["icon_fav_checked.png"];
                    break;
                case ESenseItemColor.Rare:
                    color = AmandsSensePlugin.RareItemsColor.Value;
                    break;
            }

            // SenseContainer Sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = new Color(color.r, color.g, color.b, spriteRenderer.color.a);
            }

            // SenseContainer Light
            if (light != null)
            {
                light.color = new Color(color.r, color.g, color.b, 1f);
                light.range = AmandsSensePlugin.LightRange.Value;
            }

            // SenseContainer Type
            if (typeText != null)
            {
                typeText.fontSize = 0.5f;
                //typeText.text = "Type";
                typeText.color = new Color(color.r, color.g, color.b, typeText.color.a);
            }

            // SenseContainer Name
            if (nameText != null)
            {
                nameText.fontSize = 1f;
                //nameText.text = "Name";
                nameText.color = new Color(textColor.r, textColor.g, textColor.b, nameText.color.a);
            }

            // SenseContainer Description
            if (descriptionText != null)
            {
                descriptionText.fontSize = 0.75f;
                if (AmandsSensePlugin.ContainerLootcount.Value)
                    descriptionText.text = AmandsSenseHelper.Localized("loot", EStringCase.None) + " " + itemCount;
                else
                    descriptionText.text = "";
                descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, descriptionText.color.a);
            }
        }
        else if (amandsSenseWorld != null)
            amandsSenseWorld.CancelSense();
    }

    public override void UpdateIntensity(float Intensity)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(color.r, color.g, color.b, color.a * Intensity);

        if (light != null)
            light.intensity = AmandsSensePlugin.LightIntensity.Value * Intensity * (Drawer ? 0.25f : 1f);

        if (typeText != null)
            typeText.color = new Color(color.r, color.g, color.b, Intensity);

        if (nameText != null)
            nameText.color = new Color(textColor.r, textColor.g, textColor.b, Intensity);

        if (descriptionText != null)
            descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, Intensity);
    }

    public override void RemoveSense()
    {
        //Destroy(gameObject);
    }
}
