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
using ReLogic.Content;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace HelpfulHotkeys
{
	public class HelpfulHotkeys : Mod
	{
		private Texture2D[] smartStackButtonTextures;
		private bool smartStackButtonHover;
		private float smartStackButtonScale;
		private bool smartStackButtonHovered;

		internal static List<int> RecallItems;

		internal static ModKeybind AutoRecallHotKey;
		internal static ModKeybind AutoTorchHotKey;
		internal static ModKeybind CycleAmmoHotKey;
		internal static ModKeybind QuickStackToChestsHotKey;
		internal static ModKeybind SmartQuickStackToChestsHotKey;
		internal static ModKeybind[] QuickUseItemHotkeys;
		internal static ModKeybind QuickUseConfigItemHotkey;
		internal static ModKeybind QuickBuffFavoritedOnlyHotkey;
		internal static ModKeybind QueryModOriginHotkey;
		internal static ModKeybind ToggleAutopauseHotkey;
		internal static ModKeybind ToggleRunInBackgroundHotkey;
		internal static ModKeybind SwapArmorInventoryHotkey;
		internal static ModKeybind SwapArmorVanityHotkey;
		internal static ModKeybind SwapHotbarHotkey;
		internal static ModKeybind CyclingQuickMountHotkey;
		internal static ModKeybind HoldMountHotkey;
		internal static ModKeybind SwitchFrameSkipModeHotkey;
		internal static ModKeybind DashHotkey;
		// TODO QuickRestockFromNearbyChests --> Might need server side stuff....

		internal static bool RunInBackground = false;

		public override void Load()
		{
			AutoRecallHotKey = KeybindLoader.RegisterKeybind(this, "AutoRecall", "Home") ;
			AutoTorchHotKey = KeybindLoader.RegisterKeybind(this, "AutoTorch", "OemTilde");
			CycleAmmoHotKey = KeybindLoader.RegisterKeybind(this, "CycleAmmo", "OemPeriod");
			QuickStackToChestsHotKey = KeybindLoader.RegisterKeybind(this, "QuickStackToChests", "OemMinus");
			SmartQuickStackToChestsHotKey = KeybindLoader.RegisterKeybind(this, "SmartQuickStackToChests", "OemPipe");
			QuickUseItemHotkeys = new ModKeybind[10];
			for (int i = 0; i < 10; i++) {
				if (!HelpfulHotkeysClientConfig.Instance.EnableQuickUseItems11to19 && i != 9)
					continue;
				QuickUseItemHotkeys[i] = KeybindLoader.RegisterKeybind(this, $"QuickUseItem{i + 11}", i == 9 ?"L" : "Z");
			}
			QuickUseConfigItemHotkey = KeybindLoader.RegisterKeybind(this, "QuickUseConfigItem", "Z");
			QuickBuffFavoritedOnlyHotkey = KeybindLoader.RegisterKeybind(this, "QuickBuffFavoritedOnly", "B");
			QueryModOriginHotkey = KeybindLoader.RegisterKeybind(this, "QueryModOrigin", "OemQuestion");
			ToggleAutopauseHotkey = KeybindLoader.RegisterKeybind(this, "ToggleAutopause", "P");
			ToggleRunInBackgroundHotkey = KeybindLoader.RegisterKeybind(this, "ToggleRunInBackground", "Back");
			SwapArmorInventoryHotkey = KeybindLoader.RegisterKeybind(this, "SwapArmorWithInventory", "Z");
			SwapArmorVanityHotkey = KeybindLoader.RegisterKeybind(this, "SwapArmorWithVanity", "Z");
			SwapHotbarHotkey = KeybindLoader.RegisterKeybind(this, "SwapHotbarWith1stRow", "Z");
			CyclingQuickMountHotkey = KeybindLoader.RegisterKeybind(this, "CyclingQuickMount", "Z");
			HoldMountHotkey = KeybindLoader.RegisterKeybind(this, "QuickMountHold", "Z");
			SwitchFrameSkipModeHotkey = KeybindLoader.RegisterKeybind(this, "SwitchFrameSkipMode", "Z");
			DashHotkey = KeybindLoader.RegisterKeybind(this, "Dash", "Z");

			smartStackButtonTextures = new Texture2D[]
			{
				ModContent.Request<Texture2D>("HelpfulHotkeys/SmartStack_Off", AssetRequestMode.ImmediateLoad).Value,
				ModContent.Request<Texture2D>("HelpfulHotkeys/SmartStack_On", AssetRequestMode.ImmediateLoad).Value
			};

			RecallItems = new List<int>(new int[]
			{
				ItemID.MagicMirror,
				ItemID.IceMirror,
				ItemID.CellPhone,
				ItemID.RecallPotion,
				ItemID.PotionOfReturn,
				ItemID.Shellphone,
				ItemID.ShellphoneSpawn,
				ItemID.ShellphoneOcean,
				ItemID.ShellphoneHell
			});

			Terraria.On_Main.CanPauseGame += CanPauseGame;
			Terraria.IL_Main.DoUpdate += MainDoUpdateHasFocus;
			/*var loadModsField = Assembly.GetCallingAssembly().GetType("Terraria.ModLoader.Interface").GetField("loadMods", BindingFlags.Static | BindingFlags.NonPublic);
			Main.instance.LoadNPC(NPCID.MoonLordHead);
			var face = new Terraria.GameContent.UI.Elements.UIImage(Main.npcTexture[NPCID.MoonLordHead]);
			face.HAlign = 0.5f;
			face.VAlign = 0.5f;
			(loadModsField.GetValue(null) as UIState).Append(face);*/

		}

		private void MainDoUpdateHasFocus(ILContext il) {
			try {
				ILCursor c = new(il);

				#region Main.HasFocus set
				//IL_083D: stsfld    bool Terraria.Main::hasFocus

				int index = -1;
				if (!c.TryGotoNext(MoveType.Before, i => i.MatchStsfld(typeof(Main), nameof(Main.hasFocus)))) {
					return;
				}

				c.EmitDelegate(() => RunInBackground);
				c.Emit(OpCodes.Or);
				#endregion

			}
			catch (Exception e) {
				Logger.Error(e.Message);
				return;
			}
		}

		public override void Unload()
		{
			smartStackButtonTextures = null;
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
			ToggleRunInBackgroundHotkey =
			SwapArmorVanityHotkey =
			SwapHotbarHotkey =
			CyclingQuickMountHotkey =
			HoldMountHotkey =
			SwitchFrameSkipModeHotkey = null;
		}

		public static bool CanPauseGame(On_Main.orig_CanPauseGame orig)
		{
			if (RunInBackground)
				return false;

			return orig();
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


		public void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
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
					SoundEngine.PlaySound(SoundID.MenuTick);
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

	public class HelpfulHotkeysSystems : ModSystem
	{
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) => ModContent.GetInstance<HelpfulHotkeys>().ModifyInterfaceLayers(layers);
	}
}
