using System.Collections.Generic;

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

        public float SeaLevel => BenthicProfile.GeographicAmplitude;

        public List<ZoneProfile> Zones;

        public BenthicProfile BenthicProfile => BuildBenthicProfile();

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
    }
}
