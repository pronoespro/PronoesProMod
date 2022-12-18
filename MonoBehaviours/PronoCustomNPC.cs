using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PronoesProMod.MonoBehaviours
{
    #region Classes
    [System.Serializable]
    public class DialogSettings
    {
        public LayerMask mask = 10000000;
        public bool onCollision;
        public bool nextDialogOnFinish;
        public string npcSuperName, npcName, npcSubName;

        public string[] conversation;
        public string[] dialogSounds;
        public string interactionPrompt = "Interact";

        public float dialogSpeed = 2;
        public UnityEvent onStart, onContinue, onEnd;

        public DialogSettings(LayerMask collisionMask,  string[] dialog,string[] sounds, string name="", string superName="", string subName="", float speed=2f, bool startOnCollision=false,bool nextWhenFinished=true,
            UnityEvent onStartConversation=null, UnityEvent onContinueConversation=null, UnityEvent onEndConversation=null,string inteactionDisplay="Interact")
        {
            mask = collisionMask;

            onCollision = startOnCollision;
            nextDialogOnFinish = nextWhenFinished;
            npcName = name;
            npcSubName = subName;
            npcSuperName = superName;
            conversation = dialog;
            dialogSounds = sounds;

            dialogSpeed = speed;

            onStart = (onStartConversation!=null)?onStartConversation:new UnityEvent();
            onContinue = (onContinueConversation != null) ? onContinueConversation : new UnityEvent();
            onEnd = (onEndConversation != null) ? onEndConversation : new UnityEvent();

            interactionPrompt = inteactionDisplay;

        }

        public static LayerMask GetDefaultMask()
        {
            return 10000000;
        }

    }
#endregion

    public class PronoCustomNPC:MonoBehaviour
    {

        public DialogSettings[] dialogs;

        public string sceneName;

        public void SetDialogSettings(DialogSettings[] npcDialog)
        {
            dialogs = npcDialog;
        }

        public DialogSettings GetCurrentSettings()
        { 
            if (dialogs.Length > 0)
            {
                return dialogs[0];
            }
            return null;
        }

        public void NextDialog()
        {
            if (dialogs.Length > 1)
            {
                DialogSettings[] newSettings = new DialogSettings[dialogs.Length - 1];
                for (int i = 0; i < newSettings.Length; i++)
                {
                    newSettings[i] = dialogs[i+1];
                }
                dialogs = newSettings;
            }else{
                dialogs = new DialogSettings[0];
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            DialogSettings dialog = GetCurrentSettings();
            if (dialog != null)
            {
                if (dialog.onCollision)
                {
                    if (dialog.mask == (dialog.mask | (1 << collider.gameObject.layer)))
                    {
                        PronoesProMod.Instance.ShowDialogBox(dialog.npcSuperName, dialog.npcName, dialog.npcSubName, dialog.conversation, dialog.dialogSounds, dialog.dialogSpeed);
                        PronoesProMod.Instance.ChangeDialogEvents(dialog.onStart, dialog.onContinue, dialog.onEnd);
                    }
                }else{
                    if (SceneLoader.interactionPrompt != null)
                    {
                        SceneLoader.interactionPrompt.position = new Vector3(transform.position.x, transform.position.y, SceneLoader.interactionPrompt.position.z);

                        Animator anim = SceneLoader.interactionPrompt.GetComponentInChildren<Animator>();
                        if (anim != null)
                        {
                            anim.SetBool("Appear", true);
                            PronoesProMod.Instance.Log("Interaction ready!");
                        }
                    }
                }
            }
        }

        public void OnTriggerStay2D(Collider2D collider)
        {
            DialogSettings dialog = GetCurrentSettings();
            if (dialog != null)
            {
                if (dialog.mask == (dialog.mask | (1 << collider.gameObject.layer)) && !dialog.onCollision)
                {
                    PronoesProMod.Instance.StartInteraction(transform.position, dialog.interactionPrompt);

                    if (InputHandler.Instance.inputActions.up.IsPressed)
                    {
                        PronoesProMod.Instance.ShowDialogBox(dialog.npcSuperName, dialog.npcName, dialog.npcSubName, dialog.conversation, dialog.dialogSounds, dialog.dialogSpeed);
                        PronoesProMod.Instance.ChangeDialogEvents(dialog.onStart, dialog.onContinue, dialog.onEnd);
                    }
                }
            }
        }

        public void OnTriggerExit2D(Collider2D collider){

            if (collider.transform == HeroController.instance.transform)
            {
                Animator anim = SceneLoader.interactionPrompt.GetComponentInChildren<Animator>();
                if (anim != null)
                {
                    anim.SetBool("Appear", false);
                    PronoesProMod.Instance.Log("Interaction ended!");
                }
            }
        }

    }
}
