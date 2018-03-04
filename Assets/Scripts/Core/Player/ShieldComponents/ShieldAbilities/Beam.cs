using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{

    public class Beam : MonoBehaviour
    {
        public ShieldPositioner ShieldObject;

        private LineRenderer lineRenderer;
        private BeamSettings settings;
        private Collider2D beamCollider;

        private List<HitEnemy> enemiesInBeam;

        private bool isActive;

        [System.Serializable]
        public struct BeamSettings
        {
            public EnergyTypes.Colours colour;
            public float maxBeamDistance;
            public float tickInterval;
            public int damage;

        }

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            enemiesInBeam = new List<HitEnemy>();
            beamCollider = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if (!isActive) return;

            List<HitEnemy> enemiesToRemove = new List<HitEnemy>();

            foreach (HitEnemy hit in enemiesInBeam)
            {
                if (hit.obj.IsDead())
                {
                    enemiesToRemove.Add(hit);
                }
                else if (hit.lastTickTime + settings.tickInterval < Time.time)
                {
                    DamageTarget(hit);
                }
            }

            foreach (HitEnemy hit in enemiesToRemove)
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
            RaycastHit2D result = Physics2D.CircleCast(ShieldObject.transform.position, 0.5f, transform.right, settings.maxBeamDistance, layers);
            float dist = settings.maxBeamDistance;
            if (result.collider != null)
            {
                dist = Vector2.Distance(transform.position, result.point);
            }

            lineRenderer.SetPosition(1, Vector2.right * dist);
            ((BoxCollider2D)beamCollider).size = new Vector2(dist, ((BoxCollider2D)beamCollider).size.y);

            beamCollider.offset = Vector2.right * dist / 2;
        }

        public void SetBeamSettings(BeamSettings newSettings)
        {
            settings = newSettings;
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
            enemiesInBeam = new List<HitEnemy>();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            BaseEnemy enemy = null;

            if (collision.gameObject.layer == TrixieLayers.GetMask(Layers.Enemy))
            {
                if (collision.tag == "Enemy")
                {
                    enemy = collision.GetComponent<BaseEnemy>();
                    enemy.DamageEnemy(collision.transform.position - transform.position, settings.colour, settings.damage);

                }
                else if (collision.tag == "Shield")
                {
                    collision.GetComponent<EnemyShield>().HitShield(settings.colour, settings.damage);
                    enemy = GetComponentInParent<BaseEnemy>();
                }
            }

            if (enemy)
            {
                HitEnemy hit = new HitEnemy()
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
                HitEnemy enemyToRemove = new HitEnemy()
                {
                    obj = null
                };
                foreach (HitEnemy hit in enemiesInBeam)
                {
                    if (hit.obj == enemy)
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

        private void DamageTarget(HitEnemy hit)
        {
            hit.obj.DamageEnemy(hit.obj.transform.position - transform.position, settings.colour, settings.damage);
            hit.lastTickTime = Time.time;
        }

        private struct HitEnemy
        {
            public BaseEnemy obj;
            public float lastTickTime;

        }
    }
}

