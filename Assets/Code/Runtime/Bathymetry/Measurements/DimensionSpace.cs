using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public enum DimensionSpace {
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
}