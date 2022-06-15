using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using Modding;
using PronoesProMod.Extensions;
using SFCore.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Satchel;
using Satchel.BetterMenus;

namespace PronoesProMod.MonoBehaviours
{
    public class PronoNPC:MonoBehaviour
    {

        public Animator anim;
        public bool talking;

        public void Awake()
        {
            cardPrefav = PronoesProMod.Instance.preloadedObjs["Cliffs_01"]["Cornifer Card"];
            CreateCustomDialogManager();

            AddCustomDialogue(customDialogManger);
            gameObject.GetAddCustomArrowPrompt(() => {
                customDialogManger.ShowDialogue("PronoesproText1");
                anim.Play("prono_talk");
                talking = true;
            });
            CustomArrowPrompt.Prepare(cardPrefav);
        }

        public void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        public void Update()
        {
            if (!talking)
            {
                float dir= HeroController.instance.transform.position.x- transform.position.x;
                if (Vector3.Distance(HeroController.instance.transform.position, transform.position) > 3 && Mathf.Sign(dir)!=Mathf.Sign(transform.localScale.x))
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    anim.Play("prono_turn");
                }
            }
        }

        public CustomDialogueManager customDialogManger;
        public GameObject cardPrefav;

        public void AddCustomDialogue(CustomDialogueManager cdm)
        {
            cdm.AddConversation("PronoesproText1", "Welcome, ghost of Hallownest. This is the created world, a place made by Dee and I.<page>We want to aid you in your journey, which is why you are the first to make the treck here in a long time.");
            
            cdm.OnEndConversation((string conversation)=>
            {
                if(conversation== "PronoesproText1")
                {
                    anim.Play("prono_idle");
                    talking = false;
                }
            });
        }

        public void CreateCustomDialogManager()
        {
            if (customDialogManger == null)
            {
                customDialogManger =PronoesProMod.satchel.GetCustomDialogueManager(cardPrefav);
            }
        }

    }
}
