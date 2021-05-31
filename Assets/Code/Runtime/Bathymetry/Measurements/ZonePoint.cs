using Code.Runtime.Bathymetry.Measurements;
using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class ZonePoint : Point, Spacey.IBenthic {
        public override DimensionSpace DimensionSpace => DimensionSpace.Zone;

        public readonly Terrain        WorldTerrain;
        public readonly BenthicProfile BenthicProfile;
        public readonly ZoneProfile    ZoneProfile;
        public          float          MaxGeographicBreadth => WorldTerrain.terrainData.size.x;

        public ZonePoint(Terrain worldTerrain, BenthicProfile benthicProfile, ZoneProfile zoneProfile) {
            this.WorldTerrain   = worldTerrain;
            this.BenthicProfile = benthicProfile;
            this.ZoneProfile    = zoneProfile;
        }

        public BenthicPoint Benthic {
            get {
                var geoBounds = BenthicProfile.GetZoneGeographicDistanceBoundaries(ZoneProfile);
                var geoDist   = Mathf.Lerp(geoBounds.x, geoBounds.y, Distance);
                return new BenthicPoint(WorldTerrain, BenthicProfile) {
                    Distance = BenthicProfile.GetPortion(geoDist, BenthicProfile.GeographicDistanceBoundaries),
                    Breadth  = Breadth
                };
            }
        }

        public GeographicPoint Geographic {
            get { return Benthic.Geographic; }
        }

        public TerrainPoint Terrene {
            get { return Geographic.Terrene; }
        }

        public Vector3 Worldly {
            get { return Terrene.Worldly; }
        }
    }
}