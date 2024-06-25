using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    private Vector3 rotation;
    [SerializeField] private Vector3 Startrotation;
    [SerializeField] private float IntialStrength;
    [SerializeField] private int DayLength;

    private void Start()
    {
        rotation = gameObject.transform.rotation.eulerAngles;
        gameObject.GetComponent<Light>().intensity = IntialStrength;
    }
    void Update()
    {
        rotation = Startrotation + new Vector3(8f, 0f, 0f)* Time.time;
        gameObject.transform.rotation = Quaternion.Euler(rotation);
    }
}
