using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PronoesProMod.MonoBehaviours
{
    public class DeeShield:MonoBehaviour
    {

        public float inputThreshold = 0.2f;
        public float rotationSpeed = 0.5f;

        public float scaleSpeed = 0.35f;

        public float shieldJumpTime = 0.05f;
        public float slashTimer = 2f;
        public float shieldScaleMult = 1f;

        private float curSlashTimer;
        private Rigidbody2D rb;
        private Transform projectileBlocker;

        private Coroutine jumpRoutine;
        private AudioSource attackSfx, bounceSfx;
        private Animator anim;

        public void Start()
        {
            anim = GetComponent<Animator>();

            attackSfx = transform.GetChild(2).GetComponent<AudioSource>();
            bounceSfx = transform.GetChild(3).GetComponent<AudioSource>();

            bounceSfx.outputAudioMixerGroup = PronoesProMod.actorsMixer.outputAudioMixerGroup;
            attackSfx.outputAudioMixerGroup = PronoesProMod.actorsMixer.outputAudioMixerGroup;
        }

        public void OnEnable()
        {
            transform.localScale = shieldScaleMult*Vector3.one;
            if (projectileBlocker == null){
                projectileBlocker = transform.GetChild(1);
                rb = GetComponent<Rigidbody2D>();
            }
        }

        public void Update()
        {
            if (!PlayerData.instance.GetBool("equippedCharm_38"))
            {
                gameObject.SetActive(false);
                return;
            }


            if (curSlashTimer >= 0)
            {
                curSlashTimer -= Time.deltaTime;
                transform.localScale = shieldScaleMult* Vector3.Lerp(transform.localScale, new Vector3(0.75f, 0.75f, 0.75f), scaleSpeed * Time.deltaTime * 100);

                if (projectileBlocker.gameObject.activeInHierarchy)
                {
                    projectileBlocker.gameObject.SetActive(false);
                    rb.simulated = false;
                }

            }
            else
            {
                if (!projectileBlocker.gameObject.activeInHierarchy)
                {
                    projectileBlocker.gameObject.SetActive(true);
                    rb.simulated = true;
                }

                transform.localScale = shieldScaleMult*Vector3.Lerp(transform.localScale, new Vector3(1.35f, 1.35f, 1.35f), scaleSpeed * Time.deltaTime * 100);
            }

            transform.position = HeroController.instance.transform.position + new Vector3(0, -0.25f);

            if (HeroController.instance.vertical_input > inputThreshold)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, rotationSpeed * Time.deltaTime * 100);
            }
            else if (HeroController.instance.vertical_input < -inputThreshold)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.root.eulerAngles.x, transform.rotation.eulerAngles.y, 180), rotationSpeed * Time.deltaTime * 100);
            }
            else if (HeroController.instance.move_input > inputThreshold)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.root.eulerAngles.x, transform.rotation.eulerAngles.y, 270), rotationSpeed * Time.deltaTime * 100);
            }
            else if (HeroController.instance.move_input < -inputThreshold)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.root.eulerAngles.x, transform.rotation.eulerAngles.y, 90), rotationSpeed * Time.deltaTime * 100);
            }
        }

        public void GetShieldScale()
        {
            shieldScaleMult = 1f;

            if (PlayerData.instance.GetBool("equippedCharm_30"))
            {
                shieldScaleMult *= 1.2f;
            }
            if (PlayerData.instance.GetBool("equippedCharm_5"))
            {
                shieldScaleMult *= 1.1f;
            }

        }
        
        public float GetShieldTimerMultiplier()
        {
            float mult = 1f;

            if (PlayerData.instance.GetBool("equippedCharm_19"))
            {
                mult *= 0.9f;
            }
            if (PlayerData.instance.GetBool("equippedCharm_4"))
            {
                mult *= 0.9f;
            }
            if (PlayerData.instance.GetBool("equippedCharm_14"))
            {
                mult *= 0.5f;
            }
            if (PlayerData.instance.GetBool("equippedCharm_32"))
            {
                mult *= 0.5f;
            }
            if (PlayerData.instance.GetBool("equippedCharm_40"))
            {
                mult *= 0.5f;
            }

            return mult;
        }

        public void Slash()
        {
            if (curSlashTimer <= 0)
            {
                if (anim != null){
                    anim.SetTrigger("Hit");
                }

                curSlashTimer = slashTimer * GetShieldTimerMultiplier();

                transform.localScale = shieldScaleMult * new Vector3(2f, 2f, 2f);
                bool bounce = false;

                if (Mathf.Abs(HeroController.instance.GetComponent<Rigidbody2D>().velocity.y) < HeroController.instance.DEFAULT_GRAVITY * Time.deltaTime / 2f)
                {
                    PronoesProMod.Instance.Log("On ground");
                    if (HeroController.instance.vertical_input < -inputThreshold)
                    {
                        bounce = true;
                        HeroController.instance.ShroomBounce();

                        if (jumpRoutine != null)
                        {
                            StopCoroutine(jumpRoutine);
                        }

                        jumpRoutine = StartCoroutine(ShieldJump());
                    }
                }
                else
                {
                    PronoesProMod.Instance.Log("Midair");
                    if (Mathf.Abs(HeroController.instance.move_input) > inputThreshold && Mathf.Abs(HeroController.instance.vertical_input) < inputThreshold)
                    {
                        HeroController.instance.transform.position += Mathf.Sign(HeroController.instance.move_input) * -1.25f * Vector3.right;
                    }
                }
                if (bounce){
                    bounceSfx.Play();
                }else{
                    attackSfx.Play();
                    PronoesProMod.Instance.CreateDeeShieldProjectile(transform.position + transform.up * 2, transform.rotation);
                }
            }
        }

        public IEnumerator ShieldJump()
        {
            float timer = shieldJumpTime;
            Rigidbody2D rb = HeroController.instance.GetComponent<Rigidbody2D>();
            while (timer > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, HeroController.instance.JUMP_SPEED*2);
                timer -= Time.deltaTime;

                yield return null;
            }
        }
    }
}
