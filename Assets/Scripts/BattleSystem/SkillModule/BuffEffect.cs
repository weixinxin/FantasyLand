using System.Collections.Generic;
using System.Diagnostics;
using BattleSystem.ObjectModule;
using BattleSystem.Config;
namespace BattleSystem.SkillModule
{
    /// <summary>
    /// buff效果
    /// </summary>
    public class BuffEffect
    {
        /// <summary>
        /// 效果类型
        /// </summary>
        protected BuffEffectType Type;

        /// <summary>
        /// 是否永久效果
        /// </summary>
        protected bool isPermanent;

        /// <summary>
        /// 基础值变化
        /// </summary>
        protected int BaseDelta;

        /// <summary>
        /// 当前值变化
        /// </summary>
        protected int CurDelta;

        /// <summary>
        /// 当前值的百分比改变
        /// </summary>
        protected float CurPercent;

        /// <summary>
        /// 基础值的百分比改变
        /// </summary>
        protected float BasePercent;


        public BuffEffect(int templateID)
        {
            //根据模板ID获取参数
            var config = ConfigData.BuffEffect.getRow(templateID);
            this.Type = config.BuffEffectType;
            this.isPermanent = config.isPermanent;
            this.BaseDelta = config.BaseDelta;
            this.BasePercent = config.BasePercent;
            this.CurDelta = config.CurDelta;
            this.CurPercent = config.CurPercent;
        }

        private void ModifyAttribute(Attribute attribute,bool nagative)
        {
            int sign = nagative ? -1 : 1;
            attribute.BaseModify(BaseDelta * sign);
            attribute.CurModify(CurDelta * sign);
            attribute.BasePercentageModify(BasePercent * sign);
            attribute.CurPercentageModify(CurPercent * sign);
        }

        private void ClearModifyAttribute(Attribute attribute, bool nagative)
        {
            if (!isPermanent)
            {
                int sign = nagative ? -1 : 1;
                attribute.RemoveBase(BaseDelta * sign);
                attribute.RemoveCur(CurDelta * sign);
                attribute.RemoveBasePercentage(BasePercent * sign);
                attribute.RemoveCurPercentage(CurPercent * sign);
            }
        }

        private void DoDamage(UnitBase Owner, UnitBase assailant, DamageType dt)
        {
            int delta = (int)(CurDelta + Owner.HP * CurPercent + Owner.MaxHP * BasePercent + (Owner.MaxHP - Owner.HP) * BaseDelta * 0.01f);
            Owner.LostHP(delta, assailant, dt, false);
            
        }
        private void DoHeal(UnitBase Owner, UnitBase healer)
        {
            int delta = (int)(CurDelta + Owner.HP * CurPercent + Owner.MaxHP * BasePercent + (Owner.MaxHP - Owner.HP) * BaseDelta * 0.01f);
            Owner.AddHP(delta, healer);
        }
        /// <summary>
        /// 应用效果
        /// </summary>
        /// <returns>是否需要移除</returns>
        public bool Apply(UnitBase Owner,UnitBase Caster)
        {
            //永久效果不用移除(伤害治疗都是永久效果)
            bool res = !isPermanent;
            switch(Type)
            {
                case BuffEffectType.kAttackMiss:
                case BuffEffectType.kMagicDamageImmunity:
                case BuffEffectType.kNotarget:
                case BuffEffectType.kPhysicalDamageImmunity:
                case BuffEffectType.kUnableAttack:
                case BuffEffectType.kUnableCast:
                case BuffEffectType.kUnmovable:
                case BuffEffectType.kDeathless:
                    Owner.AddState(Type);
                    break;
                case BuffEffectType.kPhysicalDamage:
                    DoDamage(Owner, Caster, DamageType.kPhysical);
                    res = false;
                    break;
                case BuffEffectType.kMagicDamage:
                    DoDamage(Owner, Caster, DamageType.kMagic);
                    res = false;
                    break;
                case BuffEffectType.kTrueDamage:
                    //直接扣血
                    DoDamage(Owner,Caster, DamageType.kTrue);
                    res = false;
                    break;
                case BuffEffectType.kHeal:
                    //回血
                    DoHeal(Owner, Caster);
                    res = false;
                    break;
                case BuffEffectType.kSpeedUp:
                    //移动加速
                    ModifyAttribute(Owner.MoveSpeed,false);
                    break;
                case BuffEffectType.kSlowDown:
                    //移动减速
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.MoveSpeed, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kIncreaseATK:
                    //增加攻击力
                    ModifyAttribute(Owner.ATK, false);
                    break;
                case BuffEffectType.kDecreaseATK:
                    //减少攻击力
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.ATK, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kIncreaseAttackSpeed:
                    //增加攻击速度
                    ModifyAttribute(Owner.AttackDuration, false);
                    break;
                case BuffEffectType.kDecreaseAttackSpeed:
                    //减少攻击速度
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.AttackDuration, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kExtendAttackRange:
                    //增加攻击距离
                    ModifyAttribute(Owner.AttackRange, false);
                    break;
                case BuffEffectType.kReduceAttackRange:
                    //缩小攻击距离
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.AttackRange, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kExtendVisualRange:
                    //增加视野距离
                    ModifyAttribute(Owner.VisualRange, false);
                    break;
                case BuffEffectType.kReduceVisualRange:
                    //缩小视野距离
                    if (!Owner.isNegativeEffectImmunity)
                        ModifyAttribute(Owner.VisualRange, true);
                    else
                        res = false;
                    break;
                case BuffEffectType.kCleanse:
                    //清除负面效果
                    if (isPermanent)
                    {
                        //永久效果直接移除目标身上的效果
                        for (int i = Owner.Buffs.Count - 1; i >= 0; --i)
                        {
                            Buff buff = Owner.Buffs[i];
                            if (buff.isClearable && buff.isNegative)
                            {
                                buff.Clear();
                                Owner.Buffs.RemoveAt(i);
                            }
                        }
                    }
                    else
                    {
                        //非永久效果则暂时让对方负面效果失效
                        for (int i = Owner.Buffs.Count - 1; i >= 0; --i)
                        {
                            Buff buff = Owner.Buffs[i];
                            if (buff.isClearable && buff.isNegative)
                            {
                                buff.Pause();
                            }
                        }
                    }
                    break;
                default:
                    Trace.Assert(false, "未实现的BuffEffect" + Type.ToString());
                    break;

            }
            return res;
        }
        
        /// <summary>
        /// 清除效果
        /// </summary>
        public void Clear(UnitBase Owner)
        {
            
            switch(Type)
            {
                case BuffEffectType.kAttackMiss:
                case BuffEffectType.kMagicDamageImmunity:
                case BuffEffectType.kNotarget:
                case BuffEffectType.kPhysicalDamageImmunity:
                case BuffEffectType.kUnableAttack:
                case BuffEffectType.kUnableCast:
                case BuffEffectType.kUnmovable:
                case BuffEffectType.kDeathless:
                    Owner.RemoveState(Type);
                    break;
                case BuffEffectType.kPhysicalDamage:
                case BuffEffectType.kMagicDamage: 
                case BuffEffectType.kTrueDamage:
                case BuffEffectType.kHeal:
                    break;
                case BuffEffectType.kSpeedUp:
                    //移动加速
                    ClearModifyAttribute(Owner.MoveSpeed, false);
                    break;
                case BuffEffectType.kSlowDown:
                    //移动减速
                    ClearModifyAttribute(Owner.MoveSpeed, true);
                    break;
                case BuffEffectType.kIncreaseATK:
                    //增加攻击力
                    ClearModifyAttribute(Owner.ATK, false);
                    break;
                case BuffEffectType.kDecreaseATK:
                    //减少攻击力
                    ClearModifyAttribute(Owner.ATK, true);
                    break;
                case BuffEffectType.kIncreaseAttackSpeed:
                    //增加攻击速度
                    ClearModifyAttribute(Owner.AttackDuration, false);
                    break;
                case BuffEffectType.kDecreaseAttackSpeed:
                    //减少攻击速度
                    ClearModifyAttribute(Owner.AttackDuration, true);
                    break;
                case BuffEffectType.kExtendAttackRange:
                    //增加攻击距离
                    ClearModifyAttribute(Owner.AttackRange, false);
                    break;
                case BuffEffectType.kReduceAttackRange:
                    //缩小攻击距离
                    ClearModifyAttribute(Owner.AttackRange, true);
                    break;
                case BuffEffectType.kExtendVisualRange:
                    //增加视野距离
                    ClearModifyAttribute(Owner.VisualRange, false);
                    break;
                case BuffEffectType.kReduceVisualRange:
                    //缩小视野距离
                    ClearModifyAttribute(Owner.VisualRange, true);
                    break;
                case BuffEffectType.kCleanse:
                    for (int i = Owner.Buffs.Count - 1; i >= 0; --i)
                    {
                        Buff buff = Owner.Buffs[i];
                        if (buff.isClearable && buff.isNegative)
                        {
                            buff.Resume();
                        }
                    }
                    break;
                default:
                    Trace.Assert(false, "未实现的BuffEffect" + Type.ToString());
                    break;

            }

        }
    }
}
