using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsScript : MonoBehaviour
{
    public static float volumeValue = 0.5f;
    public static float msensitivityValue = 50;
    public static float ssensitivityValue = 50;

    [SerializeField] private GameObject Settings;

    // Start is called before the first frame update
    void Start()
    {
        VisualElement root = Settings.GetComponent<UIDocument>().rootVisualElement;

        Slider volume = root.Q<Slider>("Volume");
        Slider msensitivity = root.Q<Slider>("MSensitivity");
        Slider ssensitivity = root.Q<Slider>("SSensitivity");
        Button close = root.Q<Button>("Close");

        volume.RegisterValueChangedCallback(v =>
        {
            volumeValue = v.newValue;
        });
        msensitivity.RegisterValueChangedCallback(v =>
        {
            msensitivityValue = v.newValue;
        });
        ssensitivity.RegisterValueChangedCallback(v =>
        {
            ssensitivityValue = v.newValue;
        });
        close.clicked += () => Close(root);
    }

    void Close(VisualElement root)
    {
        root.style.display = DisplayStyle.None;
        if (TitleScript.root.style.display == DisplayStyle.None)
        {
            TitleScript.playing = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
