using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    [SerializeField] private AudioSource titleMusic;
    [SerializeField] private AudioSource levelMusic;

    [Header("One-Shot SFX Source")]
    [SerializeField] private AudioSource sfxSource;

    [Header("Looping SFX Sources")]
    [SerializeField] private AudioSource footstepSource;

    [Header("Clips")]
    [SerializeField] private AudioClip playerScreamClip;
    [SerializeField] private AudioClip marmotScreamClip;
    [SerializeField] private AudioClip playerLoseFightClip;
    [SerializeField] private AudioClip playerWinFightClip;
    [SerializeField] private AudioClip dogRetreatClip;
    [SerializeField] private AudioClip pestStunClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip eatClip;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip dropClip;
    [SerializeField] private AudioClip footstepClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (footstepSource != null)
        {
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;

            if (footstepClip != null)
                footstepSource.clip = footstepClip;
        }
    }

    public void PlayTitleMusic()
    {
        if (titleMusic == null) return;

        if (levelMusic != null && levelMusic.isPlaying)
            levelMusic.Stop();

        if (!titleMusic.isPlaying)
            titleMusic.Play();
    }

    public void PlayLevelMusic()
    {
        if (levelMusic == null) return;

        if (titleMusic != null && titleMusic.isPlaying)
            titleMusic.Stop();

        if (!levelMusic.isPlaying)
            levelMusic.Play();
    }

    public void StopAllMusic()
    {
        if (titleMusic != null) titleMusic.Stop();
        if (levelMusic != null) levelMusic.Stop();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayPlayerScream()
    {
        PlaySFX(playerScreamClip);
    }

    public void PlayMarmotScream()
    {
        PlaySFX(marmotScreamClip);
    }

    public void PlayPlayerLoseFight()
    {
        PlaySFX(playerLoseFightClip);
    }

    public void PlayPlayerWinFight()
    {
        PlaySFX(playerWinFightClip);
    }

    public void PlayDogRetreat()
    {
        PlaySFX(dogRetreatClip);
    }

    public void PlayPestStun()
    {
        PlaySFX(pestStunClip);
    }

    public void PlayJump()
    {
        PlaySFX(jumpClip);
    }

    public void PlayEat()
    {
        PlaySFX(eatClip);
    }

    public void PlayPickup()
    {
        PlaySFX(pickupClip);
    }

    public void PlayDrop()
    {
        PlaySFX(dropClip);
    }

    public void StartFootsteps()
    {
        if (footstepSource == null || footstepClip == null) return;
        if (!footstepSource.isPlaying)
            footstepSource.Play();
    }

    public void StopFootsteps()
    {
        if (footstepSource == null) return;
        if (footstepSource.isPlaying)
            footstepSource.Stop();
    }
}