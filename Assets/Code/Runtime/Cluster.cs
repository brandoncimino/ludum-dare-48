using System.Collections.Generic;

using UnityEngine;

namespace Code.Runtime {
    public class Cluster : Decoration {
        public List<Decoration> Decorations;
        public Vector2Int       AmountRange;
        public Vector2          RadiusRange;

        /// <summary>
        /// TODO: Ask Nicole for a fancy method that will fit shapes inside of shapes! (circles inside of a circle should be fine)
        /// </summary>
        public void Populate() {
            var actualAmount = Random.Range(AmountRange.x, AmountRange.y);
            var actualRadius = Random.Range(RadiusRange.x, RadiusRange.y);
            for (int i = 0; i < actualAmount; i++) {
                var randomDecoration = Decorations[Random.Range(0, Decorations.Count)];
                var newDec           = Instantiate(randomDecoration, transform, false);
                var decPos           = Random.insideUnitCircle * actualRadius;
                newDec.transform.localPosition = new Vector3(decPos.x, 0, decPos.y);
            }
        }
    }
}
