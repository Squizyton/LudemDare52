using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonBullet : BaseBullet
{
	[SerializeField] private float radius;
	[SerializeField] private float boomForce;
	[SerializeField] private LayerMask whatIsOnlyEnemy;
	[SerializeField] private GameObject hitFX;

	override protected void OnHit(Transform hit)
	{
		FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Player/Guns/Melon_Explode", gameObject);

		int maxColliders = 10;
		var hitColliders = new Collider[maxColliders];
		int numColliders = Physics.OverlapSphereNonAlloc(this.transform.position, radius, hitColliders, whatIsOnlyEnemy);

		for (int i = 0; i < numColliders; i++)
		{
			hitColliders[i].TryGetComponent(out BasicEnemy monster);
			hitColliders[i].TryGetComponent(out Rigidbody monsterRb);
			if (monster)
			{
				Debug.Log(-1 * boomForce * (monster.transform.position - this.transform.position).normalized);
				monsterRb.AddForce(-1 * boomForce * (monster.transform.position - this.transform.position).normalized, ForceMode.Impulse);
				monster.OnHit(damage);

			}
		}

		Instantiate(hitFX, transform.position, transform.rotation);
		CameraShake.Shake(10f, 0.25f, 1f);

		Destroy(gameObject);
	}
}
