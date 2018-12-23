using BattleSystem.SkillModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public abstract class BulletBase : IMovable
    {

        private static int s_id = 1;
        protected static int id
        {
            get
            {
                return s_id++;
            }
        }

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public int ID { get;protected set; }
        /// <summary>
        /// 发射者
        /// </summary>
        public UnitBase Shooter { get; protected set; }


        /// <summary>
        /// 坐标
        /// </summary>
        public vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
#if DEBUG
                if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z) || float.IsInfinity(value.x) || float.IsInfinity(value.y) || float.IsInfinity(value.z))
                {
                    Logger.LogErrorFormat("error: {0},{1},{2}", value.x, value.y, value.z);
                    return;
                }
#endif
                _position = value;
            }
        }
        private vector3 _position = vector3.zero;


        /// <summary>
        /// 移动速度
        /// </summary>
        public float speed { get; set; }
        /// <summary>
        /// 加速度
        /// </summary>
        public float acceleration { get;  set; }

        /// <summary>
        /// 更新子弹状态
        /// </summary>
        /// <param name="dt">帧间隔时间</param>
        /// <returns>是否需要移除子弹</returns>
        public abstract bool Update(float dt);

        public bool isAttack { get; protected set; }
       


        protected int[] buffs = null;
        public void InitBuff(int[] buffs)
        {
            this.buffs = buffs;
        }

        protected float aoeRadius = 0;

        protected List<SkillModule.BuffEmitter> emitters = null;
        protected float duration;
        protected float interval;
        public void InitAoeFile(float radius, float duration, float interval, List<SkillModule.BuffEmitter> emitters)
        {
            this.duration = duration;
            this.interval = interval;
            this.aoeRadius = radius;
            for (int i = 0; i < emitters.Count; ++i)
            {
                emitters[i].Caster = this.Shooter;
            }
            this.emitters = emitters;
        }

        protected int damage = 0;
        protected DamageType damageType;
        public virtual void InitDamage(int value, DamageType dt, bool isAttack)
        {
            damage = value;
            damageType = dt;
            this.isAttack = isAttack;
        }


        public float radius
        {
            get;
            set;
        }
    }
}
