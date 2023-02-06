using System;
using UnityEngine;
using FMOD.Studio;
using static FMODUnity.RuntimeManager;
using Unity.VisualScripting;
using FMODUnity;

public class TensionControler : MonoBehaviour
{
    [SerializeField] private float tensionLevel;
    [SerializeField] private float heartRate;

    [SerializeField] private float enemycounter;
    [SerializeField] private float enemyKilledTimer;
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentStamina;

    [Header("Modifiers:")]
    [SerializeField] private float m_hit;
    [SerializeField] private float m_enemyClose;
    [SerializeField] private float m_enemySpawn;
    [SerializeField] private float m_health;
    [SerializeField] private float m_stamina;
    [SerializeField] private float m_enemycounter;
    [SerializeField] private float m_excerciseTension;

    [Header("MAX:")]
    [SerializeField][Range(0, 100f)] private float m_hitMAX;
    [SerializeField][Range(0, 100f)] private float m_enemyCloseMAX;
    [SerializeField][Range(0, 100f)] private float m_enemySpawnMAX;
    [SerializeField][Range(0, 100f)] private float m_healthMAX;
    [SerializeField][Range(0, 100f)] private float m_staminaMAX;
    [SerializeField][Range(0, 100f)] private float m_enemycounterMAX;
    [SerializeField][Range(0, 100f)] private float excerciseTensionMAX;
    [SerializeField][Range(0, 100f)] private float enemyKilledTimerMAX;

    [Header("Dropping speed:")]
    [SerializeField][Range(0, 10f)] private float m_hitDrop;
    [SerializeField][Range(0, 10f)] private float m_enemySpawnDrop;
    [SerializeField][Range(0, 10f)] private float m_enemyCloseDrop;
    [SerializeField][Range(0, 10f)] private float excerciseTensionDrop;

    GameObject[] enemiesClose;
    [SerializeField] private EventReference hearbeatEvent;
    private EventInstance fmodHeartbeat;

    private bool hasExcercised;
    private bool isSprinting;

    private float updateTimer;
    private float maxHeartBeat;

    private void Start()
    {
        maxHeartBeat = 150;
        fmodHeartbeat = CreateInstance(hearbeatEvent);
        fmodHeartbeat.start();
    }

    private void OnDestroy()
    {
        fmodHeartbeat.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        fmodHeartbeat.release();
    }

    private void Update()
    {
        updateTimer += Time.unscaledDeltaTime;
        if (updateTimer > 1f)
            OnTimeUpdate();
    }


    private void OnTimeUpdate()
    {   //For health it's done by UpdateHealth()

        //For Getting hit by enemy (burst)
        if (m_hit > 0)        
            m_hit -= m_hitDrop;

        //For Spawn of enemies (burst - lower but longer)
        if (m_enemySpawn > 0)
            m_enemySpawn -= m_enemySpawnDrop;

        //For Sprint / jump (burst, depending on stamina)
        if (!isSprinting && hasExcercised)        
            m_excerciseTension -= excerciseTensionDrop;

        if (isSprinting)
            m_excerciseTension += excerciseTensionDrop;

        //For Killing enemies (burst lowering tension)
        if (enemyKilledTimer > 0)
            enemyKilledTimer -= 1f;

        //For Enemy counter (static with max level)
        m_enemycounter = enemycounter;

        //For Enemies close (dynamic)
        if (m_enemyClose > 0)
            m_enemyClose -= m_enemyCloseDrop;


        CheckValues();
        CurrentTension();
        FMODParametersUpdate();

        updateTimer = 0f;
    }

    private void CheckValues()
    {
        if (m_hit < 0)      m_hit = 0;
        if (m_enemycounter > m_enemycounterMAX)     m_enemycounter = m_enemycounterMAX;
        if (m_enemySpawn < 0)       m_enemySpawn = 0;
        if (m_excerciseTension > excerciseTensionMAX)       m_excerciseTension = excerciseTensionMAX;
        if (m_excerciseTension < 0)     m_excerciseTension = 0;
    }

    private void CurrentTension()
    {
        //General level of tension during gamepla 
        tensionLevel = m_hit + m_enemyClose + m_enemySpawn + m_health + m_enemycounter + m_excerciseTension - enemyKilledTimer;

        //Should be more responding to health and players fear
        heartRate = 60 + m_hit + m_health * 0.5f + m_excerciseTension * 0.7f + m_enemySpawn * 0.5f;
        if (heartRate > maxHeartBeat) heartRate = maxHeartBeat; //so our heart won't just die
    }

    private void FMODParametersUpdate()
    {
        StudioSystem.setParameterByName("TensionLevel", tensionLevel);
        StudioSystem.setParameterByName("HeartRate", heartRate);
        StudioSystem.setParameterByName("EnemyCounter", enemycounter);
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckEnemiesClose();
    }
    private void OnTriggerExit(Collider other)
    {
        CheckEnemiesClose();
    }




    #region Public Callouts
    public void UpdateHealth(float newHealth)
    {
        m_health = m_healthMAX - newHealth;
        currentHealth = newHealth;
        if (m_health < 0)
            m_health = 0;
        StudioSystem.setParameterByName("health", newHealth);
    }

    public void WasHit()
    {
        m_hit = m_hitMAX;
    }
    public void UpdateStamina(float newStamina, bool sprint)
    {
        isSprinting = sprint;

        m_stamina = m_staminaMAX - newStamina;
        if (m_stamina < 0) 
            m_stamina = 0;
        
        currentStamina = newStamina;
        if (m_stamina > 0 && !sprint)
            hasExcercised = true;
    }

    public void EnemyCounter(float enemiesAlive)
    {
        enemycounter = enemiesAlive;
    }

    public void KilledEnemy()               
    {
        enemyKilledTimer = enemyKilledTimerMAX;
        CheckEnemiesClose();
    }

    public void SpawnEnemies()              //activated once on round start
    {
        m_enemySpawn = m_enemySpawnMAX;
    }
    public void AddJump()                  //called when jumped on PlayerMovement
    {
        m_excerciseTension = m_excerciseTension + 10;
        hasExcercised = true;
    }
    #endregion

    private void CheckEnemiesClose()        //checked when enemies enter/leave the trigger area, and when you kill the enemy
    {
        enemiesClose = null;
        enemiesClose = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemiesClose.Length > 0)
            m_enemyClose = m_enemyCloseMAX;
    }
}
