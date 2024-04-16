using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }
    public float PointsTimer { get; private set; }
    public float PollutionTimer { get; private set; }

    static public GameObject Hover;

    public GameObject PreviewObject;
    public Material HexagonMaterial;
    public Material DeconstructionMaterial;
    static public float BuildingRotation;
    public int PrefabSelection;
    public StructuresSO structuresSO;
    public GameObject GameOverScreen;

    //Put the UI document components on the gamemanger it self
    [SerializeField] private GameObject UI;
    private bool UIHover;

    static public int Building_ID;

    public static event Action ActionUpdate, PollutionUpdate, PointUpdate, StructureSelect, StructureDeselect;

    float TimeUpdate = 0f;
    private void Start()
    {
        Singleton = this;
        InputSystem.OnClicked += InputSystem_OnClicked;
        InputSystem.OnEscaped += InputSystem_OnEscaped;
        StructureDeselect += GameManager_OnStructureDeselect;

        foreach (Structure structure in structuresSO.Structures)
        {
            structure.Inventory = 0;
        }
        structuresSO.Structures.GetStructure(1).attributes.PollutionGenerated = 1;
        structuresSO.Structures.GetStructure(1).Inventory = 3;
        structuresSO.Structures.GetStructure(2).Inventory = 2;
        structuresSO.Structures.GetStructure(4).Inventory = 1;

        IntializeUI();

        BuildingRotation = UnityEngine.Random.Range(0, 360);
    }
    private void Update()
    {
        UpdateSelection();

        TimeUpdate += Time.deltaTime;
        if (TimeUpdate > 1f)
        {
            TimeUpdate = 0f;
            DynamicGI.UpdateEnvironment();
        }
        //Point and Pollution event clocks
        if (TitleScript.playing == true)
        {
            PointsTimer += Time.deltaTime;
            PollutionTimer += Time.deltaTime;
            if (PointsTimer > 5f)
            {
                PointsTimer = 0;
                PointUpdate?.Invoke();
            }
            if (PollutionTimer > 7.5f)
            {
                PollutionTimer = 2.5f;
                PollutionUpdate?.Invoke();
            }
            CheckGameOver();
        }
    }
    private void CheckGameOver()
    {
        if (TileManager.Instance.Pollution > 100)
        {
            TitleScript.playing = false;
            GameOverScreen.SetActive(true);
        }
    }
    private void GameManager_OnStructureDeselect()
    {
        Building_ID = 0;
    }

    void IntializeUI()
    {
        VisualElement root = UI.GetComponent<UIDocument>().rootVisualElement;
        VisualElement Buttons = root.Q<VisualElement>("ButtonsElement");

        for (int i = 0; i < structuresSO.Structures.Count; i++)
        {
            if (structuresSO.Structures[i].UI.CreateUI == true)
            {
                var j = i;
                var Selected = false;

                //Button
                Button NewButton = new Button();
                NewButton.AddToClassList("BuildingsButton");
                NewButton.style.backgroundImage = structuresSO.Structures[i].UI.ButtonIconDefault;
                NewButton.name = structuresSO.Structures[i].UI.ButtonName;

                //Inventory Count
                Label Count = new Label();
                Count.AddToClassList("InventoryCount");
                Count.text = (structuresSO.Structures[i].Inventory.ToString());

                //Tooltip
                Label Tooltip = new Label();
                Tooltip.AddToClassList("Tooltip");
                Tooltip.text = (structuresSO.Structures[i].UI.Tooltip).Replace("\\n", "\n");

                NewButton.Add(Count);

                NewButton.clicked += () => { StructureSelect?.Invoke(); Building_ID = j; PrefabSelection = UnityEngine.Random.Range(0, GameManager.Singleton.structuresSO.Structures[j].Prefabs.Count); PreviewObject.GetComponent<Preview>().SetPreview(j); Selected = true; NewButton.style.backgroundImage = structuresSO.Structures[j].UI.ButtonIconSelected; };
                StructureSelect += Deselect;
                Hexagon.CallUIUpdate += UpdateInventory;
                NewButton.RegisterCallback<MouseEnterEvent>(ButtonHover);
                void ButtonHover(MouseEnterEvent evt)
                {
                    UIHover = true;
                    if (Selected == false)
                    {
                        NewButton.style.backgroundImage = structuresSO.Structures[j].UI.ButtonIconHover;
                    }
                    NewButton.Add(Tooltip);
                }
                NewButton.RegisterCallback<MouseLeaveEvent>(ButtonUnhover);
                void ButtonUnhover(MouseLeaveEvent evt)
                {
                    UIHover = false;
                    if (Selected == false)
                    {
                        NewButton.style.backgroundImage = structuresSO.Structures[j].UI.ButtonIconDefault;
                    }
                    NewButton.Remove(Tooltip);
                }
                void Deselect()
                {
                    Selected = false;
                    if (Selected == false)
                    {
                        NewButton.style.backgroundImage = structuresSO.Structures[j].UI.ButtonIconDefault;
                    }
                }
                void UpdateInventory()
                {
                    Count.text = (structuresSO.Structures[j].Inventory.ToString());
                    if (structuresSO.Structures[j].Inventory == 0)
                    {
                        NewButton.style.display = DisplayStyle.None;
                    }
                    else NewButton.style.display = DisplayStyle.Flex;
                    {

                    }
                }

                Buttons.Add(NewButton);
            }
        }
        Hexagon.CallUIUpdate?.Invoke();
    }

    private void InputSystem_OnEscaped()
    {
        StructureDeselect?.Invoke();
    }

    private void InputSystem_OnClicked()
    {
        if (Hover != null)
        {
            if (Building_ID != 0)
            {
                Hover.GetComponent<Hexagon>().Construct(Building_ID);
                Hover.GetComponent<Hexagon>().UnHover();
                ActionUpdate?.Invoke();

            }
        }
    }

    private void UpdateSelection()
    {
        if (UIHover == false)
        {
            RaycastHit hit;
            var ray = RayFromCursor();
            if (Physics.Raycast(ray, out hit) && hit.transform.gameObject.GetComponent<Hexagon>() != null)
            {
                //If new hit then deselect old and select new
                if (Hover != hit.collider.gameObject)
                {
                    DeselectPreviousHexagon();
                    SelectCurrentHexagon(hit);
                }
            }
            //If raycast fails clear selection hover and preview
            else
            {
                DeselectPreviousHexagon();
                PreviewObject.SetActive(false);
                Hover = null;
            }
        }
        else
        {
            DeselectPreviousHexagon();
            PreviewObject.SetActive(false);
            Hover = null;
        }
    }

    Ray RayFromCursor()
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, 0f));
        return ray;
    }

    void DeselectPreviousHexagon()
    {
        if (Hover != null)
        {
            Hover.GetComponent<Hexagon>().UnHover();
            HexagonMaterial.SetVector("_Highlight", new Vector3(-100, 0, -100));
        }
    }

    void SelectCurrentHexagon(RaycastHit hit)
    {
        Hover = hit.collider.gameObject;
        Hover.GetComponent<Hexagon>().Hover();
        PreviewObject.SetActive(true);
    }



}

