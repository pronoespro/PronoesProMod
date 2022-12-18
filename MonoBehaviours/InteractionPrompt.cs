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

        public void Start()
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                anim = GetComponentInChildren<Animator>();
            }
        }

        public void SetInteractionPrompt(string prompt)
        {
            Text txt = GetComponentInChildren<Text>();
            if (txt != null)
            {
                txt.text = prompt;
            }
        }

        public void StartInteractable(Vector2 pos)
        {
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);

            canInteract = true;
        }

        public void EndInteractable()
        {
            canInteract = false;
        }

        public void Update()
        {
            if (anim != null)
            {
                Transform box = PronoesProMod.levelNameDisplay.transform.Find("DialogBox");
                box.gameObject.SetActive(true);

                DialogBox dialBox = box.GetComponent<DialogBox>();

                if (dialBox != null)
                {
                    anim.SetBool("Appear", canInteract && !dialBox.IsMidDialog);
                }
                else
                {
                    anim.SetBool("Appear", canInteract);
                }
            }
        }

    }
}
