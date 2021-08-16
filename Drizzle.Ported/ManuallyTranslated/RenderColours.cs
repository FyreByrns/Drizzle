using System;
using Drizzle.Lingo.Runtime;

namespace Drizzle.Ported {
    public sealed partial class renderColors {
        public void exitframe() {
            if (_movieScript.gViewRender) {
                if (_global._key.keypressed(48) &
                    LingoGlobal.op_ne_b(
                        _global._movie.window.sizestate,
                        new LingoSymbol("minimized"))) {
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
            LingoNumber q = 0;
            LingoPoint displacement = new();
            LingoNumber otherFogFac;
            LingoNumber rainbowFac;

            _global.sprite(59).locv = _movieScript.c - 8;

            for (int tmp_q = 1; tmp_q <= 1400; tmp_q++) {
                q = tmp_q;
                int layer = 1;
                LingoImage finalImage = _global.member("finalImage").image;
                LingoColor col = finalImage.getpixel(q - 1, _movieScript.c - 1);


                if (col.green > 7 & col.green < 11) {
                } else if (col == new LingoColor(0, 11, 0)) {
                    finalImage.setpixel(q - 1, _movieScript.c - 1, new LingoColor(10, 0, 0));
                } else {
                    if (col == LingoColor.Presets.White) {
                        layer = 0;
                    }

                    int c = (int)_movieScript.c;
                    LingoImage fogImage = _global.member("fogImage").image;
                    LingoNumber lowResDepth = (int)_movieScript.dptsL.getpos(_global.member("dpImage").image.getpixel(q - 1, c - 1));
                    LingoNumber fgdp = (int)_movieScript.fogDptsL.getpos(fogImage.getpixel(q - 1, c - 1));
                    LingoNumber fogFac = 255 - fogImage.getpixel(q - 1, c - 1).red / 255d;
                    fogFac = (fogFac - .0275) * 1d / .9411;
                    rainbowFac = 0;

                    if (fogFac <= .2) {
                        foreach (LingoPoint tmp_displacement in new LingoPoint[] {
                            new(-2,0),
                            new(0,-2),
                            new(+2,0),
                            new(0,+2),
                            new(-1,-1),
                            new(+1,-1),
                            new(+1,+1),
                            new(-1,+1)
                        }) {
                            displacement = tmp_displacement;

                            otherFogFac = 255 - fogImage.getpixel(
                                _movieScript.restrict(q - 1 + displacement.loch, 0, 1339),
                                _movieScript.restrict(c - 1, 0, 799)).red;

                            rainbowFac = rainbowFac + (Math.Abs((double)fogFac - (double)otherFogFac) > .0333 ? 1 : 0)
                                * 1d / .9411;

                            if (rainbowFac > 5) {
                                break;
                            }
                        }
                        rainbowFac = rainbowFac > 5 ? 1 : 0;
                    }


                }
            }
        }
    }
}
