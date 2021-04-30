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
            throw new NotImplementedException("I don't know how to remove the terrain details yet");
        }
    }
}
