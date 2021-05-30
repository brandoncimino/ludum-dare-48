using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Attributes;

using Code.Runtime.Bathymetry.Measurements;
using Code.Runtime.Bathymetry.Points;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Code.Runtime.Bathymetry {
    /// <summary>
    /// Makes coasts!
    /// </summary>
    /// <remarks>
    /// More specifically, this acts as a go-between between the terrain information (<see cref="BenthicProfile"/>, etc.) and the actual Unity <see cref="Terrain"/>.
    /// Should probably just be combined with the <see cref="Arborist"/>.
    /// </remarks>
    public partial class Coaster : MonoBehaviour {
        public Terrain CoastlineTerrain;
        public float   CoastlineBreadth => CoastlineTerrain.terrainData.size.x;

        public List<ZoneProfile> Zones;

        [Range(0, 10)]
        public float BlendMaterialDistance;

        public  Holder                          TreeHolder      = new Holder(nameof(TreeHolder));
        private Dictionary<ZoneProfile, Holder> ZoneTreeHolders = new Dictionary<ZoneProfile, Holder>();
        public  Transform                       StartingLine;

        public BenthicProfile BuildBenthicProfile() {
            var benthicProfile = new BenthicProfile();

            benthicProfile.AddZones(Zones);

            return benthicProfile;
        }

        [EditorInvocationButton]
        public void Terraform() {
            BuildBenthicProfile().Terraform(CoastlineTerrain);

            var zonePoint = ZonePointOf(BuildBenthicProfile().Zones[0], 0.05f, 0.5f);
            var worldPos  = zonePoint.ToWorldly();
            StartingLine.position = worldPos;
        }

        public BenthicProfile.SurveyResults Survey(Transform surveyor) {
            var geoPoint = TerrainPointOf(surveyor).ToGeographicPoint();
            return BuildBenthicProfile().Survey(geoPoint.Distance);
            ;
        }

        [EditorInvocationButton]
        public void PaintTerrain() {
            PaintTerrain(BlendMaterialDistance);
        }

        private void PaintTerrain(float blendDistance) {
            var benthicProfile = BuildBenthicProfile();
            var terrainData    = CoastlineTerrain.terrainData;
            terrainData.terrainLayers = benthicProfile.UniqueTerrainLayers.ToArray();

            var matMap = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);

            for (int x = 0; x < matMap.GetLength(0); x++) {
                var surveyResults = benthicProfile.LocalSurvey(1f / matMap.GetLength(0) * x);
                var desiredLayer = surveyResults
                                   .Zone
                                   .TerrainLayer;

                // TODO: There's some weirdness with the blending - it's always blending from shallows -> rock, no matter what order the zones' terrain layers are.
                var   desiredLayerIndex = terrainData.terrainLayers.ToList().IndexOf(desiredLayer);
                var   blendDirection    = surveyResults.PointInZone < 0.5f ? -1 : 1;
                var   blendZone         = benthicProfile.FindRelativeZone(surveyResults.Zone, blendDirection);
                float blendAmount;
                if (blendZone == null || desiredLayer == blendZone.TerrainLayer) {
                    blendAmount = 0;
                }
                else {
                    var distToBoundary = Math.Abs(surveyResults.GeographicDistance - surveyResults.ClosestGeographicBoundary);
                    blendAmount = 1 - Mathf.Clamp01(distToBoundary / blendDistance);
                }

                for (int y = 0; y < matMap.GetLength(1); y++) {
                    for (int m = 0; m < matMap.GetLength(2); m++) {
                        float layerBlend;

                        if (m == desiredLayerIndex) {
                            layerBlend = 1 - blendAmount;
                        }
                        else if (blendZone != null && m == terrainData.terrainLayers.ToList().IndexOf(blendZone.TerrainLayer)) {
                            layerBlend = blendAmount;
                        }
                        else {
                            layerBlend = 0;
                        }

                        matMap[x, y, m] = layerBlend;
                    }
                }
            }

            terrainData.SetAlphamaps(0, 0, matMap);
        }

        #region Decorations

        public List<Decoration> Decorations;

        [Tooltip("Specifies the number of decorations used by " + nameof(PlantFakeTreesInEveryZone))]
        public int DecorationsPerZone = 20;

        [EditorInvocationButton]
        public void PlantFakeTreesInEveryZone() {
            PlantFakeTreesInEveryZone(DecorationsPerZone);
        }

        private void PlantFakeTreesInEveryZone(int decorationsPerZone) {
            var bp = BuildBenthicProfile();
            for (int i = 0; i < decorationsPerZone; i++) {
                foreach (var z in bp.Zones) {
                    var decoration = RandomZoneDecoration(z.ZoneType);
                    var treePoint  = ZonePointOf(z, Random.value, Random.value);
                    PlantFakeTree(z, decoration.gameObject, treePoint, Random.Range(decoration.SizeRange.x, decoration.SizeRange.y));
                }
            }
        }

        private Decoration RandomZoneDecoration(ZoneType zoneType) {
            var zones = GetDecorationsForZone(zoneType);
            return zones[Random.Range(0, zones.Count)];
        }

        private List<Decoration> GetDecorationsForZone(ZoneType zoneType) {
            return Decorations.Where(it => it.ZoneTypes.Contains(zoneType)).ToList();
        }

        [EditorInvocationButton]
        public void ClearFakeTrees() {
            LogUtils.Log($"Clearing {TreeHolder.Name} (exists: {TreeHolder.Exists})!");
            TreeHolder.Clear();
        }

        public void PlantFakeTree(ZoneProfile zoneProfile, GameObject tree, Spacey.IWorldly treePoint, float treeScale) {
            var treeRandomizedRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            var treeRot                = TerrainCaster.SampleRotation(treePoint, CoastlineTerrain) * treeRandomizedRotation;
            var treeInstance           = Instantiate(tree, treePoint.ToWorldly(), treeRot, GetZoneTreeHolder(zoneProfile));
            treeInstance.transform.localScale = Vector3.one * treeScale;
        }

        private Holder GetZoneTreeHolder(ZoneProfile zoneProfile) {
            if (!ZoneTreeHolders.ContainsKey(zoneProfile)) {
                ZoneTreeHolders.Add(zoneProfile, new Holder(zoneProfile.ToString(), TreeHolder));
            }

            return ZoneTreeHolders[zoneProfile];
        }

        #endregion

        private void AlignToTerrain(Transform toAlign, float distanceFromTerrain = 0) {
            var terrainHit = TerrainCaster.TerrainCast(toAlign.position, CoastlineTerrain);
            toAlign.rotation = Quaternion.FromToRotation(Vector3.up, terrainHit.normal);
            toAlign.position = terrainHit.point + terrainHit.normal * distanceFromTerrain;
        }

        #region Point Factory Methods

        public ZonePoint ZonePointOf(ZoneProfile zoneProfile, float zoneDist01, float zoneBreadth01) {
            return new ZonePoint(CoastlineTerrain, BuildBenthicProfile(), zoneProfile) {
                Distance = zoneDist01,
                Breadth  = zoneBreadth01
            };
        }

        /*
         * What _is_ a "quantity"? (I'm starting to see why the Java library calls it a "quantity"!)
         *
         * It contains four parts:
         *
         *  1. A measurement
         *  2. A value
         *  3. A unit
         *  4. A space
         *
         * Examples
         *
         * |Description         |Measurement    |Measures   |Relative To
         *
         * |Speedometer         |Speed          |Car        |World
         * |Airspeed            |Speed          |Plane      |Air
         * |Ground speed        |Speed          |Plane      |World
         *
         * |Course to Steer     |Heading        |Course     |Boat
         * |Course over Water   |Heading        |Boat       |Water
         * |Course made Good    |Heading        |Boat       |World
         */

        public BenthicPoint BenthicPointOf(float benthicDist01, float benthicBreadth01) {
            return new BenthicPoint(CoastlineTerrain, BuildBenthicProfile()) {
                Distance = benthicDist01,
                Breadth  = benthicBreadth01
            };
        }

        public IDGeographicPoint GeographicPointOf(float geographicDist, float geographicBreadth) {
            return new IDGeographicPoint(CoastlineTerrain) {
                Distance = geographicDist,
                Breadth  = geographicBreadth
            };
        }

        public TerrainPoint TerrainPointOf(Vector3 worldPoint) {
            return new TerrainPoint(CoastlineTerrain) {
                Distance = worldPoint.z,
                Breadth  = worldPoint.x
            };
        }

        public TerrainPoint TerrainPointOf(Transform tf) {
            return TerrainPointOf(tf.position);
        }

        #endregion
    }
}
