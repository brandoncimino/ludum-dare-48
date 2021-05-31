using Code.Runtime.Bathymetry.Measurements;

using UnityEngine;

namespace Code.Runtime.Bathymetry.Points {
    public class GeographicPoint : Point, Spacey.IGeographic {
        public override DimensionSpace DimensionSpace => DimensionSpace.Geographic;
        public readonly Terrain        WorldTerrain;
        public          float          MaxGeographicBreadth => WorldTerrain.terrainData.size.x;

        public GeographicPoint(Terrain worldTerrain) {
            this.WorldTerrain = worldTerrain;
        }

        public TerrainPoint Terrene {
            get {
                var terrainOrigin = WorldTerrain.GetPosition();
                return new TerrainPoint(WorldTerrain) {
                    Distance = terrainOrigin.z + Distance,
                    Breadth  = terrainOrigin.x + Breadth
                };
            }
        }

        public GeographicPoint Geographic {
            get { return this; }
        }

        public Vector3 Worldly {
            get { return Terrene.Worldly; }
        }
    }
}
