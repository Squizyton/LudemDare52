using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static UnityEngine.ParticleSystem;
using static UnityEngine.Rendering.DebugUI;

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
    [SerializeField] private FMODUnity.EventReference M_level1;
    [SerializeField] private FMODUnity.EventReference M_level2;
    [SerializeField] private FMODUnity.EventReference M_EndlessMode;
    [SerializeField] private FMODUnity.EventReference M_GameOver;
    [SerializeField] private FMODUnity.EventReference M_MainMenu;

    [Header("Snapshots:")]
    [SerializeField] private FMODUnity.EventReference S_MuteFight;
    [SerializeField] private FMODUnity.EventReference S_GamePaused;

    private FMOD.Studio.Bus InGameBus;
    private FMOD.Studio.EventInstance snapMuteFight;
    private FMOD.Studio.EventInstance snapGamePaused;
    private FMOD.Studio.EventInstance Music;

    private int oldLevel;

    private bool wasPaused;
    private bool IsFPS;


    //[Header("Current Music Mode")]
    //private MusicGameMode musicMode;
    //private CurrentGun currentGun;

    #region Main
    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else 
            Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        //Music = FMODUnity.RuntimeManager.CreateInstance(M_level1);

        InGameBus = FMODUnity.RuntimeManager.GetBus("Bus:/InGame");
        snapMuteFight = FMODUnity.RuntimeManager.CreateInstance(S_MuteFight);
        snapGamePaused = FMODUnity.RuntimeManager.CreateInstance(S_GamePaused);
    }

    public void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        ChangeMusicLevel(scene.buildIndex);
    }

    private void ChangeMusicLevel(int level)
    {
        if (level == oldLevel)
            return;
        switch (level)
        {
            case 0:
                FMODSetParameterByNameWithLabel("CurrentLevel", "MainMenu");

                //PlayLevelMusic(M_MainMenu);
                oldLevel = level;
                break;

            case 1:
                FMODSetParameterByNameWithLabel("CurrentLevel", "EndlessMode");

                PlayLevelMusic(M_EndlessMode);
                oldLevel = level;
                break;

            case 2:
                FMODSetParameterByNameWithLabel("CurrentLevel", "GameOverScreen");

                //PlayLevelMusic(M_GameOver);
                oldLevel = level;
                break;

            default:
                Debug.Log("I had level number that was not added to FMOD");
                FMODSetParameterByNameWithLabel("CurrentLevel", "MainMenu");
                oldLevel = level;
                break;
        }
    }
    void Start()
    {
        //if (SceneManager.GetActiveScene().buildIndex == 1)
        {
        //    PlayLevelMusic();
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
    }
    #endregion

    #region Script Functions
    public void ChangeSoundMode(GameManager.CurrentMode newMode)
    {   
        switch (newMode)
        {
            case GameManager.CurrentMode.FPS:
                if (!IsFPS) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    FMODSetParameterByName("GameMode", 1);

                    IsFPS = true;       //so it will not repeat in Update mode
                }
                break;

            case GameManager.CurrentMode.TopDown:
                if (IsFPS)  {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    FMODSetParameterByName("GameMode", 0);

                    IsFPS = false;       //so it will not repeat in Update mode
                }
                break;

            case GameManager.CurrentMode.GameOver:
                if (IsFPS)  {
                    FMODSetParameterByName("GameMode", 0);

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
        snapGamePaused.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //snapGamePaused.release();
        wasPaused = false;
    }

    public void UpdateSoundVolumes()
    {
        FMODSetParameterByName("Master_Volume", masterVolume);
        FMODSetParameterByName("Ambient_Volume", ambientVolume);
        FMODSetParameterByName("SFX_Volume", sfxVolume);
        FMODSetParameterByName("Music_Volume", musicVolume);
        //FMODSetParameterByName("Dialogue_Volume", dialogVolume);
    }

    private void PlayLevelMusic(FMODUnity.EventReference MusicEvent)
    {
        FMOD.Studio.PLAYBACK_STATE playbackState;
        Music.getPlaybackState(out playbackState);

        if (playbackState == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            Music.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Music.release();
        }
        
        Music = FMODUnity.RuntimeManager.CreateInstance(MusicEvent);
        Music.start();
    }

    private void FMODSetParameterByName(string fmodParameter, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(fmodParameter, value);
    }

    private void FMODSetParameterByNameWithLabel(string fmodParameter, string label)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByNameWithLabel(fmodParameter, label);
    }

    #endregion

    #region EnumTypes
    /*
    private enum CurrentGun
    {
        Peashooter,
        Carrotrifle,
        MelonCanon,
        Pepperflower,
        Corngun,
    }


    private enum MusicGameMode
    {
        Menu,
        FPS,
        RTS,
    }
    */
    #endregion
}
