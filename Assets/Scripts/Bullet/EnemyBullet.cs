using System;
using UnityEngine;

namespace Bullet
{
    public class EnemyBullet : MonoBehaviour
    {
        public float speed = 20f;

        private FMOD.Studio.EventInstance BulletFlybySFX;

        private void Start()
        {
            //Rotate the bullet to face the player
            var targetRotation =
                Quaternion.LookRotation(GameManager.Instance.currentTarget.position - transform.position);
            //rotate to face the target
            transform.rotation = targetRotation;

            BulletFlybySFX = FMODUnity.RuntimeManager.CreateInstance("event:/SFX/Enemy/Enemy_Bullet_Flyby");
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(BulletFlybySFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
            BulletFlybySFX.start();

            Destroy(gameObject, 10f);
        }

        //Move the bullet forward
        private void FixedUpdate()
        {
            //Move the bullet
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));
        }
        private void OnDestroy()
        {
            BulletFlybySFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
