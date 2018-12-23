using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TrixieCore.UI
{
    public class ShieldChargeIndicator : MonoBehaviour
    {
        public Image Meter;
        public float Offset = 0.05f;

        private bool hideWhenCharged;
        private bool visible;

        private enum States
        {
            None, Charging, Charged
        }
        private States state;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {

            if (state == States.None) return;
            SetPosition();
        }

        public void SetCharged(bool disableAfterPulse)
        {
            hideWhenCharged = disableAfterPulse;

            if (state == States.Charged || !visible) return;
            SetCharge(1 + Offset);
            state = States.Charged;
            StartCoroutine(PulseCharged(disableAfterPulse));
        }

        public void SetCharge(float percent)
        {
            visible = true;
            state = States.Charging;
            SetPosition();
            gameObject.SetActive(true);
            Meter.fillAmount = percent - Offset;
        }

        public void StopCharge()
        {
            visible = false;
            if (state == States.None) return;
            state = States.None;
            gameObject.SetActive(false);
        }

        private void SetPosition()
        {
            transform.position = Trixie.Instance.transform.position + Vector3.down * 2.2f;
        }

        private IEnumerator PulseCharged(bool disableAfterPulse)
        {
            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.1f;
            yield return null;
            transform.localScale = originalScale;
            yield return null;

            if (hideWhenCharged)
            {
                visible = false;
                gameObject.SetActive(false);
            }
        }
    }
}
