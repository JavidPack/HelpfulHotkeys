﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.Audio;
using Terraria.GameContent;

namespace HelpfulHotkeys
{
	public class HelpfulHotkeys : Mod
	{
		internal static List<int> RecallItems;

		internal static ModHotKey AutoRecallHotKey;
		internal static ModHotKey AutoTorchHotKey;
		internal static ModHotKey CycleAmmoHotKey;
		internal static ModHotKey QuickStackToChestsHotKey;
		internal static ModHotKey SmartQuickStackToChestsHotKey;
		internal static ModHotKey[] QuickUseItemHotkeys;
		internal static ModHotKey QuickUseConfigItemHotkey;
		internal static ModHotKey QuickBuffFavoritedOnlyHotkey;
		internal static ModHotKey QueryModOriginHotkey;
		internal static ModHotKey ToggleAutopauseHotkey;
		internal static ModHotKey SwapArmorVanityHotkey;
		internal static ModHotKey SwapHotbarHotkey;
		internal static ModHotKey CyclingQuickMountHotkey;
		internal static ModHotKey SwitchFrameSkipModeHotkey;
		internal static ModHotKey DashHotkey;
		// TODO QuickRestockFromNearbyChests --> Might need server side stuff....

		public HelpfulHotkeys()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
			};
		}

		public override void Load()
		{
			AutoRecallHotKey = RegisterHotKey("Auto Recall", "Home");
			AutoTorchHotKey = RegisterHotKey("Auto Torch", "OemTilde");
			CycleAmmoHotKey = RegisterHotKey("Cycle Ammo", "OemPeriod");
			QuickStackToChestsHotKey = RegisterHotKey("Quick Stack to Chests", "OemMinus");
			SmartQuickStackToChestsHotKey = RegisterHotKey("Smart Quick Stack to Chests", "OemPipe");
			QuickUseItemHotkeys = new ModHotKey[10];
			for (int i = 0; i < 10; i++) {
				if (!HelpfulHotkeysClientConfig.Instance.EnableQuickUseItems11to19 && i != 9)
					continue;
				QuickUseItemHotkeys[i] = RegisterHotKey($"Quick Use Item #{i + 11}", i == 9 ? "L" : "");
			}
			QuickUseConfigItemHotkey = RegisterHotKey("Quick Use Config Item", "");
			QuickBuffFavoritedOnlyHotkey = RegisterHotKey("Quick Buff Favorited Only", "B");
			QueryModOriginHotkey = RegisterHotKey("Query Mod Origin", "OemQuestion");
			ToggleAutopauseHotkey = RegisterHotKey("Toggle Autopause", "P");
			SwapArmorVanityHotkey = RegisterHotKey("Swap Armor with Vanity", "");
			SwapHotbarHotkey = RegisterHotKey("Swap Hotbar with 1st row", "");
			CyclingQuickMountHotkey = RegisterHotKey("Cycling Quick Mount", "");
			SwitchFrameSkipModeHotkey = RegisterHotKey("Switch Frame Skip Mode", "");
			DashHotkey = RegisterHotKey("Dash", "");

			HelpfulHotkeysSystems.smartStackButtonTextures = new Texture2D[]
			{
				GetTexture("SmartStack_Off").Value,
				GetTexture("SmartStack_On").Value
			};

			RecallItems = new List<int>(new int[]
			{
				ItemID.MagicMirror,
				ItemID.IceMirror,
				ItemID.CellPhone,
				ItemID.RecallPotion
			});

			/*var loadModsField = Assembly.GetCallingAssembly().GetType("Terraria.ModLoader.Interface").GetField("loadMods", BindingFlags.Static | BindingFlags.NonPublic);
			Main.instance.LoadNPC(NPCID.MoonLordHead);
			var face = new Terraria.GameContent.UI.Elements.UIImage(Main.npcTexture[NPCID.MoonLordHead]);
			face.HAlign = 0.5f;
			face.VAlign = 0.5f;
			(loadModsField.GetValue(null) as UIState).Append(face);*/
		}

		public override void Unload()
		{
			HelpfulHotkeysSystems.smartStackButtonTextures = null;
			QuickUseItemHotkeys = null;
			AutoRecallHotKey =
			AutoTorchHotKey =
			CycleAmmoHotKey =
			QuickStackToChestsHotKey =
			SmartQuickStackToChestsHotKey =
			QuickUseConfigItemHotkey =
			QuickBuffFavoritedOnlyHotkey =
			QueryModOriginHotkey =
			ToggleAutopauseHotkey =
			SwapArmorVanityHotkey =
			SwapHotbarHotkey =
			CyclingQuickMountHotkey =
			SwitchFrameSkipModeHotkey = null;
		}

		// 1.5.4.1 - Added ("RegisterRecallItem", int[ItemID])
		public override object Call(params object[] args)
		{
			try
			{
				string messageType = args[0] as string;
				switch (messageType)
				{
					case "RegisterRecallItem":
						RecallItems.Add(Convert.ToInt32(args[1]));
						return "Success";
					default:
						Logger.Warn("Unknown Message type: " + messageType);
						return "Failure";
				}
			}
			catch (Exception e)
			{
				Logger.Warn("Call Error: " + e.StackTrace + e.Message);
			}
			return "Failure";
		}
	}

	public class HelpfulHotkeysSystems : ModSystem
    {
		public static Texture2D[] smartStackButtonTextures;
		private float smartStackButtonScale;
		private bool smartStackButtonHovered;
		private bool smartStackButtonHover;
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int vanillaInventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (vanillaInventoryLayerIndex != -1)
			{
				// After Inventory is drawn
				layers.Insert(vanillaInventoryLayerIndex + 1, new LegacyGameInterfaceLayer(
					"HelpfulHotkeys: Smart Quick Stack",
					delegate
					{
						if (Main.playerInventory)
						{
							DrawSmartStackButton(Main.spriteBatch);
						}
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}

		private void DrawSmartStackButton(SpriteBatch spriteBatch)
		{
			if (Main.player[Main.myPlayer].chest == -1 && Main.npcShop == 0)
			{
				int imageChoice = 0;
				int x = 570;//498; //534
				int y = 244;
				int width = smartStackButtonTextures[imageChoice].Width;
				int height = smartStackButtonTextures[imageChoice].Height;
				//UILinkPointNavigator.SetPosition(301, new Vector2((float)x + (float)width * 0.75f, (float)y + (float)height * 0.75f));
				if (Main.mouseX >= x && Main.mouseX <= x + width && Main.mouseY >= y && Main.mouseY <= y + height && !PlayerInput.IgnoreMouseInterface)
				{
					imageChoice = 1;
					if (!smartStackButtonHover)
					{
						SoundEngine.PlaySound(SoundID.MenuTick);
						smartStackButtonHover = true;
					}

					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.mouseLeftRelease = false;
						HelpfulHotkeysPlayer ModPlayer = Main.LocalPlayer.GetModPlayer<HelpfulHotkeysPlayer>();
						ModPlayer.smartQuickStack();
						Recipe.FindRecipes();
					}

					Main.player[Main.myPlayer].mouseInterface = true;
				}
				else if (smartStackButtonHover)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
					smartStackButtonHover = false;
				}

				Main.spriteBatch.Draw(smartStackButtonTextures[imageChoice], new Vector2((float)x, (float)y), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, smartStackButtonTextures[imageChoice].Width, smartStackButtonTextures[imageChoice].Height)), Microsoft.Xna.Framework.Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				if (!Main.mouseText && imageChoice == 1)
				{
					Main.instance.MouseText("Smart stack to nearby chests");
				}
			}
			if (Main.player[Main.myPlayer].chest != -1 && !Main.recBigList)
			{
				// 506, Main.instance.invBottom + 40
				int ID = 7;
				Player Player = Main.player[Main.myPlayer];
				int Y = Main.instance.invBottom + 40 + ID * 29;
				float num = smartStackButtonScale;
				string text = "Smart Stack";
				Vector2 vector = FontAssets.MouseText.Value.MeasureString(text);
				Color baseColor = new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor) * num;
				baseColor = Color.White * 0.97f * (1f - (255f - (float)Main.mouseTextColor) / 255f * 0.5f);
				baseColor.A = 255;
				int X = 506 + (int)(vector.X * num / 2f);
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, new Vector2((float)X, (float)Y), baseColor, 0f, vector / 2f, new Vector2(num), -1f, 1.5f);
				vector *= num;

				if (!Utils.FloatIntersect((float)Main.mouseX, (float)Main.mouseY, 0f, 0f, (float)X - vector.X / 2f, (float)Y - vector.Y / 2f, vector.X, vector.Y))
				{
					UpdateHover(false);
					return;
				}
				UpdateHover(true);
				if (!PlayerInput.IgnoreMouseInterface)
				{
					Player.mouseInterface = true;
					if (!Main.mouseLeft || !Main.mouseLeftRelease)
					{
						return;
					}
					HelpfulHotkeysPlayer ModPlayer = Player.GetModPlayer<HelpfulHotkeysPlayer>();
					ModPlayer.smartQuickStack();
					Recipe.FindRecipes();
				}
			}
		}
		public void UpdateHover(bool hovering)
		{
			if (hovering)
			{
				if (!smartStackButtonHovered)
				{
					SoundEngine.PlaySound(12, -1, -1, 1, 1f, 0f);
				}
				smartStackButtonHovered = true;
				smartStackButtonScale += 0.05f;
				if (smartStackButtonScale > 1f)
				{
					smartStackButtonScale = 1f;
					return;
				}
			}
			else
			{
				smartStackButtonHovered = false;
				smartStackButtonScale -= 0.05f;
				if (smartStackButtonScale < 0.75f)
				{
					smartStackButtonScale = 0.75f;
				}
			}
		}
	}
}
