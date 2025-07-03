using System;
using System.Linq;
using EFT;
using EFT.Interactive;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AmandsSense.Sense;

public class AmandsSenseExfil : MonoBehaviour
{
    public ExfiltrationPoint exfiltrationPoint;

    public Color color = Color.green;
    public Color textColor = Plugin.TextColor.Value;

    public SpriteRenderer spriteRenderer;
    public Sprite sprite;

    public Light light;

    public GameObject textGameObject;

    public TextMeshPro typeText;
    public TextMeshPro nameText;
    public TextMeshPro descriptionText;
    public TextMeshPro distanceText;

    public float Delay;
    public float LifeSpan;

    public bool UpdateIntensity = false;
    public bool Starting = true;
    public float Intensity = 0f;

    public void SetSense(ExfiltrationPoint ExfiltrationPoint)
    {
        exfiltrationPoint = ExfiltrationPoint;
        gameObject.transform.position = exfiltrationPoint.transform.position + Vector3.up * Plugin.ExfilVerticalOffset.Value;
        gameObject.transform.localScale = new Vector3(-50, 50, 50);
    }

    public void Construct()
    {
        // AmandsSenseExfil Sprite GameObject
        var spriteGameObject = new GameObject("Sprite");
        spriteGameObject.transform.SetParent(gameObject.transform, false);

        var spriteRectTransform = spriteGameObject.AddComponent<RectTransform>();
        spriteRectTransform.localScale /= 50f;

        // AmandsSenseExfil Sprite
        spriteRenderer = spriteGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = new Color(color.r, color.g, color.b, 0f);

        // AmandsSenseExfil Sprite Light
        light = spriteGameObject.AddComponent<Light>();
        light.color = new Color(color.r, color.g, color.b, 1f);
        light.shadows = LightShadows.None;
        light.intensity = 0f;
        light.range = Plugin.ExfilLightRange.Value;

        // AmandsSenseExfil Text
        textGameObject = new GameObject("Text");
        textGameObject.transform.SetParent(gameObject.transform, false);

        var textRectTransform = textGameObject.AddComponent<RectTransform>();
        textRectTransform.localPosition = new Vector3(0.1f, 0, 0);
        textRectTransform.pivot = new Vector2(0, 0.5f);

        // AmandsSenseExfil VerticalLayoutGroup
        var verticalLayoutGroup = textGameObject.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.spacing = -0.02f;
        verticalLayoutGroup.childForceExpandHeight = false;
        verticalLayoutGroup.childForceExpandWidth = false;
        verticalLayoutGroup.childControlHeight = true;
        verticalLayoutGroup.childControlWidth = true;

        var contentSizeFitter = textGameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var typeTextGameObject = new GameObject("Type");
        typeTextGameObject.transform.SetParent(textGameObject.transform, false);
        typeText = typeTextGameObject.AddComponent<TextMeshPro>();
        typeText.autoSizeTextContainer = true;
        typeText.fontSize = 0.5f;
        typeText.text = "Type";
        typeText.color = new Color(color.r, color.g, color.b, 0f);

        var nameTextGameObject = new GameObject("Name");
        nameTextGameObject.transform.SetParent(textGameObject.transform, false);
        nameText = nameTextGameObject.AddComponent<TextMeshPro>();
        nameText.autoSizeTextContainer = true;
        nameText.fontSize = 1f;
        nameText.text = "Name";
        nameText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);

        var descriptionTextGameObject = new GameObject("Description");
        descriptionTextGameObject.transform.SetParent(textGameObject.transform, false);
        descriptionText = descriptionTextGameObject.AddComponent<TextMeshPro>();
        descriptionText.autoSizeTextContainer = true;
        descriptionText.fontSize = 0.75f;
        descriptionText.text = "";
        descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);

        var distanceTextGameObject = new GameObject("Distance");
        distanceTextGameObject.transform.SetParent(gameObject.transform, false);
        distanceTextGameObject.transform.localPosition = new Vector3(0, -0.13f, 0);
        distanceText = distanceTextGameObject.AddComponent<TextMeshPro>();
        distanceText.alignment = TextAlignmentOptions.Center;
        distanceText.autoSizeTextContainer = true;
        distanceText.fontSize = 0.75f;
        distanceText.text = "Distance";
        distanceText.color = new Color(color.r, color.g, color.b, 0f);

        enabled = false;
        gameObject.SetActive(false);
    }

    public void ShowSense()
    {
        color = Color.green;
        textColor = Plugin.TextColor.Value;

        UpdateSenseInternal(false);
    }

    public void UpdateSense()
    {
        UpdateSenseInternal(true);
    }

    public void Update()
    {
        if (UpdateIntensity)
        {
            if (Starting)
            {
                Intensity += Plugin.IntensitySpeed.Value * Time.deltaTime;
                if (Intensity >= 1f)
                {
                    UpdateIntensity = false;
                    Starting = false;
                }
            }
            else
            {
                Intensity -= Plugin.IntensitySpeed.Value * Time.deltaTime;
                if (Intensity <= 0f)
                {
                    Starting = true;
                    UpdateIntensity = false;
                    enabled = false;
                    gameObject.SetActive(false);
                    return;
                }
            }

            if (spriteRenderer != null)
                spriteRenderer.color = new Color(color.r, color.g, color.b, color.a * Intensity);

            if (light != null)
                light.intensity = Intensity * Plugin.ExfilLightIntensity.Value;

            if (typeText != null)
                typeText.color = new Color(color.r, color.g, color.b, Intensity);

            if (nameText != null)
                nameText.color = new Color(textColor.r, textColor.g, textColor.b, Intensity);

            if (descriptionText != null)
                descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, Intensity);

            if (distanceText != null)
                distanceText.color = new Color(color.r, color.g, color.b, Intensity);
        }
        else if (!Starting)
        {
            LifeSpan += Time.deltaTime;
            if (LifeSpan > Plugin.ExfilDuration.Value)
                UpdateIntensity = true;
        }

        if (Camera.main != null)
        {
            transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));
            if (distanceText != null)
                distanceText.text = (int) Vector3.Distance(transform.position, Camera.main.transform.position) + "m";
        }
    }

    void UpdateSenseInternal(bool fUpdate)
    {
        if (exfiltrationPoint == null)
        {
            AmandsSenseClass.SenseExfils.Remove(this);
            Destroy(gameObject);
            return;
        }

        if (!exfiltrationPoint.gameObject.activeSelf ||
            AmandsSenseClass.Player == null ||
            !exfiltrationPoint.InfiltrationMatch(AmandsSenseClass.Player))
        {
            return;
        }

        sprite = AmandsSenseClass.LoadedSprites["Exfil.png"];
        bool Unmet = exfiltrationPoint.UnmetRequirements(AmandsSenseClass.Player).ToArray().Any();
        color = Unmet ? Plugin.ExfilUnmetColor.Value : Plugin.ExfilColor.Value;

        // AmandsSenseExfil Sprite
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
            spriteRenderer.color = new Color(color.r, color.g, color.b, (fUpdate ? color.a : 0f));
        }

        // AmandsSenseExfil Light
        if (light != null)
        {
            light.color = new Color(color.r, color.g, color.b, 1f);
            light.range = Plugin.ExfilLightRange.Value;

            if (!fUpdate)
                light.intensity = 0f;
        }

        // AmandsSenseExfil Type
        if (typeText != null)
        {
            typeText.fontSize = 0.5f;
            typeText.text = AmandsSenseHelper.Localized("exfil", EStringCase.None);
            typeText.color = new Color(color.r, color.g, color.b, (fUpdate ? color.a : 0f));
        }

        // AmandsSenseExfil Name
        if (nameText != null)
        {
            nameText.fontSize = 1f;
            nameText.text = "<b>" + AmandsSenseHelper.Localized(exfiltrationPoint.Settings.Name, 0) + "</b><color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + "<size=50%><voffset=0.5em> " + exfiltrationPoint.Settings.ExfiltrationTime + "s";
            nameText.color = new Color(textColor.r, textColor.g, textColor.b, (fUpdate ? textColor.a : 0f));
        }

        // AmandsSenseExfil Description
        if (descriptionText != null)
        {
            descriptionText.fontSize = 0.75f;
            string tips = "";
            if (Unmet)
                foreach (string tip in exfiltrationPoint.GetTips(AmandsSenseClass.Player.ProfileId))
                    tips = tips + tip + "\n";
            descriptionText.overrideColorTags = true;
            descriptionText.text = tips;
            descriptionText.color = new Color(textColor.r, textColor.g, textColor.b, (fUpdate ? textColor.a : 0f));
        }

        // AmandsSenseExfil Distancce
        if (distanceText != null)
        {
            distanceText.fontSize = 0.5f;
            distanceText.color = new Color(color.r, color.g, color.b, (fUpdate ? color.a : 0f));

            if (Camera.main != null)
                distanceText.text = (int) Vector3.Distance(transform.position, Camera.main.transform.position) + "m";
        }

        if (!fUpdate)
        {
            gameObject.SetActive(true);
            enabled = true;

            LifeSpan = 0f;
            Starting = true;
            Intensity = 0f;
            UpdateIntensity = true;
        }
    }
}
