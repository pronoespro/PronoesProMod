using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PronoesProMod.MonoBehaviours
{
    public class DeeShield_Projectile:MonoBehaviour
    {

        public float timeLeft;
        public float speed = 75;
        public Collider2D[] colliders;
        public DeeShield_ProjectileCollision[] collisions;

        private static float initTimer = 0.15f;

        public void OnEnable()
        {
            timeLeft = initTimer*GetProjectileDestroyTimer();
            foreach(DeeShield_ProjectileCollision col in collisions)
            {
                col.Restart();
            }
        }

        public float GetProjectileDestroyTimer()
        {
            float timer = 1f;

            if (PlayerData.instance.GetBool("equippedCharm_15"))
            {
                timer *= 0.75f;
            }

            return timer;
        }

        public void Update()
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
            {
                gameObject.SetActive(false);
            }

            transform.localScale = CalculateProjectileScale() * Vector3.Lerp(Vector3.one, Vector3.one * 3f, timeLeft / initTimer);

            transform.position += Time.deltaTime*speed*CalculateProjectileSpeed()*transform.up;

            LayerMask mask =LayerMask.NameToLayer("Enemies")+LayerMask.NameToLayer("Projectiles");

            for(int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].IsTouchingLayers(mask))
                {
                    gameObject.SetActive(false);
                    break; 
                }
            }
        }

        public float CalculateProjectileSpeed()
        {
            float speed = 1f;

            if (PlayerData.instance.GetBool("equippedCharm_18"))
            {
                speed *= 1.15f;
            }

            if (PlayerData.instance.GetBool("equippedCharm_18"))
            {
                speed *= 1.25f;
            }
            return speed;
        }

        public float CalculateProjectileScale()
        {
            float projectileScale = 1f;
            if (PlayerData.instance.GetBool("equippedCharm_35"))
            {
                projectileScale *= 1.2f;
            }
            if (PlayerData.instance.GetBool("equippedCharm_25"))
            {
                projectileScale *= 1.1f;
            }
            return projectileScale;
        }

    }
}
