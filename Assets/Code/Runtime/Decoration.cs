using System.Collections.Generic;

using Code.Runtime.Bathymetry;

using UnityEngine;

namespace Code.Runtime {
    public class Decoration : MonoBehaviour {
        public List<ZoneType> ZoneTypes;
        public int            Cost;
        public Vector2        SizeRange = Vector2.one;
    }
}
