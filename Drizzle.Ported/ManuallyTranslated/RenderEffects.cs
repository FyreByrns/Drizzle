using System;
using Drizzle.Lingo.Runtime;

using System.Collections.Generic;

namespace Drizzle.Ported {
    public sealed partial class renderEffects {
        public void exitframe() {
            if (_movieScript.gViewRender) {
                if (_global._key.keypressed(48) & !_global._movie.window.Minimized) {
                    _global.go(9);
                }

                newframe();

                if (_movieScript.keepLooping) {
                    _global.go(_global.the_frame);
                }
            } else {
                while (_movieScript.keepLooping) {
                    newframe();
                }
            }
        }

        public void newframe() {
            _movieScript.vertRepeater++;

            if (_movieScript.gEEprops.effects.count == 0) {
                _movieScript.keepLooping = false;
                return;
            }

            if (_movieScript.r == 0) {
                _movieScript.vertRepeater = 1;
                _movieScript.r = 1;
                initeffect();
            }

            if ((_movieScript.vertRepeater > 60 &
                _movieScript.gEEprops.effects[_movieScript.r].crossscreen == 0) | (
                _movieScript.vertRepeater > _movieScript.gLOprops.size.locv &
                _movieScript.gEEprops.effects[_movieScript.r].crossscreen == 1
                )) {
                exiteffect();
                _movieScript.r++;

                if (_movieScript.r > _movieScript.gEEprops.effects.count) {
                    _movieScript.keepLooping = false;
                    return;
                }

                initeffect();
                _movieScript.vertRepeater = 1;
            }

            if (_movieScript.gEEprops.effects[_movieScript.r].crossscreen == 0) {
                _global.sprite(59).locv = _movieScript.vertRepeater * 20;

                for (int q = 1; q <= 100; q++) {
                    LingoNumber q2 = q + _movieScript.gRenderCameraTilePos.loch;
                    LingoNumber c2 = _movieScript.vertRepeater + _movieScript.gRenderCameraTilePos.locv;

                    if (q2 > 0 & q2 <= _movieScript.gLOprops.size.loch &
                        c2 > 0 & c2 <= _movieScript.gLOprops.size.locv) {
                        effectontile(q, _movieScript.vertRepeater, q2, c2);
                    }
                }
            } else {
                _global.sprite(59).locv =
                    (_movieScript.vertRepeater - _movieScript.gRenderCameraTilePos.locv) * 20;

                for (int q = 1; q <= _movieScript.gLOprops.size.loch; q++) {
                    effectontile(q - _movieScript.gRenderCameraTilePos.loch,
                        _movieScript.vertRepeater - _movieScript.gRenderCameraTilePos.locv,
                        q,
                        _movieScript.vertRepeater);
                }
            }
        }

        public void effectontile(LingoNumber q, LingoNumber c, LingoNumber q2, LingoNumber c2) {
            if (_movieScript.gEEprops.effects[_movieScript.r].mtrx[q2][c2] > 0) {
                LingoNumber savseed = _global.the_randomSeed;
                _global.the_randomSeed = _movieScript.seedfortile(new LingoPoint(q2, c2), _movieScript.effectSeed);

                string nm = _movieScript.gEEprops.effects[_movieScript.r].nm;
                string currentEffectName = nm.ToLowerInvariant();

                switch (currentEffectName) {
                    case "slime":
                    case "rust":
                    case "barnacles":
                    case "erode":
                    case "melt":
                    case "roughen":
                    case "slimex3":
                    case "destructive melt":
                    case "super melt":
                    case "super erode":
                    case "decalsonlyslime": {
                            applystandarderosion(q, c, 0, nm);
                            break;
                        }
                    case "root grass":
                    case "cacti":
                    case "rubble":
                    case "rain moss":
                    case "seed pods":
                    case "grass":
                    case "horse tails":
                    case "circuit plants":
                    case "feather plants": {
                            applystandardplant(q, c, 0, nm);
                            break;
                        }
                    case "growers": {
                            if(_global.random(100) < _movieScript.gEEprops.effects[_movieScript.r].mtrx[q2][c2] &
                               _global.random(2) == 1) {
                                applyhugeflower(q, c, 0);
                            }
                            break;
                        }
                }
            }
        }
    }
}
