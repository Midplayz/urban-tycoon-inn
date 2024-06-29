using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [field: SerializeField] private List<AudioClip> receptionBell;
    [field: SerializeField] private AudioClip backgroundMusic;
    [field: SerializeField] private AudioClip cleaningSound;
    [field: SerializeField] private AudioClip checkinSound;
    [field: SerializeField] private AudioClip purchaseSound;
    [field: SerializeField] private AudioClip moneyCollectSound;

    [field: SerializeField] private AudioSource receptionBellAudioSource;
    [field: SerializeField] private AudioSource backgroundMusicAudioSource;
    [field: SerializeField] private AudioSource cleaningSoundAudioSource;
    [field: SerializeField] private AudioSource checkinSoundAudioSource;
    [field: SerializeField] private AudioSource purchaseAudioSource;
    [field: SerializeField] private AudioSource moneyCollectSoundAudioSource;

    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        backgroundMusicAudioSource.volume = 0.5f;
        backgroundMusicAudioSource.clip = backgroundMusic;
        backgroundMusicAudioSource.loop = true;
        backgroundMusicAudioSource.Play();
    }

    public void PlayReceptionBellSound()
    {
        AudioClip chosen = receptionBell[Random.Range(0, receptionBell.Count)];
        receptionBellAudioSource.clip = chosen;
        receptionBellAudioSource.loop = false;
        receptionBellAudioSource.Play();
    }
    public void PlayCleaningSound()
    {
        cleaningSoundAudioSource.clip = cleaningSound;
        cleaningSoundAudioSource.loop = true;
        cleaningSoundAudioSource.Play();
    }
    public void StopCleaningSound()
    {
        cleaningSoundAudioSource.Stop();
    }
    public void PlayCheckinSound()
    {
        checkinSoundAudioSource.clip = checkinSound;
        checkinSoundAudioSource.loop = true;
        checkinSoundAudioSource.Play();
    }
    public void StopCheckinSound()
    {
        checkinSoundAudioSource.Stop();
    }
    public void PlayPurchaseSound()
    {
        purchaseAudioSource.clip = purchaseSound;
        purchaseAudioSource.loop = false;
        purchaseAudioSource.Play();
    }
    public void PlayMoneyCollectSound()
    {
        moneyCollectSoundAudioSource.Stop();
        moneyCollectSoundAudioSource.clip = moneyCollectSound;
        moneyCollectSoundAudioSource.loop = false;
        moneyCollectSoundAudioSource.Play();
    }
}
