using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PronoesProMod.MonoBehaviours
{
    public class DeeShield_ProjectileCollision : MonoBehaviour
    {

        public Transform parent;
        private int collisionAmmount=1;

        private int collidedTimes = 0;

        public void Restart()
        {
            collidedTimes = 0;
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            if (col.gameObject.layer==11)
            {
                collidedTimes++;
                if (collidedTimes >= collisionAmmount)
                {
                    parent.gameObject.SetActive(false);
                }
            }
        }

    }
}
