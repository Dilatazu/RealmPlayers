#pragma once

namespace RP
{
	enum class WowRealm
	{
		Unknown,
		Emerald_Dream,
		Al_Akir,
		Warsong,
		All,
		Archangel,
		VanillaGaming,
		Valkyrie,
		Rebirth,
	};
	enum class PlayerRace
	{
		Unknown,
		Orc,
		Undead,
		Tauren,
		Troll,
		Human,
		Dwarf,
		Gnome,
		Night_Elf,
		Blood_Elf,
		Draenei,
	};
	enum class PlayerClass
	{
		Unknown,
		Druid,
		Warrior,
		Shaman,
		Priest,
		Mage,
		Rogue,
		Warlock,
		Hunter,
		Paladin
	};
	enum class PlayerSex
	{
		Unknown,
		Male,
		Female,
	};
	enum class ItemSlot
	{
		Unknown = 0,
		Head = 1,
		Neck = 2,
		Shoulder = 3,
		Shirt = 4,
		Chest = 5,
		Belt = 6,
		Legs = 7,
		Feet = 8,
		Wrist = 9,
		Gloves = 10,
		Finger_1 = 11,
		Finger_2 = 12,
		Trinket_1 = 13,
		Trinket_2 = 14,
		Back = 15,
		Main_Hand = 16,
		Off_Hand = 17,
		Ranged = 18,
		Tabard = 19
	};
}