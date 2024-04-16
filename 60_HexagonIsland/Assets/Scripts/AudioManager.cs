using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioSource PlacementSFX;

    private void Awake()
    {
        instance = this;
    }

    public void PlayPlacementSound()
    {

    }
}
