using System;
using Drizzle.Lingo.Runtime;

namespace Drizzle.Ported {
    public sealed partial class MovieScript {
        [LingoGlobal] public LingoRect[] lightRects;
        [LingoGlobal] public LingoNumber fac;
        [LingoGlobal] public bool gViewRender;
        [LingoGlobal] public bool keepLooping;
        [LingoGlobal] public LingoNumber c;
        [LingoGlobal] public LingoList dptsL;
        [LingoGlobal] public LingoList fogDptsL;
    }
}
