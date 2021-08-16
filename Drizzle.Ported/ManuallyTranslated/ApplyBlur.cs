using System;
using Drizzle.Lingo.Runtime;

namespace Drizzle.Ported {
    public sealed partial class applyBlur {
        public void exitframe() {
            // wrapped in an if(0)
        }

        public void newframe() {
            _global.sprite(59).locv = _movieScript.c - 8;
        }

        // doesn't appear to ever be called
        public void changelightrect(int lr, LingoPoint pnt) {
            if (pnt.loch < _movieScript.lightRects[lr].left) {
                _movieScript.lightRects[lr].left = pnt.loch;
            }
            if (pnt.loch > _movieScript.lightRects[lr].right) {
                _movieScript.lightRects[lr].right = pnt.loch;
            }
            if (pnt.locv < _movieScript.lightRects[lr].top) {
                _movieScript.lightRects[lr].top = pnt.locv;
            }
            if (pnt.locv > _movieScript.lightRects[lr].bottom) {
                _movieScript.lightRects[lr].bottom = pnt.locv;
            }
            _global.sprite((10 + lr)).rect = (_movieScript.lightRects[lr] + LingoGlobal.rect(-8, -16, -8, -16));
        }
    }
}
