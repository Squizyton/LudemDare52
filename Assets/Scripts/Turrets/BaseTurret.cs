using Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Turrets
{
    public class BaseTurret : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private TurretInfo turretInfo;
        [Header("Important Things")]
        [SerializeField] private Transform spawnPoint;

        private Collider[] enemiesInRange;
        private float timeRemaining;
        [Header("Targeting")]
        [SerializeField] private int maxTargets;
        [SerializeField] private float targetTime;
        [SerializeField] private LayerMask validTargetLayers;
        [SerializeField] private float trackingSpeed;

        [Header("UI")]
        [SerializeField] private Slider healthBar;

        private bool hasCurrentTarget;
        private BasicEnemy currentTarget;
        // Start is called before the first frame update
        void Start()
        {
            enemiesInRange = new Collider[maxTargets];
            timeRemaining = turretInfo.DecayTime;
            healthBar.maxValue = timeRemaining;
            InvokeRepeating("SearchTargets", 1.0f, targetTime);
            InvokeRepeating("Shoot", 1.0f, turretInfo.gunFireRate);
        }

        private void Update()
        {
            if (GameManager.Instance.currentMode != GameManager.CurrentMode.FPS) return;
            if(hasCurrentTarget) // Take aim
            {
                var transform1 = transform;
                transform.rotation = Quaternion.Slerp(transform1.rotation, Quaternion.LookRotation(currentTarget.transform.position - transform1.position), trackingSpeed * Time.deltaTime);
            }
            // Tick decay
            timeRemaining -= Time.deltaTime;
            healthBar.value = timeRemaining;
            if (timeRemaining <= 0f)
            {
                Destroy(gameObject, 0.1f);
            }
        }

        private void SearchTargets()
        {
            // Aggressively target one enemy as long as it's in range, regardless of whether it's the closest
            if (hasCurrentTarget && !currentTarget.GetIsDead())
            {
                float distance = Vector3.Distance(currentTarget.transform.position, transform.position);
                if (distance <= turretInfo.Range) return;
            }
            hasCurrentTarget = false;

            // No current target, check if there's any valid targets in range
            int numColliders = Physics.OverlapSphereNonAlloc(transform.position, turretInfo.Range, enemiesInRange, validTargetLayers);
            if (numColliders == 0) return;

            // Find closest target
            hasCurrentTarget = true;
            Collider nearestEnemy = enemiesInRange[0];
            float shortestDistance = Vector3.Distance(nearestEnemy.transform.position, transform.position);
            for (int i = 1; i < numColliders; i++)
            {
                float newDistance = Vector3.Distance(enemiesInRange[i].transform.position, transform.position);
                if (newDistance < shortestDistance)
                {
                    nearestEnemy = enemiesInRange[i];
                    shortestDistance = newDistance;
                }
            }
            nearestEnemy.TryGetComponent(out currentTarget);
            hasCurrentTarget = true;
        }

        private void Shoot()
        {
            if (hasCurrentTarget) Instantiate(turretInfo.bulletPrefab, spawnPoint.position, transform.rotation);
        }
    }
}
