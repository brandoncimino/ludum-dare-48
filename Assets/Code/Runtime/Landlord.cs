using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Attributes;
using BrandonUtils.Standalone.Collections;

using JetBrains.Annotations;

using UnityEngine;

namespace Code.Runtime {
    public class Landlord : MonoBehaviour {
        public enum Zone {
            Tidal,
            Chasm,
            Deeps
        }

        public GameObject TilePrefab;
        public Transform  TileHolder;

        public Vector2 TileSize;

        public Material TidalMaterial;
        public Material ChasmMaterial;
        public Material DeepsMaterial;

        public Dictionary<Zone, Material> ZoneMaterials => new Dictionary<Zone, Material>() {
            {Zone.Tidal, TidalMaterial},
            {Zone.Chasm, ChasmMaterial},
            {Zone.Deeps, DeepsMaterial}
        };

        public List<Decoration> Decorations;

        public List<LandTile> LandTiles;

        private void Start() {
            if (!TileHolder) {
                TileHolder = new GameObject(nameof(TileHolder)).transform;
            }
        }

        [EditorInvocationButton]
        public void TileNext() {
            CreateTile(Zone.Tidal);
        }

        public void CreateTile(
            Zone zone,
            [CanBeNull]
            LandTile previousTile = default
        ) {
            if (previousTile == default) {
                previousTile = LandTiles.LastOrDefault();
            }

            var tile = Instantiate(TilePrefab).GetComponent<LandTile>();

            if (previousTile != default) {
                tile.Connect(previousTile, RectTransform.Edge.Top);
            }

            LandTiles.Add(tile);
            tile.Renderer.material = ZoneMaterials[zone];
        }

        public void DecorateTile(LandTile tile, int gridCellCount, Pair<int, int> decorationsInCell) {
            //box up the tile
            var cellSize = tile.Diameter / gridCellCount;
            for (int x = 0; x < gridCellCount; x++) {
                for (int y = 0; y < gridCellCount; y++) {
                    var xRange = new Vector2(
                        x * cellSize.x,
                        (x + 1) * cellSize.x
                    );

                    var yRange = new Vector2(
                        y * cellSize.y,
                        (y + 1) * cellSize.y
                    );

                    var decsToMake = Random.Range(decorationsInCell.X, decorationsInCell.Y);
                    for (var d = 0; d < decsToMake; d++) {
                        var spawnOffset = new Vector2(
                            Random.Range(xRange.x, xRange.y),
                            Random.Range(yRange.x, yRange.y)
                        );

                        var newDec     = Instantiate(GrabDecoration(tile.Zone), tile.transform);
                        var transform1 = newDec.transform;

                        var spawnPos = tile.BottomLeft + spawnOffset;
                        transform1.localPosition = new Vector3(spawnPos.x, 0, spawnPos.y);
                        transform1.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    }
                }
            }
        }

        private Decoration GrabDecoration(Zone zone) {
            var dZone = Decorations.Where(it => it.Zones.Contains(zone)).ToList();
            // var weights   = dZone.Select(it => it.Cost);
            var weights   = new Dictionary<Decoration, int>();
            var weightSum = 0;

            foreach (var t in dZone) {
                weightSum += t.Cost;
                weights.Add(t, weightSum);
            }

            var randomWeight = Random.Range(0, weightSum);

            return weights.First(it => randomWeight < it.Value).Key;
        }
    }
}