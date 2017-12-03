using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollectOverlay : MonoBehaviour {

    public Text ItemText;   // TODO set this as a prefab so we can collect multiple objects?
    
	private void Start ()
    {
        ItemText.enabled = false;
	}
	
    public void ShowItemText(string itemText)
    {
        ItemText.text = "Item obtained:\n[" + itemText + "]";
        StartCoroutine(ShowItemTextAnimation());
    }

    public IEnumerator ShowItemTextAnimation()
    {
        ItemText.enabled = true;

        ItemText.transform.position = Player.Instance.transform.position + Vector3.up * 2f;
        yield return new WaitForSeconds(3f);
        yield return null;

        ItemText.enabled = false;
    }
}
