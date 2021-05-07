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

        public float          TileDiameter;
        public int            GridCellCount;
        public Pair<int, int> DecorationsPerCell;

        public Material TidalMaterial;
        public Material ChasmMaterial;
        public Material DeepsMaterial;

        public Dictionary<Zone, Material> ZoneMaterials => new Dictionary<Zone, Material>() {
            {Zone.Tidal, TidalMaterial},
            {Zone.Chasm, ChasmMaterial},
            {Zone.Deeps, DeepsMaterial}
        };

        public List<Decoration> Decorations;

        public           List<LandTile>              LandTiles;
        private readonly Dictionary<Zone, Transform> _zoneHolders = new Dictionary<Zone, Transform>();

        private void Start() {
            if (!TileHolder) {
                TileHolder = new GameObject(nameof(TileHolder)).transform;
            }

            // example tiles
            TileNext();
            // TileNext();
            // TileNext();
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

            var tile = Instantiate(TilePrefab, ZoneHolder(zone)).GetComponent<LandTile>();
            tile.name     = $"{nameof(LandTile)}_{zone}_{TileCountByZone(zone)}";
            tile.Diameter = TileDiameter;

            if (previousTile != default) {
                Debug.Log($"Connecting {tile.name} -> {previousTile.name}");
                tile.Connect(previousTile, RectTransform.Edge.Top);
            }

            LandTiles.Add(tile);
            tile.Renderer.material = ZoneMaterials[zone];
        }

        public int TileCountByZone(Zone zone) {
            return LandTiles.Count(it => it.Zone == zone);
        }

        private Transform ZoneHolder(Zone zone) {
            if (!_zoneHolders.ContainsKey(zone)) {
                _zoneHolders.Add(zone, new GameObject($"{zone}Holder").transform);
                _zoneHolders[zone].SetParent(TileHolder);
            }

            return _zoneHolders[zone];
        }
    }
}
