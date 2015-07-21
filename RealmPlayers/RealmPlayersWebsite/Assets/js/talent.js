/*
 talent.js version 278
 Differences from origin:
 
*/
var tc_loaded = false,
tc_object, tc_classId = -1,
tc_classIcons = {},
tc_build = "",
tc_glyphs = "";

var tc_chooseClasses = [
    1,
    2,
    3,
    4,
    5,
    7,
    8,
    9,
    11,
];
var tc_chooseClassesTBC = [
    21,
    22,
    23,
    24,
    25,
    27,
    28,
    29,
    31,
];
function tc_init() {
	var c;
	g_initPath([1, 0]);
	var e = g_sortJsonArray(g_chr_classes, g_chr_classes);
	c = ge("tc-classes-inner");
	var classTable = ce("table");
	var tableBody = ce("tbody");
	var tableRowHeader = ce("tr");
	var tableRow = ce("tr");
	var tableVanillaColumn = ce("td");
	var tableTBCColumn = ce("td");
	var tableVanillaHeaderColumn = ce("div");
	var tableTBCHeaderColumn = ce("div");

	var spaceColumn = ce("td");
	var spaceColumnDiv = ce("div");
	spaceColumnDiv.style.width = "5px";
	ae_AddElement(spaceColumn, spaceColumnDiv);

	st(tableVanillaHeaderColumn, "Classic");
	st(tableTBCHeaderColumn, "TBC");

	//ae_AddElement(tableRowHeader, tableVanillaHeaderColumn);
	//ae_AddElement(tableRowHeader, tableTBCHeaderColumn);
	//ae_AddElement(tableBody, tableRowHeader);
	ae_AddElement(tableRow, tableVanillaColumn);
	ae_AddElement(tableRow, spaceColumn);
	ae_AddElement(tableRow, tableTBCColumn);
	ae_AddElement(tableBody, tableRow);
	ae_AddElement(classTable, tableBody);
	var vanillaClasses = ce("div");
	ae_AddElement(vanillaClasses, tableVanillaHeaderColumn);
	vanillaClasses.className = "blackframe";
	vanillaClasses.style.height = "420px";
	for (var d = 0, b = tc_chooseClasses.length; d < b; ++d) {
	    var h = tc_chooseClasses[d],
		f = Icon.create("class_" + g_file_classes[h] + ".jpg", 1, null, "javascript:;"),
		g = Icon.getLink(f);
		tc_classIcons[h] = f;
		if (Browser.ie6) {
			g.onfocus = tb
		}
		g.onclick = tc_classClick.bind(g, h);
		g.onmouseover = tc_classOver.bind(g, h);
		g.onmouseout = Tooltip.hide;
		ae_AddElement(vanillaClasses, f)
	}
	ae_AddElement(tableVanillaColumn, vanillaClasses);
	var tbcClasses = ce("div");
	ae_AddElement(tbcClasses, tableTBCHeaderColumn);
	tbcClasses.className = "blackframe";
	tbcClasses.style.height = "420px";
	for (var d = 0, b = tc_chooseClassesTBC.length; d < b; ++d) {
	    var h = tc_chooseClassesTBC[d],
		f = Icon.create("class_" + g_file_classes[h] + ".jpg", 1, null, "javascript:;"),
		g = Icon.getLink(f);
	    tc_classIcons[h] = f;
	    if (Browser.ie6) {
	        g.onfocus = tb
	    }
	    g.onclick = tc_classClick.bind(g, h);
	    g.onmouseover = tc_classOver.bind(g, h);
	    g.onmouseout = Tooltip.hide;
	    ae_AddElement(tbcClasses, f)
	}
	ae_AddElement(tableTBCColumn, tbcClasses);
	//var a = ce("div");
    //a.className = "clear";
	//ae_AddElement(c, a);
	ae_AddElement(c, classTable);
	tc_object = new TalentCalc();
	tc_object.initialize("tc-itself", {
		onChange: tc_onChange,
		noAd: 1
	});
	tc_readPound();
	setInterval(tc_readPound, 1000)
}
function tc_classClick(a) {
	if (tc_object.setClass(a)) {
		Tooltip.hide()
	}
	return false
}
function tc_classOver(a) {
	Tooltip.show(this, "<b>" + g_chr_classes[a] + "</b>", 0, 0, "c" + a)
}
function tc_onChange(a, e, d) {
	var c;
	if (e.classId != tc_classId) {
		if (!tc_loaded) {
			tc_loaded = true
		}
		if (tc_classId != -1) {
			c = tc_classIcons[tc_classId];
			c.className = tc_classIcons[tc_classId].className.replace("iconmedium-gold", "")
		}
		tc_classId = e.classId;
		c = tc_classIcons[tc_classId];
		c.className += " iconmedium-gold";
		g_initPath([1, 0, tc_classId])
	}
	tc_build = a.getWhBuild();
	tc_glyphs = a.getWhGlyphs();
	var b = "#" + tc_build;
	if (tc_glyphs != "") {
		b += ":" + tc_glyphs
	}
	location.replace(b);
	var g = document.title;
	if (g.indexOf("/") != -1) {
		var f = g.indexOf("- ");
		if (f != -1) {
			g = g.substring(f + 2)
		}
	}
	document.title = g_chr_classes[tc_classId] + " (" + d[0].k + "/" + d[1].k + "/" + d[2].k + ") - " + g
}
function tc_readPound() {
	if (location.hash) {
		if (location.hash.indexOf("-") != -1) {
			var d = location.hash.substr(1).split("-"),
			a = d[0] || "",
			f = d[1] || "",
			c = -1;
			for (var g in g_file_classes) {
				if (g_file_classes[g] == a) {
					c = g;
					break
				}
			}
			if (c != -1) {
				tc_object.setBlizzBuild(c, f)
			}
		} else {
			var d = location.hash.substr(1).split(":"),
			e = d[0] || "",
			b = d[1] || "";
			if (tc_build != e) {
				tc_build = e;
				tc_object.setWhBuild(tc_build)
			}
			if (tc_glyphs != b) {
				tc_glyphs = b;
				tc_object.setWhGlyphs(tc_glyphs)
			}
		}
	}
};