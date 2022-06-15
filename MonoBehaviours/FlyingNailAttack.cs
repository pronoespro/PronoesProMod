using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class FlyingNailAttack:MonoBehaviour
    {

        public Vector2 flyDirection;
        public Vector2 destination;
        public float rotation=360f*2f;
        public float startTime=0.2f, stopTime=0.2f, flyTime=0.2f;
        public bool faceLeft;

        private float timer = 0;
        private Vector2 initPos;

        public void Start()
        {
            gameObject.SetActive(false);
        }

        public void OnEnable()
        {
            timer = 0;
            transform.position = HeroController.instance.transform.position;
        }

        public void Update()
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Clamp01(startTime+stopTime/2-timer)/startTime*rotation + 180*(faceLeft?1:0));
            if (timer < startTime){
                transform.position += (Vector3)flyDirection * Time.deltaTime;
            }else if (timer < startTime+stopTime){
                initPos = transform.position;
            }else{
                transform.position = Vector3.Lerp(initPos, destination,Mathf.Clamp01((timer - startTime - stopTime) / flyTime));
            }

            if (timer > startTime + stopTime * 2 + flyTime)
            {
                gameObject.SetActive(false);
            }

            timer += Time.deltaTime;
        }

    }
}
