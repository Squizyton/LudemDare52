using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class FMODSoundManager : MonoBehaviour
{
    public static FMODSoundManager Instance { get; private set; }

    [Header("Sound")][SerializeField] private bool isSoundVolumeModified;
    [SerializeField][Range(0, 100f)] private float masterVolume;
    [SerializeField][Range(0, 100f)] private float ambientVolume;
    [SerializeField][Range(0, 100f)] private float sfxVolume;
    [SerializeField][Range(0, 100f)] private float musicVolume;
    //[SerializeField] [Range(0, 100f)] private float dialogVolume;

    private bool IsFPS;
    FMOD.Studio.Bus InGameBus;
    FMOD.Studio.EventInstance muteSFXsnapshot;
    private FMOD.Studio.EventInstance Music;

    //[Header("Current Music Mode")]
    //private MusicGameMode musicMode;
    //private CurrentGun currentGun;


    private void Awake()
    {
        if (Instance != null && Instance != this) 
            Destroy(this.gameObject);
        else 
            Instance = this;
    }
    void Start()
    {
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Music/Music_InGame");
        InGameBus = FMODUnity.RuntimeManager.GetBus("Bus:/InGame");
        muteSFXsnapshot = FMODUnity.RuntimeManager.CreateInstance("snapshot:/MuteFightSFX");

        Music.start();
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (isSoundVolumeModified)
            UpdateSoundVolumes();
    }
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
                muteSFXsnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;

            case GameManager.CurrentMode.TopDown:
                if (IsFPS)  {
                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/UI_Game_ModeTransistion");
                    FMODSetParameterByName("GameMode", 0);
                    IsFPS = false;       //so it will not repeat in Update mode
                }
                muteSFXsnapshot.start();
                break;

            case GameManager.CurrentMode.GameOver:
                if (IsFPS)  {
                    FMODSetParameterByName("GameMode", 0);
                    IsFPS = false;       //so it will not repeat in Update mode
                }
                muteSFXsnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                InGameBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(newMode), newMode, null);
        }
    }


    public void UpdateSoundVolumes()
    {
        FMODSetParameterByName("Master_Volume", masterVolume);
        FMODSetParameterByName("Ambient_Volume", ambientVolume);
        FMODSetParameterByName("SFX_Volume", sfxVolume);
        FMODSetParameterByName("Music_Volume", musicVolume);
        //FMODSetParameterByName("Dialogue_Volume", dialogVolume);
    }

    private void FMODSetParameterByName(string fmodParameter, float value)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName(fmodParameter, value);
    }


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
