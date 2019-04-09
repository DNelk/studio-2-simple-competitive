using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


    // -----------------------------------------------------

    #region AudioStuff

    private AudioSource audioSource;

    public AudioClip[] StrikeAudioClips = new AudioClip[3];
    private AudioClip strikeAudioClip;

    public AudioClip[] GrabbedAudioClips = new AudioClip[3];
    private AudioClip grabbedAudioClip;

    public AudioClip[] BlockedAudioClips = new AudioClip[3];
    private AudioClip blockedAudioClip;

    public AudioClip[] LandingAudioClips = new AudioClip[3];
    private AudioClip landingAudioClip;

    public AudioClip[] WhiffAudioClips = new AudioClip[5];
    private AudioClip whiffAudioClip;

    public AudioClip[] CrowdAudioClips = new AudioClip[3];
    private AudioClip crowdAudioClip;

    private AudioClip pickedClip;

    #endregion

    // -----------------------------------------------------


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAudio(AudioClip[] audioClips)
    {
        AudioClip whichClip;

        //
        int indexAudio = Random.Range(0, audioClips.Length);
        whichClip = audioClips[indexAudio];
        audioSource.clip = whichClip;
        audioSource.Play();
        //

    }
}

