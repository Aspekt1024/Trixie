using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCollectOverlay : MonoBehaviour {

    public TextMeshProUGUI ItemAcquiredText;
    public TextMeshProUGUI ItemText;
    public ParticleSystem ItemParticleSystem;

    private Animator anim;
    
	private void Start ()
    {
        anim = GetComponent<Animator>();
        DisableText();
        ItemParticleSystem.Stop();
    }
	
    public void ShowItemText(string itemText)
    {
        ItemText.text = itemText;
        StartCoroutine(ShowItemTextAnimation());
    }

    public IEnumerator ShowItemTextAnimation()
    {
        EnableText();
        ItemParticleSystem.Play();
        anim.Play("Bounce", 0, 0f);
        
        yield return new WaitForSeconds(3f);
        yield return null;

        DisableText();
    }

    private void EnableText()
    {
        ItemAcquiredText.enabled = true;
        ItemText.enabled = true;
    }

    private void DisableText()
    {
        ItemAcquiredText.enabled = false;
        ItemText.enabled = false;
    }
}
