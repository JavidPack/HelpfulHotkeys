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
using Terraria.GameContent;
using Terraria.DataStructures;

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
			//Player.LaunchMinecartHook(point3.Value.X, point3.Value.Y);
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
					Main.NewText("Auto Cycle Ammo Mode has been " + (autoCycleAmmo ? "enabled" : "disabled"), Color.Aquamarine.R, Color.Aquamarine.G, Color.Aquamarine.B);
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
			for (int i = 0; i < 10; i++) {
				if (HelpfulHotkeys.QuickUseItemHotkeys[i]?.JustPressed == true) {
					QuickUseItemAt(i + 10);
				}
			}
			if (HelpfulHotkeys.QuickUseConfigItemHotkey.JustPressed) 
			{
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
			if (HelpfulHotkeys.SwapArmorInventoryHotkey.JustPressed) { 
				SwapArmorInventory();
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
			if (HelpfulHotkeys.HoldMountHotkey.JustPressed) {
				if (!Player.mount.Active)
					Player.QuickMount();
			} else if (HelpfulHotkeys.HoldMountHotkey.JustReleased) {
				if (Player.mount.Active)
					Player.mount.Dismount(Player);
			}
			if (HelpfulHotkeys.SwitchFrameSkipModeHotkey.JustPressed) {
				Main.CycleFrameSkipMode();
				Main.NewText($"Frame Skip Mode is now: {Language.GetTextValue("LegacyMenu." + (247 + Main.FrameSkipMode))}");
			}
		}

		public override void SetControls() {
			if (HelpfulHotkeys.DashHotkey.JustPressed && (!HelpfulHotkeysClientConfig.Instance.DashHotkeyDisabledWhileInChest || Player.chest == -1)) {
				if (Player.controlRight) {
					Player.dashTime = 15;
					Player.releaseRight = true;
				}
				else if (Player.controlLeft) {
					Player.dashTime = -15;
					Player.releaseLeft = true;
				}
				else if (Player.direction == 1) {
					Player.dashTime = 15;
					Player.releaseRight = true;
					Player.controlRight = true;
				}
				else if (Player.direction == -1) {
					Player.dashTime = -15;
					Player.releaseLeft = true;
					Player.controlLeft = true;
				}
			}
			else if (HelpfulHotkeysClientConfig.Instance.DashHotkeyDisablesDoubleTap) {
				Player.dashTime = 0;
			}
		}

		// if a mount, unmount and find the next. none found => dismount. unmounted-> mount default
		private void CyclingQuickMount()
		{
			int currentMount = 0;
			if (Player.mount.Active)
			{
				currentMount = Player.mount.Type;
				Player.mount.Dismount(Player);
				// look for another(next in priority), if found, mount
				Item nextMountItem = QuickMountCycle_GetItemToUse(currentMount);
				if (nextMountItem != null && nextMountItem.mountType != -1 && Player.mount.CanMount(nextMountItem.mountType, Player))
				{
					Player.mount.SetMount(nextMountItem.mountType, Player, false);
					ItemLoader.UseItem(nextMountItem, Player);
					if (nextMountItem.UseSound != null)
					{
						SoundEngine.PlaySound(nextMountItem.UseSound.Value, Player.Center);
						return;
					}
				}
				return;
			}
			if (Player.frozen || Player.tongued || Player.webbed || Player.stoned || Player.gravDir == -1f)
			{
				return;
			}
			if (Player.noItems)
			{
				return;
			}
			Item item = Player.QuickMount_GetItemToUse();
			//Item item = QuickMount_GetItemToUse();
			if (item != null && item.mountType != -1 && Player.mount.CanMount(item.mountType, Player))
			{
				bool flag = false;
				List<Point> tilesIn = Collision.GetTilesIn(Player.TopLeft - new Vector2(24f), Player.BottomRight + new Vector2(24f));
				if (tilesIn.Count > 0)
				{
					Point? point = null;
					Rectangle arg_CD_0 = Player.Hitbox;
					for (int i = 0; i < tilesIn.Count; i++)
					{
						Point point2 = tilesIn[i];
						Tile tileSafely = Framing.GetTileSafely(point2.X, point2.Y);
						if (tileSafely.HasTile && tileSafely.TileType == 314)
						{
							Vector2 vector = tilesIn[i].ToVector2() * 16f + new Vector2(8f);
							if (!point.HasValue || (Player.Distance(vector) < Player.Distance(point.Value.ToVector2() * 16f + new Vector2(8f)) && Collision.CanHitLine(Player.Center, 0, 0, vector, 0, 0)))
							{
								point = new Point?(tilesIn[i]);
							}
						}
					}
					if (point.HasValue)
					{
						object[] parametersArray = new object[] { point.Value.X, point.Value.Y };
						LaunchMinecartHookMethod.Invoke(Player, parametersArray);
						//todo reflection				Player.LaunchMinecartHook(point.Value.X, point.Value.Y);
						flag = true;
					}
				}
				if (!flag)
				{
					Player.mount.SetMount(item.mountType, Player, false);
					ItemLoader.UseItem(item, Player);
					if (item.UseSound != null)
					{
						SoundEngine.PlaySound(item.UseSound.Value, Player.Center);
						return;
					}
				}
			}
			else
			{
				int num = 0;
				int num2 = (int)(Player.position.X / 16f) - Player.tileRangeX - num + 1;
				int num3 = (int)((Player.position.X + (float)Player.width) / 16f) + Player.tileRangeX + num - 1;
				int num4 = (int)(Player.position.Y / 16f) - Player.tileRangeY - num + 1;
				int num5 = (int)((Player.position.Y + (float)Player.height) / 16f) + Player.tileRangeY + num - 2;
				num2 = Utils.Clamp<int>(num2, 10, Main.maxTilesX - 10);
				num3 = Utils.Clamp<int>(num3, 10, Main.maxTilesX - 10);
				num4 = Utils.Clamp<int>(num4, 10, Main.maxTilesY - 10);
				num5 = Utils.Clamp<int>(num5, 10, Main.maxTilesY - 10);
				List<Point> tilesIn2 = Collision.GetTilesIn(new Vector2((float)num2, (float)num4) * 16f, new Vector2((float)(num3 + 1), (float)(num5 + 1)) * 16f);
				if (tilesIn2.Count > 0)
				{
					Point? point3 = null;
					Rectangle arg_338_0 = Player.Hitbox;
					for (int j = 0; j < tilesIn2.Count; j++)
					{
						Point point4 = tilesIn2[j];
						Tile tileSafely2 = Framing.GetTileSafely(point4.X, point4.Y);
						if (tileSafely2.HasTile && tileSafely2.TileType == 314)
						{
							Vector2 vector2 = tilesIn2[j].ToVector2() * 16f + new Vector2(8f);
							if (!point3.HasValue || (Player.Distance(vector2) < Player.Distance(point3.Value.ToVector2() * 16f + new Vector2(8f)) && Collision.CanHitLine(Player.Center, 0, 0, vector2, 0, 0)))
							{
								point3 = new Point?(tilesIn2[j]);
							}
						}
					}
					if (point3.HasValue)
					{
						object[] parametersArray = new object[] { point3.Value.X, point3.Value.Y };
						LaunchMinecartHookMethod.Invoke(Player, parametersArray);
						//todo				Player.LaunchMinecartHook(point3.Value.X, point3.Value.Y);
					}
				}
			}
		}

		public Item QuickMountCycle_GetItemToUse(int lastMount)
		{
			bool lastMountFound = false;
			//bool lastMountPassed = false;
			Item item = null;
			if (item == null && Player.miscEquips[3].mountType != -1 && !MountID.Sets.Cart[Player.miscEquips[3].mountType] && ItemLoader.CanUseItem(Player.miscEquips[3], Player))
			{
				//	item = Player.miscEquips[3];
				if (lastMount == Player.miscEquips[3].mountType)
				{
					lastMountFound = true;
				}
			}
			if (item == null)
			{
				for (int i = 0; i < 58; i++)
				{
					if (Player.inventory[i].mountType != -1 && !MountID.Sets.Cart[Player.inventory[i].mountType] && ItemLoader.CanUseItem(Player.inventory[i], Player))
					{
						if (lastMountFound)
						{
							item = Player.inventory[i];
							break;
						}
						else
						{
							if (lastMount == Player.inventory[i].mountType)
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
				if (Player.itemTime == 0 && Player.itemAnimation == 0)
				{
					Player.selectedItem = originalSelectedItem;
					autoRevertSelectedItem = false;
				}
			}
		}

		public override bool CanConsumeAmmo(Item weapon, Item ammo)
		{
			if (autoCycleAmmo)
			{
				if (weapon.useAmmo != 0)
				{
					CycleAmmo();
				}
			}
			return base.CanConsumeAmmo(weapon, ammo);
		}

		public void QuickBuffFavoritedOnly()
		{
			if (this.Player.noItems)
			{
				return;
			}
			SoundStyle? legacySoundStyle = null;
			for (int i = 0; i < 58; i++)
			{
				if (this.Player.CountBuffs() == 22)
				{
					return;
				}
				if (this.Player.inventory[i].stack > 0 && this.Player.inventory[i].type > ItemID.None && this.Player.inventory[i].favorited && this.Player.inventory[i].buffType > 0 && this.Player.inventory[i].DamageType != DamageClass.Summon && this.Player.inventory[i].buffType != 90)
				{
					int num2 = this.Player.inventory[i].buffType;
					bool flag = ItemLoader.CanUseItem(this.Player.inventory[i], this.Player);
					for (int j = 0; j < 22; j++)
					{
						if (num2 == 27 && (this.Player.buffType[j] == num2 || this.Player.buffType[j] == 101 || this.Player.buffType[j] == 102))
						{
							flag = false;
							break;
						}
						if (this.Player.buffType[j] == num2)
						{
							flag = false;
							break;
						}
						if (Main.meleeBuff[num2] && Main.meleeBuff[this.Player.buffType[j]])
						{
							flag = false;
							break;
						}
					}
					if (Main.lightPet[this.Player.inventory[i].buffType] || Main.vanityPet[this.Player.inventory[i].buffType])
					{
						for (int k = 0; k < 22; k++)
						{
							if (Main.lightPet[this.Player.buffType[k]] && Main.lightPet[this.Player.inventory[i].buffType])
							{
								flag = false;
							}
							if (Main.vanityPet[this.Player.buffType[k]] && Main.vanityPet[this.Player.inventory[i].buffType])
							{
								flag = false;
							}
						}
					}
					if (this.Player.inventory[i].mana > 0 && flag)
					{
						if (this.Player.statMana >= (int)((float)this.Player.inventory[i].mana * this.Player.manaCost))
						{

							float _maxRegenDelay = (1f - (float)this.Player.statMana / (float)this.Player.statManaMax2) * 60f * 4f + 45f;
							_maxRegenDelay *= 0.7f;

							this.Player.manaRegenDelay = /*(int)this.Player.*/(int)_maxRegenDelay;
							this.Player.statMana -= (int)((float)this.Player.inventory[i].mana * this.Player.manaCost);
						}
						else
						{
							flag = false;
						}
					}
					if (this.Player.whoAmI == Main.myPlayer && this.Player.inventory[i].type == ItemID.Carrot && !Main.runningCollectorsEdition)
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
						ItemLoader.UseItem(this.Player.inventory[i], this.Player);
						legacySoundStyle = Player.inventory[i].UseSound;
						int num3 = this.Player.inventory[i].buffTime;
						if (num3 == 0)
						{
							num3 = 3600;
						}
						this.Player.AddBuff(num2, num3, true);
						if (this.Player.inventory[i].consumable)
						{
							//bool consume = true;
							//ItemLoader.ConsumeItem(this.Player.inventory[i], this.Player, ref consume);
							bool consume = ItemLoader.ConsumeItem(this.Player.inventory[i], this.Player);
							if (consume)
							{
								this.Player.inventory[i].stack--;
							}
							if (this.Player.inventory[i].stack <= 0)
							{
								this.Player.inventory[i].TurnToAir();
							}
						}
					}
				}
			}
			if (legacySoundStyle != null)
			{
				SoundEngine.PlaySound(legacySoundStyle.Value, Player.position);
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

					if (Main.mouseX < buffX + TextureAssets.Buff[buffID].Value.Width && Main.mouseY < buffY + TextureAssets.Buff[buffID].Value.Height && Main.mouseX > buffX && Main.mouseY > buffY)
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
					Main.NewText("This buff is from: " + hoverBuff.Mod.DisplayName);
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {hoverBuff.Mod.Name}, InternalName: {hoverBuff.Name}, FullName: {hoverBuff.GetType().FullName}");
				}
				else
				{
					Main.NewText("This is a vanilla buff.");
				}
			}

			// Item in inventory
			else if (Main.HoverItem.type > ItemID.None && Main.HoverItem != null)
			{
				if (Main.HoverItem.ModItem != null)
				{
					Main.NewText("This item is from: " + Main.HoverItem.ModItem.Mod.DisplayName);
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {Main.HoverItem.ModItem.Mod.Name}, InternalName: {Main.HoverItem.ModItem.Name}, FullName: {Main.HoverItem.ModItem.GetType().FullName}");
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
				if (closestNPC.ModNPC != null)
				{
					Main.NewText("This npc is from: " + closestNPC.ModNPC.Mod.DisplayName);
					if(HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {closestNPC.ModNPC.Mod.Name}, InternalName: {closestNPC.ModNPC.Name}, FullName: {closestNPC.ModNPC.GetType().FullName}");
				}
				else
				{
					Main.NewText("This is a vanilla npc.");
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: NPCID: {NPCID.Search.GetName(closestNPC.type)}, NPCID#: {closestNPC.type}");
				}
			}

			// Tile
			else if (Main.tile[mouseTile.X, mouseTile.Y].TileType >= TileID.Count)
			{
				ModTile ModTile = TileLoader.GetTile(Main.tile[mouseTile.X, mouseTile.Y].TileType);
				Main.NewText("This tile is from: " + ModTile.Mod.DisplayName);
				if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
					Main.NewText($"Developer Info: ModName: {ModTile.Mod.Name}, InternalName: {ModTile.Name}, FullName: {ModTile.GetType().FullName}");
				//Main.NewText("This tile is active: " + Main.tile[mouseTile.X, mouseTile.Y].active());
				//Main.NewText("This tile is inactive: " + Main.tile[mouseTile.X, mouseTile.Y].inActive());
				//Main.NewText("This tile is nactive: " + Main.tile[mouseTile.X, mouseTile.Y].nactive());
			}

			// Wall
			else if (Main.tile[mouseTile.X, mouseTile.Y].WallType >= WallID.Count)
			{
				ModWall ModWall = WallLoader.GetWall(Main.tile[mouseTile.X, mouseTile.Y].WallType);
				Main.NewText("This wall is from: " + ModWall.Mod.DisplayName);
				if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
					Main.NewText($"Developer Info: ModName: {ModWall.Mod.Name}, InternalName: {ModWall.Name}, FullName: {ModWall.GetType().FullName}");
			}

			// Item on ground
			else if (closestItemDistance < 5)
			{
				if (Main.item[closestItemIndex].ModItem != null)
				{
					ModItem ModItem = Main.item[closestItemIndex].ModItem;
					Main.NewText("This item is from: " + ModItem.Mod.DisplayName);
					if (HelpfulHotkeysClientConfig.Instance.ShowDeveloperInfoQueryModOrigin)
						Main.NewText($"Developer Info: ModName: {ModItem.Mod.Name}, InternalName: {ModItem.Name}, FullName: {ModItem.GetType().FullName}");
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
				Main.NewText("Hover over an item, npc, tile, or wall to identify which Mod it is from.");
			}
		}

		public void ToggleAutoPause()
		{
			Main.autoPause = !Main.autoPause;
			Main.NewText("Autopause turned " + (Main.autoPause ? "on" : "off"));
		}

		public void QuickUseItemAt(int index, bool use = true)
		{
			// TODO: As of 1.4, this doesn't seem to honor not activating while an item is already in use.
			if (!autoRevertSelectedItem && Player.selectedItem != index && Player.inventory[index].type != ItemID.None)
			{
				originalSelectedItem = Player.selectedItem;
				autoRevertSelectedItem = true;
				Player.selectedItem = index;
				Player.controlUseItem = true;
				if (use)
				{
					Player.ItemCheck();
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
			if (Player.chest != -1)
			{
				ChestUI.QuickStack(ContainerTransferContext.FromUnknown(Player)); // TODO: is this right?
			}
			else
			{
				Player.QuickStackAllChests();
			}
			Recipe.FindRecipes();
		}

		public void CycleAmmo()
		{
			int indexOfFirst = -1;
			for (int i = Main.InventoryAmmoSlotsStart; i < 57; i++)
			{
				if (Player.inventory[i].type != ItemID.None)
				{
					indexOfFirst = i;
					break;
				}
			}

			if (indexOfFirst != -1)
			{
				Item temp = Player.inventory[indexOfFirst];
				for (int i = indexOfFirst; i < 57; i++)
				{
					Player.inventory[i] = Player.inventory[i + 1];
				}
				Player.inventory[57] = temp;
			}
		}

		public void AutoTorch()
		{
			// TODO: Should this behave like the 1.4 torch placement?
			for (int i = 0; i < Player.inventory.Length; i++)
			{
				int torchTile = Player.inventory[i].createTile;
				if (torchTile != -1 && TileID.Sets.Torch[torchTile])
				{
					QuickUseItemAt(i, use: false);
					Player.tileTargetX = (int)(Player.Center.X / 16);
					Player.tileTargetY = (int)(Player.Center.Y / 16);
					int oldstack = Player.inventory[Player.selectedItem].stack;

					List<Tuple<float, Point>> targets = new List<Tuple<float, Point>>();

					int fixedTileRangeX = Math.Min(Player.tileRangeX, 50);
					int fixedTileRangeY = Math.Min(Player.tileRangeY, 50);

					for (int j = -fixedTileRangeX - Player.blockRange + (int)(Player.position.X / 16f) + 1; j <= fixedTileRangeX + Player.blockRange - 1 + (int)((Player.position.X + Player.width) / 16f); j++)
					{
						for (int k = -fixedTileRangeY - Player.blockRange + (int)(Player.position.Y / 16f) + 1; k <= fixedTileRangeY + Player.blockRange - 2 + (int)((Player.position.Y + Player.height) / 16f); k++)
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
						// Reminder: Recent tile changes mean that tile instances don't hold actual data.
						//Tile original = (Tile)Main.tile[Player.tileTargetX, Player.tileTargetY].Clone();
						Tile original = Main.tile[Player.tileTargetX, Player.tileTargetY];
						var tileDataBefore = (original.TileType, original.WallType, original.HasTile);
						Player.ItemCheck();
						var tileDataAfter = (original.TileType, original.WallType, original.HasTile);
						//Dust.QuickDust(target.Item2, Color.Aqua);
						int v = Player.itemAnimation;
						//if (!original.IsTheSameAs(Main.tile[Player.tileTargetX, Player.tileTargetY]))
						if(tileDataBefore != tileDataAfter)
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
			for (int i = 0; i < Player.inventory.Length; i++)
			{
				if (HelpfulHotkeys.RecallItems.Contains(Player.inventory[i].type))
				{
					QuickUseItemAt(i);
					break;
				}
			}
		}

		public void SwapArmorInventory() {
			bool swapHappens = false;

			//var armorIndexes = new int[] { 10, 11, 12 };
			//var inventoryIndexes = new int[] { 29, 39, 49 };

			var inventoryIndexes = HelpfulHotkeysClientConfig.Instance.SwapArmorInventorySlots;

			foreach (var inventoryIndex in inventoryIndexes) {
				if (inventoryIndex < 0 || inventoryIndex > 49)
					continue;
			//for (int i = 0; i < 3; i++) {
				//ref Item armorItem = ref Player.armor[armorIndexes[i]];
				ref Item inventoryItem = ref Player.inventory[inventoryIndex];
				//if (((armorItem.type > ItemID.None && armorItem.stack > 0 && !armorItem.vanity)
				//	&& (inventoryItem.type > ItemID.None && inventoryItem.stack > 0))) {
				//	Utils.Swap(ref armorItem, ref inventoryItem);
				//	swapHappens = true;
				//}
				int original = inventoryItem.type;
				if (!inventoryItem.IsAir && inventoryItem.maxStack == 1) {
					// private ItemSlot.ArmorSwap()
					// check not bunch of things
					if (inventoryItem.headSlot != -1 || inventoryItem.bodySlot != -1 || inventoryItem.legSlot != -1) {
						ItemSlot.SwapEquip(Player.inventory, ItemSlot.Context.InventoryItem, inventoryIndex);
						if (original != inventoryItem.type)
							swapHappens = true;
					}
				}
				// Can support accessories as well, but they won't swap out in mass.
			}

			if (swapHappens) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes();
			}
		}

		public void SwapArmorVanity()
		{
			bool swapHappens = false;
			for (int slot = 10; slot < 13; slot++)
			{
				if (((Player.armor[slot].type > ItemID.None && Player.armor[slot].stack > 0 && !Player.armor[slot].vanity)
					&& (Player.armor[slot - 10].type > ItemID.None && Player.armor[slot - 10].stack > 0)))
				{
					Utils.Swap(ref Player.armor[slot], ref Player.armor[slot - 10]);
					swapHappens = true;
				}
			}
			if (swapHappens)
			{
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes();
			}

			//Item[] tempItems = new Item[10];
			//for (int i = 0; i < 10; i++)
			//{
			//	tempItems[i] = (Item)Player.inventory[i].Clone();
			//	Player.inventory[i] = (Item)Player.inventory[40 + i].Clone();
			//}

			//for (int i = 0; i < 10; i++)
			//{
			//	Player.inventory[40 + i] = (Item)tempItems[i].Clone();
			//}
		}

		public void SwapAccessoriesVanity()
		{
			bool swapHappens = false;
			for (int slot = 13; slot < 18 + Player.SupportedSlotsAccs; slot++)
			{
				if (!Player.armor[slot].IsAir && !Player.armor[slot].vanity && !Player.armor[slot - 10].IsAir)
				{
					bool wingPrevent = true;
					if (Player.armor[slot].wingSlot > 0)
					{
						for (int i = 3; i < 10; i++)
						{
							if (Player.armor[i].wingSlot > 0 && i != slot - 10)
							{
								wingPrevent = false;
							}
						}
					}
					if (wingPrevent)
					{
						Utils.Swap(ref Player.armor[slot], ref Player.armor[slot - 10]);
						swapHappens = true;
					}
				}
			}
			if (swapHappens)
			{
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes();
			}
		}

		public void SwapHotbar()
		{
			bool swapHappens = false;
			for (int i = Main.InventoryItemSlotsStart; i < 10; i++)
			{
				if (/*!Player.inventory[i].IsAir && */!Player.inventory[i + 10].IsAir)
				{
					Utils.Swap(ref Player.inventory[i], ref Player.inventory[i + 10]);
					swapHappens = true;
				}
			}
			if (swapHappens)
			{
				SoundEngine.PlaySound(SoundID.Grab);
			}
		}

		internal void smartQuickStack()
		{
			// TODO: Fix sorts for 1.4.4. Find out how to use sort to chest animation to fly items to chests.
			if (Player.chest != -1)
			{
				// check this chest for categories, then stack all items that fit those categories into this chest.

				Item[] items = Player.bank.item;
				if (Player.chest > -1)
				{
					items = Main.chest[Player.chest].item;
				}
				else if (Player.chest == -2)
				{
					items = Player.bank.item;
				}
				else if (Player.chest == -3)
				{
					items = Player.bank2.item;
				}
				else if (Player.chest == -4)
				{
					items = Player.bank3.item;
				}
				else if (Player.chest == -5)
                {
					items = Player.bank4.item;
                }

				bool itemMoved = false;
				List<ItemSorting.ItemSortingLayer> layersInThisChest = ItemSorting.GetPassingLayers(items);
				for (int i = Main.InventoryItemSlotsStart; i < Main.InventoryItemSlotsCount; i++)
				{
					Item item = Player.inventory[i];
					// TODO: filter out Unloaded item from here and all other usages of this snippet.
					if (item.IsAir || item.favorited || item.IsACoin) continue;
					foreach (var layer in ItemSorting.layerList)
					{
						if (layer.Pass(item))
						{
							if (layersInThisChest.Contains(layer))
							{
								itemMoved |= ChestUI.TryPlacingInChest(item, false, ItemSlot.Context.InventoryItem /* Unused, but I think this makes sense. */);
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
					SoundEngine.PlaySound(SoundID.Grab);
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
			if (Player.IsStackingItems())
			{
				return;
			}
			//if (Main.netMode == NetmodeID.MultiplayerClient)
			//{
			//	Main.NewText("Smart Quick Stack to Nearby Chests not implemented in Multiplayer yet");
			//	return;
			//}
			bool itemMoved = false;

			for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
			{
				if (Main.chest[chestIndex] != null && /*!Chest.IsPlayerInChest(i) &&*/ !Chest.IsLocked(Main.chest[chestIndex].x, Main.chest[chestIndex].y))
				{
					Vector2 distanceToPlayer = new Vector2((float)(Main.chest[chestIndex].x * 16 + 16), (float)(Main.chest[chestIndex].y * 16 + 16));
					if ((distanceToPlayer - Player.Center).Length() < Chest.chestStackRange)
					{
						Player.chest = chestIndex;
						Item[] items = Main.chest[chestIndex].item;
						List<ItemSorting.ItemSortingLayer> layersInThisChest = ItemSorting.GetPassingLayers(items);
						for (int i = Main.InventoryItemSlotsStart + 10; i < Main.InventoryItemSlotsCount; i++)
						{
							Item item = Player.inventory[i];
							if (item.IsAir || item.favorited || item.IsACoin) continue;
							foreach (var layer in ItemSorting.layerList)
							{
								if (layer.Pass(item))
								{
									if (layersInThisChest.Contains(layer))
									{
										itemMoved |= ChestUI.TryPlacingInChest(item, false, ItemSlot.Context.InventoryItem /* Unused, but I think this makes sense. */);
										break;
									}
									else
									{
										break;
									}
								}

							}

							// TODO: Double check if this code is correct/works
							if (Main.netMode == NetmodeID.MultiplayerClient)
							{
								for (int l = Main.InventoryItemSlotsStart + 10; l < Main.InventoryItemSlotsCount; l++)
								{
									if (!item.IsAir && !item.favorited && !item.IsACoin)
									{
										NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, Player.whoAmI, l, (int)item.prefix);
										NetMessage.SendData(MessageID.QuickStackChests, -1, -1, null, l);
										Player.inventoryChestStack[l] = true;
									}
								}

								return;
							}
						}
					}
				}
			}
			if (itemMoved)
			{
				SoundEngine.PlaySound(SoundID.Grab);
			}
			Player.chest = -1;
		}
	}
}
