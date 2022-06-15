using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class PronoCustomNPC:MonoBehaviour
    {

        public LayerMask mask;

        public string npcSuperName, npcName, npcSubName;
        public string[] conversation;
        public float dialogSpeed = 2;

        public void OnTriggerEnter2D(Collider2D collider)
        {
            mask = 10000000;
            if (mask == (mask | (1 << collider.gameObject.layer)))
            {
                PronoesProMod.Instance.ShowDialogBox(npcSuperName, npcName, npcSubName, conversation, dialogSpeed);
            }
        }

    }
}
