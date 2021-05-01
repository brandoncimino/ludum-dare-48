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

        public float          SeaLevel       => BenthicProfile.GeographicAmplitude;
        public BenthicProfile BenthicProfile => BuildBenthicProfile();
        public Curve.Form     ShelfCurve     = Curve.Form.Quadratic_Bumpy_Negative;
        public float          ShelfDistance  = 300;
        public float          ShelfAmplitude = 50;
        public Curve.Form     SlopeCurve     = Curve.Form.Quadratic_Dippy_Negative;
        public float          SlopeDistance  = 60;
        public float          SlopeAmplitude = 100;
        public Curve.Form     AbyssCurve     = Curve.Form.Quadratic_Dippy_Negative;
        public float          AbyssDistance  = 150;
        public float          AbyssAmplitude = 5;

        [TextArea(5, 100)]
        public string Debug;

        public BenthicProfile BuildBenthicProfile() {
            var benthicProfile = new BenthicProfile();

            var shelfProfile = new ZoneProfile(ShelfCurve) {
                ZoneType           = ZoneType.Shelf,
                GeographicDistance = ShelfDistance,
                Amplitude          = ShelfAmplitude
            };

            var slopeProfile = new ZoneProfile(SlopeCurve) {
                ZoneType           = ZoneType.Slope,
                GeographicDistance = SlopeDistance,
                Amplitude          = SlopeAmplitude
            };

            var abyssProfile = new ZoneProfile(AbyssCurve) {
                ZoneType           = ZoneType.Abyss,
                GeographicDistance = AbyssDistance,
                Amplitude          = AbyssAmplitude
            };

            benthicProfile.AddZone(shelfProfile);
            benthicProfile.AddZone(slopeProfile);
            benthicProfile.AddZone(abyssProfile);

            return benthicProfile;
        }

        [EditorInvocationButton]
        public void Terraform() {
            Debug = BuildBenthicProfile().Terraform(CoastlineTerrain).JoinString("\n");
        }
    }
}
