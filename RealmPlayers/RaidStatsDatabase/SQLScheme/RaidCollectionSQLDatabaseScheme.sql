--/*
DROP TABLE FightDataFilesTable;
DROP TABLE FightEntryTable;
DROP TABLE RaidMembersTable;
DROP TABLE RaidEntryTable;
--*/


--/*
BEGIN;
CREATE TABLE RaidEntryTable (
	UniqueRaidID	serial,
	IngameRaidID	integer,
	RaidResetDate	timestamp,
	RaidInstance	text,
	RaidStartDate	timestamp,
	RaidEndDate		timestamp,
	RaidOwnerName	text,
	Realm			integer,
	PRIMARY KEY (UniqueRaidID)
);

CREATE TABLE RaidMembersTable (
	UniqueRaidID	integer REFERENCES RaidEntryTable(UniqueRaidID),
	PlayerName		text,
	PRIMARY KEY(UniqueRaidID, PlayerName)
);

CREATE TABLE FightEntryTable (
	ID					serial,
	UniqueRaidID		integer REFERENCES RaidEntryTable(UniqueRaidID),
	FightName			text,
	FightStartDate		timestamp,
	FightEndDate		timestamp,
	AttemptType			smallint,
	PRIMARY KEY(ID)
);

CREATE TABLE FightDataFilesTable (
	FightEntryID		integer REFERENCES FightEntryTable(ID),
	RecordedBy			text,
	DataFile			text,
	PRIMARY KEY(FightEntryID, RecordedBy)
);

COMMIT;
--*/