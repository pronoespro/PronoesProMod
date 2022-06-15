﻿using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using InControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using System.Collections;
using System.IO;
using FrogCore.Ext;
using Logger = Modding.Logger;
using FrogCore.Fsm;

namespace PronoesProMod.MonoBehaviours
{
    /// <summary>
    /// Allows for easily adding a npc. Add this monobehavior to a clone of the   "_NPCs\Zote Final Scene\Zote Final"   object in the   "Town"   scene and fill out all of the variables to get it working
    /// </summary>
    public class DialogueNPC : MonoBehaviour
    {
        public string NextKey;

        GameObject BoxObject;

        DialogueBox DBox;

        public string NPC_TITLE = "MyNpc";

        public string NPC_DREAM_KEY = "MyNpc_Dreamnail";


        public Dictionary<string, AudioClip> SingleClips = new Dictionary<string, AudioClip>()
        {
            {"PLACEHOLDER_1", null}
        };

        public Dictionary<string, AudioClip[]> MultiClips = new Dictionary<string, AudioClip[]>()
        {
            {
                "MULTIHOLDER_1", new AudioClip[]
                    {null}
            },
        };

        public string[] Dialogue = new string[]
        {
            "HOLDER_1",
            "PLACEHOLDER_1",
            "MULTIHOLDER_1",
            "HOLDER_2"
        };

        public Func<string> DialogueSelector;

        public void Log(object o)
        {
            Logger.Log($"[{GetType().FullName.Replace(".", "]:[")}] - {o}");
        }

        public void Start()
        {
            foreach (PlayMakerFSM fsm in gameObject.GetComponents<PlayMakerFSM>())
            {
                foreach (FsmState state in fsm.FsmStates)
                {
                    int i = 0;
                    foreach (FsmStateAction action in state.Actions)
                    {
                        if (action is CallMethodProper callMethod)
                            Log($"found possible match in {fsm.FsmName} - {state.Name} - {i}: {callMethod.gameObject.GameObject.Value.transform.parent} callMethod. gameObject.GameObject.Name");
                        i++;
                    }
                }
            }
        }
        
        public void SetUp()
        {
            BoxObject = gameObject.LocateMyFSM("Conversation Control").GetState("Repeat").GetAction<CallMethodProper>(0).gameObject.GameObject.Value;
            gameObject.LocateMyFSM("Conversation Control").GetState("Convo Choice").RemoveAction(6);
            gameObject.LocateMyFSM("Conversation Control").GetState("Convo Choice").GetAction<SetFsmString>().setValue = NPC_TITLE;
            FsmState state = gameObject.LocateMyFSM("Conversation Control").GetState("Precept");
            transform.Find("Dream Dialogue").gameObject.LocateMyFSM("npc_dream_dialogue").FsmVariables.FindFsmString("Convo Name").Value = NPC_DREAM_KEY;
            gameObject.GetComponent<AudioSource>().Stop();
            gameObject.GetComponent<AudioSource>().loop = false;
            state.Actions = new FsmStateAction[]
            {
                new CustomCallMethod(SelectDialogue)
            };
            FsmState state2 = gameObject.LocateMyFSM("Conversation Control").CreateState("More");
            state2.Actions = new FsmStateAction[]
            {
                new CustomCallCoroutine(ContinueConvo)
            };
            state.ChangeTransition("CONVO_FINISH", state2);
            state2.AddTransition("CONVO_FINISH", state2);
        }

        private IEnumerator ContinueConvo()
        {
            yield return null;
            gameObject.GetComponent<AudioSource>().Stop();
            int convonum = int.Parse(NextKey.Split('_')[2]);
            Log(NextKey);
            Log(NextKey.Split('_')[0] + "_" + NextKey.Split('_')[1] + "_" + (convonum + 1).ToString());
            if (Dialogue.Contains(NextKey.Split('_')[0] + "_" + NextKey.Split('_')[1] + "_" + (convonum + 1).ToString()))
            {
                NextKey = NextKey.Split('_')[0] + "_" + NextKey.Split('_')[1] + "_" + (convonum + 1).ToString();
                Log(NextKey);
                yield return new WaitForSeconds(0.1f);
                DecideSound();
                yield return new WaitForSeconds(0.1f);
                DBox.StartConversation(NextKey, "");
            }
            else
            {
                Log(NextKey);
                gameObject.LocateMyFSM("Conversation Control").SetState("Talk Finish");
            }

            yield return null;
        }

        private void DecideSound()
        {
            if (SingleClips.ContainsKey(NextKey))
                SingleSound();
            else if (MultiClips.ContainsKey(NextKey))
                MultiSound();
            else
                MissingSound();
        }

        private void SingleSound()
        {
            Log("Key found in single clips: " + NextKey);
            gameObject.GetComponent<AudioSource>().clip = SingleClips[NextKey];
            gameObject.GetComponent<AudioSource>().Play();
        }

        private void MultiSound()
        {
            Log("Key found in multi clips: " + NextKey);
            gameObject.GetComponent<AudioSource>().clip = MultiClips[NextKey][UnityEngine.Random.Range(0, MultiClips[NextKey].Length)];
            gameObject.GetComponent<AudioSource>().Play();
        }

        private void MissingSound()
        {
            Log("Key not found in clips: " + NextKey + "    This is not an error");
        }

        public void SelectDialogue()
        {
            if (!BoxObject)
                BoxObject = gameObject.LocateMyFSM("Conversation Control").GetState("Repeat").GetAction<CallMethodProper>(0).gameObject.GameObject.Value;
            DBox = (DialogueBox)GetOrAddComponent(BoxObject, typeof(DialogueBox));
            DBox = gameObject.AddComponent<DialogueBox>();
            NextKey = DialogueSelector.Invoke();
            DecideSound();
            foreach (Component comp in gameObject.GetComponents<Component>())
            {
                Log(comp.GetType().ToString());
            }
            Log(BoxObject.ToString());
            Log(DBox.ToString());
            DBox.StartConversation(NextKey, "");
        }

        public Component GetOrAddComponent(GameObject obj, Type comp)
        {
            var foundComponent = obj.GetComponent(comp.GetType());
            if (foundComponent != null)
            {
                return foundComponent;
            }
            else
            {
                foundComponent = obj.AddComponent(comp.GetType());
                return foundComponent;
            }
        }
    }
}