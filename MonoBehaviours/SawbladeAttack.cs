using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class SawbladeAttack : MonoBehaviour
    {

        public Animator anim;
        public List<DamageEnemies> dmgs;
        public List<SawbladeHitCounter> counters;
        public string attackAnim= "Sawblade_attack";
        public float destroyTime = 1f;

        public float collisionMult = 1f;

        public void Start()
        {
            anim = GetComponent<Animator>();
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            PronoesProMod.Instance.Log("Activated!");
            Invoke("DisableProjectile", destroyTime);

            transform.position = HeroController.instance.transform.position;
            if (anim != null)
            {
                anim.Play(attackAnim);
            }

            if (counters == null)
            {
                counters = new List<SawbladeHitCounter>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    SawbladeHitCounter counter = transform.GetChild(i).gameObject.AddComponent<SawbladeHitCounter>();
                    counters.Add(counter);
                    counter.collisionMult = collisionMult;
                }
            }

            foreach (SawbladeHitCounter counter in counters)
            {
                counter.collisionMult = collisionMult;
                counter.ResetCounter();
            }
            AudioSource s;
            for (int i = 0; i < transform.childCount; i++)
            {
                s=transform.GetChild(i).GetComponent<AudioSource>();
                if (s != null)
                {
                    s.Play();
                }
            }
        }

        public void DisableProjectile()
        {
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            CancelInvoke("DisableProjectile");
        }

        public void Update()
        {
            foreach (DamageEnemies dmg in dmgs)
            {
                dmg.damageDealt = PlayerData.instance.nailDamage;
            }
        }
        public void SetCollisionsMultiplier(float mult)
        {
            collisionMult = mult;
        }

    }
}
