using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using static FMODUnity.RuntimeManager;

public class FMODSoundManager : MonoBehaviour
{
    public static FMODSoundManager Instance { get; private set; }

    [Header("Sound Test:")]
    [SerializeField] private bool isSoundVolumeModified;
    [SerializeField][Range(0, 100f)] private float masterVolume;
    [SerializeField][Range(0, 100f)] private float ambientVolume;
    [SerializeField][Range(0, 100f)] private float sfxVolume;
    [SerializeField][Range(0, 100f)] private float musicVolume;
    //[SerializeField] [Range(0, 100f)] private float dialogVolume;
    [SerializeField] private bool isPaused;

    [Header("Music levels:")]
    [SerializeField] private EventReference M_level1;
    [SerializeField] private EventReference M_level2;
    [SerializeField] private EventReference M_EndlessMode;
    [SerializeField] private EventReference M_GameOver;
    [SerializeField] private EventReference M_MainMenu;

    [Header("Snapshots:")]
    [SerializeField] private EventReference S_MuteFight;
    [SerializeField] private EventReference S_GamePaused;

    private Bus InGameBus;
    private EventInstance snapMuteFight;
    private EventInstance snapGamePaused;
    private EventInstance Music;

    private int oldLevel;

    private bool wasPaused;
    private bool IsFPS;


    #region Main
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        InGameBus = GetBus("Bus:/Diegetic Sounds");
        snapMuteFight = CreateInstance(S_MuteFight);
        snapGamePaused = CreateInstance(S_GamePaused);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeMusicLevel(scene.buildIndex);
    }

    private void ChangeMusicLevel(int level)
    {
        UpdateHealth(100);
        if (level == oldLevel)
            return;
        switch (level)
        {
            case 0:
                StudioSystem.setParameterByNameWithLabel("CurrentLevel", "MainMenu");

                //PlayLevelMusic(M_MainMenu);
                oldLevel = level;
                break;

            case 1:
                StudioSystem.setParameterByNameWithLabel("CurrentLevel", "EndlessMode");

                PlayLevelMusic(M_EndlessMode);
                oldLevel = level;
                break;

            case 2:
                StudioSystem.setParameterByNameWithLabel("CurrentLevel", "GameOverScreen");

                //PlayLevelMusic(M_GameOver);
                oldLevel = level;
                break;

            default:
                Debug.Log("I had level number that was not added to FMOD");
                StudioSystem.setParameterByNameWithLabel("CurrentLevel", "MainMenu");

                oldLevel = level;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isSoundVolumeModified)
            UpdateSoundVolumes();
        if (isPaused)
            if (!wasPaused)
                GamePaused();
            else return;
        else
            if (wasPaused)
            GameResumed();
        else return;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        ReleaseWithFade(snapGamePaused);
        ReleaseWithFade(snapMuteFight);
        ReleaseWithFade(Music);
    }
    #endregion

    #region Script Functions
    public void ChangeSoundMode(GameManager.CurrentMode newMode)
    {
        switch (newMode)
        {
            case GameManager.CurrentMode.FPS:
                if (!IsFPS) {
                    PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    StudioSystem.setParameterByName("GameMode", 1);

                    IsFPS = true;       //so it will not repeat in Update mode
                }
                break;

            case GameManager.CurrentMode.TopDown:
                if (IsFPS) {
                    PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    StudioSystem.setParameterByName("GameMode", 0);

                    IsFPS = false;       //so it will not repeat in Update mode
                }
                break;

            case GameManager.CurrentMode.GameOver:
                if (IsFPS) {
                    StudioSystem.setParameterByName("GameMode", 0);

                    InGameBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    IsFPS = false;       //so it will not repeat in Update mode
                }
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
        }
    }

    public void GamePaused()
    {
        snapGamePaused.start();
        wasPaused = true;
    }

    public void GameResumed()
    {
        ReleaseWithFade(snapGamePaused);
        wasPaused = false;
    }

    public void UpdateSoundVolumes()
    {
        StudioSystem.setParameterByName("Master_Volume", masterVolume);
        StudioSystem.setParameterByName("Ambient_Volume", ambientVolume);
        StudioSystem.setParameterByName("SFX_Volume", sfxVolume);
        StudioSystem.setParameterByName("Music_Volume", musicVolume);
    }

    private void PlayLevelMusic(EventReference MusicEvent)
    {
        PLAYBACK_STATE playbackState;
        Music.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING)
            ReleaseWithFade(Music);

        Music = CreateInstance(MusicEvent);
        Music.start();
    }

    public void UpdateHealth(float health)
    {
        StudioSystem.setParameterByName("health", health);
    }
    private void ReleaseWithFade(EventInstance FmodEvent)
    {
        FmodEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        FmodEvent.release();
    }
    #endregion
}
