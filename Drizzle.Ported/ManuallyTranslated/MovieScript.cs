using System;
using Drizzle.Lingo.Runtime;

namespace Drizzle.Ported {
    public sealed partial class MovieScript {
        public LingoNumber diag(LingoPoint a, LingoPoint b) {
            LingoNumber h = a.locv - b.locv;
            LingoNumber w = a.loch - b.loch;
            return LingoGlobal.sqrt(w * w + h * h);
        }
    }
}
