

using Microsoft.Xna.Framework;
using npcKellogg.Content.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace npcKellogg.Content.NPCs
{
    [AutoloadBossHead]
    public class KBoss : ModNPC
    {
        public override void SetDefaults()
        {
            // 总帧数
            Main.npcFrameCount[Type] = 10;
            // NPC.aiStyle = 2;
            NPC.width = 248;
            NPC.height = 188;//这两个代表这个NPC的碰撞箱宽高，以及tr会从你的贴图里扣多大的图
            NPC.damage = 191;
            NPC.lifeMax = 9810;//npc的血量上限
            NPC.defense = 38;
            // NPC.scale = 2.33f;//npc的贴图和碰撞箱的放缩倍率
            NPC.scale = 1f;
            NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit5;//挨打时发出的声音
            NPC.DeathSound = SoundID.NPCDeath7;//趋势时发出的声音
            NPC.value = Item.buyPrice(0, 4, 50, 0);//NPC的爆出来的MONEY的数量，四个空从左到右是铂金，金，银，铜
            NPC.lavaImmune = true;//对岩浆免疫
            NPC.noGravity = true;//不受重力影响。一般BOSS都是无重力的
            NPC.noTileCollide = true;//可穿墙
            NPC.npcSlots = 20; //NPC所占用的NPC数量，在TR世界里，NPC上限是200个，通常，这个用来限制Boss战时敌怪数量，填个10，20什么的
            NPC.boss = true; //将npc设为boss 会掉弱治药水和心，会显示xxx已被击败，会有血条
            // NPC.dontTakeDamage = true;//为true则为无敌，这里的无敌意思是弹幕不会打到npc，并且npc的血条也不会显示了
            // Music = MusicLoader.GetMusicSlot("xxx/Assets/Music/xxx");
            Music = MusicID.Boss1;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("114514");
            // DisplayName.AddTranslation(7, "你是一个一个~");
            // Main.npcFrameCount[NPC.type] = 3;
            NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[] {
            BuffID.Poisoned,
        }
            };
        }

        public override void AI()
        {
            //计时器
            NPC.ai[0]++;
            // 找离得最近的玩家
            NPC.TargetClosest(true);
            Player p = Main.player[NPC.target];
            NPC.rotation = NPC.velocity.ToRotation();

            if (NPC.ai[0] <= 180)
            {
                // 向玩家创过来
                // NPC.velocity = 10 * (p.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                float _z = NPC.ai[0] % 90 + 1;
                if (_z <= 5)
                {

                    
                    Vector2 direction = p.Center - NPC.Center;
                    float distance = direction.Length();

                    // 设置一个最小移动距离阈值
                    float minMoveDistance = 0.2f; 

                    if (distance > minMoveDistance)
                    {
                        // 如果NPC与玩家太接近，则不更新速度,否则，计算归一化的方向并设置速度
                        direction.Normalize();
                        NPC.velocity = 30 * direction;
                
                    }
                    else
                    {
                        // 如果NPC与玩家太接近，则不更新速度
                        NPC.velocity = Vector2.Zero;
                    }

                    // NPC.velocity = 30 * (p.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }
                if (_z > 5)
                {
                    // 让它的速度每帧都 往0 变化 0.05倍
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.05f);
                }
                // if (_z == 60)
                // {
                //     NPC.velocity = Vector2.Zero;
                // }

            }
            if (NPC.ai[0] > 180 && NPC.ai[0] < 360)
            {
                // 原地不动回血
                NPC.velocity = Vector2.Zero;
                if (NPC.life <= NPC.lifeMax - 5)
                {
                    NPC.life += 5;
                }

            }
            if (NPC.ai[0] == 360)
            {
                // 瞬移到玩家头上
                // NPC.position = p.position + new Vector2(0, -300);
                // 计时器归零，一个循环结束
                NPC.ai[0] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                // If this NPC dies, spawn some visuals

                int dustType = 59; // Some blue dust, read the dust guide on the wiki for how to find the perfect dust

                for (int i = 0; i < 20; i++)
                {
                    Vector2 velocity = NPC.velocity + new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    Dust dust = Dust.NewDustPerfect(NPC.Center, dustType, velocity, 26, Color.White, Main.rand.NextFloat(1.5f, 2.4f));

                    dust.noLight = true;
                    dust.noGravity = true;
                    dust.fadeIn = Main.rand.NextFloat(0.3f, 0.8f);
                }
            }
        }

        public override void OnKill()
        {
            // Boss minions typically have a chance to drop an additional heart item in addition to the default chance
            Player closestPlayer = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)];

            if (Main.rand.NextBool(2) && closestPlayer.statLife < closestPlayer.statLifeMax2)
            {
                Item.NewItem(NPC.GetSource_Loot(), NPC.getRect(), ItemID.Heart);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ItemID.StoneBlock, 3, 5, 15));
            npcLoot.Add(notExpertRule);
            npcLoot.Add(ItemDropRule.BossBag(ItemID.EyeOfCthulhuBossBag));
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemID.Goggles));
            npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemID.StoneBlock));
        }
    }
}