using System;

using Code.Runtime.Bathymetry.Measurements;
using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class TerrainPoint : Point, Spacey.IWorldly {
        public override DimensionSpace DimensionSpace => DimensionSpace.World;

        public readonly Terrain          WorldTerrain;
        private         Lazy<float>      _height;
        public          float            Height => _height.Value;
        public          float            Depth  => 1 - _height.Value;
        private         Lazy<RaycastHit> _terrainHit;
        public          RaycastHit       TerrainHit => _terrainHit.Value;

        public TerrainPoint(Terrain worldTerrain, Vector3 position = default) {
            this.WorldTerrain = worldTerrain;
            this.Distance     = position.z;
            this.Breadth      = position.x;
        }

        public override Vector3 ToWorldAxes() {
            return Worldly;
        }

        public Vector3 Worldly {
            get {
                return new Vector3(
                    Breadth,
                    Height,
                    Distance
                );
            }
        }

        protected override void OnAnyChange() {
            ResetHeight();
        }

        private void ResetHeight() {
            this._height = new Lazy<float>(() => WorldTerrain.SampleHeight(new Vector3(Breadth, 0, Distance)));
        }

        private void ResetTerrainHit() {
            this._terrainHit = new Lazy<RaycastHit>(() => TerrainCaster.TerrainCast(Worldly, WorldTerrain));
        }

        public GeographicPoint ToGeographicPoint() {
            return new GeographicPoint(WorldTerrain);
        }
    }
}
