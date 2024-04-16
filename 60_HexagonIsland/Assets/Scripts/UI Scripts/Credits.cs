using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Credits : MonoBehaviour
{
    public GameObject CreditsObject;
    public static VisualElement root;
    void Start()
    {
        root = CreditsObject.GetComponent<UIDocument>().rootVisualElement;
        Button Close = root.Q<Button>("Close");
        Close.clicked += () => { root.style.display = DisplayStyle.None; };
        root.style.display = DisplayStyle.None;
    }
}
