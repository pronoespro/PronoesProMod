using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class AnimatedWalkByObject : MonoBehaviour
    {

        public LayerMask mask;
        public Animator anim;
        public Vector2 boxSize, offset;
        public bool needsDash, noDirection;

        private void Start()
        {
            if (anim == null)
            {
                anim = gameObject.GetComponent<Animator>();
            }
            mask = LayerMask.NameToLayer(LayerMask.LayerToName(9));
        }

        public void Update()
        {
            Rigidbody2D _rb=HeroController.instance.GetComponent<Rigidbody2D>();
            if (CheckCollision()){
                if (noDirection){
                    if (Mathf.Abs(_rb.velocity.x) >= (needsDash ? 15 : 0.1f))
                    {
                        anim.Play("Interact");
                    }
                }else{
                    if (_rb.velocity.x >= ( needsDash?15:0.1f))
                    {
                        anim.Play("Interact_r");
                    }
                    if (_rb.velocity.x <= -(needsDash ? 15 : 0.1f))
                    {
                        anim.Play("Interact_l");
                    }
                }
            }
        }

        public bool CheckCollision()
        {
            Vector3 _pos= HeroController.instance.transform.position;

            return Mathf.Abs(_pos.x - (transform.position.x + offset.x)) < boxSize.x
                && Mathf.Abs(_pos.y - (transform.position.y + offset.y)) < boxSize.y;

        }

    }
}