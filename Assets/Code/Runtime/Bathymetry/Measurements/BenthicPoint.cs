using Code.Runtime.Bathymetry.Measurements;
using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class BenthicPoint : Point, Spacey.IBenthic {
        public override DimensionSpace DimensionSpace => DimensionSpace.Benthic;

        public readonly BenthicProfile BenthicProfile;
        public readonly Terrain        WorldTerrain;
        public          float          MaxGeographicBreadth => WorldTerrain.terrainData.size.x;

        public BenthicPoint(Terrain worldTerrain, BenthicProfile benthicProfile) {
            this.WorldTerrain   = worldTerrain;
            this.BenthicProfile = benthicProfile;
        }

        public GeographicPoint Geographic {
            get {
                return new GeographicPoint(WorldTerrain) {
                    Distance = Mathf.Lerp(BenthicProfile.MinGeographicDistance, BenthicProfile.MaxGeographicDistance, Distance),
                    Breadth  = Mathf.Lerp(0,                                    MaxGeographicBreadth,                 Breadth)
                };
            }
        }

        public TerrainPoint Terrene {
            get { return Geographic.Terrene; }
        }

        public BenthicPoint Benthic {
            get { return this; }
        }

        public Vector3 Worldly {
            get { return Terrene.Worldly; }
        }
    }
}
