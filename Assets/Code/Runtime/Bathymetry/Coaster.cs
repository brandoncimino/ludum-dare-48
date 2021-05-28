using System;
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
        enum DimensionSpace {
            /// <summary>
            /// An absolute <see cref="Space.World"/> position.
            /// </summary>
            World,
            /// <summary>
            /// A distance in <see cref="Space.World"/> units, but with an <b>arbitrary origin</b>.
            ///
            /// Should represent a top-down view, as in, this should <b>not</b> include height (<see cref="Vector3.y"/>).
            ///
            /// Generally, <see cref="Geographic"/> describes a <b>distance</b>, while <see cref="World"/> describes a <b>position</b>.
            /// </summary>
            /// <remarks>
            /// Similar to a <see cref="Space.Self"/> (aka <see cref="Transform.localPosition"/>) value, but should <b>always</b> be in <see cref="World"/> units.
            ///
            /// This avoids <a href="https://en.wikipedia.org/wiki/Gotcha_(programming)">gotchas</a> that can arise when using values local <see cref="Transform"/>s,
            /// which are occasionally affected by <see cref="Transform.localScale"/>.
            /// </remarks>
            Geographic,
            /// <summary>
            /// A value between 0 and 1 representing a point within the boundaries of a <see cref="BenthicProfile"/>.
            /// </summary>
            /// <remarks>
            /// TODO: Decide on a nice, concise way to describe numbers like this. Options include:
            /// <code>
            /// - Normalized
            ///     - As in "normalized vector"
            ///     - Ex: BenthicNormalizedDistance
            ///     - Pro
            ///         - ???
            ///     - Con
            ///         - Isn't _quite_ the correct definition of "normalized vector"
            ///         - Akward to combine with a "context" (like "Benthic")
            ///         - Is an adjective (I would prefer a noun)
            /// - Unit
            ///     - As in "unit vector"
            ///         - Synonymous with "normalized vector"
            ///     - Pro
            ///         - Could sorta be a noun?
            ///     - Con
            ///         - See "Normalized"
            /// - Lerp
            ///     - As in Mathf.Lerp(a, b, t), i.e. "linear interpolation"
            ///     - Ex: BenthicLerp
            ///     - Pro
            ///         - Fun to say
            ///         - Really, really fun to say
            ///         - Informs the context in which it is meant to be used (i.e. a lerp function)
            ///         - Unlikely to be confused with "real" math, like how math nerds got "concave" and "convex" backwards
            ///         - One syllable
            ///         - Is a noun
            ///     - Con
            ///         - Doesn't mean anything to a normal math nerd
            ///         - "lerp" usually describes an action, as in "lerp it" or "a lerp function"
            /// - LerpAmount
            ///     - Ex: BenthicLerpAmount
            ///     - Pro
            ///         - As informative as "lerp", if not more so (because it can't be confused with a method name)
            ///     - Con
            ///         - Two words
            ///         - Uncannically-vallilly cute next to just "lerp"
            /// - Aerp
            ///     - From "[A]mount of liner int[ERP]olation"
            ///     - Pro
            ///         - Pronounced like "earp", which is fun to say
            ///         - I like making up -erps, like plerp
            ///     - Con
            ///         - I am losing my mind
            /// - Portion
            ///     - This is what I used to use in the olden days
            ///     - Invokes the phrasing "proportional to X"
            /// - Proportion
            ///     - What exactly is the difference between "portion" and "proportion"?
            ///     - Is "proportion" a noun?
            /// - 01
            ///     - Ex: BenthicDistance01
            ///     - Stands out a LOT, giving a good separation between the "variable name" and the "unit"
            ///     - Weird enough to stand out and make you check the documentation instead of making assumptions
            ///     - Intuitive enough (once you see the documentation) that it's easy to remember for next time
            /// - _01
            ///     - Same as "01" but even MORE obvious
            /// - Scalar
            ///     - Might be the opposite of what I mean?
            /// - Magnitude
            /// - Progress
            ///
            /// </code>
            /// </remarks>
            Benthic,
            /// <summary>
            /// <see cref="Zone"/> is to <see cref="ZoneProfile"/> as <see cref="Benthic"/> is to <see cref="BenthicProfile"/>.
            /// </summary>
            Zone
        }

        public Terrain CoastlineTerrain;

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
                    PlantFakeTree(z, decoration.gameObject, Random.value, Random.value, Random.Range(decoration.SizeRange.x, decoration.SizeRange.y));
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

        public void PlantFakeTree(ZoneProfile zoneProfile, GameObject tree, float zoneDist01, float zoneBreadth01, float treeScale) {
            var treePos                = ZonePointToWorldPoint(zoneProfile, zoneDist01, zoneBreadth01);
            var treeRandomizedRotation = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);
            var treeRot                = SampleRotation(treePos) * treeRandomizedRotation;
            var treeInstance           = Instantiate(tree, treePos, treeRot, GetZoneTreeHolder(zoneProfile));
            treeInstance.transform.localScale = Vector3.one * treeScale;
        }

        /// <summary>
        /// Converts from <see cref="DimensionSpace.Geographic"/> → <see cref="DimensionSpace.World"/>, including the addition of a <see cref="Vector3.y"/> via <see cref="Terrain.SampleHeight"/>
        /// </summary>
        /// <param name="geoDist"></param>
        /// <param name="geoBreadth"></param>
        /// <returns></returns>
        public Vector3 GeographicPointToWorldPoint(float geoDist, float geoBreadth) {
            var terrainOrigin    = CoastlineTerrain.transform.position;
            var offsetFromOrigin = new Vector3(geoBreadth, 0, geoDist);
            var worldPos         = terrainOrigin + offsetFromOrigin;
            worldPos.y = CoastlineTerrain.SampleHeight(worldPos);
            return worldPos;
        }

        /// <summary>
        /// Converts from <see cref="DimensionSpace.Zone"/> → <see cref="DimensionSpace.World"/>, including the addition of a <see cref="Vector3.y"/> coordinate via <see cref="Terrain.SampleHeight"/>
        /// </summary>
        /// <param name="zoneProfile"></param>
        /// <param name="zoneDist01"></param>
        /// <param name="zoneBreadth01"></param>
        /// <returns></returns>
        public Vector3 ZonePointToWorldPoint(ZoneProfile zoneProfile, float zoneDist01, float zoneBreadth01) {
            var zoneGeoDistBounds = BuildBenthicProfile().GetZoneGeographicDistanceBoundaries(zoneProfile);
            var geoDist           = Mathf.Lerp(zoneGeoDistBounds.x, zoneGeoDistBounds.y, zoneDist01);
            var geoBreadth        = CoastlineTerrain.terrainData.size.x * zoneBreadth01;
            return GeographicPointToWorldPoint(geoDist, geoBreadth);
        }

        /// <summary>
        /// Converts from <see cref="DimensionSpace.Benthic"/> → <see cref="DimensionSpace.World"/>, including the addition of a <see cref="Vector3.y"/> coordinate via <see cref="Terrain.SampleHeight"/>
        /// </summary>
        /// <param name="benthicProfile"></param>
        /// <param name="benthicDist01"></param>
        /// <param name="benthicBreadth01"></param>
        /// <returns></returns>
        public Vector3 BenthicPointToWorldPoint(BenthicProfile benthicProfile, float benthicDist01, float benthicBreadth01) {
            var geoDist    = Mathf.Lerp(benthicProfile.GeographicDistanceBoundaries.x, benthicProfile.GeographicDistanceBoundaries.y, benthicDist01);
            var geoBreadth = CoastlineTerrain.terrainData.size.x * benthicBreadth01;
            return GeographicPointToWorldPoint(geoDist, geoBreadth);
        }

        /// <inheritdoc cref="BenthicPointToWorldPoint(Code.Runtime.Bathymetry.BenthicProfile,float,float)"/>
        /// <param name="benthicDist01"></param>
        /// <param name="benthicBreadth01"></param>
        /// <returns></returns>
        public Vector3 BenthicPointToWorldPoint(float benthicDist01, float benthicBreadth01) {
            return BenthicPointToWorldPoint(BuildBenthicProfile(), benthicDist01, benthicBreadth01);
        }

        /// <summary>
        /// Similar to <see cref="Terrain.SampleHeight"/>, but returns the rotation of the <see cref="RaycastHit.normal"/> from the sky to the terrain.
        /// </summary>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        private Quaternion SampleRotation(Vector3 worldPos) {
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

        private void AlignToTerrain(Transform toAlign, float distanceFromTerrain = 0) {
            var terrainHit = TerrainHit(toAlign.position);
            toAlign.rotation = Quaternion.FromToRotation(Vector3.up, terrainHit.normal);
            toAlign.position = terrainHit.point + terrainHit.normal * distanceFromTerrain;
        }
    }
}
