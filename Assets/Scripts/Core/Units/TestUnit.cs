using UnityEngine;
using Aspekt.AI;
using TrixieCore;
using TrixieCore.Units;

public class TestUnit : BaseEnemy {

    private AIAgent ai;
    
	void Start () {
        ai = GetComponentInChildren<AIAgent>();
        ai.Activate();
        
	}
}
