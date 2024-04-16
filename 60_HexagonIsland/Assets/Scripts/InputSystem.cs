using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public static event Action OnClicked, OnEscaped;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnClicked?.Invoke();
        }
        if (Input.GetKeyDown (KeyCode.Escape))
        {
            OnEscaped?.Invoke();
        }
    }
}
