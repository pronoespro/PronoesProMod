using System.Collections;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class DeeExplosionAttack:MonoBehaviour
    {

        public float initTime=1f, waitTime=2f;
        public float range=100;

        public Rigidbody2D rb;
        public float flyVel=5;

        public void ChangeMoveDir(Vector2 dir)
        {
            rb.velocity = dir;
        }

        public void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            StartCoroutine(GoToEnemy());
        }

        public void OnDisable()
        {
            StopAllCoroutines();
        }

        public IEnumerator GoToEnemy()
        {
            float timer = 0;

            while(timer< initTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            timer = 0;

            HealthManager chosenEnemy=null;

            while (timer < waitTime) {
                rb.velocity = Vector2.zero;
                foreach (HealthManager hm in FindObjectsOfType<HealthManager>())
                {
                    if (chosenEnemy==null|| Vector2.Distance(hm.transform.position, transform.position)<
                        Vector2.Distance(chosenEnemy.transform.position, transform.position))
                    {
                        chosenEnemy = hm;
                    }
                }
                timer += Time.deltaTime;
                yield return null;
            }

            if (chosenEnemy != null)
            {
                rb.velocity = Vector3.Normalize(chosenEnemy.transform.position - transform.position) * flyVel;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            /*
            if (collider.gameObject.layer == LayerMask.NameToLayer(LayerMask.LayerToName(8)))
            {
                PronoesProMod.Instance.Log("Ouch!");
                gameObject.SetActive(false);
            }*/
        }

    }
}
