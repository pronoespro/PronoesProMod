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
using UObject = UnityEngine.Object;

namespace PronoesProMod.Extensions
{
    internal static class PlayMakerExtensions
    {
        internal class FuncAction : FsmStateAction
        {
            private readonly Action _func;

            public FuncAction(Action func)
            {
                _func = func;
            }

            public override void OnEnter()
            {
                _func();
                Finish();
            }
        }

        public static void ReplaceAction(this FsmState s, int i, Action a)
        {
            s.Actions[i] = new PlayMakerExtensions.FuncAction(a);
        }
    }
}