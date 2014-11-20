//g_ChartLabels = [1, 2, 3, 4, 5, 6, 7, 33, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22];
//g_ChartData = [1, 2, 3, 4, 5, 6, 7, 44, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22];
//g_ChartWidth = 550;
//g_ChartHeight = 250;
//g_ChartTop = 20;
//g_ChartLeft = 10;
//g_ChartID = "graphDiv";
//g_ChartPopupDataFunc = function (data) { return data + " datatext"; }
//g_ChartPopupLabelFunc = function (lbl) { return lbl + " labeltext"; }
function VF_InitializeGraph(_DataX, _DataYs, _DataYsColors, _Width, _Height, _Top, _Left, _GraphDivID, _GraphPopupDataFunc) {
    Raphael.fn.drawGrid = function (x, y, w, h, wv, hv, color) {
        color = color || "#000";
        var path = ["M", Math.round(x) + .5, Math.round(y) + .5, "L", Math.round(x + w) + .5, Math.round(y) + .5, Math.round(x + w) + .5, Math.round(y + h) + .5, Math.round(x) + .5, Math.round(y + h) + .5, Math.round(x) + .5, Math.round(y) + .5],
            rowHeight = h / hv,
            columnWidth = w / wv;
        for (var i = 1; i < hv; i++) {
            path = path.concat(["M", Math.round(x) + .5, Math.round(y + i * rowHeight) + .5, "H", Math.round(x + w) + .5]);
        }
        for (i = 1; i < wv; i++) {
            path = path.concat(["M", Math.round(x + i * columnWidth) + .5, Math.round(y) + .5, "V", Math.round(y + h) + .5]);
        }
        return this.path(path.join(",")).attr({ stroke: color });
    };
    $(function () {
        function getAnchors(p1x, p1y, p2x, p2y, p3x, p3y) {
            var l1 = (p2x - p1x) / 2,
                l2 = (p3x - p2x) / 2,
                a = Math.atan((p2x - p1x) / Math.abs(p2y - p1y)),
                b = Math.atan((p3x - p2x) / Math.abs(p2y - p3y));
            a = p1y < p2y ? Math.PI - a : a;
            b = p3y < p2y ? Math.PI - b : b;
            var alpha = Math.PI / 2 - ((a + b) % (Math.PI * 2)) / 2,
                dx1 = l1 * Math.sin(alpha + a),
                dy1 = l1 * Math.cos(alpha + a),
                dx2 = l2 * Math.sin(alpha + b),
                dy2 = l2 * Math.cos(alpha + b);
            return {
                x1: p2x - dx1,
                y1: p2y + dy1,
                x2: p2x + dx2,
                y2: p2y + dy2
            };
        }

        var g_DataX = _DataX; //array of int
        var g_DataYs = _DataYs; //array of arrays of int
        var g_DataYsColors = _DataYsColors; //array of string

        var g_GraphWidth = _Width; //int
        var g_GraphHeight = _Height; //int
        var g_GraphTop = _Top; //int
        var g_GraphLeft = _Left; //int

        var g_GraphDivID = _GraphDivID; //string
        var g_GraphPopupDataFunc = _GraphPopupDataFunc; //function(i) //i => g_DataX[i], g_DataY[*][i]
        
        var g_GraphBot = 5;

        var maxXval = g_DataX.length;//Math.max.apply(Math, g_DataX);
        var factorX = (g_GraphWidth - g_GraphLeft) / maxXval;

        var factorY = new Array(g_DataYs.length);
        for (var i = 0; i < g_DataYs.length; ++i) {
            var maxYval = Math.max.apply(Math, g_DataYs[i]);
            factorY[i] = (g_GraphHeight - g_GraphBot - g_GraphTop) / maxYval;
        }

        var drawObject = Raphael(g_GraphDivID, g_GraphWidth, g_GraphHeight);
        drawObject.drawGrid(g_GraphLeft + factorX * .5 + .5, g_GraphTop + .5, g_GraphWidth - g_GraphLeft - factorX, g_GraphHeight - g_GraphTop - g_GraphBot, 10, 10, "#000");
        
        var g_Paths = new Array(g_DataYs.length);
        var g_PathBgs = new Array(g_DataYs.length);
        for(var i = 0; i < g_DataYs.length; ++i){
            g_Paths[i] = drawObject.path().attr({ stroke: g_DataYsColors[i], "stroke-width": 2, "stroke-linejoin": "round" });
            g_PathBgs[i] = drawObject.path().attr({ stroke: "none", opacity: .1, fill: g_DataYsColors[i] });
        }

        var txtFont = { font: '12px Helvetica, Arial', fill: "#fff" };
        var label = drawObject.set();
        label.push(drawObject.text(60, 12, g_GraphPopupDataFunc(0)).attr(txtFont));
        label.hide();

        var labelFrame = drawObject.popup(0, 0, label, "left").attr({ fill: "#000", stroke: "#666", "stroke-width": 2, "fill-opacity": .7 }).hide();
        var g_Leave_Timer = null;

        var hoverRects = drawObject.set();

        var timeLine = drawObject.path("M0,0L0," + (g_GraphHeight - g_GraphBot)).attr({ stroke: "#0000ff", "stroke-width": 3 }).hide();

        var paths, pathBgs;
        for (var i = 0; i < g_DataX.length; i++) {
            var y = new Array(g_DataYs.length);
            for(var u = 0; u < g_DataYs.length; ++u){
                y[u] = Math.round(g_GraphHeight - g_GraphBot - factorY[u] * g_DataYs[u][i]);
            }
            var x = Math.round(g_GraphLeft + factorX * (i + .5));
            //var t = r.text(x, height - 6, g_ChartLabels[i]).attr(txt).toBack();
            if (i == 0) {
                paths = new Array(g_DataYs.length);
                pathBgs = new Array(g_DataYs.length);
                for(var u = 0; u < g_DataYs.length; ++u){
                    paths[u] = ["M", x, y[u], "C", x, y[u]];
                    pathBgs[u] = ["M", g_GraphLeft + factorX * .5, g_GraphHeight - g_GraphBot, "L", x, y[u], "C", x, y[u]];
                }
            }
            else if (i > 0 && i < g_DataX.length - 1) {
                for (var u = 0; u < g_DataYs.length; ++u) {
                    var Y0 = Math.round(g_GraphHeight - g_GraphBot - factorY[u] * g_DataYs[u][i - 1]),
                        X0 = Math.round(g_GraphLeft + factorX * (i - .5)),
                        Y2 = Math.round(g_GraphHeight - g_GraphBot - factorY[u] * g_DataYs[u][i + 1]),
                        X2 = Math.round(g_GraphLeft + factorX * (i + 1.5));
                    var a = getAnchors(X0, Y0, x, y[u], X2, Y2);
                    paths[u] = paths[u].concat([a.x1, a.y1, x, y[u], a.x2, a.y2]);
                    pathBgs[u] = pathBgs[u].concat([a.x1, a.y1, x, y[u], a.x2, a.y2]);
                }
            }
            else {
                for (var u = 0; u < g_DataYs.length; ++u) {
                    paths[u] = paths[u].concat([x, y[u], x, y[u]]);
                    pathBgs[u] = pathBgs[u].concat([x, y[u], x, y[u], "L", x, g_GraphHeight - g_GraphBot, "z"]);
                }
            }
            var dots = new Array(g_DataYs.length);
            for(var u = 0; u < g_DataYs.length; ++u){
                dots[u] = drawObject.circle(x, y[u], 3).attr({ fill: "#333", stroke: g_DataYsColors[u], "stroke-width": 2 });
            }
            hoverRects.push(drawObject.rect(g_GraphLeft + factorX * i, 0, factorX, g_GraphHeight - g_GraphBot).attr({ stroke: "none", fill: "#fff", opacity: 0 }));
            var currHoverRect = hoverRects[hoverRects.length - 1];
            var addHover = (function (x, y, i, dots) {
                currHoverRect.hover(function () {
                    g_GraphPopupDataFunc(i);
                    //clearTimeout(g_Leave_Timer);
                    //var side = "right";
                    //if (x + labelFrame.getBBox().width > g_GraphWidth) {
                    //    side = "left";
                    //}
                    //labelFrame = drawObject.popup(x, parseInt(g_GraphHeight / 2), label, side, 1).show();
                    //lx = label[0].transform()[0][1] + labelFrame.dx;
                    //ly = label[0].transform()[0][2] + labelFrame.dy;
                    //label[0].attr({ "text": _GraphPopupDataFunc(dataX, dataYs)/*, 'x': x, 'y': 0 */ }).transform("t" + lx + "," + ly).show();
                    timeLine.transform("t" + x + ",0");
                    for (var u = 0; u < g_DataYs.length; ++u) {
                        dots[u].attr("r", 5);
                    }
                    timeLine.show();
                }, function () {
                    for (var u = 0; u < g_DataYs.length; ++u) {
                        dots[u].attr("r", 3);
                    }
                    //timeLine.hide();
                    //g_Leave_Timer = setTimeout(function () {
                    //    labelFrame.hide();
                    //    label[0].hide();
                    //}, 1);
                });
            });

            //var currDataYs = new Array(g_DataYs.length);
            //for (var u = 0; u < g_DataYs.length; ++u) {
            //    currDataYs[u] = g_DataYs[u][i];
            //}
            addHover(x, y, i, dots);
        }
        for (var u = 0; u < g_DataYs.length; ++u) {
            g_Paths[u].attr({ path: paths[u] });
            g_PathBgs[u].attr({ path: pathBgs[u] });
        }
        //labelFrame.toFront();
        //label[0].toFront();
        hoverRects.toFront();
    });
}

function InitializeChart(_Labels, _Data, _Width, _Height, _Top, _Left, _ChartDivID, _ChartPopupDataFunc, _ChartPopupLabelFunc, _SecondData) {
    Raphael.fn.drawGrid = function (x, y, w, h, wv, hv, color) {
        color = color || "#000";
        var path = ["M", Math.round(x) + .5, Math.round(y) + .5, "L", Math.round(x + w) + .5, Math.round(y) + .5, Math.round(x + w) + .5, Math.round(y + h) + .5, Math.round(x) + .5, Math.round(y + h) + .5, Math.round(x) + .5, Math.round(y) + .5],
            rowHeight = h / hv,
            columnWidth = w / wv;
        for (var i = 1; i < hv; i++) {
            path = path.concat(["M", Math.round(x) + .5, Math.round(y + i * rowHeight) + .5, "H", Math.round(x + w) + .5]);
        }
        for (i = 1; i < wv; i++) {
            path = path.concat(["M", Math.round(x + i * columnWidth) + .5, Math.round(y) + .5, "V", Math.round(y + h) + .5]);
        }
        return this.path(path.join(",")).attr({ stroke: color });
    };

    $(function () {
        var g_ChartLabels = _Labels;
        var g_ChartData = _Data;
        var g_ChartWidth = _Width;
        var g_ChartHeight = _Height;
        var g_ChartTop = _Top;
        var g_ChartLeft = _Left;
        var g_ChartID = _ChartDivID;
        var g_ChartPopupDataFunc = _ChartPopupDataFunc;
        var g_ChartPopupLabelFunc = _ChartPopupLabelFunc;
        var g_ChartSecondData = _SecondData;
        function getAnchors(p1x, p1y, p2x, p2y, p3x, p3y) {
            var l1 = (p2x - p1x) / 2,
                l2 = (p3x - p2x) / 2,
                a = Math.atan((p2x - p1x) / Math.abs(p2y - p1y)),
                b = Math.atan((p3x - p2x) / Math.abs(p2y - p3y));
            a = p1y < p2y ? Math.PI - a : a;
            b = p3y < p2y ? Math.PI - b : b;
            var alpha = Math.PI / 2 - ((a + b) % (Math.PI * 2)) / 2,
                dx1 = l1 * Math.sin(alpha + a),
                dy1 = l1 * Math.cos(alpha + a),
                dx2 = l2 * Math.sin(alpha + b),
                dy2 = l2 * Math.cos(alpha + b);
            return {
                x1: p2x - dx1,
                y1: p2y + dy1,
                x2: p2x + dx2,
                y2: p2y + dy2
            };
        }

        // Draw
        var width = g_ChartWidth,
            height = g_ChartHeight,
            leftgutter = g_ChartLeft,
            bottomgutter = 20,
            topgutter = g_ChartTop,
            colorhue = .6 || Math.random(),
            color = "hsl(" + [colorhue, .5, .5] + ")",
            color2 = "hsl(" + [.5, colorhue, .5] + ")",
            r = Raphael(g_ChartID, width, height),
            txt = { font: '12px Helvetica, Arial', fill: "#fff" },
            txt1 = { font: '10px Helvetica, Arial', fill: "#fff" },
            txt2 = { font: '12px Helvetica, Arial', fill: "#000" },
            X = (width - leftgutter) / g_ChartLabels.length;

        var max = Math.max.apply(Math, g_ChartData);
        if (g_ChartSecondData != null) {
            max = Math.max(max, Math.max.apply(Math, g_ChartSecondData));
        }
        var Y = (height - bottomgutter - topgutter) / max;
        r.drawGrid(leftgutter + X * .5 + .5, topgutter + .5, width - leftgutter - X, height - topgutter - bottomgutter, 10, 10, "#000");
        var path = r.path().attr({ stroke: color, "stroke-width": 4, "stroke-linejoin": "round" }),
            bgp = r.path().attr({ stroke: "none", opacity: .3, fill: color }),
            label = r.set(),
            lx = 0, ly = 0,
            is_label_visible = false,
            leave_timer,
            blanket = r.set();
        var path2 = null;
        if (g_ChartSecondData != null) {
            path2 = r.path().attr({ stroke: color2, "stroke-width": 4, "stroke-linejoin": "round" });
        }

        label.push(r.text(60, 12, g_ChartPopupDataFunc(g_ChartData[0])).attr(txt));
        label.push(r.text(60, 27, g_ChartPopupDataFunc(g_ChartLabels[0])).attr(txt1).attr({ fill: color }));
        label.hide();
        var frame = r.popup(100, 100, label, "right").attr({ fill: "#000", stroke: "#666", "stroke-width": 2, "fill-opacity": .7 }).hide();

        var p, bgpp;
        var p2;
        for (var i = 0, ii = g_ChartLabels.length; i < ii; i++) {
            var y = Math.round(height - bottomgutter - Y * g_ChartData[i]),
                x = Math.round(leftgutter + X * (i + .5)),
                t = r.text(x, height - 6, g_ChartLabels[i]).attr(txt).toBack();
            var y2 = null;
            if (g_ChartSecondData != null) {
                y2 = Math.round(height - bottomgutter - Y * g_ChartSecondData[i]);
            }
            if (!i) {
                p = ["M", x, y, "C", x, y];
                bgpp = ["M", leftgutter + X * .5, height - bottomgutter, "L", x, y, "C", x, y];
                if (g_ChartSecondData != null) {
                    p2 = ["M", x, y2, "C", x, y2];
                }
            }
            if (i && i < ii - 1) {
                var Y0 = Math.round(height - bottomgutter - Y * g_ChartData[i - 1]),
                    X0 = Math.round(leftgutter + X * (i - .5)),
                    Y2 = Math.round(height - bottomgutter - Y * g_ChartData[i + 1]),
                    X2 = Math.round(leftgutter + X * (i + 1.5));
                var a = getAnchors(X0, Y0, x, y, X2, Y2);
                p = p.concat([a.x1, a.y1, x, y, a.x2, a.y2]);
                bgpp = bgpp.concat([a.x1, a.y1, x, y, a.x2, a.y2]);
                if (g_ChartSecondData != null) {
                    var a2 = getAnchors(X0, Y0, x, y2, X2, Y2);
                    p2 = p2.concat([a2.x1, a2.y1, x, y2, a2.x2, a2.y2]);
                }
            }
            var dot = r.circle(x, y, 4).attr({ fill: "#333", stroke: color, "stroke-width": 2 });
            var dot2 = null;
            if (g_ChartSecondData != null) {
                dot2 = r.circle(x, y2, 4).attr({ fill: "#333", stroke: color2, "stroke-width": 2 });
            }
            blanket.push(r.rect(leftgutter + X * i, 0, X, height - bottomgutter).attr({ stroke: "none", fill: "#fff", opacity: 0 }));
            var rect = blanket[blanket.length - 1];
            var addHover = (function (x, y, data, lbl, dot, dot2, data2) {
                var timer, i = 0;
                rect.hover(function () {
                    clearTimeout(leave_timer);
                    var side = "right";
                    if (x + frame.getBBox().width > width) {
                        side = "left";
                    }
                    var ppp = r.popup(x, y, label, side, 1),
                        anim = Raphael.animation({
                            path: ppp.path,
                            transform: ["t", ppp.dx, ppp.dy]
                        }, 200 * is_label_visible);
                    lx = label[0].transform()[0][1] + ppp.dx;
                    ly = label[0].transform()[0][2] + ppp.dy;
                    frame.show().stop().animate(anim);
                    if (data2 == null) {
                        label[0].attr({ text: g_ChartPopupDataFunc(data) }).show().stop().animateWith(frame, anim, { transform: ["t", lx, ly] }, 200 * is_label_visible);
                    }
                    else {
                        label[0].attr({ text: g_ChartPopupDataFunc(data) + " vs " + g_ChartPopupDataFunc(data2) }).show().stop().animateWith(frame, anim, { transform: ["t", lx, ly] }, 200 * is_label_visible);
                    }
                    label[1].attr({ text: g_ChartPopupLabelFunc(lbl) }).show().stop().animateWith(frame, anim, { transform: ["t", lx, ly] }, 200 * is_label_visible);
                    dot.attr("r", 6);
                    if (dot2 != null) {
                        dot2.attr("r", 6);
                    }
                    is_label_visible = true;
                }, function () {
                    dot.attr("r", 4);
                    if (dot2 != null) {
                        dot2.attr("r", 4);
                    }
                    leave_timer = setTimeout(function () {
                        frame.hide();
                        label[0].hide();
                        label[1].hide();
                        is_label_visible = false;
                    }, 1);
                });
            });
            if (g_ChartSecondData == null)
            {
                addHover(x, y, g_ChartData[i], g_ChartLabels[i], dot);
            }
            else
            {
                addHover(x, y, g_ChartData[i], g_ChartLabels[i], dot, dot2, g_ChartSecondData[i]);
            }
        }
        p = p.concat([x, y, x, y]);
        if (g_ChartSecondData != null) {
            p2 = p2.concat([x, y2, x, y2]);
        }
        bgpp = bgpp.concat([x, y, x, y, "L", x, height - bottomgutter, "z"]);
        path.attr({ path: p });
        if (g_ChartSecondData != null) {
            path2.attr({ path: p2 });
        }
        bgp.attr({ path: bgpp });
        frame.toFront();
        label[0].toFront();
        label[1].toFront();
        blanket.toFront();
    });
}