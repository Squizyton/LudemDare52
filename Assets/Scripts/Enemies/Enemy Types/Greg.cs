using UnityEngine;

namespace Enemies.Enemy_Types
{
    public class Greg : BasicEnemy
    {
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
    }
}
