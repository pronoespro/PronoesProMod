using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class DeeTransportation:MonoBehaviour
    {

        public int curPoint = 0;
        public GameObject[] points;
        public float hitChance = 5f;

        private float initXscale;

        public void Start()
        {
            initXscale = transform.localScale.x;
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            for (int i = 0; i < PronoesProMod.MeleeAttacks.Length; i++)
            {
                if (col.tag == PronoesProMod.MeleeAttacks[i])
                {
                    curPoint = (curPoint + 1) % points.Length;

                    transform.localScale = new Vector3(initXscale*(transform.position.x>points[curPoint].transform.position.x?-1:1),transform.localScale.y,transform.localScale.z);

                    transform.position = points[curPoint].transform.position;
                    HeroController.instance.transform.position = transform.position;
                    HeroController.instance.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x*250, 0);

                    float hit = Random.Range(0, hitChance);
                    if (hit < 1f)
                    {
                        HeroController.instance.TakeDamage(gameObject, GlobalEnums.CollisionSide.bottom, 1, 0);
                    }

                }
            }
        }

    }
}
