using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Intro : MonoBehaviour
{
    [SerializeField] private GameObject IntroObject;
    public static VisualElement root;

    // Start is called before the first frame update
    void Start()
    {
        root = IntroObject.GetComponent<UIDocument>().rootVisualElement;
        Button Close = root.Q<Button>("Close");
        Close.clicked += () => { TitleScript.playing = true; root.style.display = DisplayStyle.None; };
        root.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
