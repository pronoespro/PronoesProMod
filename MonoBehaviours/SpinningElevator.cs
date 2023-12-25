using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine;

namespace PronoesProMod.MonoBehaviours
{
    public class SpinningElevator:MonoBehaviour
    {

        public LayerMask mask = 10000000;
        public bool riding,goingUp;
        public Vector2 rideOffset;
        public Vector3[] ridePoints;
        public Animator anim;
        public float rideSpeed;

        private int __curPoint;

        public void Setup(Vector2 _offset,Vector3[] _points,float _speed=1, Animator _animator=null)
        {
            rideOffset = _offset;
            ridePoints = _points;
            rideSpeed = _speed;

            anim = _animator;
        }

        public void OnTriggerStay2D(Collider2D _col)
        {
            if (mask == (mask | (1 << _col.gameObject.layer)) && !PronoesProMod.IsMidDialog() && !riding){
                PronoesProMod.Instance.StartInteraction(new Vector3(transform.position.x, transform.position.y, PronoesProMod.Instance.interactionPropt.transform.position.z), "Ride");

                if (!PronoesProMod.IsGamePaused() && PronoesProMod.Instance.KnightReady()) {
                    if (InputHandler.Instance.inputActions.up.IsPressed && __curPoint<ridePoints.Length-1)
                    {
                        PronoesProMod.Instance.StartSpinningKnight();
                        goingUp = true;
                        riding = true;

                        PronoesProMod.Instance.EndInteraction();
                        PronoesProMod.Instance.ChangeDialogEvents(null, null, null);
                    }
                    else if (InputHandler.Instance.inputActions.down.IsPressed && __curPoint>0)
                    {
                        PronoesProMod.Instance.StartSpinningKnight();
                        goingUp = false;
                        riding = true;

                        PronoesProMod.Instance.EndInteraction();
                        PronoesProMod.Instance.ChangeDialogEvents(null, null, null);
                    }
                }
            }
        }

        public void OnTriggerExit2D(Collider2D _col)
        {
            if (mask == (mask | (1 << _col.gameObject.layer)))
            {
                PronoesProMod.Instance.EndInteraction();
                PronoesProMod.Instance.ChangeDialogEvents(null, null, null);
            }
        }

        private void Start()
        {
            if (ridePoints != null && ridePoints.Length > 0)
            {
                transform.position = ridePoints[0];
            }
        }

        private void Update(){
            if(ridePoints==null && ridePoints.Length== 0){return;}

            if (riding)
            {
                if (anim != null)
                {
                    anim.Play("Move");
                }
                int _curPoint = __curPoint + (goingUp ? 1 : -1);

                if (_curPoint >= 0 && _curPoint < ridePoints.Length)
                {
                    Vector3 _dir = ridePoints[_curPoint] - transform.position;

                    if (_dir.sqrMagnitude <= (rideSpeed*Time.deltaTime)* (rideSpeed*Time.deltaTime))
                    {
                        transform.position = ridePoints[_curPoint];
                        riding = false;
                        __curPoint = _curPoint;
                        PronoesProMod.Instance.EndSpinningKnight();
                    }else{
                        transform.position += rideSpeed*Time.deltaTime* _dir.normalized ;
                    }
                    Vector3 _desPos = transform.position + new Vector3(rideOffset.x, rideOffset.y);
                    HeroController.instance.transform.position = new Vector3(_desPos.x, _desPos.y, HeroController.instance.transform.position.z);
                }
            }else
            {
                if (anim != null)
                {
                    anim.Play("Idle");
                }
            }
        }

    }
}
