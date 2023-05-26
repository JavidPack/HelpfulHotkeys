using System.ComponentModel;
using Terraria.ModLoader.Config;
using Terraria.ID;
using System.Collections.Generic;

namespace HelpfulHotkeys
{
#pragma warning disable 0649
	class HelpfulHotkeysClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static HelpfulHotkeysClientConfig Instance;

		[ReloadRequired]
		public bool EnableQuickUseItems11to19;

		public ItemDefinition QuickUseConfigItem { get; set; } = new ItemDefinition(ItemID.None);

		public List<int> SwapArmorInventorySlots = new List<int>() { 29, 39, 49 };

		[DefaultValue(false)]
		public bool ShowDeveloperInfoQueryModOrigin { get; set; }

		public bool DashHotkeyDisablesDoubleTap;

		public bool DashHotkeyDisabledWhileInChest;
	}
#pragma warning restore 0649
}
