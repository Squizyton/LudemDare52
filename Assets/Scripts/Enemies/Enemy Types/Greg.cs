using UnityEngine;

namespace Enemies.Enemy_Types
{
    public class Greg : BasicEnemy
    {
        private void Start()
        {
               
            healthBar.maxValue = health;
            healthBar.value = health;
        }
    }
}
