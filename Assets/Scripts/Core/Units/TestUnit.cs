using UnityEngine;
using Aspekt.AI;

public class TestUnit : MonoBehaviour {

    private AIAgent ai;

	// Use this for initialization
	void Start () {
        ai = GetComponentInChildren<AIAgent>();
        ai.Activate();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
