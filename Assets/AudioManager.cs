using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [Header("Background Music")]
    public AudioClip[] backgroundMusicTracks;
    public float musicVolume = 0.5f;
    public bool shufflePlaylist = true;

    [Header("Sound Effects")]
    public AudioClip santaLaugh;
    public float sfxVolume = 0.7f;

    [Header("Audio Settings")]
    public bool preloadAudioClips = true;
    public float crossfadeDuration = 2f;

    private AudioSource musicSource1;
    private AudioSource musicSource2;
    private AudioSource sfxSource;
    private List<AudioClip> musicPlaylist = new List<AudioClip>();
    private int currentTrackIndex = -1;
    private static AudioManager instance;
    private bool isCrossfading = false;
    private AudioSource currentMusicSource;
    private AudioSource nextMusicSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudio();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeAudio()
    {
        // Создаем два AudioSource для кроссфейда
        musicSource1 = gameObject.AddComponent<AudioSource>();
        musicSource2 = gameObject.AddComponent<AudioSource>();

        foreach (var source in new[] { musicSource1, musicSource2 })
        {
            source.volume = 0f;
            source.loop = false;
        }

        // SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.volume = sfxVolume;

        // Предзагрузка аудио клипов
        if (preloadAudioClips && backgroundMusicTracks != null)
        {
            foreach (var clip in backgroundMusicTracks)
            {
                if (clip != null && clip.loadState == AudioDataLoadState.Unloaded)
                {
                    clip.LoadAudioData();
                }
            }
        }

        // Создаем плейлист
        if (backgroundMusicTracks != null && backgroundMusicTracks.Length > 0)
        {
            musicPlaylist.AddRange(backgroundMusicTracks);

            if (shufflePlaylist)
            {
                ShufflePlaylist();
            }

            StartCoroutine(PlayNextTrackSmooth());
        }
    }

    void Update()
    {
        // Автопереключение треков без проверки isPlaying (используем корутины)
    }

    void ShufflePlaylist()
    {
        for (int i = 0; i < musicPlaylist.Count; i++)
        {
            AudioClip temp = musicPlaylist[i];
            int randomIndex = Random.Range(i, musicPlaylist.Count);
            musicPlaylist[i] = musicPlaylist[randomIndex];
            musicPlaylist[randomIndex] = temp;
        }
    }

    IEnumerator PlayNextTrackSmooth()
    {
        if (musicPlaylist.Count == 0) yield break;

        // Ждем пока закончится кроссфейд
        while (isCrossfading)
            yield return null;

        currentTrackIndex = (currentTrackIndex + 1) % musicPlaylist.Count;
        AudioClip nextClip = musicPlaylist[currentTrackIndex];

        // Предзагрузка следующего трека
        if (nextClip.loadState == AudioDataLoadState.Unloaded)
        {
            nextClip.LoadAudioData();

            // Ждем загрузки
            while (nextClip.loadState == AudioDataLoadState.Loading)
                yield return null;
        }

        // Определяем какие источники использовать
        if (currentMusicSource == null || currentMusicSource == musicSource1)
        {
            currentMusicSource = musicSource1;
            nextMusicSource = musicSource2;
        }
        else
        {
            currentMusicSource = musicSource2;
            nextMusicSource = musicSource1;
        }

        // Начинаем кроссфейд
        StartCoroutine(CrossfadeToNextTrack(nextClip));

        Debug.Log($"Now playing: {nextClip.name}");
    }

    IEnumerator CrossfadeToNextTrack(AudioClip nextClip)
    {
        isCrossfading = true;

        // Настраиваем следующий источник
        nextMusicSource.clip = nextClip;
        nextMusicSource.Play();

        float timer = 0f;

        // Плавное увеличение громкости следующего трека и уменьшение текущего
        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / crossfadeDuration;

            nextMusicSource.volume = Mathf.Lerp(0f, musicVolume, progress);
            if (currentMusicSource != null)
                currentMusicSource.volume = Mathf.Lerp(musicVolume, 0f, progress);

            yield return null;
        }

        // Завершаем кроссфейд
        if (currentMusicSource != null)
        {
            currentMusicSource.Stop();
            currentMusicSource.volume = 0f;
        }

        nextMusicSource.volume = musicVolume;
        currentMusicSource = nextMusicSource;
        isCrossfading = false;

        // Ждем окончания трека и запускаем следующий
        float clipLength = nextClip.length - crossfadeDuration;
        yield return new WaitForSeconds(clipLength);

        StartCoroutine(PlayNextTrackSmooth());
    }

    // Воспроизвести смех Санты
    public void PlaySantaLaugh()
    {
        if (santaLaugh != null && sfxSource != null)
        {
            // Предзагрузка звука смеха
            if (santaLaugh.loadState == AudioDataLoadState.Unloaded)
            {
                santaLaugh.LoadAudioData();
            }

            sfxSource.PlayOneShot(santaLaugh);
        }
    }

    // Управление музыкой
    public void SkipTrack()
    {
        if (!isCrossfading)
        {
            StopAllCoroutines();
            StartCoroutine(PlayNextTrackSmooth());
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (currentMusicSource != null)
        {
            currentMusicSource.volume = musicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    // Статические методы
    public static void PlaySantaLaughSound()
    {
        if (instance != null)
        {
            instance.PlaySantaLaugh();
        }
    }

    public static void SkipMusicTrack()
    {
        if (instance != null)
        {
            instance.SkipTrack();
        }
    }
}