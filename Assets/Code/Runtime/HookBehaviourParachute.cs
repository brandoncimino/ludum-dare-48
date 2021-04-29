using System;
using System.Collections.Generic;

using UnityEngine;

namespace Code.Runtime
{
    public class HookBehaviourParachute : HookBehaviour
    {
        public float test = 0;
        
        private float _pressure;
        private       float _velocityPush = 0;
        private const float PushModifier  = 0.001f;
        
        public  float      StabilizerSmoothness;
        
        public float MaxLateralSpeed;
        public float LateralAccelerationFactor;
        
        // Update is called once per frame
        protected override void Update()
        {
            // falling down based on pressure and gravity
            _velocityPull = WaterManager.Single.computePull(_depth, MaxDepth);

            // movement in total (additive as an approximation)
            SinkVelocity = (PullModifier * _velocityPull) + (PushModifier * _velocityPush);
            var targetVelocity = _movementAxesRawInput.normalized * MaxLateralSpeed;
            LateralVelocity = Vector2.Lerp(LateralVelocity, targetVelocity, Time.deltaTime * LateralAccelerationFactor);
            
            // update data in the UI
            UIManager.Single.provideData("depth", (int) (_depth / 0.546807f));
        }
        
        private void ApplyStabilization() {
            // Calculate the desired rotation, which will incorporate a bit of wobble
            var targetRotation = new Vector3(
                0,
                90,
                0
            );

            transform.rotation = Quaternion.Slerp(transform.rotation, _rotationInitial, Time.deltaTime * StabilizerSmoothness);
        }
        
        public Vector2 LateralVelocity {
            get {
                var vel = MyRigidbody.velocity;
                return new Vector2(vel.x, vel.z);
            }
            set => MyRigidbody.velocity = new Vector3(value.x, MyRigidbody.velocity.y, value.y);
        }

        public float SinkVelocity {
            get => MyRigidbody.velocity.y;
            set {
                Vector3 velocity;
                MyRigidbody.velocity = new Vector3((velocity = MyRigidbody.velocity).x, value, velocity.z);
            }
        }
    }
}