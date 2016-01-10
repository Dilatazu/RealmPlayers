
--UNIQUE FOR PLAYER AND TIME
CREATE TABLE PlayerGuildTable (
	ID				integer,
	GuildName		text,
	GuildRank		text,
	GuildRankNr		smallint,
	PRIMARY KEY (ID)
);

--UNIQUE FOR PLAYER AND TIME
CREATE TABLE PlayerHonorTable (
	ID				integer,
	TodayHK			integer,
	TodayDKVanilla_Or_TodayHonorTBC integer,
	YesterdayHK		integer,
	YesterdayHonor	integer,
	LifetimeHK		integer,
	PRIMARY KEY (ID)
);

--UNIQUE FOR PLAYER AND TIME
CREATE TABLE PlayerHonorVanillaTable (
	PlayerHonorID	integer REFERENCES PlayerHonorTable(ID)
	CurrentRank		smallint,
	CurrentRankProgress	real,
	ThisWeekHK		integer,
	ThisWeekHonor	integer,
	LastWeekHK		integer,
	LastWeekHonor	integer,
	LastWeekStanding integer,
	LifetimeDK		integer,
	LifetimeHighestRank	smallint,
	PRIMARY KEY (PlayerHonorID)
);

--Not unique for Player
CREATE TABLE IngameItemTable (
	ID				integer,
	ItemID			integer,
	EnchantID		integer,
	SuffixID		integer,
	UniqueID		integer,
	PRIMARY KEY (ID)
);

--Unique for Player and Time
CREATE TABLE PlayerGearTable (
	ID				integer,
	Head			integer REFERENCES IngameItemTable(ID),
	Neck			integer REFERENCES IngameItemTable(ID),
	Shoulder		integer REFERENCES IngameItemTable(ID),
	Shirt			integer REFERENCES IngameItemTable(ID),
	Chest			integer REFERENCES IngameItemTable(ID),
	Belt			integer REFERENCES IngameItemTable(ID),
	Legs			integer REFERENCES IngameItemTable(ID),
	Feet			integer REFERENCES IngameItemTable(ID),
	Wrist			integer REFERENCES IngameItemTable(ID),
	Gloves			integer REFERENCES IngameItemTable(ID),
	Finger_1		integer REFERENCES IngameItemTable(ID),
	Finger_2		integer REFERENCES IngameItemTable(ID),
	Trinket_1		integer REFERENCES IngameItemTable(ID),
	Trinket_2		integer REFERENCES IngameItemTable(ID),
	Back			integer REFERENCES IngameItemTable(ID),
	Main_Hand		integer REFERENCES IngameItemTable(ID),
	Off_Hand		integer REFERENCES IngameItemTable(ID),
	Ranged			integer REFERENCES IngameItemTable(ID),
	Tabard			integer REFERENCES IngameItemTable(ID),
	PRIMARY KEY (ID)
);

--Unique for Player, Time and Slot
CREATE TABLE PlayerGearGemsTable (
	GearID			integer REFERENCES PlayerGearTable(ID),
	ItemSlot		smallint,
	GemID1			integer,
	GemID2			integer,
	GemID3			integer,
	GemID4			integer,
	PRIMARY KEY (GearID, ItemSlot)
);

--Unique for Player and Time
CREATE TABLE PlayerArenaTeamTable (
	ID				integer,
	TeamType		smallint,
	TeamName		text,
	TeamRating		integer,
	GamesPlayed		integer,
	GamesWon		integer,
	PlayerGamesPlayed	integer,
	PlayerRating	integer,
	PRIMARY KEY (ID, TeamType)
);

--Unique for Player and Time
CREATE TABLE TalentsInfoTable (
	ID				integer,
	Talents			text,
	PRIMARY KEY (ID)
);

--Unique for UserID
CREATE TABLE ContributorTable (
	ID 				integer,
	ContributorID 	integer,
	UserID			text,
	Name			text,
	IP				text,
	PRIMARY KEY (ID)
);

--Unique for Upload from Contributor
CREATE TABLE UpdateTable (
	ID				integer,
	DateTime		date,
	Contributor		integer REFERENCES ContributorTable(ID),
	PRIMARY KEY (ID)
);

--Unique for Character and Realm
CREATE TABLE PlayerTable (
	ID				integer,
	Name			text,
	Realm			integer,
	UpdateID		integer REFERENCES UpdateTable(ID), --Use along with ID to find PlayerDataTable ID
	FOREIGN KEY (ID, UpdateID) REFERENCES PlayerDataTable(PlayerID, UpdateID),
	PRIMARY KEY (ID)
);

--Unique for Player and Time
CREATE TABLE PlayerDataTable (
	PlayerID		integer REFERENCES PlayerTable(ID),
	UpdateID		integer REFERENCES UpdateTable(ID),
	Race			smallint,
	Class			smallint,
	Sex				smallint,
	Level			smallint,
	GuildInfo		integer REFERENCES PlayerGuildTable(ID),
	HonorInfo		integer REFERENCES PlayerHonorTable(ID)
	GearInfo		integer REFERENCES PlayerGearTable(ID),
	ArenaInfo		integer REFERENCES PlayerArenaTeamTable(ID),
	TalentsInfo		integer REFERENCES TalentsInfoTable(ID),
	PRIMARY KEY (PlayerID, UpdateID)
);