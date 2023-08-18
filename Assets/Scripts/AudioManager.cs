using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource mine = null;
    [SerializeField] AudioSource chop = null;
    [SerializeField] AudioSource poof = null;

    public void PlaySound(string soundName)
    {
        if (soundName == "mine" && mine != null)
        {
            mine.Play();
        }

        if (soundName == "chop" && chop != null)
        {
            chop.Play();
        }

        if (soundName == "poof" && poof != null)
        {
            poof.Play();
        }
    }
}
