using System;
using Drizzle.Lingo.Runtime;

namespace Drizzle.Ported {
    public sealed partial class MovieScript {
        /// <summary>
        /// Clamp
        /// </summary>
        public LingoNumber restrict(LingoNumber val, LingoNumber low, LingoNumber high) {
            if (val < low) return low;
            if (val > high) return high;
            return val;
        }
    }
}
