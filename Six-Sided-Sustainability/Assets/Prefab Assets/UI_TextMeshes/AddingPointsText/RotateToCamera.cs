using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(RotationToCamera());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public IEnumerator RotationToCamera()
    {
        while (true)
        {
            var Rotation = Vector3.RotateTowards(gameObject.transform.position, Camera.main.transform.position, 20f, 0f) * -1;
            gameObject.transform.rotation = Quaternion.LookRotation(Rotation);
            yield return null;
        }
    }
}
