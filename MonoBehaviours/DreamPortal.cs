using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using System.Collections;
using UnityEngine;

namespace PronoesProMod
{
    public class DreamPortal:MonoBehaviour
    {

		public string sceneToLoad;
		public string curScene;
		public float transitionTime = 1.5f;
		public ParticleSystem particles;

        private bool entered;

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (entered || collision.tag != "Dream Attack")
			{
				return;
			}
			entered = true;
			StartCoroutine(MakeDreamTransition());
		}

		public IEnumerator MakeDreamTransition()
		{
			Fsm dreamNailFSM = HeroController.instance.gameObject.LocateMyFSM("Dream Nail").Fsm;
			
			HeroController.instance.IgnoreInput();
			HeroController.instance.SendMessage("RelinquishControl");
			HeroController.instance.SendMessage("StopAnimationControl");
			if (particles != null)
			{
				particles.Play();
			}
			if (PronoesProMod.Instance != null)
			{
				PronoesProMod.Instance.Log("Fading in");
				PronoesProMod.Instance.CustomSceneFadeInsis();
				PronoesProMod.Instance.Log("Fade in finished");
			}

			yield return new WaitForSeconds(transitionTime);

			HeroController.instance.SendMessage("RelinquishControl");
			HeroController.instance.SendMessage("StopAnimationControl");

			HeroController.instance.gameObject.LocateMyFSM("Roar Lock")
				.GetVariable<FsmBool>("No Roar")
				.Value = false;
			HeroController.instance.FaceLeft();


			SceneLoader.DreamTransition(sceneToLoad, curScene);
			FindObjectOfType<SceneManager>().SendMessage("Start");
		}

	}
}
