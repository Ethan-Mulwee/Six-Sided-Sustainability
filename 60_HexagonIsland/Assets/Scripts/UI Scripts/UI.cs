using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEditor;
using static GameManager;

public class UIScript : MonoBehaviour
{
    [SerializeField] private GameObject UI;
    [SerializeField] private GameObject settingsUI;
    [SerializeField] private GameObject MilestoneUI;
    [SerializeField] private Texture2D HouseButtonActive;
    [SerializeField] private Texture2D HouseButtonInactive;
    [SerializeField] private BuildingDatabaseSO database;

    Label dayLabel;
    Label FPSLabel;
    ProgressBar pollutionProgressBar;
    ProgressBar PointProgressBar;

    public delegate void DisplayUpdate();
    public static DisplayUpdate displayUpdate;

    private Button Milestone;

    void Start()
    {
        displayUpdate += UpdateDisplay;
        VisualElement root = UI.GetComponent<UIDocument>().rootVisualElement;
        VisualElement settingsRoot = settingsUI.GetComponent<UIDocument>().rootVisualElement;
        VisualElement milestoneUI = MilestoneUI.GetComponent<UIDocument>().rootVisualElement;
        VisualElement Buttons = root.Q<VisualElement>("ButtonsElement");

        // Button day = root.Q<Button>("Day");
        Button settings = root.Q<Button>("Settings");
        Button Regenerate = root.Q<Button>("Regenerate");
        Button NormalSpeed = root.Q<Button>("1x");
        Button FastSpeed = root.Q<Button>("5x");
        Button Pause = root.Q<Button>("Pause");
        Milestone = root.Q<Button>("MilestoneButton");

        //Stats UI
        dayLabel = root.Q<Label>("DayLabel");
        FPSLabel = root.Q<Label>("FPSLabel");
        pollutionProgressBar = root.Q<ProgressBar>("PollutionProgressBar");
        PointProgressBar = root.Q<ProgressBar>("PointProgressBar");

        NormalSpeed.clicked += () => Time.timeScale = 1;
        FastSpeed.clicked += () => Time.timeScale = 5;
        Pause.clicked += () => Time.timeScale = 0;

        settings.clicked += () => ShowUI(settingsRoot);
    }

    void UpdateDisplay()
    {
        PointProgressBar.value = TileManager.Instance.Points;
        if (TileManager.Instance.Points >= TileManager.Instance.PointsGoal)
        {
            VisualElement milestoneUI = MilestoneUI.GetComponent<UIDocument>().rootVisualElement;
            ShowUI(milestoneUI);
            if (!Milestones.instance.PopulatedMilestones)
            {
                Milestones.instance.GenerateMilestones();
                Milestones.instance.PopulatedMilestones = true;
            }

        }
        PointProgressBar.title = "Milestone Progress: " + TileManager.Instance.Points + "/" + TileManager.Instance.PointsGoal;
        pollutionProgressBar.value = TileManager.Instance.Pollution;
        PointProgressBar.highValue = TileManager.Instance.PointsGoal;

    }
    void ShowUI(VisualElement root)
    {
        root.style.display = DisplayStyle.Flex;
        TitleScript.playing = false;
    }


}
