using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace HelpfulHotkeys
{
	public class ItemSorting
	{
		internal class ItemSortingLayer
		{
			public readonly string Name;
			public readonly Func<Item, bool> SortingMethod;

			public ItemSortingLayer(string name, Func<Item, bool> method)
			{
				this.Name = name;
				this.SortingMethod = method;
			}

			public bool Pass(Item item)
			{
				return (item.type != ItemID.Count) && !item.IsAir && SortingMethod(item);
			}

			public override string ToString()
			{
				return this.Name;
			}
		}

		private class ItemSortingLayers
		{
			public static ItemSorting.ItemSortingLayer WeaponsMelee = new ItemSorting.ItemSortingLayer("Weapons - Melee", delegate (Item item)
				{
					return item.maxStack == 1 && item.damage > 0 && item.ammo == 0 && item.melee && item.pick < 1 && item.hammer < 1 && item.axe < 1 && !item.accessory; // added !item.accessory
				});
			public static ItemSorting.ItemSortingLayer WeaponsRanged = new ItemSorting.ItemSortingLayer("Weapons - Ranged", delegate (Item item)
				{
					return item.maxStack == 1 && item.damage > 0 && item.ammo == 0 && item.ranged && !item.accessory;
				});
			public static ItemSorting.ItemSortingLayer WeaponsMagic = new ItemSorting.ItemSortingLayer("Weapons - Magic", delegate (Item item)
				{
					return item.maxStack == 1 && item.damage > 0 && item.ammo == 0 && item.magic && !item.accessory;
				});
			public static ItemSorting.ItemSortingLayer WeaponsMinions = new ItemSorting.ItemSortingLayer("Weapons - Minions", delegate (Item item)
				{
					return item.maxStack == 1 && item.damage > 0 && item.summon && !item.accessory;
				});
			public static ItemSorting.ItemSortingLayer WeaponsThrown = new ItemSorting.ItemSortingLayer("Weapons - Thrown", delegate (Item item)
				{
					return item.damage > 0 && (item.ammo == 0 || item.notAmmo) && item.shoot > 0 && item.thrown && !item.accessory;
				});
			public static ItemSorting.ItemSortingLayer WeaponsAssorted = new ItemSorting.ItemSortingLayer("Weapons - Assorted", delegate (Item item)
				{
					return item.damage > 0 && item.ammo == 0 && item.pick == 0 && item.axe == 0 && item.hammer == 0 && !item.accessory;
				});
			public static ItemSorting.ItemSortingLayer WeaponsAmmo = new ItemSorting.ItemSortingLayer("Weapons - Ammo", delegate (Item item)
				{
					return item.ammo > 0 && item.damage > 0 && !item.accessory;
				});
			public static ItemSorting.ItemSortingLayer ToolsPicksaws = new ItemSorting.ItemSortingLayer("Tools - Picksaws", delegate (Item item)
				{
					return item.pick > 0 && item.axe > 0;
				});
			public static ItemSorting.ItemSortingLayer ToolsHamaxes = new ItemSorting.ItemSortingLayer("Tools - Hamaxes", delegate (Item item)
				{
					return item.hammer > 0 && item.axe > 0;
				});
			public static ItemSorting.ItemSortingLayer ToolsPickaxes = new ItemSorting.ItemSortingLayer("Tools - Pickaxes", delegate (Item item)
				{
					return item.pick > 0;
				});
			public static ItemSorting.ItemSortingLayer ToolsAxes = new ItemSorting.ItemSortingLayer("Tools - Axes", delegate (Item item)
				{
					return item.axe > 0;
				});
			public static ItemSorting.ItemSortingLayer ToolsHammers = new ItemSorting.ItemSortingLayer("Tools - Hammers", delegate (Item item)
				{
					return item.hammer > 0;
				});
			public static ItemSorting.ItemSortingLayer ToolsTerraforming = new ItemSorting.ItemSortingLayer("Tools - Terraforming", delegate (Item item)
				{
					return item.netID > 0 && ItemID.Sets.SortingPriorityTerraforming[item.netID] > -1;
				});
			public static ItemSorting.ItemSortingLayer ToolsAmmoLeftovers = new ItemSorting.ItemSortingLayer("Weapons - Ammo Leftovers", delegate (Item item)
				{
					return item.ammo > 0;
				});
			//public static ItemSorting.ItemSortingLayer ArmorCombat = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
			//	{
			//		return (item.bodySlot >= 0 || item.headSlot >= 0 || item.legSlot >= 0) && !item.vanity;
			//	});
			public static ItemSorting.ItemSortingLayer ArmorCombatHead = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
				{
					return (item.headSlot >= 0) && !item.vanity;
				});
			public static ItemSorting.ItemSortingLayer ArmorCombatChest = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
				{
					return (item.bodySlot >= 0) && !item.vanity;
				});
			public static ItemSorting.ItemSortingLayer ArmorCombatLegs = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
				{
					return (item.legSlot >= 0) && !item.vanity;
				});
			//public static ItemSorting.ItemSortingLayer ArmorVanity = new ItemSorting.ItemSortingLayer("Armor - Vanity", delegate (Item item)
			//	{
			//		return (item.bodySlot >= 0 || item.headSlot >= 0 || item.legSlot >= 0) && item.vanity;
			//	});
			public static ItemSorting.ItemSortingLayer ArmorVanityHead = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
				{
					return (item.headSlot >= 0) && item.vanity;
				});
			public static ItemSorting.ItemSortingLayer ArmorVanityChest = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
				{
					return (item.bodySlot >= 0) && item.vanity;
				});
			public static ItemSorting.ItemSortingLayer ArmorVanityLegs = new ItemSorting.ItemSortingLayer("Armor - Combat", delegate (Item item)
				{
					return (item.legSlot >= 0) && item.vanity;
				});
			public static ItemSorting.ItemSortingLayer ArmorAccessories = new ItemSorting.ItemSortingLayer("Armor - Accessories", delegate (Item item)
				{
					return item.accessory;
				});
			public static ItemSorting.ItemSortingLayer EquipGrapple = new ItemSorting.ItemSortingLayer("Equip - Grapple", delegate (Item item)
				{
					return Main.projHook[item.shoot];
				});
			public static ItemSorting.ItemSortingLayer EquipMount = new ItemSorting.ItemSortingLayer("Equip - Mount", delegate (Item item)
				{
					return item.mountType != -1 && !MountID.Sets.Cart[item.mountType];
				});
			public static ItemSorting.ItemSortingLayer EquipCart = new ItemSorting.ItemSortingLayer("Equip - Cart", delegate (Item item)
				{
					return item.mountType != -1 && MountID.Sets.Cart[item.mountType];
				});
			public static ItemSorting.ItemSortingLayer EquipLightPet = new ItemSorting.ItemSortingLayer("Equip - Light Pet", delegate (Item item)
				{
					return item.buffType > 0 && Main.lightPet[item.buffType];
				});
			public static ItemSorting.ItemSortingLayer EquipVanityPet = new ItemSorting.ItemSortingLayer("Equip - Vanity Pet", delegate (Item item)
				{
					return item.buffType > 0 && Main.vanityPet[item.buffType];
				});
			public static ItemSorting.ItemSortingLayer PotionsLife = new ItemSorting.ItemSortingLayer("Potions - Life", delegate (Item item)
				{
					return item.consumable && item.healLife > 0 && item.healMana < 1;
				});
			public static ItemSorting.ItemSortingLayer PotionsMana = new ItemSorting.ItemSortingLayer("Potions - Mana", delegate (Item item)
				{
					return item.consumable && item.healLife < 1 && item.healMana > 0;
				});
			public static ItemSorting.ItemSortingLayer PotionsElixirs = new ItemSorting.ItemSortingLayer("Potions - Elixirs", delegate (Item item)
				{
					return item.consumable && item.healLife > 0 && item.healMana > 0;
				});
			public static ItemSorting.ItemSortingLayer PotionsBuffs = new ItemSorting.ItemSortingLayer("Potions - Buffs", delegate (Item item)
				{
					return item.consumable && item.buffType > 0;
				});
			public static ItemSorting.ItemSortingLayer PotionsDyes = new ItemSorting.ItemSortingLayer("Potions - Dyes", delegate (Item item)
				{
					return item.dye > 0;
				});
			public static ItemSorting.ItemSortingLayer PotionsHairDyes = new ItemSorting.ItemSortingLayer("Potions - Hair Dyes", delegate (Item item)
				{
					return item.hairDye >= 0;
				});
			public static ItemSorting.ItemSortingLayer MiscValuables = new ItemSorting.ItemSortingLayer("Misc - Importants", delegate (Item item)
				{
					return item.netID > 0 && ItemID.Sets.SortingPriorityBossSpawns[item.netID] > -1 && item.netID != ItemID.TreasureMap || item.netID == ItemID.PirateMap; // vanilla bug.
				});
			public static ItemSorting.ItemSortingLayer MiscWiring = new ItemSorting.ItemSortingLayer("Misc - Wiring", delegate (Item item)
				{
					return (item.netID > 0 && ItemID.Sets.SortingPriorityWiring[item.netID] > -1) || item.mech;
				});
			public static ItemSorting.ItemSortingLayer MiscMaterials = new ItemSorting.ItemSortingLayer("Misc - Materials", delegate (Item item)
				{
					return item.netID > 0 && ItemID.Sets.SortingPriorityMaterials[item.netID] > -1;
				});
			public static ItemSorting.ItemSortingLayer MiscExtractinator = new ItemSorting.ItemSortingLayer("Misc - Extractinator", delegate (Item item)
				{
					return item.netID > 0 && ItemID.Sets.SortingPriorityExtractibles[item.netID] > -1;
				});
			public static ItemSorting.ItemSortingLayer MiscPainting = new ItemSorting.ItemSortingLayer("Misc - Painting", delegate (Item item)
				{
					return (item.netID > 0 && ItemID.Sets.SortingPriorityPainting[item.netID] > -1) || item.paint > 0;
				});
			public static ItemSorting.ItemSortingLayer MiscRopes = new ItemSorting.ItemSortingLayer("Misc - Ropes", delegate (Item item)
				{
					return item.netID > 0 && ItemID.Sets.SortingPriorityRopes[item.netID] > -1;
				});
			public static ItemSorting.ItemSortingLayer LastMaterials = new ItemSorting.ItemSortingLayer("Last - Materials", delegate (Item item)
				{
					return item.createTile < 0 && item.createWall < 1;
				});
			public static ItemSorting.ItemSortingLayer LastTilesImportant = new ItemSorting.ItemSortingLayer("Last - Tiles (Frame Important)", delegate (Item item)
				{
					return item.createTile >= 0 && Main.tileFrameImportant[item.createTile];
				});
			public static ItemSorting.ItemSortingLayer LastTilesCommon = new ItemSorting.ItemSortingLayer("Last - Tiles (Common), Walls", delegate (Item item)
				{
					return item.createWall > 0 || item.createTile >= 0;
				});
			public static ItemSorting.ItemSortingLayer LastNotTrash = new ItemSorting.ItemSortingLayer("Last - Not Trash", delegate (Item item)
				{
					return item.rare >= 0;
				});
			public static ItemSorting.ItemSortingLayer LastTrash = new ItemSorting.ItemSortingLayer("Last - Trash", delegate (Item item)
				{
					return !item.IsAir;
				});
		}

		internal static List<ItemSortingLayer> GetPassingLayers(Item[] items)
		{
			SetupSortingPriorities();

			HashSet<ItemSortingLayer> passing = new HashSet<ItemSortingLayer>();

			foreach (var item in items)
			{
				foreach (var layer in layerList)
				{
					if (layer.Pass(item))
					{
						passing.Add(layer);
						break;
					}
				}
			}

			return passing.ToList();
		}

		internal static List<ItemSorting.ItemSortingLayer> layerList;

		private static void SetupSortingPriorities()
		{
			if (layerList != null) return;
			layerList = new List<ItemSortingLayer>();
			//ItemSorting._layerList.Clear();
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsMelee);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsRanged);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsMagic);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsMinions);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsThrown);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsAssorted);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.WeaponsAmmo);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsPicksaws);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsHamaxes);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsPickaxes);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsAxes);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsHammers);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsTerraforming);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ToolsAmmoLeftovers);
			//ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ArmorCombat);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorCombatHead);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorCombatChest);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorCombatLegs);
			//ItemSorting._layerList.Add(ItemSorting.ItemSortingLayers.ArmorVanity);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorVanityHead);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorVanityChest);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorVanityLegs);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.ArmorAccessories);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.EquipGrapple);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.EquipMount);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.EquipCart);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.EquipLightPet);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.EquipVanityPet);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.PotionsDyes);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.PotionsHairDyes);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.PotionsLife);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.PotionsMana);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.PotionsElixirs);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.PotionsBuffs);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.MiscValuables);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.MiscPainting);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.MiscWiring);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.MiscMaterials);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.MiscRopes);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.MiscExtractinator);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.LastMaterials);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.LastTilesImportant);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.LastTilesCommon);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.LastNotTrash);
			ItemSorting.layerList.Add(ItemSorting.ItemSortingLayers.LastTrash);
		}


		//internal static void QuerySortLayer(Item item, int index)
		//{
		//	ItemSorting.SetupSortingPriorities();
		//	List<int> list = new List<int>();
		//	list.Add(index);
		//	foreach (ItemSorting.ItemSortingLayer current in ItemSorting._layerList)
		//	{
		//		List<int> list5 = current.SortingMethod(current, inv, list);
		//		if (list5.Count > 0)
		//		{
		//			Main.NewText("This belongs in " + current.Name);
		//		}
		//	}
		//}

		//private static void Sort(Item item, params int[] ignoreSlots)
		//{
		//	ItemSorting.SetupSortingPriorities();
		//	List<int> list = new List<int>();
		//	for (int i = 0; i < inv.Length; i++)
		//	{
		//		if (!ignoreSlots.Contains(i))
		//		{
		//			Item item = item;
		//			if (item != null && item.stack != 0 && item.type != 0 && !item.favorited)
		//			{
		//				list.Add(i);
		//			}
		//		}
		//	}
		//	for (int j = 0; j < list.Count; j++)
		//	{
		//		Item item2 = inv[list[j]];
		//		if (item2.stack < item2.maxStack)
		//		{
		//			int num = item2.maxStack - item2.stack;
		//			for (int k = j; k < list.Count; k++)
		//			{
		//				if (j != k)
		//				{
		//					Item item3 = inv[list[k]];
		//					if (item2.type == item3.type && item3.stack != item3.maxStack)
		//					{
		//						int num2 = item3.stack;
		//						if (num < num2)
		//						{
		//							num2 = num;
		//						}
		//						item2.stack += num2;
		//						item3.stack -= num2;
		//						num -= num2;
		//						if (item3.stack == 0)
		//						{
		//							inv[list[k]] = new Item();
		//							list.Remove(list[k]);
		//							j--;
		//							k--;
		//							break;
		//						}
		//						if (num == 0)
		//						{
		//							break;
		//						}
		//					}
		//				}
		//			}
		//		}
		//	}
		//	List<int> list2 = new List<int>(list);
		//	for (int l = 0; l < inv.Length; l++)
		//	{
		//		if (!ignoreSlots.Contains(l) && !list2.Contains(l))
		//		{
		//			Item item4 = inv[l];
		//			if (item4 == null || item4.stack == 0 || item4.type == 0)
		//			{
		//				list2.Add(l);
		//			}
		//		}
		//	}
		//	list2.Sort();
		//	List<int> list3 = new List<int>();
		//	List<int> sortLayerCounts = new List<int>();
		//	foreach (ItemSorting.ItemSortingLayer current in ItemSorting._layerList)
		//	{
		//		List<int> list5 = current.SortingMethod(current, inv, list);
		//		if (list5.Count > 0)
		//		{
		//			sortLayerCounts.Add(list5.Count);
		//		}
		//		list3.AddRange(list5);
		//	}
		//	list3.AddRange(list);
		//	List<Item> list6 = new List<Item>();
		//	foreach (int current2 in list3)
		//	{
		//		list6.Add(inv[current2]);
		//		inv[current2] = new Item();
		//	}
		//	float num3 = 1f / (float)sortLayerCounts.Count;
		//	float num4 = num3 / 2f;
		//	for (int m = 0; m < list6.Count; m++)
		//	{
		//		int num5 = list2[0];
		//		//ItemSlot.SetGlow(num5, num4, Main.player[Main.myPlayer].chest != -1);
		//		List<int> list7;
		//		(list7 = sortLayerCounts)[0] = list7[0] - 1;
		//		if (sortLayerCounts[0] == 0)
		//		{
		//			sortLayerCounts.RemoveAt(0);
		//			num4 += num3;
		//		}
		//		inv[num5] = list6[m];
		//		list2.Remove(num5);
		//	}
		//}

		//public static void SortInventory()
		//{
		//	ItemSorting.Sort(Main.player[Main.myPlayer].inventory, new int[]
		//		{
		//			0,
		//			1,
		//			2,
		//			3,
		//			4,
		//			5,
		//			6,
		//			7,
		//			8,
		//			9,
		//			50,
		//			51,
		//			52,
		//			53,
		//			54,
		//			55,
		//			56,
		//			57,
		//			58
		//		});
		//}

		//public static void SortChest()
		//{
		//	int chest = Main.player[Main.myPlayer].chest;
		//	if (chest == -1)
		//	{
		//		return;
		//	}
		//	Item[] item = Main.player[Main.myPlayer].bank.item;
		//	if (chest == -3)
		//	{
		//		item = Main.player[Main.myPlayer].bank2.item;
		//	}
		//	if (chest > -1)
		//	{
		//		item = Main.chest[chest].item;
		//	}
		//	Tuple<int, int, int>[] array = new Tuple<int, int, int>[40];
		//	for (int i = 0; i < 40; i++)
		//	{
		//		array[i] = Tuple.Create<int, int, int>(item[i].netID, item[i].stack, (int)item[i].prefix);
		//	}
		//	ItemSorting.Sort(item, new int[0]);
		//	Tuple<int, int, int>[] array2 = new Tuple<int, int, int>[40];
		//	for (int j = 0; j < 40; j++)
		//	{
		//		array2[j] = Tuple.Create<int, int, int>(item[j].netID, item[j].stack, (int)item[j].prefix);
		//	}
		//	if (Main.netMode == 1 && Main.player[Main.myPlayer].chest > -1)
		//	{
		//		for (int k = 0; k < 40; k++)
		//		{
		//			if (array2[k] != array[k])
		//			{
		//				NetMessage.SendData(32, -1, -1, "", Main.player[Main.myPlayer].chest, (float)k, 0f, 0f, 0, 0, 0);
		//			}
		//		}
		//	}
		//}
	}
}
