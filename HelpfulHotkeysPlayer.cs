using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace HelpfulHotkeys
{
	public class HelpfulHotkeysPlayer : ModPlayer
	{
		internal const int ITEM20 = 19;
		internal int originalSelectedItem;
		internal bool autoRevertSelectedItem = false;
		internal bool autoCycleAmmo = false;

		private int CycleAmmoHotkeyHeldTime;
		private int SwapArmorVanityHotkeyHeldTime;
		MethodInfo LaunchMinecartHookMethod;

		public override void Initialize()
		{
			//player.LaunchMinecartHook(point3.Value.X, point3.Value.Y);
			LaunchMinecartHookMethod = typeof(Player).GetMethod("LaunchMinecartHook", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public override void ProcessTriggers(TriggersSet triggersSet)
		{
			if (HelpfulHotkeys.QueryModOriginHotkey.JustPressed)
			{
				QueryModOrigin();
			}
			if (HelpfulHotkeys.AutoRecallHotKey.JustPressed)
			{
				AutoRecall();
			}
			if (HelpfulHotkeys.AutoTorchHotKey.JustPressed)
			{
				AutoTorch();
			}
			if (HelpfulHotkeys.CycleAmmoHotKey.JustPressed)
			{
				CycleAmmo();
				CycleAmmoHotkeyHeldTime = 0;
			}
			if (HelpfulHotkeys.CycleAmmoHotKey.Current)
			{
				CycleAmmoHotkeyHeldTime++;
				if (CycleAmmoHotkeyHeldTime == 60)
				{
					autoCycleAmmo = !autoCycleAmmo;
					Main.NewText("Auto Cycle Ammo mode has been " + (autoCycleAmmo ? "enabled" : "disabled"), Color.Aquamarine.R, Color.Aquamarine.G, Color.Aquamarine.B);
				}
			}
			if (HelpfulHotkeys.QuickStackToChestsHotKey.JustPressed)
			{
				QuickStackToChests();
			}
			if (HelpfulHotkeys.SmartQuickStackToChestsHotKey.JustPressed)
			{
				SmartQuickStackToChests();
			}
			if (HelpfulHotkeys.QuickUseItem20Hotkey.JustPressed)
			{
				if (player.selectedItem != ITEM20)
				{
					QuickUseItemAt(ITEM20);
				}
			}
			if (HelpfulHotkeys.QuickUseConfigItemHotkey.JustPressed) {
				QuickUseConfigItem();
			}
			if (HelpfulHotkeys.QuickBuffFavoritedOnlyHotkey.JustPressed)
			{
				QuickBuffFavoritedOnly();
			}
			if (HelpfulHotkeys.ToggleAutopauseHotkey.JustPressed)
			{
				ToggleAutoPause();
			}
			if (HelpfulHotkeys.SwapArmorVanityHotkey.JustPressed)
			{
				SwapArmorVanity();
				SwapArmorVanityHotkeyHeldTime = 0;
			}
			if (HelpfulHotkeys.SwapArmorVanityHotkey.Current)
			{
				SwapArmorVanityHotkeyHeldTime++;
				if (SwapArmorVanityHotkeyHeldTime == 30)
				{
					SwapAccessoriesVanity();
				}
			}
			if (HelpfulHotkeys.SwapHotbarHotkey.JustPressed)
			{
				SwapHotbar();
			}
			if (HelpfulHotkeys.CyclingQuickMountHotkey.JustPressed)
			{
				CyclingQuickMount();
			}
			if (HelpfulHotkeys.SwitchFrameSkipModeHotkey.JustPressed) {
				Main.FrameSkipMode = (Main.FrameSkipMode + 1) % 3;
				Main.NewText($"Frame Skip Mode is now: {Language.GetTextValue("LegacyMenu." + (247 + Main.FrameSkipMode))}");
			}
		}

		// if a mount, unmount and find the next. none found => dismount. unmounted-> mount default
		private void CyclingQuickMount()
		{
			int currentMount = 0;
			if (player.mount.Active)
			{
				currentMount = player.mount.Type;
				player.mount.Dismount(player);
				// look for another(next in priority), if found, mount
				Item nextMountItem = QuickMountCycle_GetItemToUse(currentMount);
				if (nextMountItem != null && nextMountItem.mountType != -1 && player.mount.CanMount(nextMountItem.mountType, player))
				{
					player.mount.SetMount(nextMountItem.mountType, player, false);
					ItemLoader.UseItem(nextMountItem, player);
					if (nextMountItem.UseSound != null)
					{
						Main.PlaySound(nextMountItem.UseSound, player.Center);
						return;
					}
				}
				return;
			}
			if (player.frozen || player.tongued || player.webbed || player.stoned || player.gravDir == -1f)
			{
				return;
			}
			if (player.noItems)
			{
				return;
			}
			Item item = player.QuickMount_GetItemToUse();
			//Item item = QuickMount_GetItemToUse();
			if (item != null && item.mountType != -1 && player.mount.CanMount(item.mountType, player))
			{
				bool flag = false;
				List<Point> tilesIn = Collision.GetTilesIn(player.TopLeft - new Vector2(24f), player.BottomRight + new Vector2(24f));
				if (tilesIn.Count > 0)
				{
					Point? point = null;
					Rectangle arg_CD_0 = player.Hitbox;
					for (int i = 0; i < tilesIn.Count; i++)
					{
						Point point2 = tilesIn[i];
						Tile tileSafely = Framing.GetTileSafely(point2.X, point2.Y);
						if (tileSafely.active() && tileSafely.type == 314)
						{
							Vector2 vector = tilesIn[i].ToVector2() * 16f + new Vector2(8f);
							if (!point.HasValue || (player.Distance(vector) < player.Distance(point.Value.ToVector2() * 16f + new Vector2(8f)) && Collision.CanHitLine(player.Center, 0, 0, vector, 0, 0)))
							{
								point = new Point?(tilesIn[i]);
							}
						}
					}
					if (point.HasValue)
					{
						object[] parametersArray = new object[] { point.Value.X, point.Value.Y };
						LaunchMinecartHookMethod.Invoke(player, parametersArray);
						//todo reflection				player.LaunchMinecartHook(point.Value.X, point.Value.Y);
						flag = true;
					}
				}
				if (!flag)
				{
					player.mount.SetMount(item.mountType, player, false);
					ItemLoader.UseItem(item, player);
					if (item.UseSound != null)
					{
						Main.PlaySound(item.UseSound, player.Center);
						return;
					}
				}
			}
			else
			{
				int num = 0;
				int num2 = (int)(player.position.X / 16f) - Player.tileRangeX - num + 1;
				int num3 = (int)((player.position.X + (float)player.width) / 16f) + Player.tileRangeX + num - 1;
				int num4 = (int)(player.position.Y / 16f) - Player.tileRangeY - num + 1;
				int num5 = (int)((player.position.Y + (float)player.height) / 16f) + Player.tileRangeY + num - 2;
				num2 = Utils.Clamp<int>(num2, 10, Main.maxTilesX - 10);
				num3 = Utils.Clamp<int>(num3, 10, Main.maxTilesX - 10);
				num4 = Utils.Clamp<int>(num4, 10, Main.maxTilesY - 10);
				num5 = Utils.Clamp<int>(num5, 10, Main.maxTilesY - 10);
				List<Point> tilesIn2 = Collision.GetTilesIn(new Vector2((float)num2, (float)num4) * 16f, new Vector2((float)(num3 + 1), (float)(num5 + 1)) * 16f);
				if (tilesIn2.Count > 0)
				{
					Point? point3 = null;
					Rectangle arg_338_0 = player.Hitbox;
					for (int j = 0; j < tilesIn2.Count; j++)
					{
						Point point4 = tilesIn2[j];
						Tile tileSafely2 = Framing.GetTileSafely(point4.X, point4.Y);
						if (tileSafely2.active() && tileSafely2.type == 314)
						{
							Vector2 vector2 = tilesIn2[j].ToVector2() * 16f + new Vector2(8f);
							if (!point3.HasValue || (player.Distance(vector2) < player.Distance(point3.Value.ToVector2() * 16f + new Vector2(8f)) && Collision.CanHitLine(player.Center, 0, 0, vector2, 0, 0)))
							{
								point3 = new Point?(tilesIn2[j]);
							}
						}
					}
					if (point3.HasValue)
					{
						object[] parametersArray = new object[] { point3.Value.X, point3.Value.Y };
						LaunchMinecartHookMethod.Invoke(player, parametersArray);
						//todo				player.LaunchMinecartHook(point3.Value.X, point3.Value.Y);
					}
				}
			}
		}

		public Item QuickMountCycle_GetItemToUse(int lastMount)
		{
			bool lastMountFound = false;
			//bool lastMountPassed = false;
			Item item = null;
			if (item == null && player.miscEquips[3].mountType != -1 && !MountID.Sets.Cart[player.miscEquips[3].mountType] && ItemLoader.CanUseItem(player.miscEquips[3], player))
			{
				//	item = player.miscEquips[3];
				if (lastMount == player.miscEquips[3].mountType)
				{
					lastMountFound = true;
				}
			}
			if (item == null)
			{
				for (int i = 0; i < 58; i++)
				{
					if (player.inventory[i].mountType != -1 && !MountID.Sets.Cart[player.inventory[i].mountType] && ItemLoader.CanUseItem(player.inventory[i], player))
					{
						if (lastMountFound)
						{
							item = player.inventory[i];
							break;
						}
						else
						{
							if (lastMount == player.inventory[i].mountType)
							{
								lastMountFound = true;
							}
						}
					}
				}
			}
			return lastMountFound ? item : null;
		}

		public override void PostUpdate()
		{
			if (autoRevertSelectedItem)
			{
				if (player.itemTime == 0 && player.itemAnimation == 0)
				{
					player.selectedItem = originalSelectedItem;
					autoRevertSelectedItem = false;
				}
			}
		}

		public override bool ConsumeAmmo(Item weapon, Item ammo)
		{
			if (autoCycleAmmo)
			{
				if (weapon.useAmmo != 0)
				{
					CycleAmmo();
				}
			}
			return base.ConsumeAmmo(weapon, ammo);
		}

		public void QuickBuffFavoritedOnly()
		{
			if (this.player.noItems)
			{
				return;
			}
			LegacySoundStyle legacySoundStyle = null;
			for (int i = 0; i < 58; i++)
			{
				if (this.player.CountBuffs() == 22)
				{
					return;
				}
				if (this.player.inventory[i].stack > 0 && this.player.inventory[i].type > 0 && this.player.inventory[i].favorited && this.player.inventory[i].buffType > 0 && !this.player.inventory[i].summon && this.player.inventory[i].buffType != 90)
				{
					int num2 = this.player.inventory[i].buffType;
					bool flag = ItemLoader.CanUseItem(this.player.inventory[i], this.player);
					for (int j = 0; j < 22; j++)
					{
						if (num2 == 27 && (this.player.buffType[j] == num2 || this.player.buffType[j] == 101 || this.player.buffType[j] == 102))
						{
							flag = false;
							break;
						}
						if (this.player.buffType[j] == num2)
						{
							flag = false;
							break;
						}
						if (Main.meleeBuff[num2] && Main.meleeBuff[this.player.buffType[j]])
						{
							flag = false;
							break;
						}
					}
					if (Main.lightPet[this.player.inventory[i].buffType] || Main.vanityPet[this.player.inventory[i].buffType])
					{
						for (int k = 0; k < 22; k++)
						{
							if (Main.lightPet[this.player.buffType[k]] && Main.lightPet[this.player.inventory[i].buffType])
							{
								flag = false;
							}
							if (Main.vanityPet[this.player.buffType[k]] && Main.vanityPet[this.player.inventory[i].buffType])
							{
								flag = false;
							}
						}
					}
					if (this.player.inventory[i].mana > 0 && flag)
					{
						if (this.player.statMana >= (int)((float)this.player.inventory[i].mana * this.player.manaCost))
						{

							float _maxRegenDelay = (1f - (float)this.player.statMana / (float)this.player.statManaMax2) * 60f * 4f + 45f;
							_maxRegenDelay *= 0.7f;

							this.player.manaRegenDelay = /*(int)this.player.*/(int)_maxRegenDelay;
							this.player.statMana -= (int)((float)this.player.inventory[i].mana * this.player.manaCost);
						}
						else
						{
							flag = false;
						}
					}
					if (this.player.whoAmI == Main.myPlayer && this.player.inventory[i].type == 603 && !Main.cEd)
					{
						flag = false;
					}
					if (num2 == 27)
					{
						num2 = Main.rand.Next(3);
						if (num2 == 0)
						{
							num2 = 27;
						}
						if (num2 == 1)
						{
							num2 = 101;
						}
						if (num2 == 2)
						{
							num2 = 102;
						}
					}
					if (flag)
					{
						ItemLoader.UseItem(this.player.inventory[i], this.player);
						legacySoundStyle = player.inventory[i].UseSound;
						int num3 = this.player.inventory[i].buffTime;
						if (num3 == 0)
						{
							num3 = 3600;
						}
						this.player.AddBuff(num2, num3, true);
						if (this.player.inventory[i].consumable)
						{
							//bool consume = true;
							//ItemLoader.ConsumeItem(this.player.inventory[i], this.player, ref consume);
							bool consume = ItemLoader.ConsumeItem(this.player.inventory[i], this.player);
							if (consume)
							{
								this.player.inventory[i].stack--;
							}
							if (this.player.inventory[i].stack <= 0)
							{
								this.player.inventory[i].TurnToAir();
							}
						}
					}
				}
			}
			if (legacySoundStyle != null)
			{
				Main.PlaySound(legacySoundStyle, player.position);
				Recipe.FindRecipes();
			}
		}

		public void QueryModOrigin()
		{
			Point mouseTile = Main.MouseWorld.ToTileCoordinates();

			int closestNPCIndex = -1;
			float closestNPCDistance = float.MaxValue;
			for (int l = 0; l < 200; l++)
			{
				if (Main.npc[l].active)
				{
					float distance = Utils.Distance(Main.npc[l].getRect(), Main.MouseWorld);
					if (distance < closestNPCDistance)
					{
						closestNPCDistance = distance;
						closestNPCIndex = l;
					}
				}
			}

			int closestItemIndex = -1;
			float closestItemDistance = float.MaxValue;
			for (int i = 0; i < 400; i++)
			{
				if (Main.item[i].active)
				{
					float distance = Utils.Distance(Main.item[i].getRect(), Main.MouseWorld);
					if (distance < closestItemDistance)
					{
						closestItemDistance = distance;
						closestItemIndex = i;
					}
				}
			}


			int hoverBuffIndex = -1;
			int hoverBuffID = -1;
			for (int k = 0; k < 22; k++)
			{
				if (Main.player[Main.myPlayer].buffType[k] > 0)
				{
					int buffID = Main.player[Main.myPlayer].buffType[k];
					int buffX = 32 + k * 38;
					int buffY = 76;
					if (k >= 11)
					{
						buffX = 32 + (k - 11) * 38;
						buffY += 50;
					}

					if (Main.mouseX < buffX + Main.buffTexture[buffID].Width && Main.mouseY < buffY + Main.buffTexture[buffID].Height && Main.mouseX > buffX && Main.mouseY > buffY)
					{
						hoverBuffIndex = k;
						hoverBuffID = buffID;
					}
				}
			}

			//Top to Bottom:

			// Hover Buff
			if (hoverBuffIndex > -1 && !Main.playerInventory)
			{
				if (hoverBuffID >= BuffID.Count)
				{
					ModBuff hoverBuff = BuffLoader.GetBuff(hoverBuffID);
					Main.NewText("This buff is from: " + hoverBuff.mod.DisplayName);
				}
				else
				{
					Main.NewText("This is a vanilla buff.");
				}
			}

			// Item in inventory
			else if (Main.HoverItem.type > 0 && Main.HoverItem != null)
			{
				if (Main.HoverItem.modItem != null)
				{
					Main.NewText("This item is from: " + Main.HoverItem.modItem.mod.DisplayName);
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {Main.HoverItem.modItem.mod.Name}, InternalName: {Main.HoverItem.modItem.Name}, FullName: {Main.HoverItem.modItem.GetType().FullName}");
				}
				else
				{
					Main.NewText("This is a vanilla item.");
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ItemID: {ItemID.Search.GetName(Main.HoverItem.type)}, ItemID#: {Main.HoverItem.type}");
				}
			}

			// NPC
			else if (closestNPCDistance < 30)
			{
				NPC closestNPC = Main.npc[closestNPCIndex];
				if (closestNPC.modNPC != null)
				{
					Main.NewText("This npc is from: " + closestNPC.modNPC.mod.DisplayName);
					if(HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {closestNPC.modNPC.mod.Name}, InternalName: {closestNPC.modNPC.Name}, FullName: {closestNPC.modNPC.GetType().FullName}");
				}
				else
				{
					Main.NewText("This is a vanilla npc.");
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: NPCID: {NPCID.Search.GetName(closestNPC.type)}, NPCID#: {closestNPC.type}");
				}
			}

			// Tile
			else if (Main.tile[mouseTile.X, mouseTile.Y].type >= TileID.Count)
			{
				Main.NewText("This tile is from: " + TileLoader.GetTile(Main.tile[mouseTile.X, mouseTile.Y].type).mod.DisplayName);
				//Main.NewText("This tile is active: " + Main.tile[mouseTile.X, mouseTile.Y].active());
				//Main.NewText("This tile is inactive: " + Main.tile[mouseTile.X, mouseTile.Y].inActive());
				//Main.NewText("This tile is nactive: " + Main.tile[mouseTile.X, mouseTile.Y].nactive());
			}

			// Wall
			else if (Main.tile[mouseTile.X, mouseTile.Y].wall >= WallID.Count)
			{
				Main.NewText("This wall is from: " + WallLoader.GetWall(Main.tile[mouseTile.X, mouseTile.Y].wall).mod.DisplayName);
			}

			// Item on ground
			else if (closestItemDistance < 5)
			{
				if (Main.item[closestItemIndex].modItem != null)
				{
					ModItem modItem = Main.item[closestItemIndex].modItem;
					Main.NewText("This item is from: " + modItem.mod.DisplayName);
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {modItem.mod.Name}, InternalName: {modItem.Name}, FullName: {modItem.GetType().FullName}");
				}
				else
				{
					Main.NewText("This is a vanilla item.");
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ItemID: {ItemID.Search.GetName(Main.item[closestItemIndex].type)}, ItemID#: {Main.item[closestItemIndex].type}");
				}
			}

			// How to Use
			else if (true)
			{
				Main.NewText("Hover over an item, npc, tile, or wall to identify which mod it is from.");
			}
		}

		public void ToggleAutoPause()
		{
			Main.autoPause = !Main.autoPause;
			Main.NewText("Autopause turned " + (Main.autoPause ? "on" : "off"));
		}

		public void QuickUseItemAt(int index, bool use = true)
		{
			if (player.inventory[index].type != 0)
			{
				originalSelectedItem = player.selectedItem;
				autoRevertSelectedItem = true;
				player.selectedItem = index;
				player.controlUseItem = true;
				if (use)
				{
					player.ItemCheck(Main.myPlayer);
				}
			}
		}

		public void QuickUseConfigItem() {
			int type = HelpfulHotkeysClientConfig.Instance.QuickUseConfigItem.Type;
			if(type == 0) {
				Main.NewText("No item registered for Quick Use Config Item hotkey, please fix.");
				return;
			}
			int index = Main.LocalPlayer.FindItem(type);
			if (index == -1) {
				Main.NewText($"Quick Use Config Item \"{Lang.GetItemNameValue(type)}\" not found in inventory.");
				return;
			}
			QuickUseItemAt(index);
		}

		public void SmartQuickStackToChests()
		{
			smartQuickStack();
			Recipe.FindRecipes();
		}

		public void QuickStackToChests()
		{
			if (player.chest != -1)
			{
				ChestUI.QuickStack();
			}
			else
			{
				player.QuickStackAllChests();
			}
			Recipe.FindRecipes();
		}

		public void CycleAmmo()
		{
			int indexOfFirst = -1;
			for (int i = 54; i < 57; i++)
			{
				if (player.inventory[i].type != 0)
				{
					indexOfFirst = i;
					break;
				}
			}

			if (indexOfFirst != -1)
			{
				Item temp = player.inventory[indexOfFirst];
				for (int i = indexOfFirst; i < 57; i++)
				{
					player.inventory[i] = player.inventory[i + 1];
				}
				player.inventory[57] = temp;
			}
		}

		public void AutoTorch()
		{
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (TileLoader.IsTorch(player.inventory[i].createTile))
				{
					QuickUseItemAt(i, use: false);
					Player.tileTargetX = (int)(player.Center.X / 16);
					Player.tileTargetY = (int)(player.Center.Y / 16);
					int oldstack = player.inventory[player.selectedItem].stack;

					List<Tuple<float, Point>> targets = new List<Tuple<float, Point>>();

					for (int j = -Player.tileRangeX - player.blockRange + (int)(player.position.X / 16f) + 1; j <= Player.tileRangeX + player.blockRange - 1 + (int)((player.position.X + player.width) / 16f); j++)
					{
						for (int k = -Player.tileRangeY - player.blockRange + (int)(player.position.Y / 16f) + 1; k <= Player.tileRangeY + player.blockRange - 2 + (int)((player.position.Y + player.height) / 16f); k++)
						{
							targets.Add(new Tuple<float, Point>(Vector2.Distance(Main.MouseWorld, new Vector2(j * 16, k * 16)), new Point(j, k)));
						}
					}
					targets.Sort((a, b) => a.Item1.CompareTo(b.Item1));

					bool placeSuccess = false;
					foreach (var target in targets)
					{
						Player.tileTargetX = target.Item2.X;
						Player.tileTargetY = target.Item2.Y;
						Tile original = (Tile)Main.tile[Player.tileTargetX, Player.tileTargetY].Clone();
						player.ItemCheck(Main.myPlayer);
						//Dust.QuickDust(target.Item2, Color.Aqua);
						int v = player.itemAnimation;
						if (!original.isTheSameAs(Main.tile[Player.tileTargetX, Player.tileTargetY]))
						{
							placeSuccess = true;
							break;
						}
					}
					if (placeSuccess)
						break;

					//if (this.position.X / 16f - (float)Player.tileRangeX - (float)this.inventory[this.selectedItem].tileBoost - (float)this.blockRange <= (float)Player.tileTargetX 
					//	&& (this.position.X + (float)this.width) / 16f + (float)Player.tileRangeX + (float)this.inventory[this.selectedItem].tileBoost - 1f + (float)this.blockRange >= (float)Player.tileTargetX 
					//	&& this.position.Y / 16f - (float)Player.tileRangeY - (float)this.inventory[this.selectedItem].tileBoost - (float)this.blockRange <= (float)Player.tileTargetY 
					//	&& (this.position.Y + (float)this.height) / 16f + (float)Player.tileRangeY + (float)this.inventory[this.selectedItem].tileBoost - 2f + (float)this.blockRange >= (float)Player.tileTargetY)
				}
			}
		}

		public void AutoRecall()
		{
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (HelpfulHotkeys.RecallItems.Contains(player.inventory[i].type))
				{
					QuickUseItemAt(i);
					break;
				}
			}
		}

		public void SwapArmorVanity()
		{
			bool swapHappens = false;
			for (int slot = 10; slot < 13; slot++)
			{
				if (((player.armor[slot].type > 0 && player.armor[slot].stack > 0 && !player.armor[slot].vanity)
					&& (player.armor[slot - 10].type > 0 && player.armor[slot - 10].stack > 0)))
				{
					Utils.Swap(ref player.armor[slot], ref player.armor[slot - 10]);
					swapHappens = true;
				}
			}
			if (swapHappens)
			{
				Main.PlaySound(SoundID.Grab);
				Recipe.FindRecipes();
			}

			//Item[] tempItems = new Item[10];
			//for (int i = 0; i < 10; i++)
			//{
			//	tempItems[i] = (Item)player.inventory[i].Clone();
			//	player.inventory[i] = (Item)player.inventory[40 + i].Clone();
			//}

			//for (int i = 0; i < 10; i++)
			//{
			//	player.inventory[40 + i] = (Item)tempItems[i].Clone();
			//}
		}

		public void SwapAccessoriesVanity()
		{
			bool swapHappens = false;
			for (int slot = 13; slot < 18 + player.extraAccessorySlots; slot++)
			{
				if (!player.armor[slot].IsAir && !player.armor[slot].vanity && !player.armor[slot - 10].IsAir)
				{
					bool wingPrevent = true;
					if (player.armor[slot].wingSlot > 0)
					{
						for (int i = 3; i < 10; i++)
						{
							if (player.armor[i].wingSlot > 0 && i != slot - 10)
							{
								wingPrevent = false;
							}
						}
					}
					if (wingPrevent)
					{
						Utils.Swap(ref player.armor[slot], ref player.armor[slot - 10]);
						swapHappens = true;
					}
				}
			}
			if (swapHappens)
			{
				Main.PlaySound(SoundID.Grab);
				Recipe.FindRecipes();
			}
		}

		public void SwapHotbar()
		{
			bool swapHappens = false;
			for (int i = 0; i < 10; i++)
			{
				if (/*!player.inventory[i].IsAir && */!player.inventory[i + 10].IsAir)
				{
					Utils.Swap(ref player.inventory[i], ref player.inventory[i + 10]);
					swapHappens = true;
				}
			}
			if (swapHappens)
			{
				Main.PlaySound(SoundID.Grab);
			}
		}

		internal void smartQuickStack()
		{
			if (player.chest != -1)
			{
				// check this chest for cateroies, then stack all items that fit those categories into this chest.

				Item[] items = player.bank.item;
				if (player.chest > -1)
				{
					items = Main.chest[player.chest].item;
				}
				else if (player.chest == -2)
				{
					items = player.bank.item;
				}
				else if (player.chest == -3)
				{
					items = player.bank2.item;
				}
				else if (player.chest == -4)
				{
					items = player.bank3.item;
				}

				bool itemMoved = false;
				List<ItemSorting.ItemSortingLayer> layersInThisChest = ItemSorting.GetPassingLayers(items);
				for (int i = 0; i < 50; i++)
				{
					Item item = player.inventory[i];
					if (item.type == ItemID.Count || item.favorited || (item.type >= 71 && item.type <= 74)) continue;
					foreach (var layer in ItemSorting.layerList)
					{
						if (layer.Pass(item))
						{
							if (layersInThisChest.Contains(layer))
							{
								itemMoved |= ChestUI.TryPlacingInChest(item, false);
								break;
							}
							else
							{
								break;
							}
						}

					}
				}
				if (itemMoved)
				{
					Main.PlaySound(7, -1, -1, 1, 1f, 0f);
				}
			}
			else
			{
				//TODO
				smartQuickStackAllChests();
				Recipe.FindRecipes();
			}
		}

		internal void smartQuickStackAllChests()
		{
			if (player.IsStackingItems())
			{
				return;
			}
			if (Main.netMode == 1)
			{
				Main.NewText("Smart Quick Stack to Nearby Chests not implemented in Multiplayer yet");
				//TODO
				return;
			}
			bool itemMoved = false;

			for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
			{
				if (Main.chest[chestIndex] != null && /*!Chest.IsPlayerInChest(i) &&*/ !Chest.isLocked(Main.chest[chestIndex].x, Main.chest[chestIndex].y))
				{
					Vector2 distanceToPlayer = new Vector2((float)(Main.chest[chestIndex].x * 16 + 16), (float)(Main.chest[chestIndex].y * 16 + 16));
					if ((distanceToPlayer - player.Center).Length() < 200f)
					{
						player.chest = chestIndex;
						Item[] items = Main.chest[chestIndex].item;
						List<ItemSorting.ItemSortingLayer> layersInThisChest = ItemSorting.GetPassingLayers(items);
						for (int i = 0; i < 50; i++)
						{
							Item item = player.inventory[i];
							if (item.type == ItemID.Count || item.favorited || (item.type >= 71 && item.type <= 74)) continue;
							foreach (var layer in ItemSorting.layerList)
							{
								if (layer.Pass(item))
								{
									if (layersInThisChest.Contains(layer))
									{
										itemMoved |= ChestUI.TryPlacingInChest(item, false);
										break;
									}
									else
									{
										break;
									}
								}

							}
						}
					}
				}
			}
			if (itemMoved)
			{
				Main.PlaySound(7, -1, -1, 1, 1f, 0f);
			}
			player.chest = -1;
		}
	}
}
