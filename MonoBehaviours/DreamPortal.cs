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
			HeroController.instance.IgnoreInput();
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
			HeroController.instance.IgnoreInput();

			SceneLoader.DreamTransition(sceneToLoad, curScene);
		}

	}
}
