using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class FallingNailAttack:MonoBehaviour
    {

        public float deactivateTime;
        private Animator anim;
        private bool disabling;

        public void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void OnEnable(Collider2D collider)
        {
            disabling = false;
            if (anim != null)
            {
                anim.SetTrigger("Reset");
            }
        }

        public void Update()
        {
            transform.position = HeroController.instance.transform.position;
            if (!PronoesProMod.Instance.playerFalling && !disabling)
            {
                anim.SetTrigger("EndFall");
                disabling = true;
                Invoke("Deactivate",1f);
            }
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            CancelInvoke("Deactivate");
            disabling = false;
        }

    }
}
