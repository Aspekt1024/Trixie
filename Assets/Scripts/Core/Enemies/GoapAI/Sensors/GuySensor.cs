using ReGoap.Unity;
using UnityEngine;


namespace TrixieCore.Goap
{

    public class GuySensor : ReGoapSensor<GoapLabels, object>
    {
        public Transform WallSensor;
        public Transform EdgeSensor;
        public Transform GroundSensor;

        private BaseEnemy enemyScript;

        private CapsuleCollider2D capsuleCollider;

        
        private void Start()
        {
            var agent = GetComponentInParent<EnemyGoapAgent>();
            enemyScript = agent.Parent;
            capsuleCollider = enemyScript.GetComponent<CapsuleCollider2D>();
            memory = agent.GetMemory();
        }
        
        public override void UpdateSensor()
        {
            var worldState = memory.GetWorldState();

            //worldState.Set(GoapLabels.Guy_WallForward, IsAgainstWall());
            //worldState.Set(GoapLabels.Guy_GroundedForward, NotAtEdge());
            //worldState.Set(GoapLabels.Guy_Grounded, IsGrounded());
        }


    }

}
