using Terraria.ModLoader;
using Terraria;
using System;
using Terraria.DataStructures;
using System.ComponentModel;
using Newtonsoft.Json;
using Terraria.ModLoader.Config;
using System.Runtime.Serialization;
using Terraria.ID;

namespace HelpfulHotkeys
{
#pragma warning disable 0649
	[Label("Config")]
	class HelpfulHotkeysClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static HelpfulHotkeysClientConfig Instance;

		[Label("Quick Use Config Item")]
		[Tooltip("Customize the Quick Use Config Item here\nThe specified item can be quickly used from your inventory by pressing the hotkey")]
		public ItemDefinition QuickUseConfigItem { get; set; } = new ItemDefinition(ItemID.None);

		[Label("Show Developer Info")]
		[Tooltip("Enhances the Query Mod Origin hotkey to show internal names of modded items.\nVanilla ID values will also be shown.")]
		[DefaultValue(false)]
		public bool ShowDeveloperInfoQueryModOrigin { get; set; }
	}
#pragma warning restore 0649
}
