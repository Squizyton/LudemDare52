using System;
using UnityEngine;

namespace Bullet
{
    public class EnemyBullet : MonoBehaviour
    {
        public float speed = 20f;

        private void Start()
        {
            //Rotate the bullet to face the player
            var targetRotation =
                Quaternion.LookRotation(GameManager.Instance.currentTarget.position - transform.position);
            //rotate to face the target
            transform.rotation = targetRotation;
            Destroy(gameObject, 10f);
        }

        //Move the bullet forward
        private void FixedUpdate()
        {
            //Move the bullet
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
    }
}
