using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TitleScript : MonoBehaviour
{
    public static bool playing = false;

    [SerializeField] private GameObject Title;
    [SerializeField] private GameObject SettingsUI;
    [SerializeField] private GameObject Intro;
    public static VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        root = Title.GetComponent<UIDocument>().rootVisualElement;
        VisualElement settingsRoot = SettingsUI.GetComponent<UIDocument>().rootVisualElement;

        settingsRoot.style.display = DisplayStyle.None;

        Button play = root.Q<Button>("Play");
        Button settings = root.Q<Button>("Settings");
        Button credits = root.Q<Button>("Credits");

        play.clicked += () => { StartGame(root); Intro.GetComponent<UIDocument>().rootVisualElement.style.display = DisplayStyle.Flex; };
        settings.clicked += () => Settings(settingsRoot);
        credits.clicked += () => Credits.root.style.display = DisplayStyle.Flex;
    }

    public void StartGame(VisualElement root)
    {
        root.style.display = DisplayStyle.None;
    }

    public void Settings(VisualElement root)
    {
        root.style.display = DisplayStyle.Flex;
    }
}
