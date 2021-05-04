using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using BrandonUtils.Standalone.Exceptions;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class BenthicProfile {
        public class SurveyResults {
            public float       GeographicDistance;
            public float       LocalDistance01;
            public ZoneProfile Zone;
            public float       PointInZone;
            public float       HeightInZone;
            public float       GeographicAltitude;
            public float       HeightInBenthicProfile;
            public Space       Space;

            public override string ToString() {
                return JsonUtility.ToJson(this, true);
            }
        }

        private List<ZoneProfile>               _zones = new List<ZoneProfile>();
        public  ReadOnlyCollection<ZoneProfile> Zones => _zones.AsReadOnly();

        public float MaxGeographicDistance => Zones.Sum(it => it.GeographicDistance);
        public float GeographicAmplitude   => Zones.Sum(it => it.Amplitude);

        #region Managing Zones

        public void AddZone(ZoneProfile zoneProfile) {
            _zones.Add(zoneProfile);
            _zones.Sort();
        }

        public void AddZones(IEnumerable<ZoneProfile> zoneProfiles) {
            _zones.AddRange(zoneProfiles);
            _zones.Sort();
        }

        public List<TerrainLayer> UniqueTerrainLayers => Zones.Select(it => it.TerrainLayer).Distinct().ToList();

        #endregion

        #region Surveying

        public SurveyResults Survey(float geographicDistance) {
            Validate_GeographicDistance(geographicDistance);

            // find the zone that the point is in & its local position within that zone
            var foundZone   = FindZone(geographicDistance);
            var pointInZone = FindPointInZone(geographicDistance, foundZone);

            // compute the point using the zone's equation
            var heightInZone = foundZone.Invoke(pointInZone);

            var geographicAltitude = ZoneHeightToGeographicAltitude(foundZone, heightInZone);
            var surveyResults = new SurveyResults() {
                GeographicDistance     = geographicDistance,
                LocalDistance01        = GeographicToLocalDistance(geographicDistance),
                Zone                   = foundZone,
                PointInZone            = pointInZone,
                HeightInZone           = heightInZone,
                GeographicAltitude     = geographicAltitude,
                HeightInBenthicProfile = GeographicAltitudeToBenthicHeight(geographicAltitude),
                Space                  = Space.World
            };

            return surveyResults;
        }

        public SurveyResults LocalSurvey(float localDistance01) {
            if (!IsPointInRange(localDistance01, new Vector2(0, 1))) {
                throw new ArgumentException($"The local distance {localDistance01} isn't within the boundary of 0-1!");
            }

            var surveyResults = Survey(LocalToGeographicDistance(localDistance01));
            surveyResults.Space = Space.Self;
            return surveyResults;
        }

        public float LocalToGeographicDistance(float localDistance01) {
            return localDistance01 * MaxGeographicDistance;
        }

        public float GeographicToLocalDistance(float geographicDistance) {
            return geographicDistance / MaxGeographicDistance;
        }

        private float ZoneHeightToGeographicAltitude(ZoneProfile zone, float heightInZone) {
            var altitudeBoundaries = Sorted(GetZoneGeographicAltitudeBoundaries(zone));
            return Mathf.Lerp(altitudeBoundaries.x, altitudeBoundaries.y, heightInZone);
        }

        private float GeographicAltitudeToBenthicHeight(float geographicAltitude) {
            return GetPortion(geographicAltitude, new Vector2(0, -GeographicAmplitude));
        }

        private ZoneProfile FindZone(float geographicDistance) {
            return Zones.First(it => IsPointInRange(geographicDistance, GetZoneGeographicDistanceBoundaries(it)));
        }

        private float FindPointInZone(float geographicDistance, ZoneProfile zone) {
            Validate_GeographicDistance(geographicDistance);

            var zoneBounds = GetZoneGeographicDistanceBoundaries(zone);

            return GetPortion(geographicDistance, zoneBounds);
        }

        public Vector2 GetZoneGeographicDistanceBoundaries(ZoneProfile zoneProfile) {
            var min = Zones.TakeWhile(it => it != zoneProfile).Sum(it => it.GeographicDistance);
            var max = min + zoneProfile.GeographicDistance;
            return new Vector2(min, max);
        }

        public Vector2 GetZoneGeographicAltitudeBoundaries(ZoneProfile zoneProfile) {
            var zoneStart = 0 - Zones.TakeWhile(it => it != zoneProfile).Sum(it => it.Amplitude);
            var zoneEnd   = zoneStart - zoneProfile.Amplitude;
            return new Vector2(zoneStart, zoneEnd);
        }

        #endregion

        #region Validations

        private bool IsGeographicDistanceInZone(float geographicDistance, ZoneProfile zone) {
            return IsPointInRange(geographicDistance, GetZoneGeographicDistanceBoundaries(zone));
        }

        #region Some Vector2 stuff

        private static bool IsPointInRange(float point, Vector2 range) {
            var min = Mathf.Min(range.x, range.y);
            var max = Mathf.Max(range.x, range.y);
            return point >= min && point <= max;
        }

        private static float GetPortion(float value, Vector2 range) {
            if (!IsPointInRange(value, range)) {
                throw new BrandonException($"The value {value} isn't within the range {range}");
            }

            var sorted  = Sorted(range);
            var size    = sorted.y - sorted.x;
            var portion = (value - sorted.x) / size;
            return portion;
        }

        private static Vector2 Sorted(Vector2 vector2) {
            return new Vector2(
                Mathf.Min(vector2.x, vector2.y),
                Mathf.Max(vector2.x, vector2.y)
            );
        }

        #endregion

        private void Validate_Zone(ZoneProfile zone) {
            if (!Zones.Contains(zone)) {
                throw new BrandonException($"The {nameof(ZoneProfile)} {zone} is not in the {nameof(BenthicProfile)} {this}!");
            }
        }

        private void Validate_GeographicDistance(float geographicDistance) {
            if (geographicDistance < 0) {
                throw new BrandonException("Cannot calculate the benthic depth of a point on shore!");
            }

            if (geographicDistance > MaxGeographicDistance) {
                throw new BrandonException($"The {nameof(geographicDistance)} {geographicDistance} is greater than the total {nameof(geographicDistance)} of this {nameof(BenthicProfile)}, {MaxGeographicDistance}");
            }
        }

        #endregion

        #region Terraforming

        public List<string> TerraformLog;

        public float[] ComputeHeights(int resolution) {
            var heights = new float[resolution];
            for (int i = 0; i < resolution; i++) {
                var surveyPoint = (MaxGeographicDistance / resolution) * i;
                heights[i] = Survey(surveyPoint).HeightInBenthicProfile;
            }

            return heights;
        }

        public List<string> Terraform(Terrain terrain) {
            TerraformLog = new List<string> {
                $"{nameof(MaxGeographicDistance)}: {MaxGeographicDistance}",
                $"{nameof(GeographicAmplitude)}: {GeographicAmplitude}"
            };

            var terrainData    = terrain.terrainData;
            var resolution     = terrainData.heightmapResolution;
            var benthicHeights = ComputeHeights(resolution);

            //var terrainHeights = terrain.terrainData.GetHeights(0, 0, resolution, resolution);
            var terrainHeights = new float[resolution, resolution];

            for (int x = 0; x < resolution; x++) {
                for (int y = 0; y < resolution; y++) {
                    terrainHeights[x, y] = benthicHeights[x];
                }
            }

            terrainData.SetHeights(0, 0, terrainHeights);

            var width  = terrainData.size.x;
            var height = GeographicAmplitude;
            var length = MaxGeographicDistance;
            terrainData.size = new Vector3(
                width,
                height,
                length
            );

            return TerraformLog;
        }

        #endregion
    }
}
