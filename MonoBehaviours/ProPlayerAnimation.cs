using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PronoesProMod.Monobehaviors
{
    public class ProPlayerAnimation : MonoBehaviour
    {

        public tk2dSpriteAnimator heroAnimator;
        public UnityEvent _OnStarted = new UnityEvent(), _OnFinished = new UnityEvent();

        private Coroutine animRoutine;
        private bool doingAnimation;
        private bool canChangeAnim, changeAnim;
        private string curAnimationDoing;

        public bool GetIsDoingAnimation(){ return doingAnimation; }

        public void ChangeAnimation(string animationName, float maxTime = -1, bool relinquishControl = true, float delay = 0f)
        {
            if (heroAnimator == null)
            {
                heroAnimator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
            }

            if (curAnimationDoing != animationName)
            {
                if (doingAnimation){
                    changeAnim = true;
                }
                animRoutine = StartCoroutine(DoAnimation(animationName, maxTime, relinquishControl, delay));
            }

        }

        public void ForceEndAnimation()
        {
            if (doingAnimation)
            {
                changeAnim = true;
            }
        }

        public IEnumerator DoAnimation(string animName, float maxTime = -1, bool relinquishControl = true, float delay = 0f)
        {
            curAnimationDoing = animName;
            canChangeAnim = !relinquishControl;
            yield return null;

            yield return new WaitForSeconds(delay);

            doingAnimation = true;

            if (relinquishControl)
            {
                HeroController.instance.RelinquishControl();
            }

            HeroController.instance.StopAnimationControl();

            heroAnimator.Play(animName);
            _OnStarted.Invoke();

            if (maxTime > 0)
            {
                yield return new WaitForSeconds(maxTime);
            }else{
                yield return new WaitWhile(() => heroAnimator.IsPlaying(animName) && !changeAnim);
            }

            if (relinquishControl)
            {
                HeroController.instance.RegainControl();
                HeroController.instance.StartAnimationControl();
            }

            _OnFinished.Invoke();

            changeAnim = false;
            doingAnimation = false;

            _OnStarted.RemoveAllListeners();
            _OnFinished.RemoveAllListeners();
            curAnimationDoing = "";
        }
    }
}