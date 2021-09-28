﻿using System.Buffers;
using System.Collections;
using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Drizzle.Editor.ViewModels;
using Drizzle.Lingo.Runtime;
using Drizzle.Ported;
using ReactiveUI;
using Brushes = Avalonia.Media.Brushes;

namespace Drizzle.Editor.Views
{
    public sealed class LevelPreviewControl : ReactiveUserControl<EditorContentViewModel>
    {
        private const float TileSize = 5;

        private Brush _brushLayer3 = new SolidColorBrush(Colors.DarkGray);
        private Brush _brushLayer2 = new SolidColorBrush(Colors.Gray);
        private Brush _brushLayer1 = new SolidColorBrush(Colors.Black);
        private Pen _penNone = new Pen(Brushes.Transparent, 0f);

        public LevelPreviewControl()
        {
            this.WhenActivated(disposables =>
            {
                var mv = ViewModel!.Runtime.MovieScript();
                Width = (int)mv.gLOprops.size.loch * TileSize;
                Height = (int)mv.gLOprops.size.locv * TileSize;
            });

            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var mv = ViewModel!.Runtime.MovieScript();

            var sizeMaxX = (int)mv.gLOprops.size.loch;
            var sizeMaxY = (int)mv.gLOprops.size.locv;

            for (var layer = 3; layer > 0; layer--)
            {
                var brush = layer switch
                {
                    1 => _brushLayer1,
                    2 => _brushLayer2,
                    _ => _brushLayer3,
                };

                var geometry = new StreamGeometry();
                var gCtx = geometry.Open();

                for (var x = 1; x <= sizeMaxX; x++)
                {
                    var column = mv.gLEProps.matrix[x];
                    for (var y = 1; y <= sizeMaxY; y++)
                    {
                        var offsetX = x * TileSize;
                        var offsetY = y * TileSize;

                        var dat = column[y][layer];
                        switch ((TileGeometry)(int)dat[1])
                        {
                            case TileGeometry.Air:
                                break;
                            case TileGeometry.SolidWall:
                                gCtx.BeginFigure(new Point(offsetX, offsetY), true);
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY));
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize));
                                gCtx.LineTo(new Point(offsetX, offsetY + TileSize));
                                gCtx.EndFigure(true);
                                break;
                            case TileGeometry.SlopeBL:
                                gCtx.BeginFigure(new Point(offsetX, offsetY), true);
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize));
                                gCtx.LineTo(new Point(offsetX, offsetY + TileSize));
                                gCtx.EndFigure(true);
                                break;
                            case TileGeometry.SlopeBR:
                                gCtx.BeginFigure(new Point(offsetX + TileSize, offsetY), true);
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize));
                                gCtx.LineTo(new Point(offsetX, offsetY + TileSize));
                                gCtx.EndFigure(true);
                                break;
                            case TileGeometry.SlopeTL:
                                gCtx.BeginFigure(new Point(offsetX, offsetY), true);
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY));
                                gCtx.LineTo(new Point(offsetX, offsetY + TileSize));
                                gCtx.EndFigure(true);
                                break;
                            case TileGeometry.SlopeTR:
                                gCtx.BeginFigure(new Point(offsetX, offsetY), true);
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY));
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize));
                                gCtx.EndFigure(true);
                                break;
                            case TileGeometry.Floor:
                                gCtx.BeginFigure(new Point(offsetX, offsetY), true);
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY));
                                gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize / 2));
                                gCtx.LineTo(new Point(offsetX, offsetY + TileSize / 2));
                                gCtx.EndFigure(true);
                                break;
                            case TileGeometry.Glass:
                                break;
                        }

                        var features = (LingoList) dat[2];
                        foreach (var feature in features.List)
                        {
                            switch ((TileFeature)(int)(LingoNumber)feature!)
                            {
                                case TileFeature.BeamHorizontal:
                                    gCtx.BeginFigure(new Point(offsetX, offsetY + TileSize * 2f / 5f), true);
                                    gCtx.LineTo(new Point(offsetX, offsetY + TileSize * 3f / 5f));
                                    gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize * 3f / 5f));
                                    gCtx.LineTo(new Point(offsetX + TileSize, offsetY + TileSize * 2f / 5f));
                                    gCtx.EndFigure(true);
                                    break;
                                case TileFeature.BeamVertical:
                                    gCtx.BeginFigure(new Point(offsetX + TileSize * 2f / 5f, offsetY), true);
                                    gCtx.LineTo(new Point(offsetX + TileSize * 3f / 5f, offsetY));
                                    gCtx.LineTo(new Point(offsetX + TileSize * 3f / 5f, offsetY + TileSize));
                                    gCtx.LineTo(new Point(offsetX + TileSize * 2f / 5f, offsetY + TileSize));
                                    gCtx.EndFigure(true);
                                    break;
                            }
                        }
                    }
                }

                context.DrawGeometry(brush, _penNone, geometry);
            }
        }
    }
}
