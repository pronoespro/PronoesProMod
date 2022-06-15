using System.Collections;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class AppleAttack:MonoBehaviour
    {

        public GameObject targetGO;

        public void OnEnable()
        {
            transform.position = HeroController.instance.transform.position;
        }

        public void OnTriggerEnter2D(Collider2D colision)
        {
            if (colision.GetComponent<HealthManager>() != null)
            {
                StartCoroutine(DeactivateAfter(0.1f));
            }
        }

        IEnumerator DeactivateAfter(float time)
        {
            float timer = 0;
            while (timer < time)
            {
                yield return null;
            }
            if (targetGO != null)
            {
                targetGO.SetActive(false);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

    }
}
