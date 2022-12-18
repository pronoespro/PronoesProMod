using UnityEngine;

namespace PronoesProMod
{
    public class BouncingObject : MonoBehaviour
	{

		public string bounceAnimation = "Bounce";
		public ParticleSystem particles;

		public float desDepth;

		public void Start()
        {
            if (desDepth == 0)
            {
				desDepth = transform.position.z;
            }
        }

		private void OnTriggerEnter2D(Collider2D collision)
		{
			for(int i = 0; i < PronoesProMod.MeleeAttacks.Length; i++)
            {
                if (collision.tag == PronoesProMod.MeleeAttacks[i])
				{
					transform.position =new Vector3(transform.position.x,transform.position.y, desDepth);
					Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
						if (collision.name.ToLower().Contains("up")){
							rb.velocity = new Vector2(0, 30);
						}else if (collision.name.ToLower().Contains("down")){
							rb.velocity = new Vector2(0, -30);
						}else{
							rb.velocity = new Vector2(20 * (PronoesProMod.Instance.playerFaceLeft?-1:1), 20);
						}
                    }
					AudioSource source = GetComponent<AudioSource>();
                    if (source != null)
                    {
						source.Play();
                    }
					Animator anim = GetComponent<Animator>();
                    if (anim != null)
                    {
						anim.Play(bounceAnimation);
                    }
                    if (particles != null)
                    {
						Vector2 point= collision.ClosestPoint(transform.position);
						particles.transform.position =new Vector3(point.x, point.y,particles.transform.position.z);
						particles.Play(true);
                    }
				}
            }

		}
		public void OverrideDepth(float depth)
		{
			desDepth = depth;
		}

	}
}
