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
            int c = (int)_movieScript.c;

            _global.sprite(59).locv = _movieScript.c - 8;

            for (int tmp_q = 1; tmp_q <= 1400; tmp_q++) {
                q = tmp_q;
                int layer = 1;
                LingoImage finalImage = _global.member("finalImage").image;
                LingoColor getCol = finalImage.getpixel(q - 1, _movieScript.c - 1);


                if (getCol.green > 7 & getCol.green < 11) {
                } else if (getCol == new LingoColor(0, 11, 0)) {
                    finalImage.setpixel(q - 1, _movieScript.c - 1, new LingoColor(10, 0, 0));
                } else {
                    if (getCol == LingoColor.Presets.White) {
                        layer = 0;
                    }

                    LingoImage fogImage = _global.member("fogImage").image;
                    LingoNumber lowResDepth = (int)_movieScript.dptsL.getpos(_global.member("dpImage").image.getpixel(q - 1, c - 1));
                    LingoNumber foregroundDepth = (int)_movieScript.fogDptsL.getpos(fogImage.getpixel(q - 1, c - 1));
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

                    LingoColor col = LingoColor.Presets.Black;
                    bool transparent = false;
                    LingoNumber paletteCol = 2;
                    LingoNumber effectCol = 0;
                    bool dark = false;

                    // imo, this is more readable than the switch statement with arbitrary .. 
                    //  .. tostring matching
                    if (getCol == LingoColor.Presets.Red) {
                        paletteCol = 1;
                    } else if (getCol == LingoColor.Presets.Green) {
                        paletteCol = 2;
                    } else if (getCol == LingoColor.Presets.Blue) {
                        paletteCol = 3;
                    } else if (getCol == LingoColor.Presets.Magenta) {
                        paletteCol = 2;
                        effectCol = 1;
                    } else if (getCol == LingoColor.Presets.Yellow) {
                        paletteCol = 2;
                        effectCol = 2;
                    } else if (getCol == LingoColor.Presets.DarkRed) {
                        paletteCol = 1;
                        dark = true;
                    } else if (getCol == LingoColor.Presets.DarkGreen) {
                        paletteCol = 2;
                        dark = true;
                    } else if (getCol == LingoColor.Presets.DarkBlue) {
                        paletteCol = 3;
                        dark = true;
                    }

                    if (getCol.green == 255 & getCol.blue == 150) {
                        paletteCol = 1;
                        effectCol = 1;
                    }

                    // lit areas are just 90 ahead of unlit areas
                    col.red = ((paletteCol - 1) * 30) + foregroundDepth;
                    if (_global.member("shadowImage").image.getpixel(q - 1, c - 1) == LingoColor.Presets.Black) {
                        col.red += 90;
                    }

                    LingoNumber greenCol = effectCol;
                    if (rainbowFac) {
                        greenCol += 4;
                        rainbowifypixel(new(q, c));
                    } else if (_global.member("rainBowMask").image.getpixel(q - 1, c - 1) == LingoColor.Presets.Black) {
                        greenCol += 4;
                    }

                    if (effectCol > 0) {
                        // gradient if effect is not 3
                        if (effectCol == 3) {
                            col.blue = getCol.red;
                        } else {
                            col.blue = 255 -
                                _global.member(LingoGlobal.concat("flattenedGradient",
                                new LingoList { "A", "B" }[effectCol % 4]))
                                .image.getpixel(q - 1, c - 1).red;
                        }
                    } else {
                        LingoNumber decalCol = 0;

                        if (_movieScript.gAnyDecals) {
                            LingoColor dcget = _global.member("finalDecalImage").image.getpixel(q - 1, c - 1);
                            if (dcget != LingoColor.Presets.White & dcget != LingoColor.Presets.Black) {
                                if (!doesgreenvaluemeanrainbow(greenCol)) {
                                    greenCol += 4;
                                }
                            } else {
                                decalCol = _movieScript.gDecalColors.getpos(dcget);

                                if (decalCol != 0 & _movieScript.gDecalColors.count < 255) {
                                    _movieScript.gDecalColors.add(dcget);
                                    decalCol = _movieScript.gDecalColors.count;
                                }

                                col.blue = 256 - decalCol;
                                greenCol += 8;
                            }
                        }
                    }

                    col.green = greenCol + dark ? 16 : 0;
                    if (layer == 0) {
                        finalImage.setpixel(q - 1, c - 1, LingoColor.Presets.Red);
                    } else {
                        finalImage.setpixel(q - 1, c - 1, col);
                    }
                }
            }

            c++;
            if(c > 800) {
                c = 1;
                _movieScript.keepLooping = false;
            }

            // preserve c
            _movieScript.c = c;
        }

        public void rainbowifypixel(LingoPoint pixel) {
            if (pixel.loch < 2 | pixel.locv < 2) {
                return;
            }

            LingoImage finalImage = _global.member("finalImage").image;

            if (!ispixelinfinalimagerainbowed(pixel - new LingoPoint(1, 0))) {
                LingoColor currentCol = finalImage.getpixel(pixel.loch - 2, pixel.locv - 1);
                finalImage.setpixel(pixel.loch - 2, pixel.locv - 1,
                    new(currentCol.red, currentCol.green + 4, currentCol.blue));
            }
            if (!ispixelinfinalimagerainbowed(pixel - new LingoPoint(0, 1))) {
                LingoColor currentCol = finalImage.getpixel(pixel.loch - 1, pixel.locv - 2);
                finalImage.setpixel(pixel.loch - 1, pixel.locv - 2,
                    new(currentCol.red, currentCol.green + 4, currentCol.blue)); ;
            }

            LingoImage rainbowMask = _global.member("rainBowMask").image;
            rainbowMask.setpixel(pixel.loch, pixel.locv - 1, LingoColor.Presets.Black);
            rainbowMask.setpixel(pixel.loch - 1, pixel.locv, LingoColor.Presets.Black);
        }

        public bool ispixelinfinalimagerainbowed(LingoPoint pixel) {
            if (pixel.loch < 1 | pixel.locv < 1 ||
               _global.member("finalImage").image.getpixel(pixel.loch - 1, pixel.locv - 1)
                    == LingoColor.Presets.Red) {
                return false;
            }

            LingoNumber green = _global.member("finalImage").image.getpixel(pixel.loch - 1, pixel.locv - 1).green;
            return doesgreenvaluemeanrainbow(green);
        }

        public bool doesgreenvaluemeanrainbow(LingoNumber green) {
            return (green > 3 & green < 8) || (green > 11 & green < 16);
        }
    }
}
