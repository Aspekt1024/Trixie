using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationUI : MonoBehaviour {
    
    public TextMeshProUGUI ConversationText;
    public GameObject BackgroundObject;
    
    public void Show(string text)
    {
        ConversationText.text = text;
        ShowUI();
    }

    public void Hide()
    {
        BackgroundObject.SetActive(false);
        ConversationText.enabled = false;
    }

    private void Start()
    {
        Hide();
    }

    private void ShowUI()
    {
        BackgroundObject.SetActive(true);
        ConversationText.enabled = true;
    }
}
