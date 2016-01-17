
TRUNCATE TABLE PlayerTable;
DROP TABLE PlayerTable;
TRUNCATE TABLE PlayerDataTable;
DROP TABLE PlayerDataTable;
TRUNCATE TABLE UploadTable;
DROP TABLE UploadTable;
TRUNCATE TABLE ContributorTable;
DROP TABLE ContributorTable;
TRUNCATE TABLE PlayerTalentsInfoTable;
DROP TABLE PlayerTalentsInfoTable;
TRUNCATE TABLE PlayerArenaInfoTable;
DROP TABLE PlayerArenaInfoTable;
TRUNCATE TABLE PlayerArenaDataTable;
DROP TABLE PlayerArenaDataTable;
TRUNCATE TABLE PlayerGearGemsTable;
DROP TABLE PlayerGearGemsTable;
TRUNCATE TABLE PlayerGearTable;
DROP TABLE PlayerGearTable;
TRUNCATE TABLE IngameItemTable;
DROP TABLE IngameItemTable;
TRUNCATE TABLE PlayerHonorVanillaTable;
DROP TABLE PlayerHonorVanillaTable;
TRUNCATE TABLE PlayerHonorTable;
DROP TABLE PlayerHonorTable;
TRUNCATE TABLE PlayerGuildTable;
DROP TABLE PlayerGuildTable;


BEGIN;

--UNIQUE FOR PLAYER AND TIME
CREATE TABLE PlayerGuildTable (
	ID				serial,
	GuildName		text,
	GuildRank		text,
	GuildRankNr		smallint,
	PRIMARY KEY (ID)
);

INSERT INTO PlayerGuildTable VALUES(0, '', '', 0);

--UNIQUE FOR PLAYER AND TIME
CREATE TABLE PlayerHonorTable (
	ID				serial,
	TodayHK			integer,
	TodayHonor		integer,--Not used for vanilla
	YesterdayHK		integer,
	YesterdayHonor	integer,
	LifetimeHK		integer,
	PRIMARY KEY (ID)
);

INSERT INTO PlayerHonorTable VALUES(0, 0, 0, 0, 0, 0);

--UNIQUE FOR PLAYER AND TIME
CREATE TABLE PlayerHonorVanillaTable (
	PlayerHonorID	integer REFERENCES PlayerHonorTable(ID),
	CurrentRank		smallint,
	CurrentRankProgress	real,
	TodayDK			integer,
	ThisWeekHK		integer,
	ThisWeekHonor	integer,
	LastWeekHK		integer,
	LastWeekHonor	integer,
	LastWeekStanding integer,
	LifetimeDK		integer,
	LifetimeHighestRank	smallint,
	PRIMARY KEY (PlayerHonorID)
);

INSERT INTO PlayerHonorVanillaTable VALUES(0, 0, 0.0, 0, 0, 0, 0, 0, 0, 0, 0);

--Not unique for Player
CREATE TABLE IngameItemTable (
	ID				serial,
	ItemID			integer,
	EnchantID		integer,
	SuffixID		integer,
	UniqueID		integer,
	PRIMARY KEY (ID)
);

INSERT INTO IngameItemTable VALUES(0, 0, 0, 0, 0);

--Unique for Player and Time
CREATE TABLE PlayerGearTable (
	ID				serial,
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

INSERT INTO PlayerGearTable VALUES(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

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

INSERT INTO PlayerGearGemsTable VALUES(0, 0, 0, 0, 0, 0);

--Unique for Player and Time
CREATE TABLE PlayerArenaDataTable (
	ID				serial,
	TeamName		text,
	TeamRating		integer,
	GamesPlayed		integer,
	GamesWon		integer,
	PlayerGamesPlayed	integer,
	PlayerRating	integer,
	PRIMARY KEY (ID)
);

INSERT INTO PlayerArenaDataTable VALUES(0, '', 0, 0, 0, 0, 0);

--Unique for Player and Time
CREATE TABLE PlayerArenaInfoTable (
	ID				serial,
	Team_2v2		integer REFERENCES PlayerArenaDataTable(ID),
	Team_3v3		integer REFERENCES PlayerArenaDataTable(ID),
	Team_5v5		integer REFERENCES PlayerArenaDataTable(ID),
	PRIMARY KEY (ID)
);

INSERT INTO PlayerArenaInfoTable VALUES(0, 0, 0, 0);

--Unique for Player and Time
CREATE TABLE PlayerTalentsInfoTable (
	ID				serial,
	Talents			text,
	PRIMARY KEY (ID)
);

INSERT INTO PlayerTalentsInfoTable VALUES(0, '');

--Unique for UserID
CREATE TABLE ContributorTable (
	ID 				integer,
	UserID			text,
	Name			text,
	IP				text,
	PRIMARY KEY (ID)
);

--Unique for Upload from Contributor
CREATE TABLE UploadTable (
	ID				serial,
	UploadTime		timestamp,
	Contributor		integer REFERENCES ContributorTable(ID),
	PRIMARY KEY (ID)
);

--Unique for Player and Time
CREATE TABLE PlayerDataTable (
	PlayerID		integer,-- REFERENCES PlayerTable(ID),
	UploadID		integer REFERENCES UploadTable(ID),
	UpdateTime		timestamp,
	Race			smallint,
	Class			smallint,
	Sex				smallint,
	Level			smallint,
	GuildInfo		integer REFERENCES PlayerGuildTable(ID),
	HonorInfo		integer REFERENCES PlayerHonorTable(ID),
	GearInfo		integer REFERENCES PlayerGearTable(ID),
	ArenaInfo		integer REFERENCES PlayerArenaInfoTable(ID),
	TalentsInfo		integer REFERENCES PlayerTalentsInfoTable(ID),
	PRIMARY KEY (PlayerID, UploadID)
);

--Unique for Character and Realm
CREATE TABLE PlayerTable (
	ID				serial,
	Name			text,
	Realm			integer,
	UploadID		integer REFERENCES UploadTable(ID), --Use along with ID to find PlayerDataTable ID
	FOREIGN KEY (ID, UploadID) REFERENCES PlayerDataTable(PlayerID, UploadID),
	PRIMARY KEY (ID)
);

CREATE TABLE IngameMountTable (
	ID				serial,
	Name			text,
	PRIMARY KEY (ID)
);

INSERT INTO IngameMountTable VALUES(0, '');

CREATE TABLE IngamePetTable (
	ID				serial,
	Name			text,
	Level			smallint,
	CreatureFamily	text,
	CreatureType	text,
	PRIMARY KEY (ID)
);

INSERT INTO IngamePetTable VALUES(0, '', 0, '', '');

CREATE TABLE IngameCompanionTable (
	ID				serial,
	Name			text,
	Level			smallint,
	PRIMARY KEY (ID)
);

INSERT INTO IngameCompanionTable VALUES(0, '', 0);

CREATE TABLE PlayerMountTable (
	PlayerID		integer REFERENCES PlayerTable(ID),
	UploadID		integer REFERENCES UploadTable(ID),
	UpdateTime		timestamp,
	MountID			integer REFERENCES IngameMountTable(ID),
	PRIMARY KEY(PlayerID, UploadID, MountID)
);

CREATE TABLE PlayerPetTable (
	PlayerID		integer REFERENCES PlayerTable(ID),
	UploadID		integer REFERENCES UploadTable(ID),
	UpdateTime		timestamp,
	PetID			integer REFERENCES IngamePetTable(ID),
	PRIMARY KEY(PlayerID, UploadID, PetID)
);

CREATE TABLE PlayerCompanionTable (
	PlayerID		integer REFERENCES PlayerTable(ID),
	UploadID		integer REFERENCES UploadTable(ID),
	UpdateTime		timestamp,
	CompanionID		integer REFERENCES IngameCompanionTable(ID),
	PRIMARY KEY(PlayerID, UploadID, CompanionID)
);

COMMIT;