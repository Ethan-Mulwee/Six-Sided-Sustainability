using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller_2 : MonoBehaviour
{
    public Vector3 cameraPos;
    public Vector3 islandPos = new Vector3(0, 0, 0);

    public Vector3 oldMousePos;
    public Vector3 newMousePos;
    public Vector3 mouseVector;

    public float oldScroll = 0;
    public float newScroll;

    public float clickInput;
    public float scrollInput;

    public float cameraSensitivity;
    public float scrollSensitivity;

    public float scale;

    public float rotationSpeed;

    public float minorRadius;
    public float degrees;

    public float upperBound;
    public float lowerBound;
    public float outerBound;
    public float innerBound;

    public bool rotBound = false;
    public bool scrollBound = false;

    // Update is called once per frame
    void Update()
    {
        cameraSensitivity = SettingsScript.msensitivityValue / 10000;
        scrollSensitivity = SettingsScript.ssensitivityValue / 3;

        rotBound = false;
        scrollBound = false;

        cameraPos = transform.position;

        cameraPos = Vector3.Normalize(cameraPos);

        clickInput = Input.GetAxis("rightClick");
        scrollInput = Input.GetAxis("Mouse ScrollWheel");

        newScroll = scrollInput;
        newMousePos = Input.mousePosition;

        // Rotating
        if (TitleScript.playing)
        {
            if (clickInput > 0)
            {
                mouseVector = (oldMousePos - newMousePos);
            } else
            {
                mouseVector = Vector3.zero;
            }
        }
        else
        {
            degrees += rotationSpeed * Time.deltaTime;
        }

        oldMousePos = Input.mousePosition;

        degrees += mouseVector.x * cameraSensitivity;

        degrees %= 2 * Mathf.PI;

        cameraPos.x = minorRadius * Mathf.Cos(degrees);
        cameraPos.z = minorRadius * Mathf.Sin(degrees);

        // Scrolling
        if (TitleScript.playing)
        {
            if (scale - scrollInput * scrollSensitivity > outerBound)
            {
                scrollBound = true;
            }
            if (scale - scrollInput * scrollSensitivity < innerBound)
            {
                scrollBound = true;
            }

            if (!scrollBound)
            {
                scale -= scrollInput * scrollSensitivity;
            }
        }

        if (cameraPos.y + mouseVector.y * cameraSensitivity > upperBound)
        {
            rotBound = true;
        }
        if (cameraPos.y + mouseVector.y * cameraSensitivity < lowerBound)
        {
            rotBound = true;
        }

        if (!rotBound)
        {
            cameraPos.y += mouseVector.y * cameraSensitivity;
        }

        minorRadius = Mathf.Sin(Mathf.Acos(cameraPos.y));

        cameraPos *= scale;

        transform.position = cameraPos;

        Vector3 newDirection = Vector3.RotateTowards(cameraPos, islandPos, 20.0f, 0.0f) * -1;

        transform.rotation = Quaternion.LookRotation(newDirection);
    }
}
