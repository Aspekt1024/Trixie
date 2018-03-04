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

        private bool[] groundedResults;
        private bool[] wallResults;
        
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
            UpdateGroundedResults();
            UpdateWallResults();

            RaycastHit2D hit = Physics2D.Raycast(transform.position, WallSensor.position - transform.position, (WallSensor.position - transform.position).x, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Terrain));
            worldState.Set(GoapLabels.Guy_WallForward, IsAgainstWall());

            hit = Physics2D.Raycast(transform.position, EdgeSensor.position - transform.position, (EdgeSensor.position - transform.position).magnitude, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Terrain));
            worldState.Set(GoapLabels.Guy_GroundedForward, NotAtEdge());

            hit = Physics2D.Raycast(transform.position, GroundSensor.position - transform.position, (GroundSensor.position - transform.position).magnitude, 1 << TrixieCore.TrixieLayers.GetMask(TrixieCore.Layers.Terrain));
            worldState.Set(GoapLabels.Guy_Grounded, IsGrounded());
            
        }

        private void UpdateGroundedResults()
        {
            float xScale = Mathf.Abs(enemyScript.transform.localScale.x);
            float yScale = enemyScript.transform.localScale.y;


            float yPos = transform.position.y + capsuleCollider.offset.y * yScale;
            float yDist = capsuleCollider.size.y / 2f * yScale + .2f;

            Vector2[] points = new Vector2[]
            {
                new Vector2(capsuleCollider.transform.position.x + (capsuleCollider.offset.x - capsuleCollider.size.x / 2f) * xScale - 0.4f, yPos - yDist - 1f),
                new Vector2(capsuleCollider.transform.position.x + capsuleCollider.offset.x * xScale, yPos - yDist),
                new Vector2(capsuleCollider.transform.position.x + (capsuleCollider.offset.x + capsuleCollider.size.x / 2f) * xScale + 0.4f, yPos - yDist - 1f)
            };

            groundedResults = new bool[3];

            for (int i = 0; i < points.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(points[i].x, transform.position.y), Vector2.down, transform.position.y - points[i].y, 1 << TrixieLayers.GetMask(Layers.Terrain));
                groundedResults[i] = hit.collider != null;
            }
        }

        private bool IsGrounded()
        {
            return groundedResults[1];
        }

        private bool NotAtEdge()
        {
            if (enemyScript.GetHorizontalVelocity() < 0)
            {
                return groundedResults[0];
            }
            else
            {
                return groundedResults[2];
            }
        }

        private void UpdateWallResults()
        {
            wallResults = new bool[3];

            float xScale = Mathf.Abs(enemyScript.transform.localScale.x);
            float yScale = enemyScript.transform.localScale.y;

            float dirMod = Mathf.Sign(enemyScript.GetHorizontalVelocity());

            Vector2 capBottom = (Vector2)capsuleCollider.transform.position + new Vector2(capsuleCollider.offset.x * xScale, (capsuleCollider.offset.y - capsuleCollider.size.y / 2f) * yScale);
            float capHeight = capsuleCollider.size.y * yScale;

            float xDist = capsuleCollider.size.x / 2 * xScale + 0.2f;

            float[] yPos = new float[]
            {
                capBottom.y + capHeight * 0.14f,
                capBottom.y + capHeight * 0.5f,
                capBottom.y + capHeight * 0.8f,
            };

            for (int i = 0; i < yPos.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(capBottom.x, yPos[i]), dirMod * Vector2.right, xDist, 1 << TrixieLayers.GetMask(Layers.Terrain));
                wallResults[i] = hit.collider != null;
            }
        }

        private bool IsAgainstWall()
        {
            return wallResults[0] || wallResults[1] || wallResults[2];
        }

    }

}
