using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static UnityEngine.Rendering.DebugUI;

public class FMODSoundManager : MonoBehaviour
{
    public static FMODSoundManager Instance { get; private set; }

    [Header("Sound")][SerializeField] private bool isSoundVolumeModified;
    [SerializeField][Range(0, 100f)] private float masterVolume;
    [SerializeField][Range(0, 100f)] private float ambientVolume;
    [SerializeField][Range(0, 100f)] private float sfxVolume;
    [SerializeField][Range(0, 100f)] private float musicVolume;
    //[SerializeField] [Range(0, 100f)] private float dialogVolume;

    [SerializeField] private FMOD.Studio.Bus InGameBus;
    [SerializeField] private FMOD.Studio.EventInstance snapMuteFight;
    [SerializeField] private FMOD.Studio.EventInstance snapshotGamePaused;
    [SerializeField] private FMOD.Studio.EventInstance Music;
    private int level;

    private bool wasPaused;
    private bool IsFPS;

    [SerializeField] private bool isPaused;

    //[Header("Current Music Mode")]
    //private MusicGameMode musicMode;
    //private CurrentGun currentGun;

    #region monobehaviour
    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(gameObject);
        else 
            Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Music_InGame");
        InGameBus = FMODUnity.RuntimeManager.GetBus("Bus:/InGame");
        snapMuteFight = FMODUnity.RuntimeManager.CreateInstance("snapshot:/MuteFight");
        snapshotGamePaused = FMODUnity.RuntimeManager.CreateInstance("snapshot:/GamePaused");
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        level = scene.buildIndex;

        switch (level)
        {
            case 0:
                FMODSetParameterByNameWithLabel("CurrentLevel", "MainMenu");
                break;

            case 1:
                FMODSetParameterByNameWithLabel("CurrentLevel", "EndlessMode");
                PlayLevelMusic();
                snapMuteFight.start();
                break;

            case 2:
                FMODSetParameterByNameWithLabel("CurrentLevel", "GameOverScreen");
                break;

            default:
                Debug.Log("I had level number that was not added to FMOD");
                FMODSetParameterByNameWithLabel("CurrentLevel", "MainMenu");
                break;
        }
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayLevelMusic();
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

    #region functions
    public void ChangeSoundMode(GameManager.CurrentMode newMode)
    {   
        switch (newMode)
        {
            case GameManager.CurrentMode.FPS:
                if (!IsFPS) {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    FMODSetParameterByName("GameMode", 1);
                    snapMuteFight.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    snapMuteFight.release();

                    IsFPS = true;       //so it will not repeat in Update mode
                }
                break;

            case GameManager.CurrentMode.TopDown:
                if (IsFPS)  {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    FMODSetParameterByName("GameMode", 0);
                    snapMuteFight.start();

                    IsFPS = false;       //so it will not repeat in Update mode
                }
                break;

            case GameManager.CurrentMode.GameOver:
                if (IsFPS)  {
                    FMODSetParameterByName("GameMode", 0);
                    snapMuteFight.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                    snapMuteFight.release();

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
        snapshotGamePaused.start();
        wasPaused = true;
    }

    public void GameResumed()
    {
        snapshotGamePaused.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        snapshotGamePaused.release();
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

    private void PlayLevelMusic()
    {
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
