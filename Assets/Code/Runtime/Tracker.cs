using System;
using System.Collections.Generic;
using System.Linq;

using BrandonUtils.Standalone.Exceptions;

namespace Code.Runtime {
    /// <summary>
    /// TODO: What's the word for "something that includes a record of its history"?!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct Tracker<T> where T : IComparable {
        public const int Smallest_History_Capacity = 1;
        public const int Default_History_Capacity  = 1;

        private         Stack<T>         _history;
        public          Stack<T>         History   => _history;
        public          IReadOnlyList<T> AllValues => History.Prepend(_current).ToList();
        public readonly int              HistoryCapacity;

        private T _current;
        public T Current {
            get => _current;
            set {
                _history.Push(_current);
                _history = new Stack<T>(_history.Take(HistoryCapacity));
                _current = value;
            }
        }
        public T   Previous  => _history.Peek();
        public int Direction => Previous.CompareTo(Current);

        public Func<T> Supplier;

        public Tracker(Func<T> supplier, T initialValue, int historyCapacity = Default_History_Capacity) : this() {
            if (historyCapacity <= Smallest_History_Capacity) {
                throw new BrandonException($"Cannot initialize a {nameof(Tracker<T>)} with a {nameof(historyCapacity)} of {historyCapacity}: The {nameof(historyCapacity)} must be at least {Smallest_History_Capacity} in order to track anything useful!");
            }

            this.HistoryCapacity = historyCapacity;
            this.Supplier        = supplier;
            this._history        = new Stack<T>(historyCapacity);
            this.Current         = initialValue;
        }

        public Tracker(Func<T> supplier, int historySize = Default_History_Capacity) : this(supplier, supplier.Invoke(), historySize) { }
    }
}