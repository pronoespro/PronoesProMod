using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class GoAlongWall:MonoBehaviour
    {

        public float normalMoveSpeed=0.1f,fastMoveSpeed=0.5f;
        public Vector2 moveDirection;
        public float rotateDir=90;
        public float rotationOverTime=360*2;

        public float lastCollidedTime;

        public LayerMask mask;

        private CircleCollider2D col;

        public void Start()
        {
            mask = 1000;
            col = GetComponent<CircleCollider2D>();
        }

        public void OnEnable()
        {
            lastCollidedTime = 2000f;
            StartCoroutine(CheckDirection());
        }

        public IEnumerator CheckDirection()
        {
            while (true)
            {
                lastCollidedTime += Time.deltaTime;
                if (lastCollidedTime>0.1f && lastCollidedTime>Time.deltaTime*2)
                {
                    moveDirection = new Vector2(Mathf.Sign(moveDirection.x) * normalMoveSpeed, Mathf.Clamp(moveDirection.y - Time.deltaTime * 9.5f, -10, 10));

                    transform.position += (Vector3)moveDirection * Time.deltaTime;
                    transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationOverTime / 4);
                    yield return null;
                }else{
                    moveDirection = moveDirection.normalized * fastMoveSpeed;
                    transform.position += (Vector3)moveDirection * Time.deltaTime;
                    transform.rotation = transform.rotation * Quaternion.Euler(0, 0, rotationOverTime);
                    yield return null;
                }
            }
        }

        public void OnTriggerStay2D(Collider2D collider)
        {
            if (mask==(mask|( 1 << collider.gameObject.layer)))
            {
                lastCollidedTime = 0;
                Vector3 closest = collider.ClosestPoint(transform.position);
                Vector3 desDir =Vector3.Normalize( transform.position-closest);
                desDir = Quaternion.Euler(0, 0, rotateDir)*desDir;
                moveDirection = desDir;
                transform.position = closest+ (transform.position-closest).normalized*col.radius/2f;
            }
        }

        public void OnDisable()
        {
            StopCoroutine(CheckDirection());
        }

        public void ChangeMoveDir(Vector2 dir)
        {
            moveDirection = dir;
        }

        public void ChangeRotateDir(float rot)
        {
            rotateDir = rot;
        }

    }
}
