using UnityEngine;
using TrixieCore.UI;
using Aspekt.PlayerController;

namespace TrixieCore
{
    [RequireComponent(typeof(BaseShieldAbility))]
    public class ShieldAbility : PlayerAbility
    {

        public float DisableTime = 3f;
        public bool FullRotationControl = false;
        public GameObject ShieldObject;
        public Collider2D ShieldCollider;
        public Transform CenterPoint;
        public ShieldChargeIndicator ChargeIndicator;

        private ShieldStats stats;
        private ShieldPositioner positioner;
        private BaseShieldAbility[] abilities;

        private MoveComponent movement; // TODO need another hook for when the player moves - this is built into the PlayerController2D package

        private Rigidbody2D body;
        private Animator anim;
        private Animator playerAnim;

        private float shieldDisabledTimer;
        private bool activateButtonHeld;
        private bool shootButtonHeld;

        private int currentAbilityIndex;

        private enum States
        {
            None, Shielding, Firing, Disabled
        }
        private States state;


        private void Start()
        {
            stats = new ShieldStats();
            positioner = ShieldObject.GetComponent<ShieldPositioner>();

            GetAbilities();

            anim = ShieldObject.GetComponent<Animator>();
            body = ShieldObject.GetComponent<Rigidbody2D>();
            ShieldObject.SetActive(false);

            playerAnim = Trixie.Instance.GetComponent<Animator>();

            SetShieldColour(EnergyTypes.Colours.Blue);
            positioner.Setup(CenterPoint);

            movement = GameManager.Instance.Player.GetAbility<MoveComponent>();
        }

        private void Update()
        {
            if (movement.IsMoving)
            {
                Moved();
            }
            
            if (abilities != null && abilities.Length > currentAbilityIndex)
            {
                abilities[currentAbilityIndex].UpdateCharge(Time.deltaTime);
            }

            switch (state)
            {
                case States.None:
                    break;
                case States.Shielding:
                    if (FullRotationControl)
                    {
                        positioner.SetShieldPosition();
                    }
                    else
                    {
                        positioner.SetShieldPositionFixed();
                    }
                    break;
                case States.Firing:
                    break;
                case States.Disabled:
                    shieldDisabledTimer -= Time.deltaTime;
                    if (shieldDisabledTimer <= 0f)
                    {
                        GameUIManager.ShowShieldsEnabled();
                        state = States.None;
                    }
                    break;
            }
        }
        
        private void Activate()
        {
            state = States.Shielding;
            if (FullRotationControl)
            {
                positioner.SetShieldPosition();
            }
            else
            {
                positioner.SetShieldPositionFixed();
            }

            abilities[currentAbilityIndex].BeginShielding();

            if (shootButtonHeld)
            {
                abilities[currentAbilityIndex].ActivatePressed();
            }
        }

        public void Moved()
        {
            Debug.Log("moved");
            if (abilities[currentAbilityIndex].CancelOnMove)
            {
                abilities[currentAbilityIndex].DisableAbility();
            }
        }

        private void GetAbilities()
        {
            abilities = new BaseShieldAbility[3];
            abilities[0] = GetComponent<BlueShieldAbility>();
            abilities[1] = GetComponent<RedShieldAbility>();
            abilities[2] = GetComponent<GreenShieldAbility>();
        }

        public BaseShieldAbility GetAbility(EnergyTypes.Colours colour)
        {
            foreach (var ability in abilities)
            {
                if (ability.Colour == colour)
                {
                    return ability;
                }
            }
            return null;
        }

        public Vector2 GetShieldDirection()
        {
            return positioner.shieldDirection;
        }

        public void ProjectileImpact(Projectile projectile)
        {
            abilities[currentAbilityIndex].ProjectileImpact(projectile);
        }

        public void ReturnShield()
        {
            abilities[currentAbilityIndex].ReturnShield();
        }

        public void OnReturn()
        {
            body.velocity = Vector2.zero;
            if (activateButtonHeld)
            {
                Activate();
            }
            else
            {
                DisableShield();
            }
        }

        public bool HasShield()
        {
            return stats.ShieldUnlocked();
        }

        public void ObtainedUnlock(Items.ItemUnlock.UnlockType unlockType)
        {
            stats.ObtainedUnlock(unlockType);
            if (!stats.ColourUnlocked(abilities[currentAbilityIndex].Colour))
            {
                CycleShieldColour();
            }
            SetShieldColour(abilities[currentAbilityIndex].Colour);
        }

        public void CycleShieldColour()
        {
            if (!stats.ShieldUnlocked() || state == States.Disabled || state == States.Firing || shootButtonHeld) return;

            int index = currentAbilityIndex;

            index++;
            if (index == abilities.Length) index = 0;
            while (!stats.ColourUnlocked(abilities[index].Colour))
            {
                index++;
                if (index == abilities.Length) index = 0;
                if (currentAbilityIndex == index)
                {
                    break;
                }
            }
            abilities[currentAbilityIndex].DisableShield();
            currentAbilityIndex = index;
            SetShieldColour(abilities[currentAbilityIndex].Colour);
        }

        public bool ShieldActivatePressed()
        {
            activateButtonHeld = true;
            if (state == States.Disabled || state == States.Shielding || !stats.ShieldUnlocked()) return false;
            Activate();

            if (state == States.Firing)
            {
                abilities[currentAbilityIndex].ReturnShield();
            }
            else
            {
                Activate();
            }
            return true;
        }

        public bool ShieldDeactivatePressed()
        {
            activateButtonHeld = false;
            if (state == States.Firing) return false;

            DisableShield();
            return true;
        }

        public void ShootPressed()
        {
            shootButtonHeld = true;
            //if (!shieldStats.ShootUnlocked()) return;

            if (state == States.Shielding)
            {
                abilities[currentAbilityIndex].ActivatePressed();
            }
        }

        public void ShootReleased()
        {
            shootButtonHeld = false;

            if (state == States.Shielding)
            {
                bool shootSuccess = abilities[currentAbilityIndex].ActivateReleased();
                if (shootSuccess)
                {
                    state = States.Firing;
                }
            }
        }

        public void UtilityPressed()
        {
            abilities[currentAbilityIndex].ActivatePressed();
        }

        public void UtilityReleased()
        {
            abilities[currentAbilityIndex].ActivateReleased();
        }

        public bool ShieldIsDisabled() { return state == States.Disabled; }
        public bool ShieldIsCharged(EnergyTypes.Colours colour) { return abilities[currentAbilityIndex].IsAtMaxCharge(); }
        public bool IsAwaitingActivation() { return activateButtonHeld; }
        public bool IsShielding() { return state == States.Shielding; }
        public bool IsFiring() { return state == States.Firing; }
        public EnergyTypes.Colours GetColour() { return abilities[currentAbilityIndex].Colour; }

        public void DisableShield(float secondsToDisable)
        {
            shieldDisabledTimer = DisableTime;
            GameUIManager.ShowShieldsDisabled();
            DisableShield();
        }

        public void DisableShield()
        {
            if (shieldDisabledTimer <= 0f)
            {
                state = States.None;
            }
            else
            {
                state = States.Disabled;
            }
            abilities[currentAbilityIndex].DisableShield();
            abilities[currentAbilityIndex].StopShielding();
        }

        private void SetShieldColour(EnergyTypes.Colours colour)
        {
            if (stats.ShieldUnlocked())
            {
                GameUIManager.ShowShieldIndicator(stats);
                GameUIManager.SetShieldColour(abilities[currentAbilityIndex].Colour);
            }
            else
            {
                GameUIManager.HideShieldIndicator();
            }

            if (ShieldObject.activeSelf)
            {
                anim.Play("Activate" + abilities[currentAbilityIndex].Colour.ToString(), 0, 0f);
            }
        }
    }
}

