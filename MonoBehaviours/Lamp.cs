using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class Lamp:MonoBehaviour
	{
		public string[] MeleeAttacks = new string[] { "Nail Attack" };
		public string breakTrigger = "Break";
		public ParticleSystem particles;

		public GameObject brokenLampGO;
		public int brokenLampAmmount = 2;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			for (int i = 0; i < MeleeAttacks.Length; i++)
			{
				if (collision.tag == MeleeAttacks[i])
				{
					AudioSource source = GetComponent<AudioSource>();
					if (source != null)
					{
						source.Play();
					}
					Animator anim = GetComponent<Animator>();
					if (anim != null)
					{
						anim.SetTrigger(breakTrigger);
					}
					if (particles != null)
					{
						Vector2 point = collision.ClosestPoint(transform.position);
						particles.transform.position = new Vector3(point.x, point.y, particles.transform.position.z);
						particles.Play(true);
					}

					if (PronoesProMod.Instance.GOBundle.ContainsKey("brokenobjs"))
					{
						AssetBundle ab = PronoesProMod.Instance.GOBundle["brokenobjs"];
						GameObject prefav = ab.LoadAsset<GameObject>("LampTop");
						for (int obj = 0; obj < brokenLampAmmount; obj++)
						{
							GameObject go = Instantiate(prefav, new Vector3(transform.position.x,transform.position.y,0), Quaternion.identity,transform);
							go.transform.localScale = new Vector3(go.transform.localScale.x*(obj%2==0?1f:-1f), go.transform.localScale.y, 0);
							go.AddComponent<BouncingObject>();
							Rigidbody2D rb= go.GetComponent<Rigidbody2D>();
                            if (rb != null)
                            {
								rb.AddForce(new Vector2(5 * (obj % 2 == 0 ? 1f : -1f), 2));
								rb.angularVelocity = 360f*(obj%2==0?1f:-1f);
                            }
						}
					}
				}

			}

		}
	}
}
