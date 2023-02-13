using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Enemies
{
	[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider)), RequireComponent(typeof(NavMeshAgent))]
	public class BasicEnemy : MonoBehaviour, IHasHealth
	{
		[Header("Base Stats")]
		[SerializeField]
		protected float health = 5;

		[SerializeField] protected float speed;
		[SerializeField] protected float damage;

		[Header("Attack Settings")]
		[SerializeField]
		protected float attackRate;

		protected float attackTimer;

		[Header("AI")][SerializeField] protected NavMeshAgent agent;
		[SerializeField] protected State state;

		[Header("UI")][SerializeField] protected Slider healthBar;

		[Header("Animator")][SerializeField] protected Animator animator;


		private bool isDead;

		private bool isOnFire;
		private float currentFireCooldown;


		[Header("Seeds")] public float seedDropChance;
		public List<PlantInfo> seeds;
		public SeedPickup seedDropPrefab;
		private static readonly int Death = Animator.StringToHash("Death");

		public virtual void OnHit(float damage, bool fire = false)
		{
			if (isDead) return;
			UIManager.Instance.TriggerHitIndicator();
			if (fire)
			{
				isOnFire = true;
				currentFireCooldown = 10;
			}

			health -= damage;
			healthBar.value = health;

			if (health <= 0)
			{
				OnDeath();
			}
			else
			{
				animator.SetTrigger("Hit");
				if(!isOnFire) 
					FMODUnity.RuntimeManager.PlayOneShotAttached("event:/SFX/Enemy/Greg/Enemy_Greg_Hit", gameObject);
			}
		}

		protected virtual bool TicDamage()
		{
			var damage = 0f;
			if (currentFireCooldown - Time.deltaTime >= 0)
			{
				damage = Time.deltaTime * .75f;
			
				currentFireCooldown -= Time.deltaTime;
			}
			else
			{
				damage = currentFireCooldown;
				currentFireCooldown = 0;
			}

			//animator.SetTrigger("Hit");
			health -= damage;

			healthBar.value = health;

			if (!(health <= 0)) return currentFireCooldown > 0;
		
			OnDeath();

			return currentFireCooldown > 0;
		}


		protected void Update()
		{
			if (isDead) return;

			//Handle tic damage, if any
			if (isOnFire && !TicDamage()) isOnFire = false;
			var distance = Vector3.Distance(transform.position, GameManager.Instance.currentTarget.position);

			switch (state)
			{
				case State.Moving:
					OnMove(distance);
					break;
				case State.Attacking:
					OnAttack(distance);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public virtual void OnMove(float distance)
		{
			agent.SetDestination(GameManager.Instance.currentTarget.position);


			if (distance <= agent.stoppingDistance)
			{
				attackTimer = attackRate;
				state = State.Attacking;
			}
		}

		public virtual void OnAttack(float distance)
		{
			if (distance > agent.stoppingDistance)
			{
				animator.SetTrigger("Run");
				state = State.Moving;
			}

			if (attackTimer > 0)
			{
				attackTimer -= Time.deltaTime;
			}
			else
			{
				animator.SetTrigger("Attack");
				attackTimer = attackRate;
			}
		}

		protected virtual void OnDeath()
		{
			isDead = true;
			agent.enabled = false;
			healthBar.gameObject.SetActive(false);
			UIManager.Instance.TriggerKillIndicator();
		
			TryGetComponent(out Collider component);
			component.enabled = false;
			TryGetComponent(out Rigidbody rb);
			rb.isKinematic = true;
			GameManager.Instance.RemoveEnemy();

			var weight = UnityEngine.Random.Range(0, 100);
			if (weight <= seedDropChance)
			{
				var seedDrop = Instantiate(seedDropPrefab, transform.position, transform.rotation);
				seedDrop.plantInfo = seeds[UnityEngine.Random.Range(0, seeds.Count)];
				seedDrop.image.sprite = seedDrop.plantInfo.seedIcon;
			}

			animator.SetTrigger(Death);
			
			Destroy(gameObject, 10f);
		}

		public float GetDamage()
		{
			return damage;
		}

		public bool GetIsDead()
		{
			return isDead;
		}


		protected enum State
		{
			Moving,
			Attacking,
		}

		public void TakeDamage(int damageTaken)
		{
			OnHit(damageTaken);
		}
	}
}