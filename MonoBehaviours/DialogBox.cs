using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PronoesProMod.MonoBehaviours
{
    public class DialogBox:MonoBehaviour
    {

        public Text nameTxt,nameSubTxt,nameSuperTxt,dialogTxt;
        public List<string> curConversation;
        public Animator anim;
        public string[] possibleDialogSounds;
        public string sceneName="sounds";

        public UnityEvent onConversationStart=new UnityEvent(), onConversationContinue=new UnityEvent(), onConversationEnd=new UnityEvent();

        string curText;
        bool continueConv;
        bool midDialog;
        AudioSource source;

        public void Start()
        {
            source = GetComponent<AudioSource>();
            if (source == null)
            {
                source = gameObject.AddComponent<AudioSource>();
            }
            source.playOnAwake = false;
            source.outputAudioMixerGroup = PronoesProMod.actorsMixer.outputAudioMixerGroup;
        }

        public bool IsMidDialog{
            get { return midDialog; }
        }

        public void ContinueConversation()
        {
            if (midDialog)
            {
                continueConv = true;
            }
        }

        public void SetDialog(string[] nameData, string[] dialog,string[] dialogSounds,float speed=1)
        {
            if (!midDialog)
            {
                if (nameData.Length > 0)
                {
                    nameTxt.text =(nameData.Length>0 && nameData[0].Length>0)?LanguageData.englishSentences[nameData[0]]:"";
                    nameSubTxt.text = (nameData.Length > 1 && nameData[1].Length>0) ? LanguageData.englishSentences[nameData[1]]:"";
                    nameSuperTxt.text = (nameData.Length > 2 && nameData[2].Length>0) ? LanguageData.englishSentences[nameData[2]]:"";
                }

                possibleDialogSounds = dialogSounds;

                curConversation = StringArrayToList(dialog);

                StartCoroutine(ShowDialog(speed));
            }
        }

        public List<string> StringArrayToList(string[] array)
        {
            List<string> stringList = new List<string>();
            for(int i = 0; i < array.Length; i++)
            {
                stringList.Add(array[i]);
            }
            return stringList;
        }

        public IEnumerator ShowDialog(float conversationSpeed= 1f)
        {
            PronoesProMod.Instance.upgradedCharms[0] = true;
            HeroController.instance.SendMessage("RelinquishControl");
            HeroController.instance.SendMessage("StopAnimationControl");

            HeroController.instance.CancelSuperDash();
            Rigidbody2D rb = HeroController.instance.GetComponent<Rigidbody2D>();
            rb.velocity = Vector2.zero;


            while (((int)HeroController.instance.hero_state) >= 3)
            {
                rb.velocity = new Vector2(0,rb.velocity.y);
                HeroController.instance.SendMessage("RelinquishControl");
                HeroController.instance.SendMessage("StopAnimationControl");
                yield return null;
            }
            HeroController.instance.superDash.SendEvent("FSM CANCEL");
            HeroController.instance.SendMessage("RelinquishControl");
            HeroController.instance.SendMessage("StopAnimationControl");

            tk2dSprite sprite = HeroController.instance.GetComponent<tk2dSprite>();
            Texture s = sprite.GetCurrentSpriteDef().material.mainTexture,skin=null;

            if (PronoesProMod.Instance.skinsBundle.ContainsKey("skins"))
            {
                 skin=PronoesProMod.Instance.skinsBundle["skins"].LoadAsset<Texture>("Gen-Knight");
            }
            sprite.GetCurrentSpriteDef().material.mainTexture = skin;
            

            HeroController.instance.GetComponent<tk2dSpriteAnimator>().Play("Idle");

            onConversationStart.Invoke();

            PlayerData.instance.SetBool("disablePause", true);
            GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("OUT");
            midDialog = true;

            anim.SetBool("ShowCursor", false);
            anim.SetBool("ShowDialog", true);

            yield return new WaitForSeconds(0.5f);

            int soundNum = 0;
            while (curConversation.Count > 0)
            {
                continueConv = false;
                if (LanguageData.englishSentences.ContainsKey(curConversation[0]))
                {
                    //Dialog sounds
                    if (possibleDialogSounds.Length > 0 && soundNum<possibleDialogSounds.Length)
                    {
                        if (PronoesProMod.Instance.soundBundle.ContainsKey(sceneName) && PronoesProMod.Instance.soundBundle[sceneName].Contains(possibleDialogSounds[soundNum]))
                        {
                            AudioClip clip = PronoesProMod.Instance.soundBundle[sceneName].LoadAsset<AudioClip>(possibleDialogSounds[soundNum]);
                            source.clip = clip;
                            source.Play();
                            soundNum++;
                        }
                    }


                    curText = LanguageData.englishSentences[curConversation[0]];
                    curConversation.RemoveAt(0);

                    dialogTxt.text = "";

                    while (curText.Length > 0)
                    {
                        continueConv = false;

                        dialogTxt.text += curText[0];
                        curText= curText.Remove(0, 1);

                        yield return new WaitForSeconds(0.1f/ conversationSpeed);
                        if (continueConv)
                        {
                            dialogTxt.text += curText;
                            curText = "";
                            continueConv = false;
                        }
                    }

                    anim.SetBool("ShowCursor", true);

                    yield return new WaitForSeconds(0.75f);

                    while (!continueConv)
                    {
                        yield return null;
                    }
                    if (curConversation.Count > 0)
                    {
                        onConversationContinue.Invoke();
                    }
                }
                else
                {
                    curConversation.RemoveAt(0);
                }

                anim.SetBool("ShowCursor", false);
            }

            onConversationEnd.Invoke();

            dialogTxt.text = "";
            nameTxt.text = "";
            nameSubTxt.text = "";
            nameSuperTxt.text = "";

            anim.SetBool("ShowDialog", false);
            
            yield return new WaitForSeconds(0.25f);

            midDialog = false;

            GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");

            HeroController.instance.SendMessage("RegainControl");
            HeroController.instance.SendMessage("StartAnimationControl");
            
            PlayerData.instance.SetBool("disablePause", false);
            gameObject.SetActive(false);
            //sprite.GetCurrentSpriteDef().material.mainTexture=s;
        }

    }
}
