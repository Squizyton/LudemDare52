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
    }
}
