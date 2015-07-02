var MARKUP_MODE_COMMENT = 1,
	MARKUP_MODE_ARTICLE = 2,
	MARKUP_MODE_QUICKFACTS = 3,
	MARKUP_MODE_SIGNATURE = 4,
	MARKUP_CLASS_ADMIN = 40,
	MARKUP_CLASS_STAFF = 30,
	MARKUP_CLASS_PREMIUM = 20,
	MARKUP_CLASS_USER = 10;
var MarkupModeMap = {};
MarkupModeMap[MARKUP_MODE_COMMENT] = "comment";
MarkupModeMap[MARKUP_MODE_ARTICLE] = "article";
MarkupModeMap[MARKUP_MODE_QUICKFACTS] = "quickfacts";
MarkupModeMap[MARKUP_MODE_SIGNATURE] = "signature";
var Markup = {
	MODE_COMMENT: MARKUP_MODE_COMMENT,
	MODE_ARTICLE: MARKUP_MODE_ARTICLE,
	MODE_QUICKFACTS: MARKUP_MODE_QUICKFACTS,
	MODE_SIGNATURE: MARKUP_MODE_SIGNATURE,
	CLASS_ADMIN: MARKUP_CLASS_ADMIN,
	CLASS_STAFF: MARKUP_CLASS_STAFF,
	CLASS_PREMIUM: MARKUP_CLASS_PREMIUM,
	CLASS_USER: MARKUP_CLASS_USER,
	rolesToClass: function (a) {
		if (a & 4) {
			return Markup.CLASS_ADMIN
		} else {
			if (a & 15) {
				return Markup.CLASS_STAFF
			} else {
				/*if (a & 256) {
					return Markup.CLASS_PREMIUM
				} else*/ {
					return Markup.CLASS_USER
				}
			}
		}
	},
	nameCol: "name_enus",
	domainToLocale: {
		www: "enus",
		ptr: "enus",
		fr: "frfr",
		de: "dede",
		es: "eses",
		ru: "ruru"
	},
	maps: [],
	firstTags: {},
	postTags: [],
	collectTags: {},
	excludeTags: {},
	tooltipTags: {},
	attributes: {
		id: {
			req: false,
			valid: /^[a-z0-9_-]+$/i
		},
		title: {
			req: false,
			valid: /[\S ]+/
		}
	},
	tags: {
		"<text>": {
			empty: true,
			toHtml: function (b, a) {
				if (b._text == " ") {
					b._text = "&nbsp;"
				}
				b._text = b._text.replace(/\\\[/g, "[");
				if (a && a.noLink) {
					return b._text
				} else {
					if (a && a.needsRaw) {
						return b._rawText
					} else {
						var d = [];
						var e = Markup._preText(b._rawText.replace(/(https?:\/\/|www\.)([\/_a-z0-9\%\?#@\-\+~&=;:']|\.[a-z0-9\-])+/gi, function (f) {
							matchUrl = Markup._preText(f.replace(/^www/, "http://www"));
							f = Markup._preText(f);
							var g = d.length;
							d.push([matchUrl, f]);
							return "$L" + g
						}));
						e = e.replace(/\$L([\d+]) /gi, "$L$1&nbsp;");
						for (var c in d) {
							e = e.replace("$L" + c, function (g) {
								var f = '<a href="' + d[c][0] + '"';
								if (Markup._isUrlExternal(d[c][0])) {
									f += ' target="_blank"'
								}
								f += ">" + d[c][1] + "</a>";
								return f
							})
						}
						return e
					}
				}
			},
			toText: function (a) {
				return a._text
			}
		},
		achievement: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var c = "";
				var d = Markup.nameCol;
				if (g_achievements[e] && g_achievements[e][d]) {
					var b = g_achievements[e];
					return '<a href="' + c + "?achievement=" + e + '" class="icontiny" style="background-image: url(images/icons/tiny/' + b.icon.toLowerCase() + '.gif)"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(b[d]) + "</a>"
				}
				return '<a href="' + c + "?achievement=" + e + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[10][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_achievements[c] && g_achievements[c][b]) {
					return Markup._safeHtml(g_achievements[c][b])
				}
				return LANG.types[10][0] + " #" + c
			}
		},
		achievementpoints: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = '<span class="moneyachievement tip" onmouseover="Listview.funcBox.moneyAchievementOver(event)" onmousemove="Tooltip.cursorUpdate(event)" onmouseout="Tooltip.hide()"' + Markup._addGlobalAttributes(a) + ">" + a.unnamed + "</span>";
				return b
			}
		},
		anchor: {
			empty: true,
			ltrim: true,
			rtrim: true,
			attr: {
				unnamed: {
					req: false,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			validate: function (a) {
				if (!a.unnamed && !a.id) {
					return false
				}
				return true
			},
			toHtml: function (a) {
				if (a.unnamed) {
					a.id = a.unnamed
				}
				return "<span" + Markup._addGlobalAttributes(a) + "></span>"
			}
		},
		b: {
			empty: false,
			toHtml: function (a) {
				return ["<b" + Markup._addGlobalAttributes(a) + ">", "</b>"]
			}
		},
		blip: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = "http://blip.tv/play/" + a.unnamed;
				var c = "";
				c += '<object width="480" height="300"' + Markup._addGlobalAttributes(a) + '><param name="movie" value="' + b + '">';
				c += '<param name="allowfullscreen" value="true"></param>';
				c += '<param name="allowscriptaccess" value="always"></param>';
				c += '<param name="wmode" value="opaque"></param>';
				c += '<embed width="480" height="300" src="' + b + '" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" wmode="opaque"></embed>';
				c += "</object>";
				return c
			}
		},
		br: {
			empty: true,
			toHtml: function (a) {
				return "<br />"
			}
		},
		code: {
			block: true,
			empty: false,
			rtrim: true,
			itrim: true,
			allowedChildren: {
				"<text>": 1
			},
			toHtml: function (a) {
				return ['<pre class="code"' + Markup._addGlobalAttributes(a) + ">", "</pre>"]
			}
		},
		color: {
			empty: false,
			attr: {
				unnamed: {
					req: true,
					valid: /^(aqua|black|blue|fuchsia|gray|green|lime|maroon|navy|olive|purple|red|silver|teal|white|yellow|c\d+|r\d+|q\d*?|#[a-f0-9]{6})$/i
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = a.unnamed.charAt(0);
				var c = "<span " + ((b == "q" || b == "c" || (b == "r" && a.unnamed != "red")) ? 'class="' : 'style="color: ') + a.unnamed + '"' + Markup._addGlobalAttributes(a) + ">";
				return [c, "</span>"]
			}
		},
		div: {
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			attr: {
				clear: {
					req: false,
					valid: /^(left|right|both)$/i
				},
				unnamed: {
					req: false,
					valid: /^hidden$/i
				},
				"float": {
					req: false,
					valid: /^(left|right)$/i
				},
				align: {
					req: false,
					valid: /^(left|right|center)$/i
				},
				margin: {
					req: false,
					valid: /^\d+$/
				}
			},
			//allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var c = "<div" + Markup._addGlobalAttributes(a);
				var b = [];
				if (a.clear) {
					b.push("clear: " + a.clear)
				}
				if (a.unnamed) {
					b.push("display: none")
				}
				if (a["float"]) {
					b.push("float: " + a["float"]);
					if (a.margin === undefined) {
						if (a["float"] == "left") {
							b.push("margin: 0 10px 10px 0")
						} else {
							b.push("margin: 0 0 10px 10px")
						}
					}
				}
				if (a.align) {
					b.push("text-align: " + a.align)
				}
				if (a.margin) {
					b.push("margin: " + a.margin)
				}
				if (b.length > 0) {
					c += ' style="' + b.join(";") + '"'
				}
				c += ">";
				return [c, "</div>"]
			}
		},
		faction: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var b = "";
				var c = Markup.nameCol;
				if (g_factions[e] && g_factions[e][c]) {
					var d = g_factions[e];
					return '<a href="' + b + "?faction=" + e + '"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(d[c]) + "</a>"
				}
				return '<a href="' + b + "?faction=" + e + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[8][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_factions[c] && g_factions[c][b]) {
					return Markup._safeHtml(g_factions[c][b])
				}
				return LANG.types[8][0] + " #" + c
			}
		},
		hr: {
			empty: true,
			trim: true,
			toHtml: function (a) {
				return "<hr />"
			}
		},
		h2: {
			block: true,
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedClass: MARKUP_CLASS_STAFF,
			attr: {
				unnamed: {
					req: false,
					valid: /^first$/i
				},
				clear: {
					req: false,
					valid: /^(true|both|left|right)$/i
				},
				toc: {
					req: false,
					valid: /^false$/i
				}
			},
			toHtml: function (a) {
				if (!a.id) {
					a.id = g_urlize(a._textContents)
				}
				str = "<h2" + Markup._addGlobalAttributes(a);
				if (a.first || a.unnamed) {
					str += ' class="firstblock"'
				}
				if (a.clear) {
					if (a.clear == "true" || a.clear == "both") {
						str += ' style="clear: both"'
					} else {
						str += ' style="clear: ' + a.clear + '"'
					}
				}
				return [str + ">", "</h2>"]
			}
		},
		h3: {
			block: true,
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			attr: {
				unnamed: {
					req: false,
					valid: /^first$/i
				},
				toc: {
					req: false,
					valid: /^false$/i
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				if (!a.id) {
					a.id = g_urlize(a._textContents)
				}
				var b = "<h3" + Markup._addGlobalAttributes(a);
				if (a.first || a.unnamed) {
					b += ' class="first"'
				}
				return [b + ">", "</h3>"]
			}
		},
		html: {
			ltrim: true,
			rtrim: true,
			empty: false,
			allowedClass: MARKUP_CLASS_ADMIN,
			allowedChildren: {
				"<text>": 1
			},
			rawText: true,
			taglessSkip: true,
			toHtml: function (a) {
				return [a._contents]
			}
		},
		i: {
			empty: false,
			toHtml: function (a) {
				return ["<i" + Markup._addGlobalAttributes(a) + ">", "</i>"]
			}
		},
		icon: {
			empty: false,
			itrim: true,
			attr: {
				align: {
					req: false,
					valid: /^right$/i
				},
				"float": {
					req: false,
					valid: /^(left|right)$/i
				},
				name: {
					req: false,
					valid: /\S+/
				},
				size: {
					req: false,
					valid: /^(tiny|small|medium|large)$/
				},
				unnamed: {
					req: false,
					valid: /^class$/i
				},
				url: {
					req: false,
					valid: /\S+/
				},
				preset: {
					req: false,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			presets: {
				boss: "images/skull.gif"
			},
			validate: function (a) {
				if (!a.name && !a.url && !a.preset) {
					return false
				}
				if (a.preset && !Markup.tags.icon.presets[a.preset]) {
					return false
				}
				return true
			},
			toHtml: function (a) {
				var b = (a.size ? a.size : "tiny");
				if (b == "tiny") {
					var e = "<span" + Markup._addGlobalAttributes(a) + ' class="';
					if (a.unnamed == undefined) {
						e += "icontiny";
						if (a.align) {
							e += "r"
						}
						var d = "";
						if (a.name) {
							d = "images/icons/tiny/" + a.name.toLowerCase() + ".gif"
						} else {
							if (a.preset) {
								d = Markup.tags.icon.presets[a.preset]
							} else {
								if (a.url && Markup._isUrlSafe(a.url)) {
									d = a.url
								} else {
									return ""
								}
							}
						}
						e += '" style="background-image: url(' + d + ')">'
					} else {
						e += a.name + '">'
					}
					return [e, "</span>"]
				} else {
					var e = "<div" + Markup._addGlobalAttributes(a) + ' ondblclick="Icon.showIconName(this)" class="icon' + b + (a["float"] ? '" style="float: ' + a["float"] + ';">' : '">');
					var c = {
						small: 0,
						medium: 1,
						large: 2
					};
					if (a._textContents && Markup._isUrlSafe(a._textContents)) {
						icon = Icon.create(a.name.toLowerCase(), c[b], null, a._textContents)
					} else {
						icon = Icon.create(a.name.toLowerCase(), c[b])
					}
					e += icon.innerHTML + "</div>";
					return [e]
				}
			}
		},
		img: {
			empty: true,
			attr: {
				src: {
					req: false,
					valid: /\S+/
				},
				icon: {
					req: false,
					valid: /\S+/
				},
				id: {
					req: false,
					valid: /^[0-9]+$/
				},
				size: {
					req: false,
					valid: /^(thumb|resized|normal|large|medium|small|tiny)$/i
				},
				width: {
					req: false,
					valid: /^[0-9]+$/
				},
				height: {
					req: false,
					valid: /^[0-9]+$/
				},
				"float": {
					req: false,
					valid: /^(left|right|center)$/i
				},
				border: {
					req: false,
					valid: /^[0-9]+$/
				},
				margin: {
					req: false,
					valid: /^[0-9]+$/
				}
			},
			idSize: /^(thumb|resized|normal)$/i,
			iconSize: /^(large|medium|small|tiny)$/i,
			allowedClass: MARKUP_CLASS_STAFF,
			validate: function (a) {
				if (a.src) {
					return true
				} else {
					if (a.id) {
						return (a.size ? Markup.tags.img.idSize.test(a.size) : true)
					} else {
						if (a.icon) {
							return (a.size ? Markup.tags.img.iconSize.test(a.size) : true)
						}
					}
				}
				return false
			},
			toHtml: function (a) {
				var c = "<img" + Markup._addGlobalAttributes(a);
				var b = "";
				if (a.src) {
					c += ' src="' + a.src + '"'
				} else {
					if (a.id) {
						c += ' src="images/screenshots/thumb/' + a.id + '.jpg"'
					} else {
						if (a.icon) {
							c += ' src="images/icons/' + (a.size ? a.size : "large") + "/" + a.icon + '.jpg"'
						}
					}
				}
				if (a.width) {
					c += ' width="' + a.width + '"'
				}
				if (a.height) {
					c += ' height="' + a.height + '"'
				}
				if (a["float"]) {
					if (a["float"] == "center") {
						c = '<div style="text-align: center">' + c + ' style="margin: 10px auto"';
						b = "</div>"
					} else {
						c += ' style="float: ' + a["float"] + ";";
						if (!a.margin) {
							a.margin = 10
						}
						if (a["float"] == "left") {
							c += " margin: 0 " + a.margin + "px " + a.margin + 'px 0"'
						} else {
							c += " margin: 0 0 " + a.margin + "px " + a.margin + 'px"'
						}
					}
				}
				if (a.border != 0) {
					c += ' class="border"'
				}
				if (a.title) {
					c += ' alt="' + a.title + '"'
				} else {
					c += ' alt=""'
				}
				c += " />" + b;
				return c
			}
		},
		item: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				},
				icon: {
					req: false,
					valid: /^false$/i
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var f = a.unnamed;
				var b = "";
				var c = Markup.nameCol;
				if (g_items[f] && g_items[f][c]) {
					var d = g_items[f];
					var e = "<a" + Markup._addGlobalAttributes(a) + ' href="' + b + "?item=" + f + '" class="q' + d.quality;
					if (a.icon) {
						e += '">'
					} else {
						e += ' icontiny" style="background-image: url(images/icons/tiny/' + d.icon.toLowerCase() + '.gif)">'
					}
					e += Markup._safeHtml(d[c]) + "</a>";
					return e
				}
				return '<a href="' + b + "?item=" + f + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[3][0] + " #" + f + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_items[c] && g_items[c][b]) {
					return Markup._safeHtml(g_items[c][b])
				}
				return LANG.types[3][0] + " #" + c
			}
		},
		itemset: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^-?[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var b = "";
				var c = Markup.nameCol;
				if (g_itemsets[e] && g_itemsets[e][c]) {
					var d = g_itemsets[e];
					return '<a href="' + b + "?itemset=" + e + '"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(d[c]) + "</a>"
				}
				return '<a href="' + b + "?itemset=" + e + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[4][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_itemsets[c] && g_itemsets[c][b]) {
					return Markup._safeHtml(g_itemsets[c][b])
				}
				return LANG.types[4][0] + " #" + c
			}
		},
		li: {
			empty: false,
			itrim: true,
			allowedParents: {
				ul: 1,
				ol: 1
			},
			toHtml: function (a) {
				return ["<li" + Markup._addGlobalAttributes(a) + "><div>", "</div></li>"]
			}
		},
		lightbox: {
			empty: false,
			attr: {
				unnamed: {
					req: true,
					valid: /^(map|model|screenshot)$/
				},
				zone: {
					req: false,
					valid: /^-?[0-9]+[a-z]?$/i
				},
				floor: {
					req: false,
					valid: /^[0-9]+$/
				},
				pins: {
					req: false,
					valid: /^[0-9]+$/
				}
			},
			validate: function (a) {
				switch (a.unnamed) {
				case "map":
					if (a.zone) {
						return true
					}
					break;
				case "model":
					break;
				case "screenshot":
					break
				}
				return false
			},
			toHtml: function (a) {
				var b = "";
				var c = "";
				switch (a.unnamed) {
				case "map":
					b = "?maps=" + a.zone;
					if (a.floor) {
						b += "." + a.floor
					}
					if (a.pins) {
						b += ":" + a.pins
					}
					var d = b.substr(7);
					c = "if(!g_isLeftClick(event)) return; MapViewer.show({ link: '" + d + "' }); return false;";
					break
				}
				if (b && c) {
					return ['<a href="' + b + '" onclick="' + c + '"' + Markup._addGlobalAttributes(a) + ">", "</a>"]
				}
				return ""
			}
		},
		map: {
			empty: false,
			attr: {
				zone: {
					req: true,
					valid: /^-?[0-9]+[a-z]?$/i
				},
				floor: {
					req: false,
					valid: /\d+/
				},
				source: {
					req: false,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			allowedChildren: {
				pin: 1
			},
			toHtml: function (b) {
				var a = b._contents;
				b.id = "dsgdfngjkfdg" + (Markup.maps.length);
				var c = "<div" + Markup._addGlobalAttributes(b) + '></div><div style="clear: left"></div>';
				if (b.floor) {
					var d = {};
					d[b.floor] = a;
					Markup.maps.push([b.id, b.zone, d]);
				} else {
					Markup.maps.push([b.id, b.zone, a]);
				}
				return [c]
			}
		},
		markupdoc: {
			empty: true,
			attr: {
				tag: {
					req: false,
					valid: /[a-z0-9]+/i
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			validate: function (a) {
				if (a.tag && !Markup.tags[a.tag]) {
					return false
				}
				return true
			},
			toHtml: function (b) {
				var c = "";
				if (b.tag) {
					c = Markup._generateTagDocs(b.tag)
				} else {
					for (var a in Markup.tags) {
						if (c != "") {
							c += '<div class="pad3"></div>'
						}
						c += Markup._generateTagDocs(a)
					}
				}
				return c
			}
		},
		pin: {
			empty: false,
			attr: {
				url: {
					req: false,
					valid: /\S+/
				},
				type: {
					req: false,
					valid: /^[0-9]+$/
				},
				x: {
					req: true,
					valid: /^[0-9]{1,2}(\.[0-9])?$/
				},
				y: {
					req: true,
					valid: /^[0-9]{1,2}(\.[0-9])?$/
				}
			},
			taglessSkip: true,
			allowedClass: MARKUP_CLASS_STAFF,
			allowedParents: {
				map: 1
			},
			toHtml: function (a) {
				if (a.url && !Markup._isUrlSafe(a.url)) {
					a.url = ""
				}
				var b = a._contents;
				if (a.url && a.url.indexOf("?npc=") != -1) {
					b = '<b class="q">' + b + '</b><br /><span class="q2">Click to view this NPC</span>'
				}
				return [[parseFloat(a.x || 0), parseFloat(a.y || 0), {
					label: b,
					url: a.url,
					type: a.type
				}]]
			}
		},
		menu: {
			empty: true,
			trim: true,
			ltrim: true,
			rtrim: true,
			attr: {
				tab: {
					req: true,
					valid: /^[0-9]+$/
				},
				path: {
					req: true,
					valid: /^[0-9,]+$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = a.path.split(",");
				g_updateHeader(a.tab);
				g_initPath(b)
			}
		},
		minibox: {
			empty: false,
			rtrim: true,
			itrim: true,
			attr: {
				"float": {
					req: false,
					valid: /^(left|right)$/i
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = "<div" + Markup._addGlobalAttributes(a) + ' class="minibox';
				if (a["float"] == "left") {
					b += " minibox-left"
				}
				b += '">';
				return [b, "</div>"]
			}
		},
		modelviewer: {
			empty: false,
			attr: {
				item: {
					req: false,
					valid: /^[0-9]+$/
				},
				object: {
					req: false,
					valid: /^[0-9]+$/
				},
				npc: {
					req: false,
					valid: /^[0-9]+$/
				},
				itemset: {
					req: false,
					valid: /^[0-9,]+$/
				},
				slot: {
					req: false,
					valid: /^[0-9]+$/
				},
				humanoid: {
					req: false,
					valid: /^1$/
				},
				"float": {
					req: false,
					valid: /^(left|right)$/i
				},
				img: {
					req: false,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			skipSlots: {
				4: 1,
				5: 1,
				6: 1,
				7: 1,
				8: 1,
				9: 1,
				10: 1,
				16: 1,
				19: 1,
				20: 1
			},
			toHtml: function (a) {
				var b = "";
				if (a.npc) {
					b = "<a" + Markup._addGlobalAttributes(a) + ' href="#modelviewer:1:' + a.npc + ":" + (a.humanoid ? "1" : "0") + '" onclick="ModelViewer.show({ type: 1, displayId: ' + a.npc + ", slot: " + a.slot + ", " + (a.humanoid ? "humanoid: 1, " : "") + 'displayAd: 1, fromTag: 1 });"><img id="modelv" alt="' + Markup._safeHtml(a._contents) + '" title="' + Markup._safeHtml(a._contents) + '" src="' + (a.img ? a.img : "http://www.aowow.org/images/models/npc/" + a.npc + '.png" width="150" height="150') + '"';
					if (a["float"]) {
						b += 'style="float: ' + a["float"] + "; ";
						if (a["float"] == "left") {
							b += 'margin: 0 10px 10px 0" '
						} else {
							b += 'margin: 0 0 10px 10px" '
						}
					}
					b += "/></a>";
					return [b]
				} else {
					if (a.object) {
						b = "<a" + Markup._addGlobalAttributes(a) + ' href="#modelviewer:2:' + a.object + '" onclick="ModelViewer.show({ type: 2, displayId: ' + a.object + ', displayAd: 1, fromTag: 1 });"><img id="modelv" alt="' + Markup._safeHtml(a._contents) + '" title="' + Markup._safeHtml(a._contents) + '" src="' + (a.img ? a.img : "http://www.aowow.org/images/models/obj/" + a.object + '.png" width="150" height="150') + '" class="border" ';
						if (a["float"]) {
							b += 'style="float: ' + a["float"] + "; ";
							if (a["float"] == "left") {
								b += 'margin: 0 10px 10px 0" '
							} else {
								b += 'margin: 0 0 10px 10px" '
							}
						}
						b += "/></a>";
						return [b]
					} else {
						if (a.item && a.slot) {
							b = "<a" + Markup._addGlobalAttributes(a) + ' href="#modelviewer:3:' + a.item + ":" + a.slot + '" onclick="ModelViewer.show({ type: 3, displayId: ' + a.item + ", slot: " + a.slot + ', displayAd: 1, fromTag: 1 });"><img id="modelv" alt="' + Markup._safeHtml(a._contents) + '" title="' + Markup._safeHtml(a._contents) + '" src="' + (a.img ? a.img : "http://www.aowow.org/images/models/item/" + a.item + '.png" width="150" height="150') + '" ';
							if (a["float"]) {
								b += 'style="float: ' + a["float"] + "; ";
								if (a["float"] == "left") {
									b += 'margin: 0 10px 10px 0" '
								} else {
									b += 'margin: 0 0 10px 10px" '
								}
							}
							b += "/></a>";
							return [b]
						} else {
							if (a.itemset) {
								b = "<a" + Markup._addGlobalAttributes(a) + ' href="javascript:;" onclick="ModelViewer.show({ type: 4, equipList: [' + a.itemset + '], displayAd: 1, fromTag: 1 });">'
							} else {
								return ["[modelviewer]", "[/modelviewer]"]
							}
						}
					}
				}
				return [b, "</a>"]
			}
		},
		money: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				honor: {
					req: false,
					valid: /\S+/
				},
				arena: {
					req: false,
					valid: /^[0-9]+$/
				},
				items: {
					req: false,
					valid: /^[0-9,]+$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = [];
				if (a.items) {
					var d = a.items.split(",");
					if (d.length >= 2) {
						for (var c = 0; c < d.length - 1; c += 2) {
							b.push([d[c], d[c + 1]])
						}
					}
				}
				return g_getMoneyHtml2(a.unnamed, a.honor, a.arena, b)
			}
		},
		npc: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var c = "";
				var d = Markup.nameCol;
				if (g_npcs[e] && g_npcs[e][d]) {
					var b = g_npcs[e];
					return '<a href="' + c + "?npc=" + e + '"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(b[d]) + "</a>"
				}
				return '<a href="' + c + "?npc=" + e + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[1][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_npcs[c] && g_npcs[c][b]) {
					return Markup._safeHtml(g_npcs[c][b])
				}
				return LANG.types[1][0] + " #" + c
			}
		},
		object: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var b = "";
				var c = Markup.nameCol;
				if (g_objects[e] && g_objects[e][c]) {
					var d = g_objects[e];
					return '<a href="' + b + "?object=" + e + '"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(d[c]) + "</a>"
				}
				return '<a href="' + b + "?object=" + e + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[2][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_objects[c] && g_objects[c][b]) {
					return Markup._safeHtml(g_objects[c][b])
				}
				return LANG.types[2][0] + " #" + c
			}
		},
		ol: {
			block: true,
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedChildren: {
				li: 1
			},
			toHtml: function (a) {
				return ["<ol" + Markup._addGlobalAttributes(a) + ">", "</ol>"]
			}
		},
		p: {
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				return ['<p style="line-height: 1.4em; margin: 1em 0px 0px 0px;"' + Markup._addGlobalAttributes(a) + ">", "</p>"]
			}
		},
		pad: {
			empty: true,
			trim: true,
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				return '<div class="pad"' + Markup._addGlobalAttributes(a) + "></div>"
			}
		},
		pet: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var d = a.unnamed;
				var b = "";
				if (g_pet_families && g_pet_families[d] && g_pets && g_pets[d]) {
					var c = '<span class="icontiny" style="background-image: url(images/icons/tiny/' + g_pets[d]["icon"].toLowerCase() + '.gif)">';
					c += '<a href="' + b + "?pet=" + d + '"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(g_pet_families[d]) + "</a></span>";
					return c
				}
				return '<a href="' + b + "?pet=" + d + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[9][0] + " #" + d + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (a.domain) {
					b = "name_" + Markup.domainToLocale[a.domain]
				}
				if (g_pet_families[c] && g_pet_families[c][b]) {
					return Markup._safeHtml(g_pet_families[c][b])
				}
				return LANG.types[9][0] + " #" + c
			}
		},
		pre: {
			empty: false,
			rtrim: true,
			toHtml: function (a) {
				return ['<pre class="code"' + Markup._addGlobalAttributes(a) + ">", "</pre>"]
			}
		},
		quest: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var c = "";
				var d = Markup.nameCol;
				if (g_quests[e] && g_quests[e][d]) {
					var b = g_quests[e];
					return '<a href="' + c + "?quest=" + e + '" class="icontiny" style="background-image: url(images/icons/tiny/' + (b.daily ? "quest_start_daily" : "quest_start") + '.gif)"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(b[d]) + "</a>"
				}
				return '<a href="' + c + "?quest=" + e + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[5][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_quests[c] && g_quests[c][b]) {
					return Markup._safeHtml(g_quests[c][b])
				}
				return LANG.types[5][0] + " #" + c
			}
		},
		quote: {
			block: true,
			empty: false,
			rtrim: true,
			ltrim: true,
			itrim: true,
			attr: {
				unnamed: {
					req: false,
					valid: /[\S ]+/
				},
				url: {
					req: false,
					valid: /\S+/
				},
				blizzard: {
					req: false,
					valid: /^true$/
				}
			},
			allowedModes: {
				article: 1,
				quickfacts: 1,
				comment: 1
			},
			validate: function (a) {
				if (a.blizzard || a.url) {
					if (Markup.allow < Markup.CLASS_STAFF) {
						return false
					}
				}
				return true
			},
			toHtml: function (a) {
				var c = "<div" + Markup._addGlobalAttributes(a) + ' class="quote';
				if (a.first) {
					c += " firstmargin"
				}
				if (a.blizzard) {
					if (a.unnamed && a.blizzard) {
						var b = a.unnamed.trim();
						if (b.length <= 0) {
							return ["", ""]
						}
						c += ' comment-blue"><small class="blizzard"><b>' + (a.url && Markup._isUrlSafe(a.url) ? '<a href="' + Markup._fixUrl(a.url) + '" target="_blank">' + b + "</a>" : b) + "</b> " + LANG.markup_said + '</small><div class="pad"></div>';
						return [c, "</div>"]
					}
					return ["", ""]
				} else {
					c += '">';
					if (a.unnamed) {
						var b = a.unnamed.trim();
						if (b.length > 0) {
							c += "<small><b>";
							if (a.url && Markup._isUrlSafe(a.url)) {
								c += '<a href="' + Markup._fixUrl(a.url) + '"' + (Markup._isUrlExternal(a.url) ? ' target="_blank"' : "") + ">" + b + "</a>"
							} else {
								if (g_isUsernameValid(b)) {
									c += '<a href="?user=' + b + '">' + b + "</a>"
								} else {
									c += b
								}
							}
							c += "</b> " + LANG.markup_said + '</small><div class="pad"></div>'
						}
					}
					return [c, "</div>"]
				}
			}
		},
		s: {
			empty: false,
			toHtml: function (a) {
				return ["<del" + Markup._addGlobalAttributes(a) + ">", "</del>"]
			}
		},
		screenshot: {
			empty: false,
			attr: {
				id: {
					req: false,
					valid: /^[0-9]+$/
				},
				url: {
					req: false,
					valid: /\S+/
				},
				thumb: {
					req: false,
					valid: /\S+/
				},
				width: {
					req: false,
					valid: /^[0-9]+$/
				},
				height: {
					req: false,
					valid: /^[0-9]+$/
				},
				"float": {
					req: false,
					valid: /^(left|right)$/i
				},
				border: {
					req: false,
					valid: /^[0-9]+$/
				}
			},
			taglessSkip: true,
			allowedClass: MARKUP_CLASS_STAFF,
			validate: function (a) {
				if (a.url && !a.thumb) {
					return false
				} else {
					if (!a.id && !a.url) {
						return false
					}
				}
				return true
			},
			toHtml: function (a) {
				var d = "";
				var c = "";
				if (a.id) {
					d = "images/screenshots/" + a.id + ".jpg";
					if (!a.thumb) {
						c = "images/screenshots/thumb/" + a.id + ".jpg"
					}
				} else {
					if (a.url) {
						d = a.url
					}
				}
				if (a.thumb) {
					c = a.thumb
				}
				var b = a._contents.replace(/\n/g, "<br />");
				if (!g_screenshots[Markup.uid]) {
					g_screenshots[Markup.uid] = []
				}
				var e = '<a href="' + d + '" onclick="ScreenshotViewer.show({screenshots: \'' + Markup.uid + "', pos: " + g_screenshots[Markup.uid].length + '}); return false;"' + Markup._addGlobalAttributes(a) + ">";
				e += '<img src="' + c + '" ';
				if (a.border != 0) {
					e += 'class="border" '
				}
				if (a["float"]) {
					e += 'style="float: ' + a["float"] + "; ";
					if (a["float"] == "left") {
						e += "margin: 0 10px 10px 0"
					} else {
						e += "margin: 0 0 10px 10px"
					}
					e += '" '
				}
				e += 'alt="" ';
				var f = {
					caption: b,
					width: a.width,
					height: a.height
				};
				if (a.id) {
					f.id = a.id
				} else {
					f.url = a.url
				}
				g_screenshots[Markup.uid].push(f);
				return [e + "/></a>"]
			}
		},
		script: {
			ltrim: true,
			rtrim: true,
			empty: false,
			allowedClass: MARKUP_CLASS_ADMIN,
			allowedChildren: {
				"<text>": 1
			},
			rawText: true,
			taglessSkip: true,
			toHtml: function (a) {
				g_globalEval(a._contents);
				return [""]
			}
		},
		small: {
			empty: false,
			toHtml: function (a) {
				return ["<small" + Markup._addGlobalAttributes(a) + ">", "</small>"]
			}
		},
		span: {
			empty: false,
			attr: {
				unnamed: {
					req: false,
					valid: /hidden/i
				},
				"class": {
					req: false,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var c = "<span" + Markup._addGlobalAttributes(a);
				if (a["class"]) {
					c += ' class="' + a["class"] + '"'
				}
				var b = [];
				if (a.unnamed) {
					b.push("display: none")
				}
				if (b.length > 0) {
					c += ' style="' + b.join(";") + '"'
				}
				c += ">";
				return [c, "</span>"]
			}
		},
		spell: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				},
				buff: {
					req: false,
					valid: /^true$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var e = a.unnamed;
				var c = "";
				var d = Markup.nameCol;
				if (g_spells[e] && g_spells[e][d]) {
					var b = g_spells[e];
					return '<a href="' + c + "?spell=" + e + '" class="icontiny" style="background-image: url(images/icons/tiny/' + b.icon.toLowerCase() + '.gif)"' + (a.buff ? " rel='buff'" : "") + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(b[d]) + "</a>"
				}
				return '<a href="' + c + "?spell=" + e + '"' + (a.buff ? " rel='buff'" : "") + ">(" + LANG.types[6][0] + " #" + e + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_spells[c] && g_spells[c][b]) {
					return Markup._safeHtml(g_spells[c][b])
				}
				return LANG.types[6][0] + " #" + c
			}
		},
		style: {
			ltrim: true,
			rtrim: true,
			empty: false,
			allowedClass: MARKUP_CLASS_ADMIN,
			allowedChildren: {
				"<text>": 1
			},
			rawText: true,
			taglessSkip: true,
			toHtml: function (a) {
				g_addCss(a._contents);
				return [""]
			}
		},
		sub: {
			empty: false,
			toHtml: function (a) {
				return ["<sub" + Markup._addGlobalAttributes(a) + ">", "</sub>"]
			}
		},
		sup: {
			empty: false,
			toHtml: function (a) {
				return ["<sup" + Markup._addGlobalAttributes(a) + ">", "</sup>"]
			}
		},
		tabs: {
			block: true,
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedChildren: {
				tab: 1
			},
			attr: {
				name: {
					req: true,
					valid: /\S+/
				}
			},
			toHtml: function (b) {
				b.id = g_urlize(b.name);
				var a = ge("dsf67g4d-" + b.id);
				var f = '<div class="clear"></div><div id="dsf67g4d-' + b.id + (a ? "-preview" : "") + '"></div>';
				f += "<div>";
				f += '<div class="tabbed-contents"';
				var d = b._contents;
				for (var c = 0; c < d.length; ++c) {
					var e = d[c];
					f += '<div id="tab-' + b.id + "-" + e.id + (a ? "-preview" : "") + '" style="display: none">';
					f += e.content;
					f += "</div>"
				}
				f += "</div>";
				f += "</div>";
				setTimeout(Markup.createTabs.bind(null, b, d, (a ? "preview" : "")), 10);
				return [f]
			}
		},
		tab: {
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedParents: {
				tabs: 1
			},
			attr: {
				name: {
					req: true,
					valid: /[\S ]+/
				},
				icon: {
					req: false,
					valid: /\S+/
				}
			},
			toHtml: function (a) {
				a.id = g_urlize(a.name);
				a.name = str_replace(a.name, "_", " ");
				return [{
					content: a._contents,
					id: a.id,
					name: a.name,
					icon: a.icon
				}]
			}
		},
		table: {
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedChildren: {
				tr: 1
			},
			attr: {
				border: {
					req: false,
					valid: /^[0-9]+$/
				},
				cellspacing: {
					req: false,
					valid: /^[0-9]+$/
				},
				cellpadding: {
					req: false,
					valid: /^[0-9]+$/
				},
				"class": {
					req: false,
					valid: /[\S ]+/
				},
				width: {
					req: false,
					valid: /^[0-9]+(px|em|\%)$/
				}
			},
			toHtml: function (a) {
				var b = "<table" + Markup._addGlobalAttributes(a);
				if (a.border != undefined) {
					b += ' border="' + a.border + '"'
				}
				if (a.cellspacing != undefined) {
					b += ' cellspacing="' + a.cellspacing + '"'
				}
				if (a.cellpadding != undefined) {
					b += ' cellpadding="' + a.cellpadding + '"'
				}
				if (a["class"] != undefined) {
					b += ' class="' + a["class"] + '"'
				}
				if (a.width != undefined) {
					b += ' style="width: ' + a.width + '"'
				}
				b += "><tbody>";
				return [b, "</tbody></table>"]
			}
		},
		tr: {
			empty: false,
			itrim: true,
			allowedChildren: {
				td: 1
			},
			allowedParents: {
				table: 1
			},
			toHtml: function (a) {
				return ["<tr" + Markup._addGlobalAttributes(a) + ">", "</tr>"]
			}
		},
		td: {
			empty: false,
			itrim: true,
			allowedParents: {
				tr: 1
			},
			attr: {
				align: {
					req: false,
					valid: /^(right|left|center|justify)$/i
				},
				valign: {
					req: false,
					valid: /^(top|middle|bottom|baseline)$/i
				},
				colspan: {
					req: false,
					valid: /^[0-9]+$/
				},
				rowspan: {
					req: false,
					valid: /^[0-9]+$/
				},
				width: {
					req: false,
					valid: /^[0-9]+(px|em|\%)$/
				}
			},
			toHtml: function (a) {
				var b = "<td" + Markup._addGlobalAttributes(a);
				if (a.align != undefined) {
					b += ' align="' + a.align + '"'
				}
				if (a.valign != undefined) {
					b += ' valign="' + a.valign + '"'
				}
				if (a.colspan != undefined) {
					b += ' colspan="' + a.colspan + '"'
				}
				if (a.rowspan != undefined) {
					b += ' rowspan="' + a.rowspan + '"'
				}
				if (a.width != undefined) {
					b += ' style="width: ' + a.width + '"'
				}
				b += ">";
				return [b, "</td>"]
			}
		},
		time: {
			empty: true,
			count: 0,
			attr: {
				until: {
					req: false,
					valid: /^\d+$/
				},
				since: {
					req: false,
					valid: /^\d+$/
				}
			},
			validate: function (a) {
				if (!a.until && !a.since) {
					return false
				}
				return true
			},
			toHtml: function (a) {
				var c = Markup.tags.time.count++;
				var b = '<span title="' + (new Date((a.until ? a.until : a.since) * 1000)).toLocaleString() + '" id="markupTime' + c + '">' + Markup.tags.time.getTime(a) + "</span>";
				setInterval(Markup.tags.time.updateTime.bind(null, c, a), 5000);
				return b
			},
			getTime: function (a) {
				var c = (new Date()).getTime() / 1000;
				var b = 0;
				if (a.until) {
					b = a.until - c
				} else {
					b = c - a.since
				}
				if (b > 0) {
					return g_formatTimeElapsed(b)
				} else {
					return "0 " + LANG.timeunitspl[6]
				}
			},
			updateTime: function (c, a) {
				var b = ge("markupTime" + c);
				if (!b) {
					return
				}
				b.firstChild.nodeValue = Markup.tags.time.getTime(a)
			}
		},
		toc: {
			block: true,
			post: true,
			trim: true,
			ltrim: true,
			rtrim: true,
			collect: {
				h2: 1,
				h3: 1
			},
			exclude: {
				tabs: {
					h2: 1,
					h3: 1
				},
				minibox: {
					h2: 1,
					h3: 1
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			attr: {
				h3: {
					req: false,
					valid: /^false$/
				}
			},
			postHtml: function (f, a) {
				var h = "";
				if (f.first) {
					h += '<h3 class="firstblock"' + Markup._addGlobalAttributes(f) + ">"
				} else {
					h += "<h3" + Markup._addGlobalAttributes(f) + ">"
				}
				h += LANG.markup_toc + "</h3><ul>";
				var g = "";
				var e = 1;
				var j = (f.h3 != "false");
				var b = [];
				for (var c in a.h2) {
					b.push(a.h2[c])
				}
				for (var c in a.h3) {
					b.push(a.h3[c])
				}
				b.sort(function (k, i) {
					return k.offset - i.offset
				});
				for (var d in b) {
					c = b[d];
					if (c.name == "h2" && c.attr.toc !== false) {
						if (g == "h3") {
							h += "</ul>";
							e--
						}
						h += "<li><b><a href='#" + (c.attr.id ? g_urlize(c.attr.id) : g_urlize(c.attr._textContents)) + "'>" + c.attr._textContents + "</a></b></li>";
						g = "h2"
					}
					if (c.name == "h3" && j && c.attr.toc !== false && (g != "" || a.h2.length == 0)) {
						if (g == "h2") {
							h += "<ul>";
							e++
						}
						h += "<li><b><a href='#" + (c.attr.id ? g_urlize(c.attr.id) : g_urlize(c.attr._textContents)) + "'>" + c.attr._textContents + "</a></b></li>";
						g = "h3"
					}
				}
				for (var d = 0; d < e; d++) {
					h += "</ul>"
				}
				return h
			}
		},
		toggler: {
			empty: false,
			attr: {
				id: {
					req: true,
					valid: /^[a-z0-9_-]+$/i
				},
				unnamed: {
					req: false,
					valid: /^hidden$/i
				}
			},
			//allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = '<a href="javascript:;" class="disclosure-' + (a.unnamed ? "off" : "on") + '" onclick="return g_disclose(ge(\'' + a.id + "'), this)\">";
				return [b, "</a>"]
			}
		},
		tooltip: {
			empty: false,
			attr: {
				unnamed: {
					req: false,
					valid: /\S+/
				},
				name: {
					req: false,
					valid: /\S+/
				}
			},
			taglessSkip: true,
			allowedClass: MARKUP_CLASS_STAFF,
			validate: function (a) {
				if (!a.unnamed && !a.name) {
					return false
				}
				return true
			},
			toHtml: function (a) {
				if (a.unnamed) {
					return ['<span class="tip" onmouseover="Tooltip.showAtCursor(event, LANG[\'' + a.unnamed + '\'], 0, 0, \'q\')" onmousemove="Tooltip.cursorUpdate(event)" onmouseout="Tooltip.hide()">', "</span>"]
				} else {
					Markup.tooltipTags[a.name] = a._contents;
					return [""]
				}
			}
		},
		video: {
			empty: true,
			ltrim: true,
			rtrim: true,
			toHtml: function (a) {
				return '<div id="companion-ad" style="width:300px;height:250px;float:right;"></div><embed width="512" height="312" src="http://www.zam.com/shared/fp/flowplayer.commercial-3.1.5.swf" flashvars="config=http://www.zam.com/cgi-bin/video-ad-test.pl%3Fembed%3D1" allowfullscreen="true" allowscriptaccess="always" quality="high" bgcolor="#000000" type="application/x-shockwave-flash" pluginspage="http://www.adobe.com/go/getflashplayer" wmode="opaque"></embed>'
			}
		},
		u: {
			empty: false,
			toHtml: function (a) {
				return ["<ins" + Markup._addGlobalAttributes(a) + ">", "</ins>"]
			}
		},
		ul: {
			block: true,
			empty: false,
			ltrim: true,
			rtrim: true,
			itrim: true,
			allowedChildren: {
				li: 1
			},
			toHtml: function (a) {
				return ["<ul" + Markup._addGlobalAttributes(a) + ">", "</ul>"]
			}
		},
		url: {
			empty: false,
			attr: {
				unnamed: {
					req: false,
					valid: /\S+/
				},
				rel: {
					req: false,
					valid: /(item|quest|spell|achievement|npc|object|ench)=([0-9]+)/
				},
				onclick: {
					req: false,
					valid: /[\S ]+/
				},
				tooltip: {
					req: false,
					valid: /\S+/
				}
			},
			validate: function (a) {
				if (a.onclick && Markup.allow < Markup.CLASS_ADMIN) {
					return false
				}
				if (a.tooltip && Markup.allow < Markup.CLASS_STAFF) {
					return false
				}
				var b = "";
				if (a.unnamed && /^(mailto:|irc:)/i.test(a.unnamed.trim()) && Markup.allow < Markup.CLASS_STAFF) {
					return false
				}
				return true
			},
			toHtml: function (a) {
				var c;
				if (a.unnamed) {
					c = a.unnamed;
					c = c.replace(/&amp;/, "&");
					if (Markup._isUrlSafe(c, true)) {
						var b = "<a" + Markup._addGlobalAttributes(a) + ' href="' + Markup._fixUrl(c) + '"';
						if (Markup._isUrlExternal(c)) {
							b += ' target="_blank"'
						}
						if (a.rel) {
							b += ' rel="' + a.rel + '"'
						}
						if (a.onclick) {
							b += ' onclick="' + a.onclick + '"'
						}
						if (a.tooltip && Markup.tooltipTags[a.tooltip]) {
							b += " onmouseover=\"Tooltip.showAtCursor(event, Markup.tooltipTags['" + a.tooltip + '\'], 0, 0, \'q\')" onmousemove="Tooltip.cursorUpdate(event)" onmouseout="Tooltip.hide()"'
						}
						b += ">";
						return [b, "</a>"]
					} else {
						return ["", ""]
					}
				} else {
					c = a._textContents;
					c = c.replace(/&amp;/, "&");
					if (Markup._isUrlSafe(c)) {
						var b = "<a" + Markup._addGlobalAttributes(a) + ' href="' + Markup._fixUrl(c) + '"';
						if (Markup._isUrlExternal(c)) {
							b += ' target="_blank"'
						}
						if (a.rel) {
							b += ' rel="' + a.rel + '"'
						}
						if (a.onclick) {
							b += ' onclick="' + a.onclick + '"'
						}
						b += ">";
						return [b + c + "</a>"]
					} else {
						return ["", ""]
					}
				}
			}
		},
		youtube: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /\S+/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var b = "http://www.youtube.com/v/" + a.unnamed + "&fs=1";
				var c = "";
				c += '<object width="480" height="300"' + Markup._addGlobalAttributes(a) + '><param name="movie" value="' + b + '">';
				c += '<param name="allowfullscreen" value="true"></param>';
				c += '<param name="allowscriptaccess" value="always"></param>';
				c += '<param name="wmode" value="opaque"></param>';
				c += '<embed width="480" height="300" src="' + b + '" type="application/x-shockwave-flash" allowscriptaccess="always" allowfullscreen="true" wmode="opaque"></embed>';
				c += "</object>";
				return c
			}
		},
		zone: {
			empty: true,
			attr: {
				unnamed: {
					req: true,
					valid: /^[0-9]+$/
				},
				domain: {
					req: false,
					valid: /^(ptr|www|de|es|fr|ru)$/
				}
			},
			allowedClass: MARKUP_CLASS_STAFF,
			toHtml: function (a) {
				var d = a.unnamed;
				var b = "";
				var c = Markup.nameCol;
				if (g_zones[d]) {
					return '<a href="' + b + "?zone=" + d + '"' + Markup._addGlobalAttributes(a) + ">" + Markup._safeHtml(g_zones[d]) + "</a>"
				}
				return '<a href="' + b + "?zone=" + d + '"' + Markup._addGlobalAttributes(a) + ">(" + LANG.types[7][0] + " #" + d + ")</a>"
			},
			toText: function (a) {
				var c = a.unnamed;
				var b = Markup.nameCol;
				if (g_zones[c]) {
					return Markup._safeHtml(g_zones[c])
				}
				return LANG.types[7][0] + " #" + c
			}
		}
	},
	_addGlobalAttributes: function (a) {
		var b = "";
		if (a.id) {
			b += ' id="' + a.id + '"'
		}
		if (a.title) {
			b += ' title="' + Markup._safeQuotes(a.title) + '"'
		}
		return b
	},
	_generateTagDocs: function (d) {
		var b = Markup.tags[d];
		if (!b) {
			return ""
		}
		var g = '<div><h3 class="first">Tag: [' + Markup._safeHtml(d) + "]</h3>";
		g += '<table class="grid">';
		if (b.attr) {
			g += '<tr><td align="right" width="200">Attributes:</td><td>';
			for (var c in b.attr) {
				g += '<div style="margin: 5px; display: inline-block"><table><tr><th style="background-color: #242424; font-weight: bolder" colspan="2">';
				if (c == "unnamed") {
					g += "Self ([" + d + "=???])"
				} else {
					g += c
				}
				g += "</th></tr>";
				g += '<tr><td align="right">Required:</td><td>' + (b.attr[c].req ? "Yes" : "No") + "</td></tr>";
				g += '<tr><td align="right">Valid:</td><td>' + (b.attr[c].valid ? Markup._safeHtml(b.attr[c].valid.toString()) : "--") + "</td></tr></table></div>"
			}
			g += "</td></tr>"
		}
		g += '<tr><td align="right" width="200">Has closing tag:</td><td>' + (b.empty ? "No" : "Yes") + "</td></tr>";
		g += '<tr><td align="right">Required group:</td><td>';
		if (b.allowedClass == MARKUP_CLASS_ADMIN) {
			g += "Administrator"
		} else {
			if (b.allowedClass == MARKUP_CLASS_STAFF) {
				g += "Staff"
			} else {
				if (b.allowedClass == MARKUP_CLASS_PREMIUM) {
					g += "Premium"
				} else {
					g += "None"
				}
			}
		}
		g += "</td></tr>";
		if (b.allowedChildren) {
			g += '<tr><td align="right">Allowed children:</td><td>';
			for (var e in b.allowedChildren) {
				g += Markup._safeHtml(e) + "<br />"
			}
			g += "</td></tr>"
		}
		if (b.allowedParents) {
			g += '<tr><td align="right">Allowed parents:</td><td>';
			for (var e in b.allowedParents) {
				g += Markup._safeHtml(e) + "<br />"
			}
			g += "</td></tr>"
		}
		if (b.presets) {
			g += '<tr><td align="right">Preset values:</td><td><table>';
			for (var f in b.presets) {
				g += '<tr><td align="right">' + f + "</td><td>" + Markup._safeHtml(b.presets[f]) + "</td></tr>"
			}
			g += "</table></td></tr>"
		}
		if (b.trim) {
			g += '<tr><td colspan="2">Trim whitespace</td></tr>'
		}
		if (b.ltrim) {
			g += '<tr><td colspan="2">Trim preceding whitespace</td></tr>'
		}
		if (b.rtrim) {
			g += '<tr><td colspan="2">Trim following whitespace</td></tr>'
		}
		if (b.itrim) {
			g += '<tr><td colspan="2">Trim whitespace around interior content</td></tr>'
		}
		if (b.block) {
			g += '<tr><td colspan="2">Automatically remove top padding if not the first item</td></tr>'
		}
		g += "</table></div>";
		return g
	},
	_init: function () {
		if (!this.inited) {
			var b = [],
				c = [],
				e = [];
			for (var a in Markup.tags) {
				if (Markup.tags[a].block) {
					this.firstTags[a] = true
				}
				if (Markup.tags[a].exclude) {
					for (var d in Markup.tags[a].exclude) {
						if (!this.excludeTags[d]) {
							this.excludeTags[d] = {}
						}
						this.excludeTags[d][a] = Markup.tags[a].exclude[d]
					}
				}
				if (Markup.tags[a].post) {
					this.postTags.push(a)
				}
				if (Markup.tags[a].trim) {
					e.push(a)
				}
				if (Markup.tags[a].ltrim) {
					b.push(a)
				}
				if (Markup.tags[a].rtrim) {
					c.push(a)
				}
			}
			if (b.length > 0) {
				this.ltrimRegex = new RegExp("\\s*\\[(" + b.join("|") + ")([^a-z0-9]+.*)?]", "ig")
			}
			if (c.length > 0) {
				this.rtrimRegex = new RegExp("\\[/(" + c.join("|") + ")\\]\\s*", "ig")
			}
			if (e.length > 0) {
				this.trimRegex = new RegExp("\\s*\\[(" + e.join("|") + ")([^\\[]*)?\\]\\s*", "ig")
			}
			this.inited = true
		}
	},
	_safeJsString: function (a) {
		return a.replace(/'/g, "'")
	},
	_safeQuotes: function (a) {
		return a.replace('"', '"').replace("'", "'")
	},
	_safeHtml: function (a) {
		var b = ["nbsp", "ndash"];
		a = a.replace(/&/g, "&amp;");
		if (b.length > 0) {
			a = a.replace(new RegExp("&amp;(" + b.join("|") + ");", "g"), "&$1;")
		}
		return a.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;")
	},
	_preText: function (a) {
		a = Markup._safeHtml(a);
		a = a.replace(/\n/g, "<br />");
		return a
	},
	_isUrlSafe: function (b, a) {
		if (!b) {
			return true
		}
		if (a) {
			return !b.match(/^[a-z0-9]+?:/i) || b.match(/^(https?:|mailto:|irc:)/i) || b == "javascript:;"
		} else {
			return !b.match(/^[a-z0-9]+?:/i) || b.match(/^https?:/i)
		}
	},
	_fixUrl: function (a) {
		if (!a) {
			return ""
		}
		var b = a.charAt(0);
		/*if (b == "/" || b == "?") {
			a = a.replace(/^[\/\?]+/, "");
			a = "/" + a
		}*/
		return a
	},
	_isUrlExternal: function (a) {
		if (!a) {
			return false
		}
		return (a.indexOf("wowhead.com") == -1 && a.match(/^https?:/i))
	},
	_nodeSearch: function (b, a, c) {
		if (!c) {
			c = 0
		}
		if (c >= 3) {
			return
		}
		if (b.name == a) {
			return true
		} else {
			if (b.parent) {
				return Markup._nodeSearch(b.parent, a, c + 1)
			}
		}
	},
	_parse: function (p, f) {
		if (g_locale.name != "ptr") {
			Markup.nameCol = "name_" + g_locale.name
		} else {
			Markup.nameCol = "name_enus"
		}
		p = p.replace(/\r/g, "");
		if (!f) {
			f = {}
		}
		Markup.uid = f.uid || "abc";
		Markup.root = f.root;
		if (Markup.uid != "abc") {
			g_screenshots[Markup.uid] = []
		}
		if (f.roles && (f.roles & 190) && f.mode != Markup.MODE_SIGNATURE) {
			f.mode = Markup.MODE_ARTICLE
		}
		Markup.mode = f.mode || Markup.MODE_ARTICLE;
		Markup.allow = f.allow || Markup.CLASS_STAFF;
		if (f.stopAtBreak) {
			var v = p.indexOf("[break]");
			if (v != -1) {
				p = p.substring(0, v)
			}
		} else {
			p = p.replace("[break]", "")
		}
		var m = new MarkupTree();
		p = p.trim();
		if (this.postTags.length) {
			for (var t in this.postTags) {
				var D = this.postTags[t];
				if (p.indexOf("[" + D) != -1) {
					if (! (Markup.tags[D].allowedModes && Markup.tags[D].allowedModes[MarkupModeMap[f.mode]] == undefined)) {
						for (var j in Markup.tags[D].collect) {
							this.collectTags[j] = true
						}
					}
				}
			}
		}
		p = p.replace(/\n(\s*)\n/g, "\n\n");
		var u = p.length;
		var x = 0,
			k = 0,
			g = -1,
			l = -1,
			b = true,
			q = false;
		var c = function (G) {
			var i, F, E;
			if (G.charAt(0) == '"' || G.charAt(0) == "'") {
				i = G.charAt(0);
				var a = G.indexOf(i, 1);
				if (a > -1) {
					E = G.substring(1, a);
					G = G.substring(a + 1).trim();
					return {
						value: Markup._safeHtml(E),
						str: G
					}
				}
			}
			F = G.indexOf(" ");
			if (F > -1) {
				E = G.substring(0, F);
				G = G.substring(F + 1).trim()
			} else {
				E = G;
				G = ""
			}
			return {
				value: E,
				str: G
			}
		};
		var r = /^\s*[a-z0-9]+\s*=/;
		while (k < u) {
			g = p.indexOf("[", k);
			if (g > -1) {
				k = g + 1;
				if (g > 0 && p.charAt(g - 1) == "\\") {
					b = false;
					g = -1
				} else {
					l = p.indexOf("]", k)
				}
			} else {
				k = u
			}
			var d, n = {};
			if (l > -1) {
				var z = p.substring(g + 1, l);
				if (z.charAt(0) == "/") {
					q = true;
					d = z.substr(1).trim().toLowerCase()
				}
				if (!q) {
					var y = z.indexOf(" "),
						w = z.indexOf("=");
					var A;
					if ((w < y || y == -1) && w > -1) {
						d = z.substring(0, w).toLowerCase();
						z = z.substring(w + 1).trim();
						var C = c(z);
						z = C.str;
						if (Markup.tags[d] == undefined || Markup.tags[d].attr == undefined || Markup.tags[d].attr.unnamed == undefined) {
							b = false
						} else {
							n.unnamed = C.value
						}
					} else {
						if (y > -1) {
							d = z.substring(0, y).toLowerCase();
							z = z.substring(y + 1).trim();
							if (z.indexOf("=") == -1) {
								if (Markup.tags[d] == undefined || Markup.tags[d].attr == undefined || Markup.tags[d].attr.unnamed == undefined) {
									b = false
								} else {
									n.unnamed = z
								}
								z = ""
							}
						} else {
							d = z.toLowerCase();
							z = ""
						}
					}
					if (Markup.tags[d] == undefined) {
						b = false
					} else {
						if (b) {
							var D = Markup.tags[d];
							while (z != "") {
								var o = "";
								if (!r.test(z)) {
									o = "unnamed"
								} else {
									w = z.indexOf("=");
									if (w == -1) {
										b = false;
										break
									}
									o = z.substring(0, w).trim().toLowerCase();
									z = z.substring(w + 1).trim()
								}
								var C = c(z);
								z = C.str;
								if (D.attr == undefined || D.attr[o] == undefined) {
									if (Markup.attributes[o] == undefined || (Markup.attributes[o].valid != undefined && !Markup.attributes[o].valid.test(C.value))) {
										b = false;
										break
									}
								}
								n[o] = C.value
							}
							if (b && D.attr) {
								for (var B in D.attr) {
									if (D.attr[B].req && n[B] == undefined) {
										b = false;
										break
									} else {
										if (n[B] == undefined) {
											continue
										}
									}
									if (D.attr[B].valid != undefined && !D.attr[B].valid.test(n[B])) {
										b = false;
										break
									}
								}
								if (b && D.validate != undefined) {
									b = D.validate(n)
								}
							}
						}
					}
				} else {
					if (Markup.tags[d] == undefined) {
						b = false
					}
				}
			} else {
				b = false
			}
			if (b) {
				if (x != g) {
					var h = p.substring(x, g).replace(/\\\[/g, "[");
					var e = {
						_rawText: h
					};
					m.openTag("<text>", e)
				}
				if (q) {
					b = m.closeTag(d)
				} else {
					b = m.openTag(d, n)
				}
				if (b) {
					x = k = l + 1
				} else {
					x = g
				}
			}
			b = true;
			q = false;
			g = l = -1
		}
		if (x < u) {
			var h = p.substr(x);
			var e = {
				_rawText: h
			};
			m.openTag("<text>", e)
		}
		return m
	},
	createMaps: function () {
		for (var b = 0; b < Markup.maps.length; ++b) {
			var a = Markup.maps[b];
			new Mapper({
				parent: a[0],
				zone: a[1],
				coords: a[2],
				unique: b
			})
		}
		Markup.maps = []
	},
	toHtml: function (d, c) {
		if (!c) {
			c = {}
		}
		if (!c.allow) {
			if (c.roles) {
				c.allow = Markup.rolesToClass(c.roles)
			} else {
				c.allow = Markup.CLASS_STAFF
			}
		}
		var a = Markup._parse(d, c);
		var b = a.toHtml();
		if (c.prepend) {
			b = c.prepend + b
		}
		if (c.append) {
			b += append
		}
		setTimeout(Markup.createMaps, 250);
		return b
	},
	removeTags: function (c, b) {
		var a = Markup._parse(c, b);
		return a.tagless()
	},
	getImageUploadIds: function (c, b) {
		var a = Markup._parse(c, b);
		return a.imageUploadIds()
	},
	printHtml: function (c, d, b) {
		d = ge(d);
		var a = Markup.toHtml(c, b);
		d.innerHTML = a;
		Markup.createMaps()
	},
	mapperPreview: function (c) {
		try {
			window.mapper = Markup.maps[c];
			var b = window.open("?edit=mapper-preview", "mapperpreview", "toolbar=no,location=no,directories=no,status=yes,menubar=no,scrollbars=no,resizable=no,width=800,height=540");
			b.focus()
		} catch(a) {}
	},
	createTabs: function (a, d, f) {
		var b = new Tabs({
			parent: ge("dsf67g4d-" + a.id + (f ? "-preview" : "")),
			forum: 1
		});
		for (var c = 0; c < d.length; ++c) {
			var e = d[c];
			b.add(e.name, {
				id: a.id + "-" + e.id + (f ? "-preview" : ""),
				icon: e.icon
			})
		}
		b.flush()
	}
};
var MarkupUtil = {
	ltrimText: function (a) {
		a._rawText = a._rawText.ltrim();
		return a
	},
	rtrimText: function (a) {
		a._rawText = a._rawText.rtrim();
		return a
	},
	checkSiblingTrim: function (a, b) {
		if (b.name == "<text>" && (Markup.tags[a.name].rtrim || Markup.tags[a.name].trim)) {
			b.attr = MarkupUtil.ltrimText(b.attr)
		} else {
			if (a.name == "<text>" && (Markup.tags[b.name].ltrim || Markup.tags[b.name].trim)) {
				a.attr = MarkupUtil.rtrimText(a.attr)
			}
		}
		return [a, b]
	}
};
var MarkupTree = function () {
	this.nodes = [];
	this.currentNode = null
};
MarkupTree.prototype = {
	openTag: function (b, c) {
		if (!Markup.tags[b]) {
			return false
		} else {
			if (Markup.tags[b].allowedModes && Markup.tags[b].allowedModes[MarkupModeMap[Markup.mode]] == undefined) {
				return false
			} else {
				if (Markup.tags[b].allowedClass && Markup.tags[b].allowedClass > Markup.allow) {
					return false
				}
			}
		}
		var d = {
			name: b,
			attr: c,
			parent: null,
			nodes: []
		};
		if (this.currentNode) {
			d.parent = this.currentNode
		}
		if (Markup.tags[b].allowedParents) {
			if (d.parent != null) {
				if (Markup.tags[b].allowedParents[d.parent.name] === undefined) {
					return false
				}
			} else {
				if (Markup.root == undefined || Markup.tags[b].allowedParents[Markup.root] == undefined) {
					return false
				}
			}
		}
		if (d.parent && Markup.tags[d.parent.name].allowedChildren && Markup.tags[d.parent.name].allowedChildren[b] == undefined) {
			return false
		}
		if (this.currentNode) {
			if (this.currentNode.nodes.length == 0 && d.name == "<text>" && Markup.tags[this.currentNode.name].itrim) {
				d.attr = MarkupUtil.ltrimText(d.attr)
			} else {
				if (this.currentNode.nodes.length > 0) {
					var a = this.currentNode.nodes.length - 1;
					var e = MarkupUtil.checkSiblingTrim(this.currentNode.nodes[a], d);
					this.currentNode.nodes[a] = e[0];
					d = e[1]
				}
			}
			if (d.name == "<text>") {
				d.attr._text = Markup._preText(d.attr._rawText)
			}
			this.currentNode.nodes.push(d)
		} else {
			if (this.nodes.length > 0) {
				var a = this.nodes.length - 1;
				var e = MarkupUtil.checkSiblingTrim(this.nodes[a], d);
				this.nodes[a] = e[0];
				d = e[1]
			}
			if (d.name == "<text>") {
				d.attr._text = Markup._preText(d.attr._rawText)
			}
			this.nodes.push(d)
		}
		if (!Markup.tags[b].empty && !Markup.tags[b].post) {
			this.currentNode = d
		}
		return true
	},
	closeTag: function (c) {
		if (Markup.tags[c].empty || Markup.tags[c].post) {
			return false
		}
		if (!this.currentNode) {
			return false
		} else {
			if (this.currentNode.name == c) {
				if (this.currentNode.nodes.length > 0) {
					var b = this.currentNode.nodes.length - 1;
					if (Markup.tags[this.currentNode.name].itrim && this.currentNode.nodes[b].name == "<text>") {
						var e = this.currentNode.nodes[b];
						e.attr = MarkupUtil.rtrimText(e.attr);
						e.attr._text = Markup._preText(e.attr._rawText);
						this.currentNode.nodes[b] = e
					}
				}
				this.currentNode = this.currentNode.parent
			} else {
				var d = function (g, f) {
					for (var h = f.length - 1; h >= 0; --h) {
						if (f[h].name == g) {
							return h
						}
					}
					return -1
				};
				var a;
				if (this.currentNode.parent) {
					a = d(c, this.currentNode.parent.nodes)
				} else {
					a = d(c, this.nodes)
				}
				if (a == -1) {
					return false
				}
			}
		}
		return true
	},
	toHtml: function () {
		var c = [];
		var b = {};
		for (var h in Markup.collectTags) {
			b[h] = []
		}
		this.tagless(true);
		var g = 0;
		var a = function (k, n, q) {
			var u = "";
			for (var m = 0; m < k.length; ++m) {
				var l = k[m];
				if (n == 0 && m == 0 && Markup.firstTags[l.name]) {
					l.attr.first = true
				}
				if (Markup.excludeTags[l.name]) {
					q[l.name] = (q[l.name] ? q[l.name] + 1 : 1)
				}
				for (var r in q) {
					for (var v in Markup.excludeTags[r]) {
						if (Markup.excludeTags[r][v][l.name]) {
							l.attr[v] = false
						}
					}
				}
				if (Markup.collectTags[l.name]) {
					l.offset = g++;
					b[l.name].push(l)
				}
				if (Markup.tags[l.name].post) {
					c.push([l, u.length])
				} else {
					if (Markup.tags[l.name].empty) {
						var p;
						if (l.parent && Markup.tags[l.parent.name].rawText) {
							p = Markup.tags[l.name].toHtml(l.attr, {
								needsRaw: true
							})
						} else {
							p = Markup.tags[l.name].toHtml(l.attr)
						}
						if (typeof p == "string") {
							u += p
						} else {
							if (p !== undefined) {
								if (u == "") {
									u = []
								}
								u.push(p)
							}
						}
					} else {
						var o = arguments.callee(l.nodes, n + 1, q);
						l.attr._contents = o;
						var w = Markup.tags[l.name].toHtml(l.attr);
						if (w.length == 2) {
							u += w[0] + o + w[1]
						} else {
							if (w.length == 1) {
								if (typeof w[0] == "string") {
									u += w[0]
								} else {
									if (u == "") {
										u = []
									}
									u.push(w[0])
								}
							}
						}
					}
				}
				if (q[l.name]) {
					q[l.name]--;
					if (q[l.name] == 0) {
						delete q[l.name]
					}
				}
			}
			return u
		};
		str = a(this.nodes, 0, []);
		for (var e = 0; e < c.length; ++e) {
			var d = c[e][0];
			var j = c[e][1];
			var f = Markup.tags[d.name].postHtml(d.attr, b);
			if (typeof f == "string") {
				str = str.substr(0, j) + f + str.substr(j)
			}
		}
		return str
	},
	tagless: function (c) {
		var a = function (e) {
			var h = "";
			for (var f = 0; f < e.length; ++f) {
				var g = e[f];
				var d = arguments.callee(g.nodes);
				if (c) {
					g.attr._textContents = d
				} else {
					g.attr._contents = d
				}
				if (g.name == "<text>") {
					h += Markup.tags[g.name].toHtml(g.attr, {
						noLink: true
					})
				} else {
					if (Markup.tags[g.name].toText) {
						h += Markup.tags[g.name].toText(g.attr)
					}
				}
				if (!Markup.tags[g.name].taglessSkip) {
					h += d
				}
			}
			return h
		};
		if (c) {
			a(this.nodes)
		} else {
			var b = a(this.nodes);
			b = b.replace(/&amp;/g, "&").replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&quot;/g, '"');
			return b
		}
	},
	imageUploadIds: function () {
		var b = [];
		var a = function (c) {
			for (var d = 0; d < c.length; ++d) {
				var e = c[d];
				if (e.name == "img" && e.attr.upload) {
					b.push(e.attr.upload)
				}
				arguments.callee(e.nodes)
			}
		};
		a(this.nodes);
		return b
	}
};
Markup.tags.admin = {
	empty: false,
	allowedModes: {
		article: 1,
		quickfacts: 1
	},
	toHtml: function (a) {
		return ["",""];
	}
};
Markup.tags.flashtext = {
	empty: false,
	allowedModes: {
		article: 1
	},
	attr: {
		font: {
			req: true,
			valid: /\S+/ // todo
		},
		width: {
			req: false,
			valid: /^[0-9]+(px|em|\%)$/
		},
		height: {
			req: false,
			valid: /^[0-9]+(px|em|\%)$/
		}
	},
	toHtml: function (a) {
		var d = 'sdju9fe-' + Math.random() + '-' + Math.round(new Date().getTime() / 1000);
		var f = 'sdjosijo-' + Math.random() + '-' + Math.round(new Date().getTime() / 1000);
		var t = "<div id='" + d + "'><div id='" + f + "'></div></div>";
		var flashvars = { xmlpath:"http://wow.tltgame.ru/aowow/flash/flashtext.xml", classname:a.font, divid:d, text:urlencode(a._contents) };
		var params = { menu:"false", wmode:"transparent", base:".", allowScriptAccess:"true" };
		var h = a.height ? a.height : "100%";
		var w = a.width ? a.width : "100%";
		setTimeout(swfobject.embedSWF.bind(null, "http://wow.tltgame.ru/aowow/flash/flashtext.swf", f, w, h, "9.0.0", false, flashvars, params), 10);
		/*var t = '<div id="' + d + '" style="visibility: visible;"></div>';
		t += '<object style="visibility: visible;" id="' + + '" data="http://wow.tltgame.ru/aowow/flash/flashtext.swf" type="application/x-shockwave-flash" height="' + h + '" width="' + w + '">';
		t += '<param value="false" name="menu">';
		t += '<param value="transparent" name="wmode">';
		t += '<param value="." name="base">';
		t += '<param value="true" name="allowScriptAccess">';
		t += '<param value="xmlpath=http://wow.tltgame.ru/aowow/flash/flashtext.xml&classname=' + a.font + '&divid=' + d + '&text=' + urlencode(a._contents) + '" name="flashvars">';
		t += '</object>';*/
		return [t]
	}
};
Markup.tags.bugreport = {
	empty: true,
	attr: {
		id: {
			req: true,
			valid: /^[0-9]+$/
		},
		unnamed: {
			req: false,
			valid: /^closed$/i
		}
	},
	allowedModes: {
		article: 1,
		quickfacts: 1
	},
	toHtml: function (a) {
		var e = a.id;
		var c = a.unnamed;
		var t = '<a href="?bugtracker&issue=' + e + '"';
		if(c == "closed") {
			t += " style='text-decoration:line-through;'"
		}
		t += ">#" + e + "</a>";
		return t
	}
};
Markup.tags.facepalm = {
	empty: true,
	attr: {},
	allowedModes: {
		article: 1,
		quickfacts: 1
	},
	toHtml: function (a) {
		return '<img src="images/facepalm.jpg" width="400px" height="320px" />'
	}
};
Markup.tags.trinitycore = {
	empty: true,
	attr: {
		unnamed: {
			req: true,
			valid: /^[0-9a-fA-F]+$/
		}
	},
	allowedModes: {
		article: 1
	},
	toHtml: function (a) {
		var d = a.unnamed.toLowerCase();
		var b = "http://code.google.com/p/trinitycore/source/detail?r=" + d;
		var c = d.substr(0, 6);
		return "<a href='" + b + "'>" + c + "</a>"
	}
};
Markup._init();