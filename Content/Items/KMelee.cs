using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using UtfUnknown.Core.Models.SingleByte.Turkish;

namespace npcKellogg.Content.Items
{ 
	// This is a basic item template.
	// Please see tModLoader's ExampleMod for every other example:
	// https://github.com/tModLoader/tModLoader/tree/stable/ExampleMod
	public class KMelee : ModItem
	{
		// The Display Name and Tooltip of this item can be edited in the 'Localization/en-US_Mods.npcKellogg.hjson' file.
		public override void SetDefaults()
		{

			Item.width = 40;
			Item.height = 40;

			// 伤害
			Item.damage = 1000;
			// 决定了这个武器的伤害类型
			// Default 代表无属性（也就是不吃任何加成）
			// Generic 代表全属性（也就是全部加成都吃）所谓1.3的allDamage就是它了
			// MagicSummonHybrid 代表什么我不知道，但是可以同时吃到魔法和召唤加成
			// MeleeNoSpeed 代表近战，但是不吃攻速加成
			// Melee 代表近战
			// Ranged 代表远程
			// Magic 代表马猴烧酒，不，魔法
			// Summon 代表召唤
			// SummonMeleeSpeed 代表额......看看鞭子吧，可以吃到近战和召唤加成
			// Throwing 代表投掷（没错虽然1.4没了投掷武器，全给远程了，但是这个伤害类型还在！）
			Item.DamageType = DamageClass.Melee;
			// 击退 0~20f
			Item.knockBack = 6f;
			
			// 暴击率 会加 4%（默认）
			Item.crit = 50;

			// 这是控制贴图是否造成伤害，默认是造成伤害
			// 但如果你是枪类的远程武器，你不希望拿着枪敲人的话就要把它设置为true
			Item.noMelee = false; // 这是一把剑所以贴图是造成伤害的

			// 发射弹幕 ProjectileID.TerraBeam是泰拉剑气
			Item.shoot = ProjectileID.TerraBeam;
			Item.shootSpeed = 6f;



// 这是控制使用时是否显示贴图，默认是有的
Item.noUseGraphic = false;
// 吸血飞刀这个就是true，它使用时不显示贴图
// 假如这是一个法杖类型，不写默认false，这里就用到物品Type了
Item.staff[Type] = false; // enn，这是把剑
// 一般来说，法杖类武器会使用Shoot的那个使用方式，但它的贴图不像枪一样是水平朝向而是向右上倾斜
// 让它变成true就会导致使用时贴图再转45度，变成法杖尖端朝着射击方向
// 这是使用消耗的魔力值，不写就不消耗
Item.mana = 0;


			// 使用速度和使用动画持续时间！
			Item.useTime = 10;
			Item.useAnimation = 10;

			// 使用类型，这个值决定了武器使用时到底是按什么样的动画播放
			// 0 或 None 代表......字面意思，就是啥都没有！你写了之后甚至无法使用！
			// 1 或 Swing 代表挥动，也就是剑类武器！
			// 2 或 EatFood 代表像食物一样，拥有物品，手持，放在盘子上三帧的贴图，具体可见exmod里头的🥧（派）https://github.com/Cyrillya/Example-Mod-Zh-Project/blob/master/Content/Items/Consumables/ExampleFoodItem.cs
			// 3 或 Thrust 代表像1.3的同志短剑一样刺x 出去（也就是朝左或右刺出）（如果你想要写全方位刺出的剑，那你还是得看exmod）
			// 4 或 HoldUp 唔，这个一般不是用在武器上的，想象一下生命水晶使用的时候的动作
			// 5 或 Shoot 手持，枪、弓、法杖类武器的动作，用途最广
			// 6 或 DrinkLong 代表直接猛喝，感兴趣可以自己看看，挺好玩的（
			// 7 或 DrinkOld 代表1.3的喝药水动作
			// 8 或 GolfPlay 代表高尔夫球杆的动作
			// 9 或 DrinkLiquid 代表1.4的喝药水动作，相比1.3的来说，这个动作的手臂更加流畅，持握位置会在瓶口
			// 10 或 HiddenAnimation 代表使用时无动画
			// 11 或 MowTheLawn 代表割草机的动作，神奇，这玩意还有单独的动作
			// 12 或 Guitar 代表常春藤、雨之歌等物品的动作，对这玩意也是单独的动作（爱抚剑ing
			// 13 或 Rapier 代表标尺、星光等武器的动作
			// 14 或 RaiseLamp 代表夜光的动作，好吧这也单独写一个动作的吗？话说这玩意翻译过来叫吊灯......夜光大吊灯（bushi
			// Item.useStyle = 1; 请不要写魔法值
			Item.useStyle = ItemUseStyleID.Swing;
			// 自动挥舞
			Item.autoReuse = true;
			// 是否能转身（点名火山不能转向问题！）
			Item.useTurn = true;
			// 体积缩放，所以想让武器变大是调这里而不是上面的width
			Item.scale = 3f;

			Item.value = Item.buyPrice(silver: 1);
			// 铂金币，金币，银币和铜币
			Item.value = Item.sellPrice(0,1,60,0);

			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;

			// 最大堆叠数量
			// Item.maxStack = Item.CommonMaxStack;

		}

        // 重写shoot方法
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Projectile p = Projectile.NewProjectileDirect(source, position, -velocity, ProjectileID.WaterGun, damage, knockback, player.whoAmI);
            // return base.Shoot(player, source, position, velocity, type, damage, knockback);
			return true;
        }

        public override void SetStaticDefaults(){
			Item.ResearchUnlockCount = 2;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			// recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}

		// 重写近战武器特性
        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
			// 粒子特性
			Dust d = Dust.NewDustDirect(hitbox.TopLeft(),hitbox.Width,hitbox.Height,DustID.Ichor);
			d.velocity *= 0;
			d.noGravity = true;
            // base.MeleeEffects(player, hitbox);
        }
    }
}
