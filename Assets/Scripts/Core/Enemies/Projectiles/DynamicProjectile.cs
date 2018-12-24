using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{
    public class DynamicProjectile : Projectile
    {

        public GameObject Particles;
        private TrailRenderer trailRenderer;

        protected override void Awake()
        {
            base.Awake();

            if (Particles == null)
            {
                FindParticlesGameobject();
            }

            trailRenderer = GetComponent<TrailRenderer>();
            Particles.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Particles.SetActive(false);
            trailRenderer.enabled = true;
        }

        protected override void ShowImpact()
        {
            base.ShowImpact();
            trailRenderer.Clear();
            trailRenderer.enabled = false;
            Particles.SetActive(true);
        }

        protected override IEnumerator DeactivateAfterSeconds(float sec)
        {
            yield return base.DeactivateAfterSeconds(sec);
            Particles.SetActive(false);
        }

        protected override void PersistingExplosion()
        {
            base.PersistingExplosion();
            Particles.SetActive(true); // TODO need to set this to loop
        }

        protected override void Deactivate()
        {
            base.Deactivate();
            trailRenderer.Clear();
        }

        protected override void SetColourGraphic(Color color)
        {
            base.SetColourGraphic(color);
            GetComponent<SpriteRenderer>().color = color;
            trailRenderer.startColor = color;
            trailRenderer.endColor = new Color(color.r, color.g, color.b, 0f);
        }

        private void FindParticlesGameobject()
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == "Effects")
                {
                    Particles = child.gameObject;
                }
            }
        }
    }
}
