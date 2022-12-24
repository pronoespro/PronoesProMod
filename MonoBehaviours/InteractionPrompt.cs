using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace PronoesProMod.MonoBehaviours
{
    public class InteractionPrompt:MonoBehaviour
    {

        public bool canInteract;
        public Animator anim;

        private Transform box;
        private DialogBox dialog;
        private Vector3 finalPos;
        Canvas canv;

        public void Start()
        {
            /*
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
            */
            anim = transform.GetChild(0).GetChild(0).GetComponent<Animator>();

            box = PronoesProMod.levelNameDisplay.transform.Find("DialogBox");
            if (box != null)
            {
                box.gameObject.SetActive(true);

                dialog= box.GetComponent<DialogBox>();
                box.gameObject.SetActive(false);
            }
            canv = GetComponentInChildren<Canvas>();
        }

        public void StartInteractable(Vector2 pos)
        {
            finalPos =pos;

            canInteract = true;
        }

        public void SetInteractionPrompt(string prompt)
        {
            Text txt = GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = prompt;
            }
        }

        public void EndInteractable()
        {
            canInteract = false;
        }

        public void Update()
        {
            RectTransform trans = transform.GetChild(0).GetComponent<RectTransform>();
            Vector2 screenPoint = Camera.main.WorldToViewportPoint(finalPos);
            screenPoint = (screenPoint * trans.sizeDelta - trans.pivot * trans.sizeDelta);
            screenPoint.x *= 6f;
            screenPoint.y *=3.25f;
            transform.GetChild(0).GetChild(0).GetComponent<RectTransform>().anchoredPosition = screenPoint;

            if (anim != null)
            {
                if (box == null)
                {
                    box = PronoesProMod.levelNameDisplay.transform.Find("DialogBox");

                    if (box != null)
                    {
                        box.gameObject.SetActive(true);

                        dialog = box.GetComponent<DialogBox>();
                        box.gameObject.SetActive(false);
                    }
                }
                anim.SetBool("Appear", canInteract);
            }
            else
            {
                PronoesProMod.Instance.Log("No animator");
            }
        }

    }
}
