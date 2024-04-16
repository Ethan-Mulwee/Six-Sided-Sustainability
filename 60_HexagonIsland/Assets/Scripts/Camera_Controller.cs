using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private Vector3 cameraPos;
    private Vector3 islandPos;

    private Vector3 oldMousePos;
    private Vector3 newMousePos;
    private Vector3 mouseVector;

    private float rightClickInput;
    private float scrollInput;

    public float maxRotationRadians = 20.0f;

    private float cameraSensitivity = 0.005f;
    public float scrollSensitivity = 0f;
    public float sensitivityConstant = 0f;
    public float cameraDeceleration = 0.9f;

    public float radius = 30.0f;
    public float size = 15.0f;

    public float rotationSpeed = 1f;

    private float minorRadius;
    private float myVar;

    private bool bound;

    void Update()
    {
        if (SettingsScript.msensitivityValue != 0)
        {
            cameraSensitivity = SettingsScript.msensitivityValue / 10000;
        }
        if (SettingsScript.ssensitivityValue != 0)
        {
            scrollSensitivity = SettingsScript.ssensitivityValue / 3;
        }

        bound = false;

        float upperBound = radius - 2.5f;
        float lowerBound = 2.5f;

        cameraPos = transform.position;
        islandPos = new Vector3(0, 0, 0);

        rightClickInput = Input.GetAxis("rightClick");
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (TitleScript.playing)
        {
            if (radius - scrollInput * scrollSensitivity > 15 && radius - scrollInput * scrollSensitivity < 50)
            {
                radius -= scrollInput * scrollSensitivity;
            }
        }

        minorRadius = radius * Mathf.Sin(Mathf.Acos(cameraPos.y / radius));

        if (TitleScript.playing)
        {
            if (rightClickInput > 0)
            {
                newMousePos = Input.mousePosition;

                mouseVector = (oldMousePos - newMousePos);


            }
            else
            {
                if (mouseVector.x <= 0.01 || mouseVector.y <= 0.01)
                {
                    mouseVector *= cameraDeceleration;
                }
                else if (mouseVector.x > 0 && mouseVector.y > 0)
                {
                    mouseVector = Vector3.zero;
                }
            }
        } else
        {
            myVar += rotationSpeed;
        }

        myVar += mouseVector.x * sensitivityConstant * cameraSensitivity;

        cameraPos.x = minorRadius * Mathf.Cos(myVar);

        cameraPos.z = minorRadius * Mathf.Sin(myVar);

        if (cameraPos.y + mouseVector.y * sensitivityConstant * cameraSensitivity * 10f > upperBound)
        {
            bound = true;
        }
        if (cameraPos.y + mouseVector.y * sensitivityConstant * cameraSensitivity * 10f < lowerBound)
        {
            bound = true;
        }

        if (!bound)
        {
            cameraPos.y += mouseVector.y * sensitivityConstant * cameraSensitivity * 10f;
        }

        transform.position = cameraPos;

        Vector3 newDirection = Vector3.RotateTowards(cameraPos, islandPos, maxRotationRadians, 0.0f) * -1;

        transform.rotation = Quaternion.LookRotation(newDirection);

        // Calculates the old vector so it is not overwritten next frame
        oldMousePos = Input.mousePosition;
    }
}

