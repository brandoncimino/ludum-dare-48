using System;
using System.Collections.Generic;

using BrandonUtils.Spatial;

using UnityEngine;

namespace Code.Runtime {
    public static class HoverHelper {
        public static List<RaycastHit> ArcCast(Vector3 origin, Vector3 arcCenter, Vector3 arcExtreme, float arc, int iterations, LayerMask? layerMask = null) {
            var allHits    = new List<RaycastHit>();
            var degPerPart = arc / 2 / iterations;
            for (int i = 0; i < iterations; i++) {
                var fHit = Raytate(origin, arcCenter, arcExtreme, degPerPart * i, layerMask);
                if (fHit.HasValue) {
                    allHits.Add(fHit.Value);
                }

                // skip the first back-hit, so we don't double-up on straight-down raycasts
                if (i == 0) {
                    continue;
                }

                var bHit = Raytate(origin, arcCenter, arcExtreme, -degPerPart * i, layerMask);
                if (bHit.HasValue) {
                    allHits.Add(bHit.Value);
                }
            }

            return allHits;
        }

        public static List<RaycastHit> ArcCast(Transform transform, Cube.Face down, Cube.Face forward, float arc, int iterations, LayerMask? terrainMask = null) {
            var forwardVec = forward.Of(transform);
            var downVec    = down.Of(transform);
            var origin     = transform.position;

            return ArcCast(origin, downVec, forwardVec, arc, iterations, terrainMask);
        }

        private static RaycastHit? Raytate(Vector3 origin, Vector3 startVec, Vector3 targetVec, float maxDegreesDelta, LayerMask? layerMask = null) {
            var dir = Vector3.RotateTowards(startVec, targetVec, maxDegreesDelta * Mathf.Deg2Rad, 0);
            var ray = new Ray(origin, dir);

            // A LayerMask of int.MaxValue will hit everything
            if (Physics.Raycast(ray, out var hit, float.MaxValue, layerMask.GetValueOrDefault(int.MaxValue))) {
                Debug.DrawLine(ray.origin, hit.point, Color.green);
                return hit;
            }

            // Reaching here means we didn't hit anything
            Debug.DrawRay(ray.origin, ray.direction * 15, Color.red);
            return null;
        }

        private static Vector3 Of(this Cube.Face face, Transform transform) {
            // Dunno why rider thinks this is more efficient, 'cus it definitely isn't
            var forward = transform.forward;
            var right   = transform.right;
            var up      = transform.up;
            return face switch {
                Cube.Face.Forward  => forward,
                Cube.Face.Backward => -forward,
                Cube.Face.Left     => -right,
                Cube.Face.Right    => right,
                Cube.Face.Up       => up,
                Cube.Face.Down     => -up,
                _                  => throw new ArgumentOutOfRangeException(nameof(face), face, null)
            };
        }
    }
}
