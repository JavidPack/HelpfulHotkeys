﻿using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.UI.Chat;
using System;
using System.Reflection;
using Terraria.UI;

namespace HelpfulHotkeys
{
	public class HelpfulHotkeys : Mod
	{
		private Texture2D[] smartStackButtonTextures;
		private bool smartStackButtonHover;
		private float smartStackButtonScale;
		private bool smartStackButtonHovered;
		
		internal static List<int> RecallItems;

		internal static ModHotKey AutoRecallHotKey;
		internal static ModHotKey AutoTorchHotKey;
		internal static ModHotKey CycleAmmoHotKey;
		internal static ModHotKey QuickStackToChestsHotKey;
		internal static ModHotKey SmartQuickStackToChestsHotKey;
		internal static ModHotKey QuickUseItem20Hotkey;
		internal static ModHotKey QuickBuffFavoritedOnlyHotkey;
		internal static ModHotKey QueryModOriginHotkey;
		internal static ModHotKey ToggleAutopauseHotkey;
		internal static ModHotKey SwapArmorVanityHotkey;
		internal static ModHotKey CyclingQuickMountHotkey;

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
			QuickUseItem20Hotkey = RegisterHotKey("Quick Use Item #20", "L");
			QuickBuffFavoritedOnlyHotkey = RegisterHotKey("Quick Buff Favorited Only", "B");
			QueryModOriginHotkey = RegisterHotKey("Query Mod Origin", "OemQuestion");
			ToggleAutopauseHotkey = RegisterHotKey("Toggle Autopause", "P");
			SwapArmorVanityHotkey = RegisterHotKey("Swap Armor with Vanity", "");
			CyclingQuickMountHotkey = RegisterHotKey("Cycling Quick Mount", "");

			smartStackButtonTextures = new Texture2D[]
			{
				GetTexture("SmartStack_Off"),
				GetTexture("SmartStack_On")
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
		
		public override object Call(params object[] args)
		{
			string messageType = args[0] as string;
			switch(messageType)
			{
				case "RegisterRecallItem":
					RecallItems.Add(Convert.ToInt32(args[1]));
					return true;
			}
			return base.Call(args);
		}

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
						Main.PlaySound(SoundID.MenuTick);
						smartStackButtonHover = true;
					}

					if (Main.mouseLeft && Main.mouseLeftRelease)
					{
						Main.mouseLeftRelease = false;
						HelpfulHotkeysPlayer modPlayer = Main.LocalPlayer.GetModPlayer<HelpfulHotkeysPlayer>(this);
						modPlayer.smartQuickStack();
						Recipe.FindRecipes();
					}

					Main.player[Main.myPlayer].mouseInterface = true;
				}
				else if (smartStackButtonHover)
				{
					Main.PlaySound(SoundID.MenuTick);
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
				Player player = Main.player[Main.myPlayer];
				int Y = Main.instance.invBottom + 40 + ID * 26;
				float num = smartStackButtonScale;
				string text = "Smart Stack";
				Vector2 vector = Main.fontMouseText.MeasureString(text);
				Color baseColor = new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor) * num;
				baseColor = Color.White * 0.97f * (1f - (255f - (float)Main.mouseTextColor) / 255f * 0.5f);
				baseColor.A = 255;
				int X = 506 + (int)(vector.X * num / 2f);
				ChatManager.DrawColorCodedStringWithShadow(spriteBatch, Main.fontMouseText, text, new Vector2((float)X, (float)Y), baseColor, 0f, vector / 2f, new Vector2(num), -1f, 1.5f);
				vector *= num;

				if (!Utils.FloatIntersect((float)Main.mouseX, (float)Main.mouseY, 0f, 0f, (float)X - vector.X / 2f, (float)Y - vector.Y / 2f, vector.X, vector.Y))
				{
					UpdateHover(false);
					return;
				}
				UpdateHover(true);
				if (!PlayerInput.IgnoreMouseInterface)
				{
					player.mouseInterface = true;
					if (!Main.mouseLeft || !Main.mouseLeftRelease)
					{
						return;
					}
					HelpfulHotkeysPlayer modPlayer = player.GetModPlayer<HelpfulHotkeysPlayer>(this);
					modPlayer.smartQuickStack();
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
					Main.PlaySound(12, -1, -1, 1, 1f, 0f);
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
