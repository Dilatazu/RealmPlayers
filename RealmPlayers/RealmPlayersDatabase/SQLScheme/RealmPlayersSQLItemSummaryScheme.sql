--/*
--DROP TABLE PlayerTable;
--DROP TABLE IngameItemTable;

DROP TABLE ItemOwnerTable
--*/

--/*
BEGIN;

--Not unique for Player
--CREATE TABLE IngameItemTable (
--	ID				serial,
--	ItemID			integer,
--	EnchantID		integer,
--	SuffixID		integer,
--	UniqueID		integer,
--	PRIMARY KEY (ID)
--);

--Unique for Character and Realm
--CREATE TABLE PlayerTable (
--	ID				serial,
--	Name			text,
--	Realm			integer,
--	LatestUploadID	integer REFERENCES UploadTable(ID), --Use along with ID to find PlayerDataTable ID Maybe should be renamed to "LatestUploadID"?
--	--This foreign key causes too much headache atm FOREIGN KEY (ID, LatestUploadID) REFERENCES PlayerDataTable(PlayerID, UploadID),
--	PRIMARY KEY (ID)
--);
--ALTER TABLE playertable DROP CONSTRAINT playertable_id_fkey

CREATE TABLE ItemOwnerTable (
	ItemID			integer,
	SuffixID		integer,
	PlayerID		integer REFERENCES PlayerTable(ID),
	DateAquired		timestamp,
	LatestItemInfo	integer REFERENCES IngameItemTable(ID),
	PRIMARY KEY (ItemID, SuffixID, PlayerID)
);

COMMIT;
--*/