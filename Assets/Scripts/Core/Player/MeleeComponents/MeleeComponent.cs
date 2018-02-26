using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShieldComponent))]
public class MeleeComponent : MonoBehaviour {

    public float PowerupTime = 0.6f;
    public float MeleeCooldown = 0.1f;
    public float MeleeDuration = 0.1f;
    public MeleeCollider meleeColliderHorizontal;
    public MeleeCollider meleeColliderVertical;

    private bool isActive;
    private Coroutine meleeRoutine;
    private ShieldComponent shieldComponent;

    private float heldTime;

    private enum States
    {
        None, Held, Disabled, PoweredUp
    }
    private States state;

    private Animator anim;

    private void Start()
    {
        shieldComponent = GetComponent<ShieldComponent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        switch (state)
        {
            case States.None:
                break;
            case States.Held:
                heldTime = Time.deltaTime;
                if (heldTime >= PowerupTime)
                {
                    state = States.PoweredUp;
                }
                break;
            case States.PoweredUp:
                break;
            case States.Disabled:
                break;
            default:
                break;
        }
    }

    public void MeleePressed()
    {
        if (state == States.Disabled) return;

        if (shieldComponent.HasShield())
        {
            shieldComponent.DisableShield();
            Activate();
        }
        heldTime = 0f;
        state = States.Held;
    }

    public void MeleeReleased()
    {
        if (heldTime >= PowerupTime)
        {

        }
        state = States.None;
    }

    private bool Activate()
    {
        if (state == States.None)
        {
            if (meleeRoutine != null)
            {
                StopCoroutine(meleeRoutine);
                TurnOffTriggers();
            }
            meleeRoutine = StartCoroutine(Melee());
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Disable()
    {
        state = States.Disabled;
    }

    public EnergyTypes.Colours GetMeleeColour()
    {
        return shieldComponent.GetColour();
    }

    private IEnumerator Melee()
    {
        Vector2 direction = GameManager.GetMoveDirection();
        string animationName = "Melee";
        if (direction.y > Mathf.Abs(direction.x))
        {
            animationName += "Up";
            meleeColliderVertical.EnableCollider();
            meleeColliderVertical.transform.localEulerAngles = Vector3.zero;
        }
        else if (direction.y < -Mathf.Abs(direction.x))
        {
            animationName += "Down";
            meleeColliderVertical.EnableCollider();
            meleeColliderVertical.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
        }
        else
        {
            meleeColliderHorizontal.EnableCollider();
        }

        animationName += Player.Instance.GetComponent<ShieldComponent>().GetColour().ToString();

        anim.Play(animationName, 0, 0f);
        isActive = true;
        float meleeTimer = 0f;
        AudioMaster.PlayAudio(AudioMaster.AudioClips.Melee1);

        while (meleeTimer < MeleeCooldown)
        {
            meleeTimer += Time.deltaTime;
            yield return null;
        }
        TurnOffTriggers();
    }

    private void TurnOffTriggers()
    {
        isActive = false;
        meleeColliderHorizontal.DisableCollider();
        meleeColliderVertical.DisableCollider();
    }

    public bool MeleeIsActive()
    {
        return isActive;
    }
}
