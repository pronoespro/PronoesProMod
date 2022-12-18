using System.Collections;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class DeeExplosionAttack:MonoBehaviour
    {

        public float initTime=0.2f, waitTime=1f,disappearTimer=1f;
        public float range=100;

        public Rigidbody2D rb;
        private float flyVel=35f;
        private const float speedUpSpeed = 0.025f;
        private const float slowDownSpeed = 0.075f;
        private bool dissapearOnCollision = false;

        public void Start()
        {
            transform.parent.gameObject.SetActive(false);
        }

        public void ChangeMoveDir(Vector2 dir)
        {
            rb.velocity = dir;
        }

        public void OnEnable()
        {
            transform.localPosition = Vector3.zero;
            StartCoroutine(GoToEnemy());
        }

        public void ResetAttack()
        {
            StopAllCoroutines();
            transform.localPosition = Vector3.zero;
            StartCoroutine(GoToEnemy());
        }

        public void OnDisable()
        {
            StopAllCoroutines();
            transform.parent.gameObject.SetActive(false);
        }

        public IEnumerator GoToEnemy()
        {
            dissapearOnCollision = false;
            float timer = 0;

            PronoesProMod.Instance.Log("Init phase");
            while(timer< initTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            PronoesProMod.Instance.Log("Wait phase");
            timer = 0;
            while (timer < waitTime) {
                rb.velocity /= 1+ Mathf.Min(1,Mathf.Max(0,1-Time.deltaTime))*slowDownSpeed;
                timer += Time.deltaTime;
                yield return null;
            }
            dissapearOnCollision = true;

            HealthManager chosenEnemy = null;

            foreach (HealthManager hm in FindObjectsOfType<HealthManager>())
            {
                if (chosenEnemy == null || Vector2.Distance(hm.transform.position, transform.position) <
                    Vector2.Distance(chosenEnemy.transform.position, transform.position))
                {
                    PronoesProMod.Instance.Log("Chose enemy " + hm.name);
                    chosenEnemy = hm;
                }
            }

            PronoesProMod.Instance.Log("Disappear phase");
            timer = 0;
            while (timer < disappearTimer)
            {

                if (chosenEnemy != null){
                    Vector2 dir = chosenEnemy.transform.position - transform.position;
                    rb.velocity = Vector3.Normalize(dir) *flyVel;
                }else{
                    rb.velocity =Vector2.Lerp(rb.velocity, new Vector2(flyVel * Mathf.Sign(transform.lossyScale.x), -0.5f),Time.deltaTime/speedUpSpeed);
                }
                timer += Time.deltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (dissapearOnCollision && collider.gameObject.layer == LayerMask.NameToLayer(LayerMask.LayerToName(8)))
            {
                PronoesProMod.Instance.Log("Ouch!");
                gameObject.SetActive(false);
            }
            else if(collider.GetComponent<HealthManager>()!=null)
            {
                PronoesProMod.Instance.Log("Death to you!");
                gameObject.SetActive(false);
            }
        }

    }
}
