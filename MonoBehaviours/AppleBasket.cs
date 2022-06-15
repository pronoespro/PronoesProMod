using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class AppleBasket : MonoBehaviour
    {

        public string appleName="apple";
        public int applesToReward = 3;
        public int orbReward;

        public AudioSource audio;
        public ParticleSystem correctParticles;

        public void Start() {
            audio = GetComponent<AudioSource>();
            orbReward = applesToReward * 2;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name.ToLower().StartsWith(appleName))
            {
                PronoesProMod.Instance.Log("An apple a day keeps the Radiance away!");
                applesToReward--;

                Destroy(collision.gameObject);

                if (correctParticles != null)
                {
                    correctParticles.transform.position = transform.position;
                    correctParticles.Play();
                }

                if (audio != null)
                {
                    audio.Play();
                }

                if (applesToReward <= 0)
                {
                    PronoesProMod.Instance.Log("Great job!");
                    PlayerData.instance.dreamOrbs += orbReward;
                    EventRegister.SendEvent("DREAM ORB COLLECT");
                }
            }
        }
    }
}
