using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class SawbladeHitCounter:MonoBehaviour
    {

        public int collisionAmmount;
        public int maxCollisions = 5;

        public float collisionMult = 1f;

        public void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.gameObject.GetComponent<HealthManager>() != null || collider.gameObject.GetComponentInChildren<HealthManager>()!=null || collider.GetComponentInParent<HealthManager>()!=null)
            {
                PronoesProMod.Instance.Log("Collided with enemy");
                collisionAmmount++;
                if (collisionAmmount > maxCollisions * collisionMult)
                {
                    transform.parent.gameObject.SetActive(false);
                }
            }
        }

        public void ResetCounter()
        {
            collisionAmmount = 0;
        }

        public void ModifyMultiplier(float mult)
        {
            collisionMult =Mathf.Max(0, mult);
        }

    }
}
