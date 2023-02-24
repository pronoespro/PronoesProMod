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
        public bool repeat;
        public string npcSuperName, npcName, npcSubName;

        public string[] conversation;
        public string[] dialogSounds;
        public string interactionPrompt = "Interact";
        public string[] requirements;

        public string[] dreamNailDialog;
        public string[] dreamNailSounds;

        public float dialogSpeed = 2;
        public UnityEvent onStart, onContinue, onEnd;

        public DialogSettings(LayerMask collisionMask, string[] dialog, string[] sounds, string name = "", string superName = "", string subName = "", float speed = 2f, bool startOnCollision = false, bool nextWhenFinished = true,
            UnityEvent onStartConversation = null, UnityEvent onContinueConversation = null, UnityEvent onEndConversation = null, string inteactionDisplay = "Interact", string[] dialogRequirements = null,string[] dialogDreamNail=null,string[] soundDreamNail=null,bool canRepeat=false)
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

            onStart = (onStartConversation != null) ? onStartConversation : new UnityEvent();
            onContinue = (onContinueConversation != null) ? onContinueConversation : new UnityEvent();
            onEnd = (onEndConversation != null) ? onEndConversation : new UnityEvent();

            interactionPrompt = inteactionDisplay;
            requirements = dialogRequirements;

            dreamNailDialog = dialogDreamNail;
            dreamNailSounds = soundDreamNail;
            repeat = canRepeat;
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
        public List<int> dialogsDone;

        public string sceneName;

        public void Start(){
            dialogsDone = new List<int>();
        }

        public void SetDialogSettings(DialogSettings[] npcDialog)
        {
            dialogs = npcDialog;
            PronoesProMod.Instance.Log("Added dialogs");

            for(int i = 0; i < dialogs.Length; i++){
                if (dialogs[i].nextDialogOnFinish){
                    dialogs[i].onEnd.AddListener(() => AddDialogDone());
                    PronoesProMod.Instance.Log("Added dialog continuing to "+i.ToString());
                }
            }
        }

        public int GetCurrentDialogNum()
        {
            if (dialogs.Length > 0)
            {
                for (int i = 0; i < dialogs.Length; i++)
                {
                    if (dialogs[i].repeat || !dialogsDone.Contains(i)) {
                        if (dialogs[i].requirements != null)
                        {
                            if (RequirementsMet(dialogs[i].requirements))
                            {
                                return i;
                            }
                        }
                        else
                        {
                            return i;
                        }
                    }
                }
                return 0;
            }
            return -1;
        }

        public DialogSettings GetCurrentSettings()
        {
            if (GetCurrentDialogNum() >= 0)
            {
                return dialogs[GetCurrentDialogNum()];
            }
            else
            {
                return null;
            }
        }

        public bool RequirementsMet(string[] requirements)
        {
            for(int i = 0; i < requirements.Length; i++) {

                if (requirements[i].Contains(":"))
                {
                    string[] splitRequirement = requirements[i].Split(':');
                    int requiredNum;

                    if(int.TryParse(splitRequirement[1],out requiredNum)){
                        switch (splitRequirement[0].ToLower())
                        {
                            default:
                                break;
                            case "charm":
                                if (!PronoesProMod.HasCharm(requiredNum))
                                {
                                    return false;
                                }
                                break;
                            case "hp-more":
                                if (PlayerData.instance.GetInt("HP")<requiredNum)
                                {
                                    return false;
                                }
                                break;
                            case "hp-less":
                                if (PlayerData.instance.GetInt("HP")>requiredNum)
                                {
                                    return false;
                                }
                                break;
                            case "variable":
                            case "var":
                                switch (requiredNum){
                                    default:
                                    case 0:
                                        if (PronoesProMod.Instance.IsIntroDone())
                                        {
                                            return false;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            return true;
        }

        public void AddDialogDone()
        {
            if (GetCurrentDialogNum() >= 0 && !dialogsDone.Contains(GetCurrentDialogNum()))
            {
                dialogsDone.Add(GetCurrentDialogNum());
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            DialogSettings dialog = GetCurrentSettings();
            if (dialog != null)
            {
                if (dialog.mask == (dialog.mask | (1 << collider.gameObject.layer)))
                {
                    if (dialog.onCollision)
                    {
                        PronoesProMod.Instance.ShowDialogBox(dialog.npcSuperName, dialog.npcName, dialog.npcSubName, dialog.conversation, dialog.dialogSounds, dialog.dialogSpeed);
                        PronoesProMod.Instance.ChangeDialogEvents(dialog.onStart, dialog.onContinue, dialog.onEnd);
                    }
                    else
                    {
                        if (PronoesProMod.Instance.interactionPropt != null)
                        {
                            PronoesProMod.Instance.StartInteraction(new Vector3(transform.position.x, transform.position.y, PronoesProMod.Instance.interactionPropt.transform.position.z), dialog.interactionPrompt);
                            PronoesProMod.Instance.ChangeDialogEvents(dialog.onStart, dialog.onContinue, dialog.onEnd);
                        }
                    }
                }
                else if(collider.tag== "Dream Attack" && dialog.dreamNailDialog!=null && dialog.dreamNailDialog.Length>0)
                {
                    PronoesProMod.Instance.ShowDialogBox(dialog.npcSuperName, dialog.npcName, dialog.npcSubName, dialog.dreamNailDialog, dialog.dreamNailSounds, dialog.dialogSpeed);
                    PronoesProMod.Instance.ChangeDialogEvents(null,null,null);
                }
            }
        }

        public bool KnightReady()
        {
            return HeroController.instance.CheckTouchingGround() 
                && Mathf.Abs(HeroController.instance.GetComponent<Rigidbody2D>().velocity.y) < 0.1f 
                && HeroController.instance.acceptingInput;
        }

        public void OnTriggerStay2D(Collider2D collider)
        {
            DialogSettings dialog = GetCurrentSettings();
            if (dialog != null && !PronoesProMod.IsMidDialog()){
                if (dialog.mask == (dialog.mask | (1 << collider.gameObject.layer)) && !dialog.onCollision)
                {
                    PronoesProMod.Instance.StartInteraction(new Vector3(transform.position.x,transform.position.y,PronoesProMod.Instance.interactionPropt.transform.position.z), dialog.interactionPrompt);

                    if (InputHandler.Instance.inputActions.up.IsPressed && !PronoesProMod.IsGamePaused() && KnightReady())
                    {
                        PronoesProMod.Instance.ShowDialogBox(dialog.npcSuperName, dialog.npcName, dialog.npcSubName, dialog.conversation, dialog.dialogSounds, dialog.dialogSpeed);
                        PronoesProMod.Instance.ChangeDialogEvents(dialog.onStart, dialog.onContinue, dialog.onEnd);
                    }
                }
            }
        }

        public void OnTriggerExit2D(Collider2D collider){

            DialogSettings dialog = GetCurrentSettings();
            if (dialog != null)
            {
                if (dialog.mask == (dialog.mask | (1 << collider.gameObject.layer)) && !dialog.onCollision)
                {
                    PronoesProMod.Instance.EndInteraction();
                    PronoesProMod.Instance.ChangeDialogEvents(null, null, null);
                }
            }
        }

    }
}
