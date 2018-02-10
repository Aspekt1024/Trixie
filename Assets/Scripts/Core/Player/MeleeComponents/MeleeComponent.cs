using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeComponent : MonoBehaviour {

    public float MeleeCooldown = 0.4f;
    public float MeleeDuration = 0.1f;
    public MeleeCollider meleeColliderHorizontal;
    public MeleeCollider meleeColliderVertical;

    private bool isActive;

    private enum States
    {
        None, Attacking, Disabled
    }
    private States state;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool Activate()
    {
        if (state == States.None)
        {
            StartCoroutine(Melee());
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
        state = States.Attacking;
        float meleeTimer = 0f;
        AudioMaster.PlayAudio(AudioMaster.AudioClips.Melee1);

        while (meleeTimer < MeleeCooldown)
        {
            if (meleeTimer > MeleeDuration)
            {
                isActive = false;
                meleeColliderHorizontal.DisableCollider();
                meleeColliderVertical.DisableCollider();
            }
            meleeTimer += Time.deltaTime;
            yield return null;
        }
        state = States.None;
    }

    public bool MeleeIsActive()
    {
        return isActive;
    }
}
