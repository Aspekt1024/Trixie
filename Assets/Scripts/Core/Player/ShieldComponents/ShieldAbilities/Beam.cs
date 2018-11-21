using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrixieCore.Units;

namespace TrixieCore
{
    public class Beam : MonoBehaviour
    {
        public ShieldPositioner ShieldObject;

        private LineRenderer lineRenderer;
        private BeamStats beamStats;
        private Collider2D beamCollider;

        private List<HitTarget> enemiesInBeam;

        private bool isActive;

        [System.Serializable]
        public struct BeamStats
        {
            public EnergyTypes.Colours colour;
            public float maxBeamDistance;
            public float tickInterval;
            public int damage;

        }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            enemiesInBeam = new List<HitTarget>();
            beamCollider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (!isActive) return;

            List<HitTarget> enemiesToRemove = new List<HitTarget>();

            foreach (HitTarget hit in enemiesInBeam)
            {
                if (hit.obj.IsDestroyed())
                {
                    enemiesToRemove.Add(hit);
                }
                else if (hit.lastTickTime + beamStats.tickInterval < Time.time)
                {
                    DamageTarget(hit);
                }
            }

            foreach (HitTarget hit in enemiesToRemove)
            {
                enemiesInBeam.Remove(hit);
            }

            PositionBeam();

        }

        private void PositionBeam()
        {
            transform.position = ShieldObject.transform.position;
            transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(ShieldObject.shieldDirection.y, ShieldObject.shieldDirection.x) * Mathf.Rad2Deg);

            LayerMask layers = 1 << TrixieLayers.GetMask(Layers.Enemy) | 1 << TrixieLayers.GetMask(Layers.Terrain);
            RaycastHit2D result = Physics2D.CircleCast(ShieldObject.transform.position, 0.5f, transform.right, beamStats.maxBeamDistance, layers);
            float dist = beamStats.maxBeamDistance;
            if (result.collider != null)
            {
                dist = Vector2.Distance(transform.position, result.point);
            }

            lineRenderer.SetPosition(1, Vector2.right * dist);
            ((BoxCollider2D)beamCollider).size = new Vector2(dist, ((BoxCollider2D)beamCollider).size.y);

            beamCollider.offset = Vector2.right * dist / 2;
        }

        public void SetBeamSettings(BeamStats newSettings)
        {
            beamStats = newSettings;
        }

        public void Activate()
        {
            isActive = true;
            gameObject.SetActive(true);
            PositionBeam();
        }

        public void Deactivate()
        {
            isActive = false;
            enemiesInBeam = new List<HitTarget>();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IDamageable enemy = null;

            if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
            {
                if (collision.tag == "Enemy")
                {
                    enemy = collision.GetComponent<IDamageable>();
                    enemy.TakeDamage(beamStats.damage, collision.transform.position - transform.position, beamStats.colour);

                }
                else if (collision.tag == "Shield")
                {
                    collision.GetComponent<Units.EnemyShield>().HitShield(beamStats.colour, beamStats.damage);
                    enemy = GetComponentInParent<BaseEnemy>();
                }
            }

            if (enemy != null)
            {
                HitTarget hit = new HitTarget()
                {
                    obj = enemy,
                    lastTickTime = Time.time
                };

                enemiesInBeam.Add(hit);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            return;
            BaseEnemy enemy = collision.GetComponent<BaseEnemy>();
            if (enemy)
            {
                HitTarget enemyToRemove = new HitTarget()
                {
                    obj = null
                };
                foreach (HitTarget hit in enemiesInBeam)
                {
                    if ((MonoBehaviour)hit.obj == enemy)
                    { 
                        enemyToRemove = hit;
                        break;
                    }
                }

                if (enemyToRemove.obj != null)
                {
                    enemiesInBeam.Remove(enemyToRemove);
                }
            }
        }

        private void DamageTarget(HitTarget hit)
        {
            var monoObj = (MonoBehaviour)hit.obj;
            hit.obj.TakeDamage(beamStats.damage, monoObj.transform.position - transform.position, beamStats.colour);
            hit.lastTickTime = Time.time;
        }

        private struct HitTarget
        {
            public IDamageable obj;
            public float lastTickTime;

        }
    }
}

