﻿using UnityEngine;
using EFT;
using EFT.Interactive;
using UnityEngine.UI;
using TMPro;

namespace AmandsSense.Sense;

public class AmandsSenseConstructor : MonoBehaviour
{
    public AmandsSenseWorld amandsSenseWorld;

    public Color color = AmandsSensePlugin.ObservedLootItemColor.Value;
    public Color textColor = AmandsSensePlugin.TextColor.Value;

    public SpriteRenderer spriteRenderer;
    public Sprite sprite;

    public Light light;

    public GameObject textGameObject;

    public TextMeshPro typeText;
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;

    virtual public void Construct()
    {
        // SenseConstructor Sprite GameObject
        GameObject spriteGameObject = new GameObject("Sprite");
        spriteGameObject.transform.SetParent(gameObject.transform, false);
        RectTransform spriteRectTransform = spriteGameObject.AddComponent<RectTransform>();
        spriteRectTransform.localScale = Vector3.one * AmandsSensePlugin.IconSize.Value;

        // SenseConstructor Sprite
        spriteRenderer = spriteGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        // SenseConstructor Sprite Light
        light = spriteGameObject.AddComponent<Light>();
        light.color = new Color(color.r, color.g, color.b, 1f);
        light.shadows = AmandsSensePlugin.LightShadows.Value ? LightShadows.Hard : LightShadows.None;
        light.intensity = 0f;
        light.range = AmandsSensePlugin.LightRange.Value;

        if (AmandsSensePlugin.EnableSense.Value != EEnableSense.OnText)
            return;

        // SenseConstructor Text
        textGameObject = new GameObject("Text");
        textGameObject.transform.SetParent(gameObject.transform, false);
        RectTransform textRectTransform = textGameObject.AddComponent<RectTransform>();
        textRectTransform.localPosition = new Vector3(AmandsSensePlugin.TextOffset.Value, 0, 0);
        textRectTransform.pivot = new Vector2(0, 0.5f);

        // SenseConstructor VerticalLayoutGroup
        VerticalLayoutGroup verticalLayoutGroup = textGameObject.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.spacing = -0.02f;
        verticalLayoutGroup.childForceExpandHeight = false;
        verticalLayoutGroup.childForceExpandWidth = false;
        verticalLayoutGroup.childControlHeight = true;
        verticalLayoutGroup.childControlWidth = true;
        ContentSizeFitter contentSizeFitter = textGameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // SenseConstructor Type
        GameObject typeTextGameObject = new GameObject("Type");
        typeTextGameObject.transform.SetParent(textGameObject.transform, false);
        typeText = typeTextGameObject.AddComponent<TextMeshPro>();
        typeText.autoSizeTextContainer = true;
        typeText.fontSize = 0.5f;
        typeText.text = "Type";
        typeText.color = new Color(color.r, color.g, color.b, 0f);

        // SenseConstructor Name
        GameObject nameTextGameObject = new GameObject("Name");
        nameTextGameObject.transform.SetParent(textGameObject.transform, false);
        nameText = nameTextGameObject.AddComponent<TextMeshPro>();
        nameText.autoSizeTextContainer = true;
        nameText.fontSize = 1f;
        nameText.text = "Name";
        nameText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);

        // SenseConstructor Description
        GameObject descriptionTextGameObject = new GameObject("Description");
        descriptionTextGameObject.transform.SetParent(textGameObject.transform, false);
        descriptionText = descriptionTextGameObject.AddComponent<TextMeshPro>();
        descriptionText.autoSizeTextContainer = true;
        descriptionText.fontSize = 0.75f;
        descriptionText.text = "";
        descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
    }
    virtual public void SetSense(ObservedLootItem observedLootItem)
    {

    }
    virtual public void SetSense(LootableContainer lootableContainer)
    {

    }
    virtual public void SetSense(LocalPlayer DeadPlayer)
    {

    }
    virtual public void SetSense(ExfiltrationPoint ExfiltrationPoint)
    {

    }
    virtual public void UpdateSense()
    {

    }
    virtual public void UpdateSenseLocation()
    {

    }
    virtual public void UpdateIntensity(float Intensity)
    {

    }
    virtual public void RemoveSense()
    {

    }
}
