﻿using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Attributes;

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
    public class Coaster : MonoBehaviour {
        public Terrain CoastlineTerrain;

        public List<ZoneProfile> Zones;

        public Vector2 TreeSizeRange;

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

            StartingLine.position = ZonePointToWorldPoint(BuildBenthicProfile().Zones[0], 0.05f, 0.5f);
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

        public List<Decoration> Decorations;

        [EditorInvocationButton]
        public void PlantSomeFakeTrees() {
            var bp = BuildBenthicProfile();
            for (int i = 0; i < 20; i++) {
                foreach (var z in bp.Zones) {
                    PlantFakeTree(z, RandomZoneDecoration(z.ZoneType).gameObject, Random.value, Random.value);
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

        public void PlantFakeTree(ZoneProfile zoneProfile, GameObject tree, float zoneDist01, float zoneBreadth01) {
            var treePos                = ZonePointToWorldPoint(zoneProfile, zoneDist01, zoneBreadth01);
            var treeRandomizedRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            var treeRot                = GetTerrainRotation(treePos) * treeRandomizedRotation;
            var treeInstance           = Instantiate(tree, treePos, treeRot, GetZoneTreeHolder(zoneProfile));
            var treeScale              = Random.Range(TreeSizeRange.x, TreeSizeRange.y);
            treeInstance.transform.localScale = Vector3.one * treeScale;
        }

        private Vector3 ZonePointToWorldPoint(ZoneProfile zoneProfile, float zoneDist01, float zoneBreadth01) {
            var zoneGeoDistBounds = BuildBenthicProfile().GetZoneGeographicDistanceBoundaries(zoneProfile);
            var terrainOrigin     = CoastlineTerrain.transform.position;
            var plantDist         = Mathf.Lerp(zoneGeoDistBounds.x, zoneGeoDistBounds.y, zoneDist01);
            var plantBreadth      = CoastlineTerrain.terrainData.size.x * zoneBreadth01;
            var plantWorld        = terrainOrigin + new Vector3(plantBreadth, 0, plantDist);
            plantWorld.y = CoastlineTerrain.SampleHeight(plantWorld);
            return plantWorld;
        }

        private Quaternion GetTerrainRotation(Vector3 worldPos) {
            return Quaternion.FromToRotation(Vector3.up, TerrainHit(worldPos).normal);
        }

        private RaycastHit TerrainHit(Vector3 worldPos) {
            var worldPoint = new Vector3(
                worldPos.x,
                CoastlineTerrain.transform.position.y + (CoastlineTerrain.terrainData.size.y * 2),
                worldPos.z
            );
            var ray = new Ray(worldPoint, Vector3.down);
            if (Physics.Raycast(ray, out var raycastHit)) {
                return raycastHit;
            }
            else {
                throw new ArgumentException($"The point {worldPoint} is not inside of the {nameof(CoastlineTerrain)}!");
            }
        }

        private Holder GetZoneTreeHolder(ZoneProfile zoneProfile) {
            if (!ZoneTreeHolders.ContainsKey(zoneProfile)) {
                ZoneTreeHolders.Add(zoneProfile, new Holder(zoneProfile.ToString(), TreeHolder));
            }

            return ZoneTreeHolders[zoneProfile];
        }

        private void AlignToTerrain(Transform toAlign) {
            var terrainHit = TerrainHit(toAlign.position);
            toAlign.rotation = Quaternion.FromToRotation(Vector3.up, terrainHit.normal);
            toAlign.position = terrainHit.point;
        }
    }
}
