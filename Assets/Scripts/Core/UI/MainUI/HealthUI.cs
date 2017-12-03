using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour {

    public GameObject Health1;
    public GameObject Health2;
    public GameObject Health3;
    public GameObject Health4;
    public GameObject Health5;

    public void UpdateHealth(int health)
    {
        Health1.SetActive(health >= 1);
        Health2.SetActive(health >= 2);
        Health3.SetActive(health >= 3);
        Health4.SetActive(health >= 4);
        Health5.SetActive(health >= 5);
    }
}
