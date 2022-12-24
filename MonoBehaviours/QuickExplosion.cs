using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class QuickExplosion:MonoBehaviour
    {

        private float explosionTime = 0.1f;
        private List<DamageHero> damages;

        public IEnumerator Start()
        {
            damages = new List<DamageHero>();
            DamageHero foundRb = GetComponent<DamageHero>();
            if (foundRb != null){
                damages.Add(foundRb);
            }
            foreach(DamageHero rb in GetComponentsInChildren<DamageHero>())
            {
                damages.Add(rb);
            }

            foreach(DamageHero rb in damages){
                rb.enabled=true;
            }
            float timer = 0;
            while (timer < explosionTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            foreach (DamageHero dmg in damages)
            {
                dmg.enabled = false;
            }
        }

    }
}
