using System;
using System.Collections.Generic;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Exceptions;

using UnityEngine;

namespace Code.Runtime {
    public class LandTile : MonoBehaviour {
        public Vector2 CenterPoint;

        private float _diameter;
        public float Diameter {
            get => _diameter;

            set {
                _diameter = value;
                if (!Renderer) {
                    throw new BrandonException("NO RENDERER!");
                }

                var tf = Renderer.transform;
                if (!tf) {
                    throw new BrandonException("NO TRANSFORM ON RENDERER!");
                }

                Renderer.transform.localScale = new Vector3(
                    _diameter,
                    _diameter,
                    _diameter
                );
            }
        }
        public Landlord.Zone Zone;

        private Renderer _renderer;
        public Renderer Renderer {
            get {
                if (!_renderer) {
                    _renderer = GetComponentInChildren<Renderer>();
                }

                if (!_renderer) {
                    throw new BrandonException("NO RENDERER!");
                }

                return _renderer;
            }
        }

        public Vector2 BottomLeft => CenterPoint - (new Vector2(0.5f, 0.5f) * Diameter);

        public Vector3 GetEdgePosition_World(RectTransform.Edge edge) {
            // get the center point
            var centerOffset = CenterPoint * Diameter;
            var centerPoint  = transform.position + new Vector3(centerOffset.x, 0, centerOffset.y);

            var edgeOffset   = GetEdgeOffset(edge);
            var edgeOffset3d = new Vector3(edgeOffset.x, 0, edgeOffset.y);

            LogUtils.Log(
                new Dictionary<object, object>() {
                    {nameof(centerOffset), centerOffset},
                    {nameof(centerPoint), centerPoint},
                    {nameof(edgeOffset), edgeOffset},
                    {nameof(edgeOffset3d), edgeOffset3d}
                }
            );

            return centerPoint + edgeOffset3d;
        }

        public Vector2 GetEdgeOffset(RectTransform.Edge edge) {
            return GetEdgeCenter(edge) * Diameter;
        }

        public void Connect(LandTile otherTile, RectTransform.Edge otherConnectedEdge) {
            LogUtils.Log(
                new Dictionary<object, object>() {
                    {$"{otherTile.name}.{nameof(GetEdgePosition_World)}({otherConnectedEdge})", otherTile.GetEdgePosition_World(otherConnectedEdge)}
                }
            );

            var offsetFromOther       = GetEdgeOffset(otherConnectedEdge);
            var offsetFromOther_world = new Vector3(offsetFromOther.x, 0, offsetFromOther.y);

            transform.position = otherTile.GetEdgePosition_World(otherConnectedEdge) + offsetFromOther_world;
        }

        public static Vector2 GetEdgeCenter(RectTransform.Edge edge) {
            var eUnit = edge switch {
                RectTransform.Edge.Left   => Vector2.left,
                RectTransform.Edge.Right  => Vector2.right,
                RectTransform.Edge.Top    => Vector2.up,
                RectTransform.Edge.Bottom => Vector2.down,
                _                         => throw new ArgumentOutOfRangeException(nameof(edge), edge, null)
            };

            return eUnit * 0.5f;
        }
    }
}
