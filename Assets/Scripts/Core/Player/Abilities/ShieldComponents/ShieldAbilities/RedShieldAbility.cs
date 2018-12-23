using Aspekt.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrixieCore
{

    public class RedShieldAbility : BaseShieldAbility
    {
        public bool ShootRequiresFullCharge = true;
        public Beam.BeamStats beamSettings;
        public enum ChargeTypes
        {
            Chargeup, Cooldown, Deplete
        }
        public ChargeTypes ChargeType;
        public float ChargeupTime = 1f;
        public float CooldownTime = 1f;

        public Beam Beam;

        private float timer;

        protected override void Start()
        {
            base.Start();
            Colour = EnergyTypes.Colours.Red;
            
            Beam.SetBeamSettings(beamSettings);

            Beam.Deactivate();
        }

        public override void DisableAbility()
        {
            Beam.Deactivate();
        }

        public override void ActivatePressed()
        {
            if (ShootRequiresFullCharge && !power.ShieldFullyCharged()) return;

            if (ChargeType == ChargeTypes.Chargeup)
            {
                state = States.Charging;
            }
            else if (ChargeType == ChargeTypes.Deplete)
            {
                state = States.Activating;
                Beam.Activate();
            }
        }

        public override bool ActivateReleased()
        {
            shield.ChargeIndicator.StopCharge();
            bool shootSuccess = false;

            if (state == States.Charged && shield.IsShielding())
            {
                shootSuccess = true;
                ActivateShoot();
            }

            if (ChargeType == ChargeTypes.Chargeup)
            {
                timer = 0f;
                if (state == States.Charging)
                {
                    state = States.None;
                }
            }
            else if (ChargeType == ChargeTypes.Deplete)
            {
                state = States.None;
                Beam.Deactivate();
            }

            return shootSuccess;
        }

        public override void ReturnShield()
        {
            if (state != States.Returning && shield.gameObject.activeSelf)
            {
                state = States.Returning;
                StartCoroutine(ReturnShieldRoutine());
            }
        }

        public override void DisableShield()
        {
            if (state == States.Activating)
            {
                timer = 0f;
            }
            state = States.None;
            shield.ChargeIndicator.StopCharge();
            Beam.Deactivate();
        }

        public override void UpdateCharge(float deltaTime)
        {
            switch (state)
            {
                case States.None:
                    if (ChargeType == ChargeTypes.Cooldown)
                    {
                        if (timer > CooldownTime)
                        {
                            state = States.Charged;
                        }
                        else
                        {
                            state = States.Charging;
                        }
                    }
                    else
                    {
                        timer = 0f;
                    }
                    break;
                case States.Charging:
                    timer += deltaTime;
                    shield.ChargeIndicator.SetCharge(timer / (ChargeType == ChargeTypes.Chargeup ? ChargeupTime : CooldownTime));
                    TransitionIfCharged();
                    break;
                case States.Charged:
                    break;
                case States.Activating:
                    float shieldDistance = (transform.position - shield.CenterPoint.position).magnitude;
                    //if (shieldDistance >= MaxBeamDistance)
                    //{
                    //    ReturnShield();
                    //}
                    break;
                case States.Returning:
                    break;
                default:
                    break;
            }
        }

        private void ActivateShoot()
        {
            state = States.Activating;

            shield.ShieldCollider.isTrigger = true;
            body.isKinematic = false;
            // body.velocity = ShootSpeed * GetMoveDirection().normalized;
            anim.Play("Shoot", 0, 0f);
        }

        private IEnumerator ReturnShieldRoutine()
        {
            body.isKinematic = true;

            while (Vector2.Distance(transform.position, shield.CenterPoint.position) > 1f)
            {
                Vector2 distVector = (shield.CenterPoint.position - transform.position).normalized;
                //transform.position += new Vector3(distVector.x, distVector.y, 0f) * ShootSpeed * Time.deltaTime;

                // Ensure we don't overshoot the target position
                Vector2 newDistVector = shield.CenterPoint.position - transform.position;
                if (Mathf.Sign(newDistVector.x) != Mathf.Sign(distVector.x) || Mathf.Sign(newDistVector.y) != Mathf.Sign(distVector.y))
                {
                    transform.position = new Vector3(shield.CenterPoint.position.x, shield.CenterPoint.position.y, transform.position.z);
                }
                yield return null;
            }

            shield.ShieldCollider.isTrigger = false;
            state = States.None;

            if (ChargeType == ChargeTypes.Cooldown)
            {
                timer = 0f;
            }
            shield.OnReturn();
        }

        private void TransitionIfCharged()
        {
            if (ChargeType == ChargeTypes.Chargeup && timer > ChargeupTime)
            {
                shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
                state = States.Charged;
            }
            else if (ChargeType == ChargeTypes.Cooldown && timer > CooldownTime)
            {
                shield.ChargeIndicator.SetCharged(ChargeType == ChargeTypes.Cooldown);
                state = States.Charged;
            }
        }

        private Vector2 GetMoveDirection()
        {
            Vector2 dir = Trixie.Instance.GetComponent<PlayerController>().GetMoveDirection();
            if (dir.y > Mathf.Abs(dir.x))
            {
                return Vector2.up;
            }
            else if (dir.y < -Mathf.Abs(dir.x))
            {
                return Vector2.down;
            }
            else if (Trixie.Instance.IsFacingRight())
            {
                return Vector2.right;
            }
            else
            {
                return Vector2.left;
            }
        }
    }
}

