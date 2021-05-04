using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Attributes;
using BrandonUtils.Standalone.Collections;

using UnityEngine;

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
        public string Debug;

        public BenthicProfile BuildBenthicProfile() {
            var benthicProfile = new BenthicProfile();

            benthicProfile.AddZones(Zones);

            return benthicProfile;
        }

        [EditorInvocationButton]
        public void Terraform() {
            Debug = BuildBenthicProfile().Terraform(CoastlineTerrain).JoinString("\n");
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
    }
}
