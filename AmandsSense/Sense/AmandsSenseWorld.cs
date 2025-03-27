using UnityEngine;
using EFT;
using System.Threading.Tasks;
using EFT.Interactive;
using System;

namespace AmandsSense.Sense;

public class AmandsSenseWorld : MonoBehaviour
{
    public bool Lazy = true;
    public ESenseWorldType eSenseWorldType = ESenseWorldType.Item;
    public GameObject OwnerGameObject;
    public Collider OwnerCollider;

    public LocalPlayer SenseDeadPlayer;

    public int Id;

    public float Delay;
    public float LifeSpan;

    public bool Waiting = false;
    public bool WaitingRemoveSense = false;
    public bool UpdateIntensity = false;
    public bool Starting = true;
    public float Intensity = 0f;

    public GameObject amandsSenseConstructorGameObject;
    public AmandsSenseConstructor amandsSenseConstructor;

    public void Start()
    {
        enabled = false;
        WaitAndStart();
    }

    private async void WaitAndStart()
    {
        Waiting = true;

        await Task.Delay((int) (Delay * 1000));

        if (WaitingRemoveSense)
        {
            RemoveSense();
            return;
        }

        if (OwnerGameObject == null || OwnerGameObject != null & !OwnerGameObject.activeSelf)
        {
            RemoveSense();
            return;
        }

        if (Starting)
        {
            if (OwnerGameObject != null)
                transform.position = OwnerGameObject.transform.position;

            if (HeightCheck())
            {
                RemoveSense();
                return;
            }

            enabled = true;
            UpdateIntensity = true;

            amandsSenseConstructorGameObject = new GameObject("Constructor");
            amandsSenseConstructorGameObject.transform.SetParent(gameObject.transform, false);
            amandsSenseConstructorGameObject.transform.localScale = Vector3.one * AmandsSensePlugin.Size.Value;

            if (Lazy)
            {
                ObservedLootItem observedLootItem = OwnerGameObject.GetComponent<ObservedLootItem>();
                if (observedLootItem != null)
                {
                    eSenseWorldType = ESenseWorldType.Item;
                    amandsSenseConstructor = amandsSenseConstructorGameObject.AddComponent<AmandsSenseItem>();
                    amandsSenseConstructor.amandsSenseWorld = this;
                    amandsSenseConstructor.Construct();
                    amandsSenseConstructor.SetSense(observedLootItem);
                }
                else
                {
                    LootableContainer lootableContainer = OwnerGameObject.GetComponent<LootableContainer>();
                    if (lootableContainer != null)
                    {
                        if (lootableContainer.Template == "578f87b7245977356274f2cd")
                        {
                            eSenseWorldType = ESenseWorldType.Drawer;
                            amandsSenseConstructorGameObject.transform.localPosition = new Vector3(-0.08f, 0.05f, 0);
                            amandsSenseConstructorGameObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
                        }
                        else
                            eSenseWorldType = ESenseWorldType.Container;

                        amandsSenseConstructor = amandsSenseConstructorGameObject.AddComponent<AmandsSenseContainer>();
                        amandsSenseConstructor.amandsSenseWorld = this;
                        amandsSenseConstructor.Construct();
                        amandsSenseConstructor.SetSense(lootableContainer);
                    }
                    else
                    {
                        RemoveSense();
                        return;
                    }
                }
            }
            else
            {
                switch (eSenseWorldType)
                {
                    case ESenseWorldType.Item:
                        break;
                    case ESenseWorldType.Container:
                        break;
                    case ESenseWorldType.Drawer:
                        break;
                    case ESenseWorldType.Deadbody:
                        amandsSenseConstructor = amandsSenseConstructorGameObject.AddComponent<AmandsSenseDeadPlayer>();
                        amandsSenseConstructor.amandsSenseWorld = this;
                        amandsSenseConstructor.Construct();
                        amandsSenseConstructor.SetSense(SenseDeadPlayer);
                        break;
                }
            }

            // SenseWorld Starting Posittion
            switch (eSenseWorldType)
            {
                case ESenseWorldType.Item:
                case ESenseWorldType.Container:
                    gameObject.transform.position = new Vector3(OwnerCollider.bounds.center.x, OwnerCollider.ClosestPoint(OwnerCollider.bounds.center + Vector3.up * 10f).y + AmandsSensePlugin.VerticalOffset.Value, OwnerCollider.bounds.center.z);
                    break;

                case ESenseWorldType.Drawer:
                    if (OwnerCollider != null)
                    {
                        BoxCollider boxCollider = OwnerCollider as BoxCollider;
                        if (boxCollider != null)
                        {
                            Vector3 position = OwnerCollider.transform.TransformPoint(boxCollider.center);
                            gameObject.transform.position = position;
                            gameObject.transform.rotation = OwnerCollider.transform.rotation;
                        }
                    }
                    break;

                case ESenseWorldType.Deadbody:
                    amandsSenseConstructor?.UpdateSenseLocation();
                    break;
            }
        }
        else
        {
            LifeSpan = 0f;

            if (HeightCheck())
            {
                RemoveSense();
                return;
            }


            amandsSenseConstructor?.UpdateSense();

            // SenseWorld Position
            switch (eSenseWorldType)
            {
                case ESenseWorldType.Item:
                    gameObject.transform.position = new Vector3(OwnerCollider.bounds.center.x, OwnerCollider.ClosestPoint(OwnerCollider.bounds.center + Vector3.up * 10f).y + AmandsSensePlugin.VerticalOffset.Value, OwnerCollider.bounds.center.z);
                    break;
                case ESenseWorldType.Container:
                    break;
                case ESenseWorldType.Deadbody:
                    amandsSenseConstructor?.UpdateSenseLocation();
                    break;
                case ESenseWorldType.Drawer:
                    break;
            }
        }

        Waiting = false;
    }

    public void RestartSense()
    {
        if (Waiting || UpdateIntensity)
            return;

        LifeSpan = 0f;
        Delay = Math.Min(0, Vector3.Distance(AmandsSenseClass.Player.Position, gameObject.transform.position) / AmandsSensePlugin.Speed.Value);
        WaitAndStart();
    }

    public bool HeightCheck()
    {
        switch (eSenseWorldType)
        {
            case ESenseWorldType.Item:
            case ESenseWorldType.Container:
            case ESenseWorldType.Drawer:
            case ESenseWorldType.Deadbody:
                return AmandsSenseClass.Player != null && (transform.position.y < AmandsSenseClass.Player.Position.y + AmandsSensePlugin.MinHeight.Value || transform.position.y > AmandsSenseClass.Player.Position.y + AmandsSensePlugin.MaxHeight.Value);
        }
        return false;
    }
    public void RemoveSense()
    {
        amandsSenseConstructor?.RemoveSense();
        AmandsSenseClass.SenseWorlds.Remove(Id);
        if (gameObject != null)
            Destroy(gameObject);
    }
    public void CancelSense()
    {
        UpdateIntensity = true;
        Starting = false;
    }
    public void Update()
    {
        if (UpdateIntensity)
        {
            if (Starting)
            {
                Intensity += AmandsSensePlugin.IntensitySpeed.Value * Time.deltaTime;
                if (Intensity >= 1f)
                {
                    UpdateIntensity = false;
                    Starting = false;
                }
            }
            else
            {
                Intensity -= AmandsSensePlugin.IntensitySpeed.Value * Time.deltaTime;
                if (Intensity <= 0f)
                {
                    if (Waiting)
                        WaitingRemoveSense = true;
                    else
                        RemoveSense();
                    return;
                }
            }

            amandsSenseConstructor?.UpdateIntensity(Intensity);

        }
        else if (!Starting && !Waiting)
        {
            LifeSpan += Time.deltaTime;
            if (LifeSpan > AmandsSensePlugin.Duration.Value)
                UpdateIntensity = true;
        }

        if (Camera.main != null)
        {
            switch (eSenseWorldType)
            {
                case ESenseWorldType.Item:
                case ESenseWorldType.Container:
                case ESenseWorldType.Deadbody:
                    transform.rotation = Camera.main.transform.rotation;
                    transform.localScale = Vector3.one * Mathf.Min(AmandsSensePlugin.SizeClamp.Value, Vector3.Distance(Camera.main.transform.position, transform.position));
                    break;
                case ESenseWorldType.Drawer:
                    break;
            }
        }
    }
}
