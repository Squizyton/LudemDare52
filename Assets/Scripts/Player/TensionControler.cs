using FMOD.Studio;
using FMODUnity;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using static FMODUnity.RuntimeManager;

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
    [SerializeField] private float m_UnderFire;

    [Header("MAX:")]
    [SerializeField][Range(0, 100f)] private float m_hitMAX;
    [SerializeField][Range(0, 100f)] private float m_enemySpawnMAX;
    [SerializeField][Range(0, 100f)] private float m_enemyCloseMAX;
    [SerializeField][Range(0, 100f)] private float m_enemyKilledTimerMAX;
    [SerializeField][Range(0, 100f)] private float m_excerciseTensionMAX;
    [SerializeField][Range(0, 100f)] private float l_healthMAX;
    [SerializeField][Range(0, 100f)] private float l_staminaMAX; 
    [SerializeField][Range(0, 100f)] private float l_enemycounterMAX; 
    [SerializeField][Range(0, 100f)] private float m_UnderFireMAX;

    [Header("Dropping speed:")]
    [SerializeField][Range(0, 10f)] private float m_hitDrop;
    [SerializeField][Range(0, 10f)] private float m_enemySpawnDrop;
    [SerializeField][Range(0, 10f)] private float m_enemyCloseDrop;
    [SerializeField][Range(0, 10f)] private float m_enemyKilledDrop;
    [SerializeField][Range(0, 10f)] private float m_excerciseTensionDrop;

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
    {                                                       //For health it's done by UpdateHealth()

        if (m_hit > 0)                                      //For Getting hit by enemy (burst)        
            m_hit -= m_hitDrop;
                
        if (m_enemySpawn > 0)                               //For Spawn of enemies (burst - lower but longer)   
            m_enemySpawn -= m_enemySpawnDrop;
                
        if (!isSprinting && hasExcercised)                  //For Sprint / jump (burst, depending on stamina)        
            m_excerciseTension -= m_excerciseTensionDrop;

        if (enemyKilledTimer > 0)                           //For Killing enemies (burst lowering tension)
            enemyKilledTimer -= m_enemyKilledDrop;

        m_enemycounter = enemycounter * 2.5f;                      //For Enemy counter it's static number with max level)

        if (m_enemyClose > 0)                               //For Enemies close (dynamic)
            m_enemyClose -= m_enemyCloseDrop;

        if (m_UnderFire > 0)                                //If Lenny shoot us
            m_UnderFire -= 1f;


        CheckValues();
        CurrentTension();
        FMODParametersUpdate();

        updateTimer = 0f;
    }

    private void CheckValues()
    {
        if (m_enemycounter > l_enemycounterMAX)             m_enemycounter = l_enemycounterMAX;
        if (m_excerciseTension > m_excerciseTensionMAX)     m_excerciseTension = m_excerciseTensionMAX;
        if (m_hit < 0)                                      m_hit = 0;
        if (m_enemySpawn < 0)                               m_enemySpawn = 0;
        if (m_excerciseTension < 0)                         m_excerciseTension = 0;
        if (m_UnderFire < 0)                                m_UnderFire = 0;
    }

    private void CurrentTension()
    {
        //General level of tension during gameplay 
        tensionLevel = m_hit + m_enemyClose + m_UnderFire + m_enemySpawn + m_health * 0.5f + m_enemycounter + m_excerciseTension - enemyKilledTimer;
        if(tensionLevel < 0) tensionLevel = 0;

        //Should be more responding to health and players fear
        heartRate = 60 + m_hit + m_health * 0.5f + m_excerciseTension * 0.7f + m_enemySpawn * 0.5f + m_UnderFire;
        if (heartRate > maxHeartBeat) heartRate = maxHeartBeat;         //so our heart won't just die
    }

    private void FMODParametersUpdate()
    {
        StudioSystem.setParameterByName("TensionLevel", tensionLevel);
        StudioSystem.setParameterByName("HeartRate", heartRate);
        StudioSystem.setParameterByName("EnemyCounter", enemycounter);
    }

    private void OnTriggerEnter(Collider other)                             
    {
        if (other.gameObject.tag == "Enemy")            m_enemyClose = m_enemyCloseMAX;
        if (other.gameObject.tag == "EnemyBullet")      EnemyShooting();                    // !!!not detected due to layer "HitBox"!!!
    }
    public void EnemyShooting()
    {
        Debug.Log("They are shooting at us!");
        m_UnderFire += 5;

        if (m_UnderFire > m_UnderFireMAX)
            m_UnderFire = m_UnderFireMAX;
    }

    #region Public Functions
    public void UpdateHealth(float newHealth)
    {
        StudioSystem.setParameterByName("health", newHealth);
        if(newHealth < 40)
            m_health = 40 - newHealth;
        currentHealth = newHealth;

    }

    public void WasHit()
    {
        m_hit = m_hitMAX;
    }
    public void UpdateStamina(float newStamina, bool sprint)
    {
        isSprinting = sprint;
        currentStamina = newStamina;
        m_stamina = l_staminaMAX - newStamina;

        if (isSprinting)                    m_excerciseTension += m_excerciseTensionDrop;
        if (m_stamina < 0)                  m_stamina = 0;
        if (m_stamina > 0 && !sprint)       hasExcercised = true;
    }

    public void EnemyCounter(float enemiesAlive)
    {
        enemycounter = enemiesAlive;
    }

    public void KilledEnemy()               
    {
        enemyKilledTimer = m_enemyKilledTimerMAX;
    }

    public void SpawnEnemies()              
    {
        m_enemySpawn = m_enemySpawnMAX;
    }
    public void AddJump()                   
    {
        m_excerciseTension = m_excerciseTension + 10;
        hasExcercised = true;
    }
    #endregion
}
