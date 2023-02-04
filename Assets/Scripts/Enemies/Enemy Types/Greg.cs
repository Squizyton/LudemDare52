using UnityEngine;

namespace Enemies.Enemy_Types
{
    public class Greg : BasicEnemy
    {
        [SerializeField] private bool ifDoNotPlaySound;

        [SerializeField] private FMODUnity.EventReference FmodFootstepEvent;
        [SerializeField] private FMODUnity.EventReference FmodBodyfallEvent;

        private void Start()
        {
            agent.speed = speed;
            healthBar.maxValue = health;
            healthBar.value = health;
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
            PlaySound(FmodFootstepEvent);
        }

        public void BodyfallSound()
        {
            PlaySound(FmodBodyfallEvent);
        }

        public void PlaySound(FMODUnity.EventReference sound)
        {
            if (!ifDoNotPlaySound) FMODUnity.RuntimeManager.PlayOneShotAttached(sound, gameObject);   //FMOD
        }
        #endregion
    }
}
