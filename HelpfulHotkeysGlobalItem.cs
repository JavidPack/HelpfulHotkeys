/*
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace HelpfulHotkeys
{
	internal class HelpfulHotkeysGlobalItem : GlobalItem
	{
		public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
			var indexes = HelpfulHotkeysClientConfig.Instance.SwapArmorInventorySlots;
			foreach (var index in indexes) {
				if(item == Main.LocalPlayer.inventory[index]) {
					// position is item draw position, not slot position, and this method not called for slots with no items....
					spriteBatch.Draw(TextureAssets.InventoryBack6.Value, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
					break;
				}
			}
			return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
		}

		public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
			var indexes = HelpfulHotkeysClientConfig.Instance.SwapArmorInventorySlots;
			foreach (var index in indexes) {
				// Doesn't work, item is clone.
				if (item == Main.LocalPlayer.inventory[index]) {
					tooltips.Add(new TooltipLine(Mod, "HelpfulHotkeys:SwapArmorInventorySlotsRemider", "Use Swap Armor with Inventory Slots hotkey to swap this slot with equipped armor"));
					break;
				}
			}
		}
	}
}
*/