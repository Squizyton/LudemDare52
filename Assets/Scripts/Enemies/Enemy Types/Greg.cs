using FMOD;
using System.Reflection;
using UnityEngine;
using static Unity.VisualScripting.Member;

namespace Enemies.Enemy_Types
{
    public class Greg : BasicEnemy
    {
        [SerializeField] private bool ifDoNotPlaySound;

        [SerializeField] private FMODUnity.EventReference FmodFootstepEvent;
        [SerializeField] private FMODUnity.EventReference FmodBodyfallEvent;
        [SerializeField] private FMODUnity.EventReference FmodDeathEvent;
        [SerializeField] private GameObject head;
        private void Start()
        {
            agent.speed = speed;
            healthBar.maxValue = health;
            healthBar.value = health;
            if (head == null)               head = gameObject;
        }


        public override void OnAttack(float distance)
        {
            if (distance > agent.stoppingDistance)
            {
                animator.SetTrigger("Run");
                state=State.Moving;
            }

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
                var transform1 = transform;
                //Rotate Greg after Every attack
                transform.rotation = Quaternion.Slerp(transform1.rotation, Quaternion.LookRotation(GameManager.Instance.currentTarget.position - transform1.position), 5 * Time.deltaTime);
                
            }
            else
            {
                animator.SetTrigger("Attack");
                attackTimer = attackRate;
            }
        }

        #region FMOD
        public void FmodPostFootstepsEvent()
        {
            string surfaceLayer = this.GetComponent<TerrainTextureFinder>().CheckLayers(this.transform.position);
            if (!ifDoNotPlaySound) FMODUnity.RuntimeManager.PlayOneShotAttached(FmodFootstepEvent, gameObject, "SurfaceLayer", surfaceLayer);   //FMOD
        }

        public void BodyfallSound()
        {
            PlaySound(FmodBodyfallEvent, gameObject);
        }
        public void FmodPostDeathEvent()
        {
            PlaySound(FmodDeathEvent, head);
        }

        public void PlaySound(string sound)
        {
            if (!ifDoNotPlaySound) FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);   //FMOD
        }

        public void PlaySound(FMODUnity.EventReference sound, GameObject source)
        {
            if (!ifDoNotPlaySound) FMODUnity.RuntimeManager.PlayOneShotAttached(sound, source);   //FMOD
        }
        #endregion
    }
}
