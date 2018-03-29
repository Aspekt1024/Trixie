using UnityEngine;
using Aspekt.AI;
using TestUnitLabels;

public class TestSensor : AISensor {
    
    private void Update()
    {
        Transform target = GameObject.Find("Cube").transform;
        if (Vector3.Distance(target.position, agent.transform.position) > 4f)
        {
            agent.GetMemory().Set(AILabels.TargetReached.ToString(), false);
        }
    }
}
