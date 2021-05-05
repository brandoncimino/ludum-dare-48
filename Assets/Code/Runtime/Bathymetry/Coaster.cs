using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Logging;
using BrandonUtils.Standalone.Attributes;
using BrandonUtils.Standalone.Collections;

using UnityEngine;

using Random = UnityEngine.Random;

namespace Code.Runtime.Bathymetry {
    /// <summary>
    /// Makes coasts!
    ///
    /// <ul>
    /// <li>Supplanted the <see cref="Landlord"/>.</li>
    /// <li>Can be used in conjunction with the <see cref="Arborist"/>.</li>
    /// </ul>
    /// </summary>
    public class Coaster : MonoBehaviour {
        public Terrain CoastlineTerrain;

        public List<ZoneProfile> Zones;

        [TextArea(5, 100)]
        public string DebugText;

        public  Holder                          TreeHolder      = new Holder(nameof(TreeHolder));
        private Dictionary<ZoneProfile, Holder> ZoneTreeHolders = new Dictionary<ZoneProfile, Holder>();

        public BenthicProfile BuildBenthicProfile() {
            var benthicProfile = new BenthicProfile();

            benthicProfile.AddZones(Zones);

            return benthicProfile;
        }

        [EditorInvocationButton]
        public void Terraform() {
            DebugText = BuildBenthicProfile().Terraform(CoastlineTerrain).JoinString("\n");
        }

        [EditorInvocationButton]
        public void PaintTerrain() {
            var benthicProfile = BuildBenthicProfile();
            var terrainData    = CoastlineTerrain.terrainData;
            terrainData.terrainLayers = benthicProfile.UniqueTerrainLayers.ToArray();

            var matMap = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);

            for (int x = 0; x < matMap.GetLength(0); x++) {
                var desiredLayer = BuildBenthicProfile()
                                   .LocalSurvey(1f / matMap.GetLength(0) * x)
                                   .Zone
                                   .TerrainLayer;

                var desiredLayerIndex = terrainData.terrainLayers.ToList().IndexOf(desiredLayer);

                for (int y = 0; y < matMap.GetLength(1); y++) {
                    for (int m = 0; m < matMap.GetLength(2); m++) {
                        matMap[x, y, m] = m == desiredLayerIndex ? 1 : 0;
                    }
                }
            }

            terrainData.SetAlphamaps(0, 0, matMap);
        }

        public List<GameObject> Trees;

        [EditorInvocationButton]
        public void PlantSomeTrees() {
            for (int i = 0; i < 10; i++) {
                var randomTree = Trees[Random.Range(0, Trees.Count)];
                var zones      = BuildBenthicProfile().Zones;
                var randomZone = zones[Random.Range(0, zones.Count)];
                Plant(randomZone, randomTree);
            }
        }

        [EditorInvocationButton]
        public void PlantSomeFakeTrees() {
            var bp = BuildBenthicProfile();
            for (int i = 0; i < 10; i++) {
                var randomTree = Trees[Random.Range(0, Trees.Count)];
                foreach (var z in bp.Zones) {
                    // var randomZone = bp.Zones[Random.Range(0, bp.Zones.Count)];
                    PlantFakeTree(z, randomTree, Random.value, Random.value);
                }
            }
        }

        [EditorInvocationButton]
        public void ClearFakeTrees() {
            LogUtils.Log($"Clearing {TreeHolder.Name} (exists: {TreeHolder.Exists})!");
            TreeHolder.Clear();
        }

        public void PlantFakeTree(ZoneProfile zoneProfile, GameObject tree, float zoneDist01, float zoneBreadth01) {
            LogUtils.Log($"Attempting to plant tree:\nzone = {zoneProfile}\n{nameof(zoneDist01)}={zoneDist01}\b{nameof(zoneBreadth01)}={zoneBreadth01}");
            var treePos      = ZonePointToWorldPoint(zoneProfile, zoneDist01, zoneBreadth01);
            var treeRot      = GetRotationAtPoint(treePos);
            var treeInstance = Instantiate(tree, treePos, treeRot, GetZoneTreeHolder(zoneProfile));
        }

        public void Plant(ZoneProfile zoneProfile, GameObject tree) {
            var treePos = ZonePointToWorldPoint(zoneProfile, Random.value, Random.value);
            LogUtils.Log($"Creating tree {tree} at {treePos}");
            // var treeRot       = GetRotationAtPoint(treePos);
            var treePrototype = GetTreePrototypeForPrefab(tree);
            var inst          = new TreeInstance {prototypeIndex = GetTreePrototypeIndex(treePrototype)};
            CoastlineTerrain.AddTreeInstance(inst);
        }

        private TreePrototype GetTreePrototypeForPrefab(GameObject prefab) {
            var terrainData = CoastlineTerrain.terrainData;
            var prototype   = terrainData.treePrototypes.FirstOrDefault(it => it.prefab.Equals(prefab));

            if (prototype == default) {
                var newPrototype = new TreePrototype {prefab = prefab};
                terrainData.treePrototypes = terrainData.treePrototypes.Concat(new[] {newPrototype}).ToArray();
                prototype                  = newPrototype;
            }

            return prototype;
        }

        private int GetTreePrototypeIndex(TreePrototype prototype) {
            return CoastlineTerrain.terrainData.treePrototypes.ToList().IndexOf(prototype);
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

        private Quaternion GetRotationAtPoint(Vector3 worldPos) {
            var worldPoint = new Vector3(
                worldPos.x,
                CoastlineTerrain.transform.position.y + (CoastlineTerrain.terrainData.size.y * 2),
                worldPos.z
            );
            var ray = new Ray(worldPoint, Vector3.down);
            if (Physics.Raycast(ray, out var raycastHit)) {
                return Quaternion.FromToRotation(Vector3.up, raycastHit.normal);
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
    }
}
