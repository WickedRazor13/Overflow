using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Music : MonoBehaviour
{
    public GameObject PlayerCube;
    public GameObject Water;

    public double musicDuration;
    public double goalTime;
    public AudioSource introAudioSource;
    public AudioSource endAudioSource;
    public AudioSource[] audioSources;
    public AudioSource[] fastSources;
    public int audioToggle = 0;

    public bool gameEnd = false;
    private bool loop = false;
    private bool water = false;

    // Start is called before the first frame update
    void Start()
    {
        double introDuration = (double)introAudioSource.clip.samples / introAudioSource.clip.frequency;
        goalTime = AudioSettings.dspTime + 0.5;
        introAudioSource.PlayScheduled(goalTime);
        goalTime = goalTime + introDuration;
        loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (loop & (AudioSettings.dspTime > goalTime - 1))
        {
            PlayScheduledClip();
        }

        // Determine Player in Water
        Collider PlayerCollider = PlayerCube.GetComponent<Collider>();
        Collider WaterCollider = Water.GetComponent<Collider>();

        if (PlayerCollider.bounds.Intersects(WaterCollider.bounds))
        {
            water = true;
        }
        else
        {
            if (water)
            {
                water = false;  // Reset water if player leaves
            }
        }

        // Determine Game end state FIX LATER
        float playerY = PlayerCube.transform.position.y;
        if (playerY >= 42.0)
        {
            gameEnd = true;
            Debug.Log("GameOver");
        }
    }



    private void PlayScheduledClip()
    {
        if (gameEnd)
        {
            endAudioSource.PlayScheduled(goalTime);
            loop = false;   // End looping audio
        }
        else if (!water)
        {
            audioSources[audioToggle].PlayScheduled(goalTime);

            musicDuration = (double)audioSources[audioToggle].clip.samples / audioSources[audioToggle].clip.frequency;
            goalTime = goalTime + musicDuration;

            audioToggle = 1 - audioToggle;
        }  
        else if (water)
        {
            fastSources[audioToggle].PlayScheduled(goalTime);

            musicDuration = (double)fastSources[audioToggle].clip.samples / fastSources[audioToggle].clip.frequency;
            goalTime = goalTime + musicDuration;

            audioToggle = 1 - audioToggle;
        }
    }
}
