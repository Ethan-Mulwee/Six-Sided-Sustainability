using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Milestones : MonoBehaviour
{
[SerializeField] private GameObject milestones;
    [SerializeField] MilestonesSO milestonesSO;
    public static Milestones instance;
    public bool PopulatedMilestones = false;
    public int Milestonecount = 0;

    public void GenerateMilestones()
    {
        VisualElement root = milestones.GetComponent<UIDocument>().rootVisualElement;
        VisualElement Menu = root.Q<VisualElement>("Menu");
        for ( int i = 0; i < 3 ; i++ )
        {
            var RandomMilestone = Random.Range(0, milestonesSO.milestones.Count);
            VisualElement Milestone = new VisualElement();
            Milestone.AddToClassList("TestStyle");

            Button button = new Button();
            button.AddToClassList("MilestoneButton");
            button.text = milestonesSO.milestones[RandomMilestone].Title;
            button.clicked+= () => { 
                switch (milestonesSO.milestones[RandomMilestone].Action)
                {
                    case 0:
                        GameManager.Singleton.structuresSO.Structures[1].Inventory += 2;
                        GameManager.Singleton.structuresSO.Structures[4].Inventory += 1;
                        break;
                    case 1:
                        GameManager.Singleton.structuresSO.Structures[2].Inventory += 2;
                        break;
                    case 2:
                        GameManager.Singleton.structuresSO.Structures[8].Inventory += 1;
                        break; 
                    case 3:
                        GameManager.Singleton.structuresSO.Structures[4].Inventory += 2;
                        break;
                    case 4:
                        GameManager.Singleton.structuresSO.Structures[3].Inventory += 2;
                        break;
                    case 5:
                        GameManager.Singleton.structuresSO.Structures[5].Inventory += 1;
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                }
                Menu.Clear();
                Close(root);
                TileManager.Instance.RaiseMilestone();
                PopulatedMilestones = false;
                Hexagon.CallUIUpdate?.Invoke();
                
            };
            Milestone.Add(button);

            Label Description = new Label();
            Description.AddToClassList("Description");
            Description.text = milestonesSO.milestones[RandomMilestone].Description;
            Milestone.Add(Description);

            Label Title = new Label();
            Title.AddToClassList("Title");
            Title.text = milestonesSO.milestones[RandomMilestone].Title;
            Milestone.Add(Title);

            VisualElement Image = new VisualElement();
            Image.AddToClassList("Image");
            Image.style.backgroundImage = milestonesSO.milestones[RandomMilestone].Image;
            Milestone.Add(Image);


            Menu.Add(Milestone);
        }
        switch (Milestonecount)
        {
            case 0:
                root.Q<Label>("Story").text = "As your island moves towards a sustainable future, the citizens of Island Town have begun to start projects to restore the world. Recently, the people have begun to remove trash from the ocean and mainland, and the world feels a lot cleaner.\r\n";
                break;
            case 1:
                root.Q<Label>("Story").text = "The citizens of Island Town have become even more motivated to save the environment. They have started a project to plant trees throughout the mainland in order to restore the natural world.\r\n";
                break;
            case 2:
                root.Q<Label>("Story").text = " Island Town has further established its construction of clean energy. People from all over the island have worked to restore the mainland by putting out forest fires and destroying harmful machines.\r\n";
                break;
            case 3:
                root.Q<Label>("Story").text = " As the people of Island Town have become closer and closer to clean energy, the adverse effects of climate change have started being resolved. The ocean has retreated as glaciers have re-frozen, and the air is cleaner than ever.\r\n";
                break;
            case 4:
                root.Q<Label>("Story").text = " With [Island Name] approaching a completely sustainable state, the citizens have been able to spread out and re-populate the rest of the world. Now, the Earth has been restored and clean energy has led your island to prosperity.";
                break;
            case 5:
                break;
        }
        Milestonecount += 1;
    }

    private void Start()
    {
        instance = this;
        VisualElement root = milestones.GetComponent<UIDocument>().rootVisualElement;
        root.style.display = DisplayStyle.None;
    }
    void Close(VisualElement root)
    {
        root.style.display = DisplayStyle.None;
        if (TitleScript.root.style.display == DisplayStyle.None)
        {
            TitleScript.playing = true;
        }
    }
}
