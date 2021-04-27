using System;

using BrandonUtils.UI;

using UnityEngine;

namespace Code.Runtime {
    public class LandTile : MonoBehaviour {
        public Vector2 AnchorPoint;

        private Vector2 _diameter;
        public Vector2 Diameter {
            get => _diameter;

            set {
                _diameter                      = value;
                _renderer.transform.localScale = Vector3.one * _diameter;
            }
        }
        public Landlord.Zone Zone;

        private Renderer _renderer;
        public Renderer Renderer {
            get {
                if (!_renderer) {
                    _renderer = GetComponentInChildren<Renderer>();
                }

                return _renderer;
            }
        }

        public Vector2 BottomLeft => AnchorPoint - (new Vector2(0.5f, 0.5f) * Diameter);

        public Vector2 GetEdgePosition(RectTransform.Edge edge) {
            // get the center point
            var anchorOffset   = AnchorPoint * Diameter;
            var anchorOffset3d = new Vector3(anchorOffset.x, 0, anchorOffset.y);
            var centerPoint    = transform.position + anchorOffset3d;

            var edgeOffset   = GetEdgeOffset(edge);
            var edgeOffset3d = new Vector3(edgeOffset.x, 0, edgeOffset.y);

            return centerPoint + edgeOffset3d;
        }

        public Vector2 GetEdgeOffset(RectTransform.Edge edge) {
            return GetEdgeCenter(edge) * Diameter;
        }

        public void Connect(LandTile otherTile, RectTransform.Edge otherConnectedEdge) {
            transform.position = otherTile.GetEdgePosition(otherConnectedEdge) + GetEdgeOffset(otherConnectedEdge.Inverse());
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