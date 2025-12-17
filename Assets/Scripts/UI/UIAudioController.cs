using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudioController : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Sound Effects")]
    public AudioClip buttonClickClip;

    // Play sound for general UI button clicks
    public void PlayButtonClip() {
        if (buttonClickClip != null && audioSource != null) {
            audioSource.PlayOneShot(buttonClickClip);
        }
    }
}
