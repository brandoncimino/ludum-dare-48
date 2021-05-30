using System;

using UnityEngine;

namespace Code.Runtime.Bathymetry.Measurements {
    public class TerrainCaster {
        public static RaycastHit TerrainCast(Vector3 worldPos, Terrain terrain) {
            var worldPoint = new Vector3(
                worldPos.x,
                terrain.transform.position.y + (terrain.terrainData.size.y * 2),
                worldPos.z
            );
            var ray = new Ray(worldPoint, Vector3.down);
            if (Physics.Raycast(ray, out var raycastHit)) {
                return raycastHit;
            }
            else {
                throw new ArgumentException($"The point {worldPoint} is not inside of the {nameof(terrain)}!");
            }
        }

        public static RaycastHit TerrainCast(Spacey.IWorldly point, Terrain terrain) {
            return TerrainCast(point.ToWorldly(), terrain);
        }

        /// <summary>
        /// Similar to <see cref="Terrain.SampleHeight"/>, but returns the rotation of the <see cref="RaycastHit.normal"/> from the sky to the terrain.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public static Quaternion SampleRotation(Vector3 worldPos, Terrain terrain) {
            return Quaternion.FromToRotation(Vector3.up, TerrainCast(worldPos, terrain).normal);
        }

        public static Quaternion SampleRotation(Spacey.IWorldly point, Terrain terrain) {
            return SampleRotation(point.ToWorldly(), terrain);
        }
    }
}