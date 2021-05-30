﻿using System;

using Code.Runtime.Bathymetry.Measurements;
using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class TerrainPoint : Point, Spacey.IWorldly {
        public override DimensionSpace DimensionSpace => DimensionSpace.World;

        public readonly Terrain          WorldTerrain;
        private         Lazy<float>      _height;
        public          float            Height => _height.Value;
        private         Lazy<RaycastHit> _terrainHit;
        public          RaycastHit       TerrainHit => _terrainHit.Value;

        public TerrainPoint(Terrain worldTerrain, Vector3 position = default) {
            this.WorldTerrain = worldTerrain;
            this.Distance     = position.z;
            this.Breadth      = position.x;
        }

        public override Vector3 ToWorldAxes() {
            return ToWorldly();
        }

        public Vector3 ToWorldly() {
            return new Vector3(
                Breadth,
                Height,
                Distance
            );
        }

        public override void OnAnyChange() {
            ResetHeight();
        }

        private void ResetHeight() {
            this._height = new Lazy<float>(() => WorldTerrain.SampleHeight(new Vector3(Breadth, 0, Distance)));
        }

        private void ResetTerrainHit() {
            this._terrainHit = new Lazy<RaycastHit>(() => TerrainCaster.TerrainCast(ToWorldly(), WorldTerrain));
        }

        public IDGeographicPoint ToGeographicPoint() {
            return new IDGeographicPoint(WorldTerrain);
        }
    }
}
