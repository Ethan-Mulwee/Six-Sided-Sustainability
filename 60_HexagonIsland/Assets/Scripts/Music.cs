using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip clip1;
    [SerializeField] private AudioClip clip2;
    [SerializeField] private AudioClip clip3;
    [SerializeField] private AudioClip clip4;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        
    }
    void Update()
    {
    }
}
