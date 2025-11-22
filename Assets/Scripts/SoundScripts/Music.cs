using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip explorationMusic;
    public AudioClip battleMusic;
    
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = explorationMusic;
        audioSource.loop = true;
        audioSource.Play();
        
    }

    public void PlayBattleMusic()
    {
        audioSource.clip = battleMusic;
        audioSource.Play();
    }

    public void PlayExplorationMusic()
    {
        audioSource.clip = explorationMusic;
        audioSource.Play();
    }
}
