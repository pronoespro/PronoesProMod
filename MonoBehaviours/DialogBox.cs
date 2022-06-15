using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PronoesProMod.MonoBehaviours
{
    public class DialogBox:MonoBehaviour
    {

        public Text nameTxt,nameSubTxt,nameSuperTxt,dialogTxt;
        public List<string> curConversation;
        public Animator anim;

        string curText;
        bool continueConv;
        bool midDialog;

        public void ContinueConversation()
        {
            if (midDialog)
            {
                continueConv = true;
            }
        }

        public void SetDialog(string[] nameData, string[] dialog,float speed=1)
        {
            if (!midDialog)
            {
                if (nameData.Length > 0)
                {
                    nameTxt.text = nameData[0];
                    nameSubTxt.text = nameData[1];
                    nameSuperTxt.text = nameData[2];
                }

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

            HeroController.instance.SendMessage("RelinquishControl");
            HeroController.instance.SendMessage("StopAnimationControl");
            PlayerData.instance.SetBool("disablePause", true);
            GameCameras.instance.hudCanvas.SendMessage("OUT");
            midDialog = true;

            anim.SetBool("ShowCursor", false);
            anim.SetBool("ShowDialog", true);

            yield return new WaitForSeconds(0.5f);

            while (curConversation.Count > 0)
            {
                continueConv = false;
                if (LanguageData.englishSentences.ContainsKey(curConversation[0]))
                {
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
                }
                else
                {
                    curConversation.RemoveAt(0);
                }

                anim.SetBool("ShowCursor", false);
            }

            dialogTxt.text = "";
            nameTxt.text = "";
            nameSubTxt.text = "";
            nameSuperTxt.text = "";

            anim.SetBool("ShowDialog", false);
            
            yield return new WaitForSeconds(0.25f);

            midDialog = false;

            GameCameras.instance.hudCanvas.SendMessage("IN");
            HeroController.instance.SendMessage("RegainControl");
            HeroController.instance.SendMessage("StartAnimationControl");
            
            PlayerData.instance.SetBool("disablePause", false);
            gameObject.SetActive(false);
        }

    }
}
