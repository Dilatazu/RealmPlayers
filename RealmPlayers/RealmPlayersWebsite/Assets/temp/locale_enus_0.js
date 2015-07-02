var mn_forums=[
	[0,"AoWoW General"],
	[-2,"Versions","",[
		[10,"Classic"],
		[11,"TBC"],
		[12,"WotLK"]
	]],
	[3, "Guides"]
];
var mn_characters =[
		[1,"Human",,[
			[1,"Warrior"],
			[2,"Paladin"],
			[4,"Rogue"],
			[5,"Priest"],
			[8,"Mage"],
			[9,"Warlock"]
		]],
		[3,"Dwarf",,[
			[1,"Warrior"],
			[2,"Paladin"],
			[3,"Hunter"],
			[4,"Rogue"],
			[5,"Priest"]
		]],
		[4,"Night Elf",,[
			[1,"Warrior"],
			[3,"Hunter"],
			[4,"Rogue"],
			[5,"Priest"],
			[11,"Druid"]
		]],
		[7,"Gnome",,[
			[1,"Warrior"],
			[4,"Rogue"],
			[8,"Mage"],
			[9,"Warlock"]
		]],
		[2,"Orc",,[
			[1,"Warrior"],
			[3,"Hunter"],
			[4,"Rogue"],
			[7,"Shaman"],
			[9,"Warlock"]
		]],
		[5,"Undead",,[
			[1,"Warrior"],
			[4,"Rogue"],
			[5,"Priest"],
			[8,"Mage"],
			[9,"Warlock"]
		]],
		[6,"Tauren",,[
			[1,"Warrior"],
			[3,"Hunter"],
			[7,"Shaman"],
			[11,"Druid"]
		]],
		[8,"Troll",,[
			[1,"Warrior"],
			[3,"Hunter"],
			[4,"Rogue"],
			[5,"Priest"],
			[7,"Shaman"],
			[8,"Mage"]
		]]
		];
/* mn_items[0].tinyIcon = "achievement_boss_onyxia";
mn_items[2].tinyIcon = "spell_nature_starfall";
mn_items[3].tinyIcon = "inv_misc_head_dragon_black";
mn_items[4].tinyIcon = "achievement_dungeon_naxxramas_normal"; */
var mn_itemSets=[
	[11,"Druid"],
	[3,"Hunter"],
	[8,"Mage"],
	[2,"Paladin"],
	[5,"Priest"],
	[4,"Rogue"],
	[7,"Shaman"],
	[9,"Warlock"],
	[1,"Warrior"]
];
var mn_npcs=[
	[1,"Beasts"],
	[8,"Critters"],
	[3,"Demons"],
	[2,"Dragonkin"],
	[4,"Elementals"],
	[5,"Giants"],
	[7,"Humanoids"],
	[9,"Mechanicals"],
	[6,"Undead"],
	[10,"Uncategorized"]
];
var mn_objects=[
	[9,"Books"],
	[3,"Containers"],
	[-5,"Footlockers"],
	[-3,"Herbs"],
	[-4,"Mineral Veins"],
	[-2,"Quest"]
];
var mn_quests=[
	[0,"Eastern Kingdoms",,[
		[36,"Alterac Mountains"],
		[45,"Arathi Highlands"],
		[3,"Badlands"],
		[25,"Blackrock Mountain"],
		[4,"Blasted Lands"],
		[46,"Burning Steppes"],
		[41,"Deadwind Pass"],
		[2257,"Deeprun Tram"],
		[1,"Dun Morogh"],
		[10,"Duskwood"],
		[139,"Eastern Plaguelands"],
		[12,"Elwynn Forest"],
		[9,"Northshire Abbey (Elwynn Forest)"],
		[267,"Hillsbrad Foothills"],
		[1537,"Ironforge"],
		[38,"Loch Modan"],
		[44,"Redridge Mountains"],
		[51,"Searing Gorge"],
		[130,"Silverpine Forest"],
		[1519,"Stormwind City"],
		[33,"Stranglethorn Vale"],
		[8,"Swamp of Sorrows"],
		[47,"The Hinterlands"],
		[85,"Tirisfal Glades"],
		[1497,"Undercity"],
		[28,"Western Plaguelands"],
		[40,"Westfall"],
		[11,"Wetlands"]
	]],
	[1,"Kalimdor",,[
		[331,"Ashenvale"],
		[16,"Azshara"],
		[148,"Darkshore"],
		[1657,"Darnassus"],
		[405,"Desolace"],
		[14,"Durotar"],
		[15,"Dustwallow Marsh"],
		[361,"Felwood"],
		[357,"Feralas"],
		[493,"Moonglade"],
		[215,"Mulgore"],
		[1637,"Orgrimmar"],
		[1377,"Silithus"],
		[406,"Stonetalon Mountains"],
		[440,"Tanaris"],
		[141,"Teldrassil"],
		[17,"The Barrens"],
		[400,"Thousand Needles"],
		[1638,"Thunder Bluff"],
		//[1216,"Timbermaw Hold"],
		[490,"Un'Goro Crater"],
		[618,"Winterspring"]
	]],
	[2,"Dungeons",,[
		[719,"Blackfathom Deeps"],
		[1584,"Blackrock Depths"],
		[1583,"Blackrock Spire"],
		[2557,"Dire Maul"],
		[133,"Gnomeregan"],
		[2100,"Maraudon"],
		[2437,"Ragefire Chasm"],
		[722,"Razorfen Downs"],
		[1717,"Razorfen Kraul"],
		[796,"Scarlet Monastery"],
		[2057,"Scholomance"],
		[209,"Shadowfang Keep"],
		[2017,"Stratholme"],
		[1417,"Sunken Temple"],
		[1581,"The Deadmines"],
		[717,"The Stockade"],
		[1517,"Uldaman"],
		[718,"Wailing Caverns"],
		[978,"Zul'Farrak"]
	]],
	[3,"Raids",,[
		[2677,"Blackwing Lair"],
		/* [3606,"Hyjal Summit"], */
		[2717,"Molten Core"],
		[3456,"Naxxramas"],
		[2159,"Onyxia's Lair"],
		[3429,"Ruins of Ahn'Qiraj"],
		[3428,"Temple of Ahn'Qiraj"],
		[19,"Zul'Gurub"]
	]],
	[4,"Classes",,[
		[-263,"Druid"],
		[-261,"Hunter"],
		[-161,"Mage"],
		[-141,"Paladin"],
		[-262,"Priest"],
		[-162,"Rogue"],
		[-82,"Shaman"],
		[-61,"Warlock"],
		[-81,"Warrior"]
	]],
	[5,"Professions",,[
		[-181,"Alchemy"],
		[-121,"Blacksmithing"],
		[-304,"Cooking"],
		[-201,"Engineering"],
		[-324,"First Aid"],
		[-101,"Fishing"],
		[-24,"Herbalism"],
		[-182,"Leatherworking"],
		[-264,"Tailoring"]
	]],
	[6,"Battlegrounds",,[
		[2597,"Alterac Valley"],
		[3358,"Arathi Basin"],
		[-25,"Battlegrounds"],
		[3277,"Warsong Gulch"]
	]],
	[9,"World Events",,[
		[-370,"Brewfest"],
		[-284,"Children's Week"],
		[-364,"Darkmoon Faire"],
		[-41,"Day of the Dead"],
		[-22,"Hallow's End"],
		[-22,"Harvest Festival"],
		[-22,"Love is in the Air"],
		[-22,"Lunar Festival"],
		[-369,"Midsummer"],
		[-22,"New Year's Eve"],
		[-375,"Pilgrim's Bounty"],
		[-374,"Noblegarden"],
		[-22,"Winter Veil"]
	]],
	[7,"Miscellaneous",,[
		[-365,"Ahn'Qiraj War Effort"],
		[-1,"Epic"],
		[-344,"Legendary"],
		[-367,"Reputation"],
		[-368,"Scourge Invasion"],
	]],
	[-2,"Uncategorized"]
];
var mn_titles=[
	[0,"General"],
	[4,"Quests"],
	[1,"Player vs. Player"],
	[3,"Dungeons & Raids"],
	[5,"Professions"],
	[2,"Reputation"],
	[6,"World Events"]
];
var mn_spells=[
	[,"Character"],
	[7,"Class Skills",,[
		[11,"Druid",,[
			[574,"Balance"],
			[134,"Feral Combat"],
			[573,"Restoration"]
		]],
		[3,"Hunter",,[
			[50,"Beast Mastery"],
			[163,"Marksmanship"],
			[51,"Survival"]
		]],
		[8,"Mage",,[
			[237,"Arcane"],
			[8,"Fire"],
			[6,"Frost"]
		]],
		[2,"Paladin",,[
			[594,"Holy"],
			[267,"Protection"],
			[184,"Retribution"]
		]],
		[5,"Priest",,[
			[613,"Discipline"],
			[56,"Holy"],
			[78,"Shadow Magic"]
		]],
		[4,"Rogue",,[
			[253,"Assassination"],
			[38,"Combat"],
			[39,"Subtlety"],
			[633,"Lockpicking"]
		]],
		[7,"Shaman",,[
			[375,"Elemental Combat"],
			[373,"Enhancement"],
			[374,"Restoration"]
		]],
		[9,"Warlock",,[
			[355,"Affliction"],
			[354,"Demonology"],
			[593,"Destruction"]
		]],
		[1,"Warrior",,[
			[26,"Arms"],
			[256,"Fury"],
			[257,"Protection"]
		]]
	]],
	[-2,"Talents",,[
		[11,"Druid",,[
			[574,"Balance"],
			[134,"Feral Combat"],
			[573,"Restoration"]
		]],
		[3,"Hunter",,[
			[50,"Beast Mastery"],
			[163,"Marksmanship"],
			[51,"Survival"]
		]],
		[8,"Mage",,[
			[237,"Arcane"],
			[8,"Fire"],
			[6,"Frost"]
		]],
		[2,"Paladin",,[
			[594,"Holy"],
			[267,"Protection"],
			[184,"Retribution"]
		]],
		[5,"Priest",,[
			[613,"Discipline"],
			[56,"Holy"],
			[78,"Shadow Magic"]
		]],
		[4,"Rogue",,[
			[253,"Assassination"],
			[38,"Combat"],
			[39,"Subtlety"]
		]],
		[7,"Shaman",,[
			[375,"Elemental Combat"],
			[373,"Enhancement"],
			[374,"Restoration"]
		]],
		[9,"Warlock",,[
			[355,"Affliction"],
			[354,"Demonology"],
			[593,"Destruction"]
		]],
		[1,"Warrior",,[
			[26,"Arms"],
			[256,"Fury"],
			[257,"Protection"]
		]]
	]],
	[-4,"Racial Traits"],
	/* [-3,"Pet Skills",,[
		[,"Hunter"],
		[270,"Generic"],
		[653,"Bat"],
		[210,"Bear"],
		[211,"Boar"],
		[209,"Cat"],
		[214,"Crab"],
		[212,"Crocolisk"],
		[215,"Gorilla"],
		[654,"Hyena"],
		[217,"Raptor"],
		[236,"Scorpid"],
		[768,"Serpent"],
		[203,"Spider"],
		[218,"Tallstrider"],
		[251,"Turtle"],
		[208,"Wolf"],
		[,"Warlock"],
		[189,"Felhunter"],
		[188,"Imp"],
		[205,"Succubus"],
		[204,"Voidwalker"]
	]], */
	[,"Professions & skills"],
	[11,"Professions",,[
		[171,"Alchemy"],
		[164,"Blacksmithing"/* ,,[
			[9788,"Armorsmithing"],
			[9787,"Weaponsmithing"],
			[17041,"Master Axesmithing"],
			[17040,"Master Hammersmithing"],
			[17039,"Master Swordsmithing"]
		] */],
		[333,"Enchanting"],
		[202,"Engineering"/* ,,[
			[20219,"Gnomish Engineering"],
			[20222,"Goblin Engineering"]
		] */],
		[182,"Herbalism"],
		[165,"Leatherworking"/* ,,[
			[10656,"Dragonscale Leatherworking"],
			[10658,"Elemental Leatherworking"],
			[10660,"Tribal Leatherworking"]
		] */],
		[186,"Mining"],
		[393,"Skinning"],
		[197,"Tailoring"]
	]],
	[9,"Secondary Skills",,[
		[185,"Cooking"],
		[129,"First Aid"],
		[356,"Fishing"],
		[762,"Riding"]
	]],
	[,"Other"],
	[8,"Armor Proficiencies"],
	//[-6,"Companions"],
	[10,"Languages"],
	//[-5,"Mounts"],
	[6,"Weapon Skills"],
	[0,"Uncategorized"]
];
var mn_zones=[
	[0,"Eastern Kingdoms"],
	[1,"Kalimdor"],
	[2,"Dungeons"],
	[3,"Raids"],
	[6,"Battlegrounds"]
];
var mn_factions=[
		[469,"Alliance"],
		[891,"Alliance Forces"],
		[67,"Horde"],
		[892,"Horde Forces"],
		[169,"Steamwheedle Cartel"],
	[0,"Other"]
];
var mn_pets=[
	[2,"Cunning"],
	[0,"Ferocity"],
	[1,"Tenacity"]
];
/* var mn_achievements=[
	[92,"General"],
	[96,"Quests",,[
		[14861,"Classic"],
		[14862,"The Burning Crusade"],
		[14863,"Wrath of the Lich King"]
	]],
	[97,"Exploration",,[
		[14777,"Eastern Kingdoms"],
		[14778,"Kalimdor"],
		[14779,"Outland"],
		[14780,"Northrend"]
	]],
	[95,"Player vs. Player",,[
		[165,"Arena"],
		[14801,"Alterac Valley"],
		[14802,"Arathi Basin"],
		[14803,"Eye of the Storm"],
		[14804,"Warsong Gulch"],
		[14881,"Strand of the Ancients"],
		[14901,"Wintergrasp"],
		[15003,"Isle of Conquest"]
	]],
	[168,"Dungeons & Raids",,[
		[14808,"Classic"],
		[14805,"The Burning Crusade"],
		[14806,"Lich King Dungeon"],
		[14921,"Lich King Heroic"],
		[14922,"Lich King 10-Player Raid"],
		[14923,"Lich King 25-Player Raid"],
		[14961,"Secrets of Ulduar 10-Player Raid"],
		[14962,"Secrets of Ulduar 25-Player Raid"],
		[15001,"Call of the Crusade 10-Player Raid"],
		[15002,"Call of the Crusade 25-Player Raid"],
		[15041,"Fall of the Lich King 10-Player Raid"],
		[15042,"Fall of the Lich King 25-Player Raid"]
	]],
	[169,"Professions",,[
		[170,"Cooking"],
		[171,"Fishing"],
		[172,"First Aid"]
	]],
	[201,"Reputation",,[
		[14864,"Classic"],
		[14865,"The Burning Crusade"],
		[14866,"Wrath of the Lich King"]
	]],
	[155,"World Events",,[
		[160,"Lunar Festival"],
		[187,"Love is in the Air"],
		[159,"Noblegarden"],
		[163,"Children's Week"],
		[161,"Midsummer"],
		[162,"Brewfest"],
		[158,"Hallow's End"],
		[14981,"Pilgrim's Bounty"],
		[156,"Winter Veil"],
		[14941,"Argent Tournament"]
	]],
	[81,"Feats of Strength"]
]; */
var mn_talentCalc=[
	[11,"Druid","?talent#0"],
	[3,"Hunter","?talent#c"],
	[8,"Mage","?talent#o"],
	[2,"Paladin","?talent#s"],
	[5,"Priest","?talent#b"],
	[4,"Rogue","?talent#f"],
	[7,"Shaman","?talent#h"],
	[9,"Warlock","?talent#I"],
	[1,"Warrior","?talent#L"]
];
var mn_petCalc=[
	[24,"Bat","?petcalc#MR"],
	[4,"Bear","?petcalc#0R"],
	[26,"Bird of Prey","?petcalc#Mb"],
	[5,"Boar","?petcalc#0a"],
	[7,"Carrion Bird","?petcalc#0r"],
	[2,"Cat","?petcalc#0m"],
	[45,"Core Hound","?petcalc#ma"],
	[38,"Chimaera","?petcalc#cw"],
	[8,"Crab","?petcalc#0w"],
	[6,"Crocolisk","?petcalc#0b"],
	[39,"Devilsaur","?petcalc#ch"],
	[30,"Dragonhawk","?petcalc#c0"],
	[9,"Gorilla","?petcalc#0h"],
	[25,"Hyena","?petcalc#Ma"],
	[37,"Moth","?petcalc#cr"],
	[34,"Nether Ray","?petcalc#cR"],
	[11,"Raptor","?petcalc#zM"],
	[31,"Ravager","?petcalc#cM"],
	[43,"Rhino","?petcalc#mo"],
	[20,"Scorpid","?petcalc#M0"],
	[35,"Serpent","?petcalc#ca"],
	[41,"Silithid","?petcalc#mM"],
	[3,"Spider","?petcalc#0o"],
	[46,"Spirit Beast","?petcalc#mb"],
	[33,"Sporebat","?petcalc#co"],
	[12,"Tallstrider","?petcalc#zm"],
	[21,"Turtle","?petcalc#MM"],
	[32,"Warp Stalker","?petcalc#cm"],
	[44,"Wasp","?petcalc#mR"],
	[27,"Wind Serpent","?petcalc#Mr"],
	[1,"Wolf","?petcalc#0M"],
	[42,"Worm","?petcalc#mm"]
];
var mn_holidays=[
	[1,"Holidays","?events=1"],
	[2,"Recurring","?events=2"],
	[3,"Player vs. Player","?events=3"]
];
var mn_database=[
	[0,"Items","?items",mn_items],
	[2,"Item Sets","?itemsets"/*,mn_itemSets*/],
	[4,"NPCs","?npcs",mn_npcs],
	[3,"Quests","?quests",mn_quests],
	[6,"Zones","?zones",mn_zones],
	[1,"Spells","?spells",mn_spells],
	//[9,"Achievements","?achievements",mn_achievements],
	[5,"Objects","?objects",mn_objects],
	//[10,"Characters","?characters",mn_characters]
	[7,"Factions","?factions=1118",mn_factions]
	//[10,"Titles","?titles",mn_titles],
	//[8,"Hunter Pets","?pets",mn_pets],
	//[11,"World Events","?events"/*,mn_holidays*/],
	//[8,"Users","?users"]
];
var mn_tools=[
	[0,"Talent Calculator","?talent",mn_talentCalc],
	//[2,"Hunter Pet Calculator","?petcalc",mn_petCalc],
	//[5,"Profiler","?profiles"],
	//[3,"Item Comparison","?compare"],
	[1,"Maps","?maps"],
	[,"Other"],
	/* [6,"Guides","?guide",[
		[6,"PvE", "?guide=pve"],
		[,"Expansions"],
		[3,"Cataclysm","?guide=cataclysm"],
		[2,"Wrath of the Lich King","?guide=wotlk"],
		[,"Patches"],
		[0,"3.3: Fall of the Lich King","?guide=3.3"],
		[,"World Events"],
		[5,"Lunar Festival","?guide=lunar-festival"],
		[4,"Love is in the Air","?guide=love-is-in-the-air"],
		[1,"Feast of Winter Veil","?guide=winter-veil"]
	]],
	[4,"Patch Notes","",[
		[,"Wrath of the Lich King"],
		[14,"3.3.3","?patchnotes=3.3.3"],
		[13,"3.3.2","?patchnotes=3.3.2"],
		[12,"3.3.0","?patchnotes=3.3.0"],
		[0,"3.2.2","?patchnotes=3.2.2"],
		[1,"3.2.0","?patchnotes=3.2.0"],
		[2,"3.1.3","?patchnotes=3.1.3"],
		[3,"3.1.2","?patchnotes=3.1.2"],
		[4,"3.1.0","?patchnotes=3.1.0"],
		[5,"3.0.9","?patchnotes=3.0.9"],
		[6,"3.0.8","?patchnotes=3.0.8"],
		[7,"3.0.3","?patchnotes=3.0.3"],
		[8,"3.0.2","?patchnotes=3.0.2"],
		[,"The Burning Crusade"],
		[9,"2.4.3","?patchnotes=2.4.3"],
		[10,"2.4.2","?patchnotes=2.4.2"],
		[11,"2.4.0","?patchnotes=2.4.0"]
	]], */
	/* [8,"Utilities",,[
		[,"Database"],
		[0,"Latest Additions","?latest-additions"],
		[1,"Latest Articles","?latest-articles"],
		[2,"Latest Comments","?latest-comments"],
		[3,"Latest Screenshots","?latest-screenshots"],
		[9,"New Items in Patch",,[
			[2,"3.3","?new-items=3.3"],
			[1,"3.2","?new-items=3.2"],
			[0,"3.1","?new-items=3.1"]
		]],
		[4,"Random Page","?random"],
		[5,"Unrated Comments","?unrated-comments"],
		[,"Forums"],
		[6,"Latest Replies","?latest-replies"],
		[7,"Latest Topics","?latest-topics"],
		[8,"Unanswered Topics","?unanswered-topics"]
	]], */
	[30,"Latest Comments","?latest=comments"],
	[31,"Latest Screenshots","?latest=screenshots"],
	[,"Goodies"],
	[32,"Tooltips for your site", "?tooltips"]
];
var mn_more=[
	[,"All About Wowhead"],
	[0,"About Us & Contact","?aboutus"],
	[3,"FAQ","?faq"],
	[13,"Help",,[
		[0,"Commenting and You","?help=commenting-and-you"],
		[5,"Item Comparison","?help=item-comparison"],
		[1,"Model Viewer","?help=modelviewer"],
		[6,"Profiler","?help=profiler"],
		[2,"Screenshots: Tips & Tricks","?help=screenshots-tips-tricks"],
		[3,"Stat Weighting","?help=stat-weighting"],
		[4,"Talent Calculator","?help=talent-calculator"]
	]],
	[12,"Jobs","?jobs"],
	[4,"Premium","?premium"],
	[7,"What's New","?whats-new"],
	[2,"Wowhead Client","?client"],
	[4,"Wowhead Store","http://store.wowhead.com/"],
	[,"Goodies"],
	[99,"LMWHTFY","http://www.lmwhtfy.com"],
	[10,"Powered by Wowhead","?powered"],
	[8,"Search Plugins","?searchplugins"],
	[9,"Spread Wowhead","?spread"],
	[,"Even More"],
	[5,"Network Sites",,[
		[99,"ZAM","http://www.zam.com/",[
			[99,"Aion","http://aion.zam.com"],
			[99,"Dark Age of Camelot","http://camelot.allakhazam.com"],
			[99,"EVE Online","http://eve.allakhazam.com"],
			[99,"EverQuest","http://everquest.allakhazam.com"],
			[99,"EverQuest II","http://eq2.allakhazam.com"],
			[99,"EverQuest Online Adventures","http://eqoa.allakhazam.com"],
			[99,"Final Fantasy XI","http://ffxi.allakhazam.com"],
			[99,"Final Fantasy XIV","http://ffxiv.zam.com"],
			[99,"FreeRealms","http://fr.zam.com"],
			[99,"Legends of Norrath","http://lon.allakhazam.com"],
			[99,"Lord of the Rings Online","http://lotro.allakhazam.com"],
			[99,"Star Wars Galaxies","http://swg.allakhazam.com"],
			[99,"Warhammer Online","http://war.allakhazam.com"],
			[99,"World of Warcraft","http://wow.allakhazam.com"]
		]],
		[99,"MMOUI","http://www.mmoui.com/",[
			[99,"EverQuest","http://www.eqinterface.com"],
			[99,"EverQuest II","http://www.eq2interface.com"],
			[99,"Lord of the Rings Online","http://www.lotrointerface.com"],
			[99,"Vanguard: Saga of Heroes","http://www.vginterface.com"],
			[99,"Warhammer Online","http://war.mmoui.com"],
			[99,"World of Warcraft","http://www.wowinterface.com"]
		]],
		[99,"Online Gaming Radio","http://www.onlinegamingradio.com/"],
		[99,"Thottbot","http://www.thottbot.com/"]
	]]
];
var mn_path=[
	[0,"Database",,mn_database],
	[1,"Tools",,mn_tools],
	[2,"Forums",,mn_forums]
	//[3,"More",,mn_more]
];
var g_chr_classes={
	1:"Warrior",
	2:"Paladin",
	3:"Hunter",
	4:"Rogue",
	5:"Priest",
	7:"Shaman",
	8:"Mage",
	9:"Warlock",
	11:"Druid"
	//13:"Rogue, Druid",
	//14:"Hunter, Shaman",
	//15:"Warrior, Paladin",
	//16:"Priest, Mage, Warlock",
};
var g_itemset_classes={
	1:"Warrior",
	2:"Paladin",
	3:"Hunter",
	4:"Rogue",
	5:"Priest",
	7:"Shaman",
	8:"Mage",
	9:"Warlock",
	11:"Druid",
	13:"Rogue, Druid",
	14:"Hunter, Shaman",
	15:"Warrior, Paladin",
	16:"Priest, Mage, Warlock"
};
var g_chr_races={
	1:"Human",
	2:"Orc",
	3:"Dwarf",
	4:"Night Elf",
	5:"Undead",
	6:"Tauren",
	7:"Gnome",
	8:"Troll",
	10:"Blood Elf",
	11:"Draenei"
};