<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="TalentCalculator.aspx.cs" Inherits="RealmPlayersServer.TalentCalculator" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
	
    <%--//https://github.com/mangostools/aowow
    //https://github.com/Sarjuuk/aowow
    //https://github.com/mangoszero
    // https://github.com/mangostools--%>

    <%= RealmPlayersServer.PageUtilityExtension.HTMLAddResources("assets/temp/global.css", "assets/temp/global.js", "assets/css/talentcalc.css", "assets/css/talent.css", "assets/temp/fx.js", "assets/temp/locale_enus.js", "assets/temp/locale_enus_0.js"
    , "assets/temp/Markup-2.js", "assets/js/TalentCalc_enus.js", "assets/js/TalentCalc.js", "assets/js/talent.js") %>

	<script type="text/javascript">
		var g_serverTime = new Date('2015/06/29 12:36:36');
		g_locale = { id: 0, name: 'enus' };
		g_glyphs = [];
ss_conf = 3;	</script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <div id="layers"></div>
    <div id="main-precontents"></div>
	<div id="main-contents" class="main-contents">

    <div id="tc-classes" class="choose">
		<div id="tc-classes-outer">
			<div id="tc-classes-inner"><p>Choose a class:</p></div>
		</div>
	</div>
    <div id="tc-itself"></div>

<script type="text/javascript">
	    //var Lightbox = new
        //function () {
        //    var d, m, n, h = {},
        //    c = {},
        //    i, f;
        //    function o() {
        //        aE(d, "click", e);
        //        aE(document, Browser.opera ? "keypress" : "keydown", g);
        //        aE(window, "resize", a);
        //        if (Browser.ie6) {
        //            aE(window, "scroll", j)
        //        }
        //    }
        //    function l() {
        //        dE(d, "click", e);
        //        dE(document, Browser.opera ? "keypress" : "keydown", g);
        //        dE(window, "resize", a);
        //        if (Browser.ie6) {
        //            dE(window, "scroll", j)
        //        }
        //    }
        //    function b() {
        //        if (i) {
        //            return
        //        }
        //        i = 1;
        //        var p = ge("layers");
        //        d = ce("div");
        //        d.className = "lightbox-overlay";
        //        m = ce("div");
        //        m.className = "lightbox-outer";
        //        n = ce("div");
        //        n.className = "lightbox-inner";
        //        d.style.display = m.style.display = "none";
        //        ae(p, d);
        //        ae(m, n);
        //        ae(p, m)
        //    }
        //    function g(p) {
        //        p = $E(p);
        //        switch (p.keyCode) {
        //            case 27:
        //                e();
        //                break
        //        }
        //    }
        //    function a(p) {
        //        if (p != 1234) {
        //            if (c.onResize) {
        //                c.onResize()
        //            }
        //        }
        //        d.style.height = document.body.offsetHeight + "px";
        //        if (Browser.ie6) {
        //            j()
        //        }
        //    }
        //    function j() {
        //        var q = g_getScroll().y,
        //        p = g_getWindowSize().h;
        //        m.style.top = (q + p / 2) + "px"
        //    }
        //    function e() {
        //        l();
        //        if (c.onHide) {
        //            c.onHide()
        //        }
        //        d.style.display = m.style.display = "none";
        //        Ads.restoreHidden();
        //        g_enableScroll(true)
        //    }
        //    function k() {
        //        d.style.display = m.style.display = h[f].style.display = ""
        //    }
        //    this.setSize = function (p, q) {
        //        n.style.visibility = "hidden";
        //        n.style.width = p + "px";
        //        n.style.height = q + "px";
        //        n.style.left = -parseInt(p / 2) + "px";
        //        n.style.top = -parseInt(q / 2) + "px";
        //        n.style.visibility = "visible"
        //    };
        //    this.show = function (t, s, p) {
        //        c = s || {};
        //        Ads.hideAll();
        //        b();
        //        o();
        //        if (f != t && h[f] != null) {
        //            h[f].style.display = "none"
        //        }
        //        f = t;
        //        var r = 0,
        //        q;
        //        if (h[t] == null) {
        //            r = 1;
        //            q = ce("div");
        //            ae(n, q);
        //            h[t] = q
        //        } else {
        //            q = h[t]
        //        }
        //        if (c.onShow) {
        //            c.onShow(q, r, p)
        //        }
        //        a(1234);
        //        k();
        //        g_enableScroll(false)
        //    };
        //    this.reveal = function () {
        //        k()
        //    };
        //    this.hide = function () {
        //        e()
        //    };
        //    this.isVisible = function () {
        //        return (d && d.style.display != "none")
        //    }
        //};
	    //function g_initPath(p, f) {
	    //    var h = mn_path,
        //    c = null,
        //    k = null,
        //    o = 0,
        //    l = ge("main-precontents"),
        //    n = ce("div");
	    //    ee(l);
	    //    if (g_initPath.lastIt) {
	    //        g_initPath.lastIt.checked = null
	    //    }
	    //    n.className = "path";
	    //    if (f != null) {
	    //        var m = ce("div");
	    //        m.className = "path-right";
	    //        var q = ce("a");
	    //        q.href = "javascript:;";
	    //        q.id = "fi_toggle";
	    //        ns(q);
	    //        q.onclick = fi_toggle;
	    //        if (f) {
	    //            q.className = "disclosure-on";
	    //            ae(q, ct(LANG.fihide))
	    //        } else {
	    //            q.className = "disclosure-off";
	    //            ae(q, ct(LANG.fishow))
	    //        }
	    //        ae(m, q);
	    //        ae(l, m)
	    //    }
	    //    if (o && k) {
	    //        k.className = ""
	    //    } else {
	    //        if (c && c[3]) {
	    //            k.className = "menuarrow";
	    //            q = ce("a");
	    //            b = ce("span");
	    //            q.href = "javascript:;";
	    //            ns(q);
	    //            q.style.textDecoration = "none";
	    //            q.style.paddingRight = "16px";
	    //            q.style.color = "white";
	    //            q.style.cursor = "default";
	    //            ae(q, ct("..."));
	    //            q.menu = c[3];
	    //            q.onmouseover = Menu.show;
	    //            q.onmouseout = Menu.hide;
	    //            ae(b, q);
	    //            ae(n, b)
	    //        }
	    //    }
	    //    var m = ce("div");
	    //    m.className = "clear";
	    //    ae(n, m);
	    //    //gb = ce("div");
	    //    //gb.className = "g-plusone";
	    //    //gb.href = "http://www.aowow.org/";
	    //    //gb.id = "g-plusone";
	    //    //ae(n, gb);
	    //    ae(l, n);
	    //    g_initPath.lastIt = c
	    //}
	    tc_init();
	</script>

	<div class="clear"></div>
	</div>
</asp:Content>
