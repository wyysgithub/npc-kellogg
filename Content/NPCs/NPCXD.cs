
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using npcKellogg.Content.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Creative;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

// 这里应该是文件路径
namespace npcKellogg.Content.NPCs
{

    [AutoloadHead]
    public class NPCXD : ModNPC
    {

        //这里是拿一个原版npc的id过来，后面很多数据可以直接复用他的
        int npcID = NPCID.Guide;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("测试小伙");

            // base.SetStaticDefaults();
            // 总帧数
            Main.npcFrameCount[Type] = Main.npcFrameCount[npcID];


            NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[npcID];
            //攻击帧的数量，取决于你的NPC属于哪种攻击类型，如何填写见上文的分类讲解
            NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[npcID];
            //NPC的攻击方式，同样取决于你的NPC属于哪种攻击类型，投掷型填0，远程型填1，魔法型填2，近战型填3，
            //如果是宠物没有攻击手段那么这条将不产生影响
            NPCID.Sets.AttackType[Type] = NPCID.Sets.AttackType[npcID];
            //NPC的帽子位置中Y坐标的偏移量，这里特指派对帽，
            //当你觉得帽子戴的太高或太低时使用这个做调整（所以为什么不给个X的）         
            NPCID.Sets.HatOffsetY[Type] = NPCID.Sets.HatOffsetY[npcID];
            //这个名字比较抽象，可以理解为 [记录了NPC的某些帧带来的身体起伏量的数组] 的索引值，
            //而这个数组的名字叫 NPCID.Sets.TownNPCsFramingGroups ，详情请在源码的NPCID.cs与Main.cs内进行搜索。
            //举个例子：你应该注意到了派对帽或是机械师背后的扳手在NPC走动时是会不断起伏的，靠的就是用这个进行调整，
            //所以说在画帧图时最好比着原版NPC的帧图进行绘制，方便各种数据调用
            //补充：这个属性似乎是针对城镇NPC的。
            NPCID.Sets.NPCFramingGroup[Type] = NPCID.Sets.NPCFramingGroup[npcID];
            //魔法型NPC在攻击时产生的魔法光环的颜色，如果NPCID.Sets.AttackType不为2那就不会产生光环
            //如果NPCID.Sets.AttackType为2那么默认为白色
            NPCID.Sets.MagicAuraColor[Type] = Color.White;
            //NPC的单次攻击持续时间，如果你的NPC需要持续施法进行攻击可以把这里设置的很长，
            //比如树妖的这个值就高达600
            //补充说明一点：如果你的NPC的AttackType为3即近战型，
            //这里最好选择套用，因为近战型NPC的单次攻击时间是固定的
            NPCID.Sets.AttackTime[Type] = NPCID.Sets.AttackTime[npcID];
            //NPC的危险检测范围，以像素为单位，这个似乎是半径
            NPCID.Sets.DangerDetectRange[Type] = 500;
            //NPC在遭遇敌人时发动攻击的概率，如果为0则该NPC不会进行攻击（待验证）
            //遇到危险时，该NPC在可以进攻的情况下每帧有 1 / (NPCID.Sets.AttackAverageChance * 2) 的概率发动攻击
            //注：每帧都判定
            NPCID.Sets.AttackAverageChance[Type] = 10;


            //图鉴设置部分
            //将该NPC划定为城镇NPC分类
            NPCID.Sets.TownNPCBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                //为NPC设置图鉴展示状态，赋予其Velocity即可展现出行走姿态
                Velocity = 1f,
            };
            //添加信息至图鉴
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
            //设置对邻居和环境的喜恶，也就是幸福度设置
            //幸福度相关对话需要写在hjson里，见下文所讲
            NPC.Happiness
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Hate)//憎恶丛林环境
                .SetBiomeAffection<UndergroundBiome>(AffectionLevel.Dislike)//讨厌地下环境
                .SetBiomeAffection<SnowBiome>(AffectionLevel.Like)//喜欢雪地环境
                .SetBiomeAffection<OceanBiome>(AffectionLevel.Love)//最爱海洋环境
                .SetNPCAffection(NPCID.Angler, AffectionLevel.Dislike)//讨厌与渔夫做邻居
                .SetNPCAffection(NPCID.Guide, AffectionLevel.Like);//喜欢与向导做邻居
                                                                   //邻居的喜好级别和环境的AffectionLevel是一样的

        }

        public override void SetDefaults()
        {
            //判断该NPC是否为城镇NPC，决定了这个NPC是否拥有幸福度对话，是否可以对话以及是否会被地图保存
            //当然以上这些属性也可以靠其他的方式开启或关闭，我们日后再说
            NPC.townNPC = true;
            //该NPC为友好NPC，不会被友方弹幕伤害且会被敌对NPC伤害
            NPC.friendly = true;
            //碰撞箱宽，不做过多解释，此处为标准城镇NPC数据
            NPC.width = 18;
            //碰撞箱高，不做过多解释，此处为标准城镇NPC数据
            NPC.height = 40;
            //套用原版城镇NPC的AIStyle，这样我们就不用自己费劲写AI了，
            //同时根据我以往的观测结果发现这个属性也决定了NPC是否会出现在入住列表里，还请大佬求证
            NPC.aiStyle = NPCAIStyleID.Passive;
            //伤害，由于城镇NPC没有体术所以这里特指弹幕伤害（虽然弹幕伤害也是单独设置的所以理论上这个可以不写？）
            NPC.damage = 10;
            //防御力
            NPC.defense = 15;
            //最大生命值，此处为标准城镇NPC数据
            NPC.lifeMax = 250;
            //受击音效
            NPC.HitSound = SoundID.NPCHit1;
            //死亡音效
            NPC.DeathSound = SoundID.NPCDeath1;
            //抗击退性，数据越小抗性越高
            NPC.knockBackResist = 0.5f;
            //模仿的动画类型，这样就不用自己费劲写动画播放了
            AnimationType = NPCID.Merchant;
        }

        //设置图鉴内信息
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                    //设置所属环境，一般填写他最喜爱的环境
                    BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Ocean,
                    //图鉴内描述
                    new FlavorTextBestiaryInfoElement("一个人见人爱，花见花开，月总见了直接裂开的测试小伙")
            });
        }

        //设置姓名
        public override List<string> SetNPCNameList()
        {
            //所有可能出现的名字
            return new List<string>() {
                "[2025]",
                "[天顶空岛]",
                "[灾厄炼狱]"
            };
        }

        //决定NPC会被哪座雕像传送
        public override bool CanGoToStatue(bool toKingStatue)
        {
            //可以被国王雕像传送
            return toKingStatue;
        }

        //当NPC在被雕像传送时会发生什么
        public override void OnGoToStatue(bool toKingStatue)
        {
            //在左下角弹出一句话
            if (toKingStatue)
                Main.NewText("测试小伙受到了国王雕像的召唤！");
            else
                Main.NewText("测试小伙受到了王后雕像的召唤！");
        }



        //设置对话
        public override string GetChat()
        {
            //声明一个int类型变量，查找一个whoAmI最靠前的、种类为向导的NPC并返回他的whoAmI
            int guide = NPC.FindFirstNPC(NPCID.Guide);
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                //当血月和日食都没有发生时
                if (!Main.bloodMoon && !Main.eclipse)
                {
                    //无家可归时
                    if (NPC.homeless)
                    {
                        chat.Add("我想我们已经认识对方了，所以....能给个住的地方吗？");
                    }
                    else
                    {
                        //自我介绍，NPC.FullName就是带上称呼的姓名，比如“测试小伙Den-o”
                        chat.Add($"你好！我是{NPC.FullName}");
                        //当查找到向导NPC时
                        if (guide != -1)
                        {
                            //GivenName上面有提
                            chat.Add($"{Main.npc[guide].GivenName}博学多识，是个人才。");
                        }
                        //正在举行派对时
                        if (BirthdayParty.PartyIsUp)
                        {
                            chat.Add("老兄，我TMD最喜欢派对了！");
                        }
                    }
                }
                //日食时
                if (Main.eclipse)
                {
                    chat.Add("我相信你可以挺过去，让我TMD没事就行！");
                }
                //血月时
                if (Main.bloodMoon)
                {
                    chat.Add("TMD！不要血月！！！");
                }
                return chat;
            }
        }

        //设置对话按钮的文本
        public override void SetChatButtons(ref string button, ref string button2)
        {
            //直接引用原版的“商店”文本
            button = Language.GetTextValue("LegacyInterface.28");
            //设置第二个按钮
            button2 = "打招呼";
        }

        //设置当对话按钮被摁下时会发生什么

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            //当第一个按钮被按下时
            if (firstButton)
            {
                //打开商店
                shop = "Shop";
            }
            //如果是第二个按钮被按下时
            else
            {
                //出现一句对话，使用这个属性可以直接设置NPC要说的话。
                Main.npcChatText = "必胜克给你祝福！";
            }
        }

        public override void AddShops()
        {
            new NPCShop(Type)
                .Add<回血枪>()
                .Add<回血杖>()
                .Add<老克的音响>()
                .Register();
        }


        //设置商店内容
        // public override void SetupShop(Chest shop, ref int nextSlot)
        // {
        //     //将小型血瓶加入商店内
        //     shop.item[nextSlot].SetDefaults(ItemID.LesserHealingPotion);
        //     //进入下一个物品栏
        //     nextSlot++;
        //     //将小型魔瓶加入商店内
        //     shop.item[nextSlot].SetDefaults(ItemID.LesserManaPotion);
        //     nextSlot++;
        //     //将土块加入商店内
        //     shop.item[nextSlot].SetDefaults(ItemID.DirtBlock);
        //     //以一个金币的价格卖出（倒爷说的就是你是吧）
        //     //如果不对这个进行设置，那么商品的售价默认按照该物品的Item.value设置
        //     shop.item[nextSlot].value = Item.buyPrice(0, 1, 0, 0);
        //     nextSlot++;
        //     //当 击败克苏鲁之眼 且 处于专家模式 时
        //     if (NPC.downedBoss1 && Main.expertMode)
        //     {
        //         //将克苏鲁之盾加入商店内
        //         shop.item[nextSlot].SetDefaults(ItemID.EoCShield);
        //         nextSlot++;
        //     }
        // }


    }
}