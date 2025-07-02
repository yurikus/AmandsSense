using EFT;
using EFT.Interactive;
using EFT.InventoryLogic;
using UnityEngine;

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
        UpdateSenseInternal(false);
    }

    public override void UpdateSense() =>
        UpdateSenseInternal(true);

    public override void UpdateIntensity(float Intensity)
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(color.r, color.g, color.b, color.a * Intensity);

        if (light != null)
            light.intensity = Plugin.LightIntensity.Value * Intensity * (Drawer ? 0.25f : 1f);

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

    void UpdateSenseInternal(bool fUpdate)
    {
        if (lootableContainer == null || !lootableContainer.gameObject.activeSelf)
        {
            if (amandsSenseWorld != null)
                amandsSenseWorld.CancelSense();

            return;
        }

        Drawer = amandsSenseWorld.eSenseWorldType == ESenseWorldType.Drawer;

        // SenseContainer Defaults
        emptyLootableContainer = false;
        itemCount = 0;

        ContainerId = lootableContainer.Id;

        // SenseContainer Items
        ESenseItemColor eSenseItemColor = ESenseItemColor.Default;
        if (lootableContainer.ItemOwner != null && AmandsSenseClass.Player.Profile != null)
        {
            var lootItemClass = lootableContainer.ItemOwner.RootItem as CompoundItem;
            if (lootItemClass != null)
            {
                var Grids = lootItemClass.Grids;
                if (Grids != null)
                {
                    foreach (var grid in Grids)
                    {
                        var Items = grid.Items;
                        if (Items == null)
                            continue;

                        foreach (Item item in Items)
                        {
                            itemCount++;

                            if (AmandsSenseClass.itemsJsonClass.RareItems.Contains(item.TemplateId))
                                eSenseItemColor = ESenseItemColor.Rare;

                            if (eSenseItemColor != ESenseItemColor.Rare)
                            {
                                if (AmandsSenseClass.Player != null && AmandsSenseClass.Player.Profile != null && AmandsSenseClass.Player.Profile.WishlistManager != null)
                                {
                                    var wm = AmandsSenseClass.Player.Profile.WishlistManager;
                                    if (wm.IsInWishlist(item.TemplateId, includeQol: true, out var wmGroup))
                                        eSenseItemColor = ESenseItemColor.WishList;
                                }
                            }

                            if (eSenseItemColor != ESenseItemColor.Rare && eSenseItemColor != ESenseItemColor.WishList && item.Template != null &&
                                !item.Template.CanSellOnRagfair && !AmandsSenseClass.itemsJsonClass.NonFleaExclude.Contains(item.TemplateId))
                            {
                                if (!Plugin.FleaIncludeAmmo.Value && TemplateIdToObjectMappingsClass.TypeTable["5485a8684bdc2da71d8b4567"].IsAssignableFrom(item.GetType()))
                                    continue;
                                else if (Plugin.EnableFlea.Value)
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
        AmandsSenseClass.LoadedSprites.TryGetValue("LootableContainer.png", out sprite);

        //if (AmandsSenseClass.LoadedSprites.ContainsKey("LootableContainer.png"))
        //    sprite = AmandsSenseClass.LoadedSprites["LootableContainer.png"];

        switch (eSenseItemColor)
        {
            case ESenseItemColor.Default:
                color = Plugin.ObservedLootItemColor.Value;
                break;

            case ESenseItemColor.Kappa:
                color = Plugin.KappaItemsColor.Value;
                break;

            case ESenseItemColor.Rare:
                color = Plugin.RareItemsColor.Value;
                break;

            case ESenseItemColor.NonFlea:
                color = Plugin.NonFleaItemsColor.Value;
                AmandsSenseClass.LoadedSprites.TryGetValue("icon_barter.png", out sprite);
                //if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_barter.png"))
                //    sprite = AmandsSenseClass.LoadedSprites["icon_barter.png"];
                break;

            case ESenseItemColor.WishList:
                color = Plugin.WishListItemsColor.Value;
                AmandsSenseClass.LoadedSprites.TryGetValue("icon_fav_checked.png", out sprite);
                //if (AmandsSenseClass.LoadedSprites.ContainsKey("icon_fav_checked.png"))
                //    sprite = AmandsSenseClass.LoadedSprites["icon_fav_checked.png"];
                break;
        }

        // SenseContainer Sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = new Color(color.r, color.g, color.b, fUpdate ? spriteRenderer.color.a : 0f);
        }

        // SenseContainer Light
        if (light != null)
        {
            light.color = new Color(color.r, color.g, color.b, 1f);
            light.range = Plugin.LightRange.Value;

            if (!fUpdate)
                light.intensity = 0f;
        }

        // SenseContainer Type
        if (typeText != null)
        {
            typeText.fontSize = 0.5f;
            typeText.color = new Color(color.r, color.g, color.b, fUpdate ? typeText.color.a : 0f);

            if (!fUpdate)
                typeText.text = AmandsSenseHelper.Localized("container", EStringCase.None);

        }

        // SenseContainer Name
        if (nameText != null)
        {
            nameText.fontSize = 1f;
            nameText.color = new Color(textColor.r, textColor.g, textColor.b, fUpdate ? nameText.color.a : 0f);

            if (!fUpdate)
                nameText.text = "<b>" + lootableContainer.ItemOwner.ContainerName + "</b>";
        }

        // SenseContainer Description
        if (descriptionText != null)
        {
            descriptionText.fontSize = 0.75f;
            descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, fUpdate ? descriptionText.color.a : 0f);

            if (!fUpdate)
            {
                descriptionText.text = Plugin.ContainerLootcount.Value
                    ? AmandsSenseHelper.Localized("loot", EStringCase.None) + " (" + itemCount + ")"
                    : "";
            }
        }
    }
}
