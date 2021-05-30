using Code.Runtime.Bathymetry.Measurements;

using UnityEngine;

namespace Code.Runtime.Bathymetry.Points {
    public class IDGeographicPoint : Point, Spacey.IGeographic {
        public override DimensionSpace DimensionSpace => DimensionSpace.Geographic;
        public readonly Terrain        WorldTerrain;
        public          float          MaxGeographicBreadth => WorldTerrain.terrainData.size.x;

        public IDGeographicPoint(Terrain worldTerrain) {
            this.WorldTerrain = worldTerrain;
        }

        public TerrainPoint ToTerrene() {
            var terrainOrigin = WorldTerrain.GetPosition();
            return new TerrainPoint(WorldTerrain) {
                Distance = terrainOrigin.z + Distance,
                Breadth  = terrainOrigin.x + Breadth
            };
        }

        public IDGeographicPoint ToGeographic() {
            return this;
        }

        public Vector3 ToWorldly() {
            return ToTerrene().ToWorldly();
        }
    }
}
