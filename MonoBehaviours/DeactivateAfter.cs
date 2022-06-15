using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class DeactivateAfter:MonoBehaviour
    {

        public float timer = 0f;

        public void OnEnable()
        {
            Invoke("Deactivate", timer);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

    }
}
