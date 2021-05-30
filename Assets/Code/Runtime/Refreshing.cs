using System;

using UnityEngine;

namespace Code.Runtime {
    /// <summary>
    /// In theory, this will produce a value that can be called as much as you want but will only ever be evaluated if it <see cref="IsStale"/>!
    ///
    /// This is UNFINISHED, and needs to be moved into <see cref="BrandonUtils"/>
    /// </summary>
    /// <example>
    /// I want to retrieve the leader in a race, once per frame.
    /// <ul>
    /// <li>
    /// <see cref="ValueSupplier"/>: <c>() => FindLeadingPlayer();</c>
    /// </li>
    /// <li>
    /// <see cref="StalenessSupplier"/>
    /// </li>
    /// </ul>
    /// </example>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TStaleness"></typeparam>
    public class Refreshing<TValue, TStaleness> {
        /// <summary>
        /// The expensive <see cref="Func{TResult}"/> that generates the <see cref="_value"/>.
        /// </summary>
        public Func<TValue> ValueSupplier { get; set; }

        /// <summary>
        /// The <see cref="Func{TResult}"/> that generates the <see cref="StalenessBasis"/>.
        /// </summary>
        /// <example>
        /// To refresh once per frame:
        /// </example>
        public Func<TStaleness> StalenessSupplier { get; set; }
        /// <summary>
        /// The <see cref="Func{T, TResult}"/> that genera
        /// </summary>
        public Func<TStaleness, bool> StalenessCheck { get; set; }
        public bool IsStale => StalenessCheck.Invoke(PreviousStalenessBasis).CompareTo(StalenessCheck.Invoke(CurrentStalenessBasis)) != 0;

        public  TStaleness CurrentStalenessBasis  => StalenessSupplier.Invoke();
        public  TStaleness PreviousStalenessBasis { get; set; }
        private TValue     _value;

        public Refreshing(Func<TValue> valueSupplier, Func<TStaleness> stalenessSupplier, Func<TStaleness, bool> stalenessCheck) {
            this.ValueSupplier     = valueSupplier;
            this.StalenessSupplier = stalenessSupplier;
            this.StalenessCheck    = stalenessCheck;
        }

        public TValue Peek() {
            return _value;
        }

        public TValue Refresh() {
            _value = ValueSupplier.Invoke();
            return Peek();
        }

        public TValue Get() {
            return IsStale ? Refresh() : Peek();
        }

        //region Factory Methods
        public static Refreshing<TValue, int> PerFrame(Func<TValue> supplier) {
            return new Refreshing<TValue, int>(
                supplier,
                () => Time.frameCount,
                frame => Time.frameCount > frame
            );
        }
        //endregion
    }
}
