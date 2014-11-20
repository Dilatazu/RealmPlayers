var instanceImages = new Array();
instanceImages["MC"] = "raid-moltencore.png";
instanceImages["Ony"] = "raid-onyxia.png";
instanceImages["BWL"] = "raid-blackwinglair.png";
instanceImages["ZG"] = "raid-zulgurub.png";
instanceImages["AQ20"] = "raid-aqruins.png";
instanceImages["AQ40"] = "raid-aqtemple.png";
instanceImages["Naxx"] = "raid-naxxramas.png";
instanceImages["WB"] = "raid-open.png";

instanceImages["Big-MC"] = "lfgicon-moltencore.png";
instanceImages["Big-Ony"] = "lfgicon-raid.png";
instanceImages["Big-BWL"] = "lfgicon-blackwinglair.png";
instanceImages["Big-ZG"] = "lfgicon-zulgurub.png";
instanceImages["Big-AQ20"] = "lfgicon-aqruins.png";
instanceImages["Big-AQ40"] = "lfgicon-aqtemple.png";
instanceImages["Big-Naxx"] = "lfgicon-naxxramas.png";
instanceImages["Big-WB"] = "lfgicon-zone.png";

var instanceNames = new Array();
instanceNames["MC"] = "Molten Core";
instanceNames["Ony"] = "Onyxia";
instanceNames["BWL"] = "Blackwing Lair";
instanceNames["ZG"] = "Zul'Gurub";
instanceNames["AQ20"] = "Ruins of Ahn'Qiraj";
instanceNames["AQ40"] = "Temple of Ahn'Qiraj";
instanceNames["Naxx"] = "Naxxramas";
instanceNames["WB"] = "World Bosses";

var instanceBosses = new Array();
instanceBosses["MC"] = new Array();
instanceBosses["MC"][0] = "Lucifron";
instanceBosses["MC"][1] = "Magmadar";
instanceBosses["MC"][2] = "Gehennas";
instanceBosses["MC"][3] = "Garr";
instanceBosses["MC"][4] = "Baron Geddon";
instanceBosses["MC"][5] = "Shazzrah";
instanceBosses["MC"][6] = "Sulfuron Harbinger";
instanceBosses["MC"][7] = "Golemagg the Incinerator";
instanceBosses["MC"][8] = "Majordomo Executus";
instanceBosses["MC"][9] = "Ragnaros";

instanceBosses["Ony"] = new Array();
instanceBosses["Ony"][0] = "Onyxia";

instanceBosses["BWL"] = new Array();
instanceBosses["BWL"][0] = "Razorgore the Untamed";
instanceBosses["BWL"][1] = "Vaelastrasz the Corrupt";
instanceBosses["BWL"][2] = "Broodlord Lashlayer";
instanceBosses["BWL"][3] = "Firemaw";
instanceBosses["BWL"][4] = "Ebonroc";
instanceBosses["BWL"][5] = "Flamegor";
instanceBosses["BWL"][6] = "Chromaggus";
instanceBosses["BWL"][7] = "Nefarian";

instanceBosses["ZG"] = new Array();
instanceBosses["ZG"][0] = "High Priestess Jeklik";
instanceBosses["ZG"][1] = "High Priest Venoxis";
instanceBosses["ZG"][2] = "High Priestess Arlokk";
instanceBosses["ZG"][3] = "High Priest Thekal";
instanceBosses["ZG"][4] = "High Priestess Mar'li";
instanceBosses["ZG"][5] = "Hakkar the Soulflayer";
instanceBosses["ZG"][6] = "Broodlord Mandokir";
instanceBosses["ZG"][7] = "Jin'do the Hexxer";
instanceBosses["ZG"][8] = "Gahz'ranka";
instanceBosses["ZG"][9] = "Edge of Madness";
instanceBosses["ZG"][10] = "Renataki_Of_The_Thousand_Blades";
instanceBosses["ZG"][11] = "Wushoolay_the_Storm_Witch";
instanceBosses["ZG"][12] = "Gri_Lek_Of_The_Iron_Blood";
instanceBosses["ZG"][13] = "Hazzarah_The_Dreamweaver";

instanceBosses["AQ20"] = new Array();
instanceBosses["AQ20"][0] = "Kurinnaxx";
instanceBosses["AQ20"][1] = "General Rajaxx";
instanceBosses["AQ20"][2] = "Ossirian the Unscarred";
instanceBosses["AQ20"][3] = "Buru the Gorger";
instanceBosses["AQ20"][4] = "Moam";
instanceBosses["AQ20"][5] = "Ayamiss the Hunter";

instanceBosses["AQ40"] = new Array();
instanceBosses["AQ40"][0] = "The Prophet Skeram";
instanceBosses["AQ40"][1] = "Battleguard Satura";
instanceBosses["AQ40"][2] = "Fankriss the Unyielding";
instanceBosses["AQ40"][3] = "Huhuran";
instanceBosses["AQ40"][4] = "The Twin Emperors";
instanceBosses["AQ40"][5] = "C'Thun";
instanceBosses["AQ40"][6] = "Three Bugs";
instanceBosses["AQ40"][7] = "Viscidus";
instanceBosses["AQ40"][8] = "Ouro";
instanceBosses["AQ40"][9] = "Three Bugs (Vem Last)";
instanceBosses["AQ40"][10] = "Three Bugs (Princess Yauj Last)";
instanceBosses["AQ40"][11] = "Three Bugs (Lord Kri Last)";

instanceBosses["Naxx"] = new Array();
instanceBosses["Naxx"][0] = "Anub'Rekhan";
instanceBosses["Naxx"][1] = "Grand Widow Faerlina";
instanceBosses["Naxx"][2] = "Maexxna";
instanceBosses["Naxx"][3] = "Patchwerk";
instanceBosses["Naxx"][4] = "Grobbulus";
instanceBosses["Naxx"][5] = "Gluth";
instanceBosses["Naxx"][6] = "Thaddius";
instanceBosses["Naxx"][7] = "Noth the Plaguebringer";
instanceBosses["Naxx"][8] = "Heigan the Unclean";
instanceBosses["Naxx"][9] = "Loatheb";
instanceBosses["Naxx"][10] = "Instructor Razuvious";
instanceBosses["Naxx"][11] = "Gothik the Harvester";
instanceBosses["Naxx"][12] = "The Four Horsemen";
instanceBosses["Naxx"][13] = "Sapphiron";
instanceBosses["Naxx"][14] = "Kel'Thuzad";

instanceBosses["WB"] = new Array();
instanceBosses["WB"][0] = "Azuregos";
instanceBosses["WB"][1] = "Kazzak";
instanceBosses["WB"][2] = "Emeriss";
instanceBosses["WB"][3] = "Taerar";
instanceBosses["WB"][4] = "Lethon";
instanceBosses["WB"][5] = "Ysondre";

function generateProgressValue(progressStr) {
    var progressValue = 0;
    for (var i = 0; i < progressStr.length; i++) {
        if (progressStr.charAt(i) == "1")
            progressValue++;
    }
    return { val: progressValue, max: progressStr.length };
}
function padNumber(num, size) {
    var sNum = "" + num;
    if ((sNum).length == 1)
        return "&nbsp;" + num;
    return sNum;
}
function generateProgress(guildName, instance) {
    var progressValue = generateProgressValue(guildProgress[guildName][instance]);
    var preStr = '<div class="raid_progress" id="' + guildName + '-' + instance + '" rel="tooltip" >';

    var valueStr = "" + padNumber(progressValue.val, 2) + '/' + progressValue.max;
    if (progressValue.val == 0)
    { }
    else if (progressValue.val == progressValue.max) {
        valueStr = '<span class="progress_clear">' + valueStr + '</span>';
    }
    else {
        valueStr = '<span class="progress_in_work">' + valueStr + '</span>';
    }
    return preStr + valueStr + '<br/><img src="assets/img/raid/' + instanceImages[instance] + '" /></div>';
}

function generateProgressGuildView(guildName, instance) {
    var progressValue = generateProgressValue(guildProgress[guildName][instance]);
    var preStr = '<div class="raid_progress" id="' + guildName + '-' + instance + '" rel="tooltip" style="white-space: nowrap;">';

    var valueStr = instanceNames[instance] + ' ' + progressValue.val + '/' + progressValue.max;
    if (progressValue.val == 0)
    { }
    else if (progressValue.val == progressValue.max) {
        valueStr = '<span class="progress_clear">' + valueStr + '</span>';
    }
    else {
        valueStr = '<span class="progress_in_work">' + valueStr + '</span>';
    }
    return preStr + '<img src="assets/img/raid/' + instanceImages[instance] + '" /> ' + valueStr + '</div>';
}

function generateProgressTooltip(guildName, instance) {
    var progressStr = guildProgress[guildName][instance];
    var progressValue = generateProgressValue(progressStr);
    var returnStr = "<img src='assets/img/raid/" + instanceImages["Big-" + instance] + "' /><div id='progress_tooltip'><span id='raid_name'>"
    + instanceNames[instance] + " " + progressValue.val + '/' + progressValue.max + "</span><br/>";

    var currSectionKill = false;
    for (var i = 0; i < progressStr.length; ++i) {
        if (progressStr.charAt(i) == "1") {
            if (currSectionKill == false) {
                returnStr = returnStr + "<span id='boss_killed'>";
                currSectionKill = true;
            }
        }
        else {
            if (currSectionKill == true) {
                returnStr = returnStr + "</span>";
                currSectionKill = false;
            }
        }
        if ((instance == "ZG" && i == 6)
        || (instance == "AQ20" && i == 3)
        || (instance == "AQ40" && i == 6)) {
            returnStr = returnStr + "<span id='optional'>Optional</span><br/>";
        }
        else if (instance == "Naxx" && (i == 0 || i == 3 || i == 7 || i == 10 || i == 13)) {
            var sectionName = "null";
            if (i == 0) sectionName = "Arachnid Quarter";
            else if (i == 3) sectionName = "Construct Quarter";
            else if (i == 7) sectionName = "Plague Quarter";
            else if (i == 10) sectionName = "Millitary Quarter";
            else if (i == 13) sectionName = "Frostwyrm Lair";

            returnStr = returnStr + "<span id='optional'>" + sectionName + "</span><br/>";
        }
        else if (instance == "WB" && i == 2) {
            returnStr = returnStr + "<span id='optional'>Emerald Dragons</span><br/>";
        }
        returnStr = returnStr + instanceBosses[instance][i] + "<br/>";
        /*if((instance == "ZG" && i == 9)
        || (instance == "AQ20" && i == 5)
        || (instance == "AQ40" && i == 10)
        || (instance == "Naxx" && (i == 6 || i == 9 || i == 12 || i == 14))
        || (instance == "WB" && i == 5))
        {
        
        }*/
    }
    return returnStr + "</div>";
}
function createProgressTooltip($, guildName, instance) {
    $("div#" + guildName + "-" + instance).tooltip({
        html: true,
        placement: "bottom",
        title: generateProgressTooltip(guildName, instance)
    });
}
function createProgressTooltipGuildView($, guildName, instance) {
    $("div#" + guildName + "-" + instance).tooltip({
        html: true,
        placement: "left",
        title: generateProgressTooltip(guildName, instance)
    });
}