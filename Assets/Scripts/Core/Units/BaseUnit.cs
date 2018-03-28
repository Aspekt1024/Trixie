using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aspekt.AI;

namespace TrixieCore.Units
{
    public abstract class BaseUnit : MonoBehaviour
    {
        public enum UnitAlignments
        {
            Friendly, Enemy
        }

        public UnitAlignments alignment;
        public float AggroRadius = 15f;
        public event System.Action OnDeathCallback = delegate { }; // Used for events

        protected Transform AI;
        protected Transform Model;
        protected UnitEffectsHandler effects = new UnitEffectsHandler();
        protected UnitAbilityHandler abilities = new UnitAbilityHandler();

        private Animator anim;
        private Rigidbody2D body;
        private Collider2D coll;

        private bool directionFlipped;
        
        public IAIMovementBehaviour GetMovementBehaviour()
        {
            return (IAIMovementBehaviour)abilities.GetAbility<IAIMovementBehaviour>();
        }

        public T GetAbility<T>() where T : UnitAbility
        {
            return (T)abilities.GetAbility<T>();
        }

        public Rigidbody2D GetBody()
        {
            return body;
        }

        private void Awake()
        {
            GetUnitComponents();
        }
        
        private void GetUnitComponents()
        {
            anim = GetComponent<Animator>();
            body = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();

            Transform[] childComponents = GetComponentsInChildren<Transform>();
            for (int i = 0; i < childComponents.Length; i++)
            {
                switch (childComponents[i].name)
                {
                    case "AI":
                        AI = childComponents[i];
                        break;
                    case "Model":
                        Model = childComponents[i];
                        break;
                    case "Effects":
                        effects.Initialise(childComponents[i]);
                        break;
                    case "Abilities":
                        abilities.Initialise(childComponents[i]);
                        break;
                    default:
                        break;
                }
            }
        }
        
        public void LookAtPosition(Vector2 position)
        {
            if (transform.position.x > position.x)
            {
                FaceInitialDirection();
            }
            else
            {
                FaceOppositeDirection();
            }
        }

        protected void FaceInitialDirection()
        {
            if (directionFlipped)
            {
                directionFlipped = false;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
            }
        }

        protected void FaceOppositeDirection()
        {
            if (!directionFlipped)
            {
                directionFlipped = true;
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
            }
        }

        public bool DirectionFlipped { get { return directionFlipped; } }
    }
}

