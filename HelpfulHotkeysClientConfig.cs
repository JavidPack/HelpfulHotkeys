using Terraria.ModLoader;
using Terraria;
using System;
using Terraria.DataStructures;
using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
using System.Runtime.Serialization;
using Terraria.ID;
using System.Collections.Generic;

namespace HelpfulHotkeys
{
#pragma warning disable 0649
	[Label("Config")]
	class HelpfulHotkeysClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static HelpfulHotkeysClientConfig Instance;

		[Label("Quick Use Item 11-19 Hotkeys")]
		[Tooltip("Enable to allow these hotkeys. Keep disabled to keep keybinding menu clean.")]
		[ReloadRequired]
		public bool EnableQuickUseItems11to19;

		[Label("Quick Use Config Item")]
		[Tooltip("Customize the Quick Use Config Item here\nThe specified item can be quickly used from your inventory by pressing the hotkey")]
		public ItemDefinition QuickUseConfigItem { get; set; } = new ItemDefinition(ItemID.None);

		[Label("Swap Armor with Inventory Slots")]
		[Tooltip("Customize the Swap Armor with Inventory slots here\nCount from 0. Use value from 0 to 49.\nDefault values correspond to lower right slots in the inventory.")]
		public List<int> SwapArmorInventorySlots = new List<int>() { 29, 39, 49 };

		[Label("Show Developer Info")]
		[Tooltip("Enhances the Query Mod Origin hotkey to show internal names of Modded items.\nVanilla ID values will also be shown.")]
		[DefaultValue(false)]
		public bool ShowDeveloperInfoQueryModOrigin { get; set; }

		[Label("Disable Double Tap Dash Behavior")]
		[Tooltip("Enable this to prevent the game from recognizing double taps for dashing.\nUse this if you intend to only use the dash hotkey.")]
		public bool DashHotkeyDisablesDoubleTap;
	}
#pragma warning restore 0649
}
