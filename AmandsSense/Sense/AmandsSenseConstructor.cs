using EFT;
using EFT.Interactive;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AmandsSense.Sense;

public class AmandsSenseConstructor : MonoBehaviour
{
    public AmandsSenseWorld amandsSenseWorld;

    public Color color = Plugin.ObservedLootItemColor.Value;
    public Color textColor = Plugin.TextColor.Value;

    public GameObject textGameObject;
    public SpriteRenderer spriteRenderer;
    public Sprite sprite;
    public Light light;

    public TextMeshPro typeText;
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;

    public virtual void SetSense(ObservedLootItem observedLootItem) { }
    public virtual void SetSense(LootableContainer lootableContainer) { }
    public virtual void SetSense(LocalPlayer DeadPlayer) { }
    public virtual void SetSense(ExfiltrationPoint ExfiltrationPoint) { }

    public virtual void UpdateSense() { }
    public virtual void UpdateSenseLocation() { }
    public virtual void UpdateIntensity(float Intensity) { }
    public virtual void RemoveSense() { }

    public virtual void Construct()
    {
        // SenseConstructor Sprite GameObject
        GameObject spriteGameObject = new GameObject("Sprite");
        spriteGameObject.transform.SetParent(gameObject.transform, false);
        RectTransform spriteRectTransform = spriteGameObject.AddComponent<RectTransform>();
        spriteRectTransform.localScale = Vector3.one * Plugin.IconSize.Value;

        // SenseConstructor Sprite
        spriteRenderer = spriteGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        // SenseConstructor Sprite Light
        light = spriteGameObject.AddComponent<Light>();
        light.color = new Color(color.r, color.g, color.b, 1f);
        light.shadows = Plugin.LightShadows.Value ? LightShadows.Hard : LightShadows.None;
        light.intensity = 0f;
        light.range = Plugin.LightRange.Value;

        if (Plugin.EnableSense.Value != EEnableSense.OnText)
            return;

        // SenseConstructor Text
        textGameObject = new GameObject("Text");
        textGameObject.transform.SetParent(gameObject.transform, false);
        RectTransform textRectTransform = textGameObject.AddComponent<RectTransform>();
        textRectTransform.localPosition = new Vector3(Plugin.TextOffset.Value, 0, 0);
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
}
