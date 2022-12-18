using HutongGames.PlayMaker;
using Modding;
using Satchel.Futils;
using System.Collections;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class Door_Prono : MonoBehaviour
	{

		public string curScene;
		public float transitionTime = 1.5f;

		public string targetScene, entryPoint;
		public GameManager.SceneLoadVisualizations sceneLoadVisualization;
		public string playerAnimationToPlay="";

		private bool entered;


		private void OnTriggerStay2D(Collider2D collision)
		{
			if (entered || !InputHandler.Instance.inputActions.up || collision.gameObject.layer!=LayerMask.NameToLayer("Player") || !HeroController.instance.CanInteract())
			{
				return;
			}
			entered = true;
			PronoesProMod.Instance.Log("Enters door?");
			StartCoroutine(DoSceneTransition());
		}

		public IEnumerator DoSceneTransition()
		{
			PronoesProMod.Instance.Log("Starting transition");
			HeroController.instance.SendMessage("RelinquishControl");
			HeroController.instance.SendMessage("StopAnimationControl");
			PlayerData.instance.SetBool("disablePause", true);

			if (playerAnimationToPlay.Length > 0)
			{
				tk2dSpriteAnimator anim = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
				if (anim != null)
				{
					anim.SendMessage("Play", playerAnimationToPlay);
					PronoesProMod.Instance.Log("Played animation");
				}
			}

			PronoesProMod.Instance.CustomSceneFadeInsis();

			yield return new WaitForSeconds(0.5f);

			PronoesProMod.Instance.Log("Scene Transition");
			GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
			{
				SceneName = targetScene,
				EntryGateName = entryPoint,
				HeroLeaveDirection = GlobalEnums.GatePosition.door,
				EntryDelay = 0,
				WaitForSceneTransitionCameraFade = true,
				//PreventCameraFadeOut = (customFadeFSM != null),
				Visualization = sceneLoadVisualization,
				AlwaysUnloadUnusedAssets = false,
				forceWaitFetch = false
			});
			PronoesProMod.Instance.Log("Ended transition");
		}

	}
}
