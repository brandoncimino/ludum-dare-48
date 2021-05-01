using System;

using BrandonUtils.Standalone.Attributes;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class Arborist : MonoBehaviour {
        public Terrain Arboretum;

        [EditorInvocationButton]
        public void Deforest() {
            Arboretum.terrainData.SetTreeInstances(Array.Empty<TreeInstance>(), false);
        }

        [EditorInvocationButton]
        public void Weed() {
            var terrainData = Arboretum.terrainData;
            terrainData.SetDetailLayer(0, 0, 0, new int[terrainData.detailResolution, terrainData.detailResolution]);
        }
    }
}
