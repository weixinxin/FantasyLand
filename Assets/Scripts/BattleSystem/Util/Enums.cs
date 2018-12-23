using System.Xml.Serialization;
namespace BattleSystem
{

    public enum BuffEffectType
    {
        //物理伤害
        [XmlEnum("0")]
        kPhysicalDamage = 0,
        //魔法伤害
        [XmlEnum("1")]
        kMagicDamage = 1,
        //真实伤害
        [XmlEnum("2")]
        kTrueDamage =2,
        //治疗
        [XmlEnum("3")]
        kHeal =3,
        //移动加速
        [XmlEnum("4")]
        kSpeedUp = 4,
        //移动减速
        [XmlEnum("5")]
        kSlowDown = 5,
        //减少技能CD
        //kCDReduction,
        //增加攻击力
        [XmlEnum("6")]
        kIncreaseATK = 6,
        //减少攻击力
        [XmlEnum("7")]
        kDecreaseATK = 7,
        //增加攻击速度
        [XmlEnum("8")]
        kIncreaseAttackSpeed = 8,
        //减少攻击速度
        [XmlEnum("9")]
        kDecreaseAttackSpeed = 9,
        //增加攻击距离
        [XmlEnum("10")]
        kExtendAttackRange =10,
        //缩小攻击距离
        [XmlEnum("11")]
        kReduceAttackRange = 11,
        //增加视野距离
        [XmlEnum("12")]
        kExtendVisualRange = 12,
        //缩小视野距离
        [XmlEnum("13")]
        kReduceVisualRange =13,
        //无法移动
        [XmlEnum("14")]
        kUnmovable = 14,
        //无法施法
        [XmlEnum("15")]
        kUnableCast = 15,
        //物理伤害免疫
        [XmlEnum("16")]
        kPhysicalDamageImmunity = 16,
        //魔法伤害免疫
        [XmlEnum("17")]
        kMagicDamageImmunity = 17,
        //无法攻击
        [XmlEnum("18")]
        kUnableAttack = 18,
        //不被选中
        [XmlEnum("19")]
        kNotarget = 19,
        //攻击失效
        [XmlEnum("20")]
        kAttackMiss = 20,
        //不死
        [XmlEnum("21")]
        kDeathless = 21,
        //负面效果免疫
        [XmlEnum("22")]
        kNegativeEffectImmunity = 22,
        //清除负面效果
        [XmlEnum("23")]
        kCleanse = 23,
    }

    public enum DamageType
    {
        //物理伤害  
        [XmlEnum("0")]
        kPhysical = 0,
        //魔法伤害
        [XmlEnum("1")]
        kMagic = 1,
        //真实伤害
        [XmlEnum("2")]
        kTrue = 2,
    }
    public enum EventCode
    {
        //单位死亡
        UnitDead,
        //即将造成伤害

    }

    public enum TargetFilter
    {
        [XmlEnum("0")]
        kSelf = 0,//自己
        [XmlEnum("1")]
        kNearestEnemy = 1,//最近的敌人
        [XmlEnum("2")]
        kNearestAlly = 2,//最近的盟友
        [XmlEnum("3")]
        kLowestHPEnemy = 3,//血量最低的敌人
        [XmlEnum("4")]
        kLowestHPAlly = 4,//血量最低的盟友
    }

    public enum TargetRange
    {
        [XmlEnum("0")]
        kBattlefield = 0,//整个战场
        [XmlEnum("1")]
        kCirclefield = 1,//半径范围内
    }

    public enum RegionType
    {
        [XmlEnum("0")]
        kCircle = 0,//圆形区域
        [XmlEnum("1")]
        kRect = 1,//矩形区域
        [XmlEnum("2")]
        kSector = 2,//扇形区域
    }

    public enum AoeFilter
    {
        [XmlEnum("0")]
        kEnemy = 0,
        [XmlEnum("1")]
        kAlly = 1,
        [XmlEnum("2")]
        kAll = 2,
    }


    public enum BulletType
    {
        [XmlEnum("0")]
        kCoordBullet = 0,
        [XmlEnum("1")]
        kLineBullet = 1,
        [XmlEnum("2")]
        kPenteralBullet = 2,
        [XmlEnum("3")]
        kReturnBullet = 3,
        [XmlEnum("4")]
        kTrackBullet = 4,
    }

    /// <summary>
    /// 相同buff的叠加策略
    /// </summary>
    public enum OverlayTactics
    {
        [XmlEnum("0")]
        kCoexist = 0,//多个共存
        [XmlEnum("1")]
        kSingleton = 1,//不允许叠加
        [XmlEnum("2")]
        kReplace = 2,//以新代旧
        [XmlEnum("3")]
        kAddTime = 3,//增加持续时间
    }
}

