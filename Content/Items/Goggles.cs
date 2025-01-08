
using npcKellogg.Content.NPCs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace npcKellogg.Content.Items
{
    public class Goggles : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 20;
            Item.value = 114514;
            Item.rare = 8;
            Item.useAnimation = 30;
            Item.useTime = 30;
            // 手持
            Item.useStyle = ItemUseStyleID.HoldUp;
            // 是否可消耗
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            // 当世界上不存在该boss才可以召唤
            return !NPC.AnyNPCs(ModContent.NPCType<KBoss>());

        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {

                SoundEngine.PlaySound(SoundID.Roar, player.position);//播放吼叫音效
                int type = ModContent.NPCType<KBoss>();
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    //生成Boss
                    NPC.SpawnOnPlayer(player.whoAmI, type);
                }
                else
                {
                    //发包，用来联机同步
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: type);
                }
            }
            return true;
        }
        // 配方
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Goggles, 1);
            recipe.AddIngredient(ItemID.LunarOre, 2);
            // 工匠作坊
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }

    }
}