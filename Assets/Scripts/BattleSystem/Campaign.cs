using BattleSystem.Config;
using BattleSystem.ObjectModule;
using BattleSystem.SkillModule;
using BattleSystem.SpaceModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem
{
    public class Campaign
    {
        public static Campaign Instance{get; private set; }


        public Campaign()
        {
            if (Instance != null)
                Instance.Destroy();

            Instance = this;
        }
        
        private List<UnitBase> mUnits = new List<UnitBase>();

        private List<UnitBase>[] mClampGroup;


        private List<AoeField> mAoeFields = new List<AoeField>();

        private List<BulletBase> mBullets = new List<BulletBase>();
        public WorldSpace world { get; private set; }

        public bool Init(int campCount)
        {
            var n_world = new WorldSpace();
            n_world.Init(100, 100, 5, 60);

            return Init(campCount, n_world);
        }
        public bool Init(int campCount, WorldSpace world)
        {
            this.world = world;
            mClampGroup = new List<UnitBase>[campCount];
            for (int i = 0; i < campCount;++i)
            {
                mClampGroup[i] = new List<UnitBase>();
            }
            GameTimeElapsed = 0;
            return true;
        }

        public void Update(float dt)
        {
            deltaTime = dt;
            GameTimeElapsed += dt;
            for (int i = mAoeFields.Count - 1; i >= 0; i--)
            {
                if (mAoeFields[i].Update(deltaTime))
                {
                    mAoeFields.RemoveAt(i);
                }
            }
            for(int i = mUnits.Count - 1;i >=0;i--)
            {
                if (mUnits[i].Update(deltaTime))
                {
                    mClampGroup[mUnits[i].CampID].Remove(mUnits[i]);
                    mUnits.RemoveAt(i);
                }
            }
            for (int i = mBullets.Count - 1; i >= 0; i--)
            {

                if (mBullets[i].Update(deltaTime))
                {
                    mBullets.RemoveAt(i);
                }
            }
        }
        public float GameTimeElapsed { get; private set; }
        public float deltaTime { get; private set; }
        public UnitBase AddUnit(int templateID, int campID, int level)
        {
            UnitBase unit = new UnitBase(world,templateID, campID, level);
            mUnits.Add(unit);
            mClampGroup[campID].Add(unit);
            return unit;
        }

        public void AddUnit(UnitBase unit)
        {
            mUnits.Add(unit);
            if (!mClampGroup[unit.CampID].Contains(unit))
                mClampGroup[unit.CampID].Add(unit);
        }
        public void OnUnitDie(UnitBase unit)
        {
            mClampGroup[unit.CampID].Remove(unit);
            world.RemoveUnit(unit);
        }
        public void AddAoeField(AoeField aoe)
        {
            mAoeFields.Add(aoe);
        }

        public void AddBullet(BulletBase bullet)
        {
            mBullets.Add(bullet);
        }
        
        public BulletBase ShootBullet(int templateID,UnitBase shooter,UnitBase target,bool isAttack)
        {
            var config = ConfigData.Bullet.getRow(templateID);
            BulletBase bullet = null;
            switch(config.BulletType)
            {
                case BulletType.kCoordBullet:
                    bullet = new CoordBullet(shooter, target.position);
                    break;
                case BulletType.kLineBullet:
                    if(config.FixRange > 0)
                    {
                        var offset = (target.position - shooter.position).normalized * config.FixRange;
                        bullet = new LineBullet(shooter, shooter.position + offset);
                    }
                    else
                    {
                        bullet = new LineBullet(shooter, target.position);
                    }
                    break;
                case BulletType.kPenteralBullet:
                    if (config.FixRange > 0)
                    {
                        var offset = (target.position - shooter.position).normalized * config.FixRange;
                        bullet = new PenetraBullet(shooter, config.DecayScale, shooter.position + offset);
                    }
                    else
                    {
                        bullet = new PenetraBullet(shooter,  config.DecayScale, target.position);
                    }
                    break;
                case BulletType.kReturnBullet:
                    if (config.FixRange > 0)
                    {
                        var offset = (target.position - shooter.position).normalized * config.FixRange;
                        bullet = new ReturnBullet(shooter, config.DecayScale, shooter.position + offset);
                    }
                    else
                    {
                        bullet = new ReturnBullet(shooter, config.DecayScale, target.position);
                    }
                    break;
                case BulletType.kTrackBullet:
                    bullet = new TrackBullet(shooter, target);
                    break;
                default:
                    throw new NotImplementedException("not implemented type " + config.BulletType.ToString());
            }
            bullet.radius = config.Radius;
            bullet.speed = config.Speed;
            bullet.acceleration = config.Acceleration;
            if (isAttack)
            {
                bullet.InitDamage((int)shooter.ATK.value, config.DamageType, isAttack);
                
            }
            else if (config.Damage > 0)
            {
                bullet.InitDamage(config.Damage, config.DamageType, isAttack);
            }
            if(config.AoeRadius > 0)
            {
                List<SkillModule.BuffEmitter> emitters = new List<SkillModule.BuffEmitter>(config.BuffEmitter.Length);
                for(int i = 0;i <config.BuffEmitter.Length;++i)
                {
                    var emitter = new SkillModule.BuffEmitter();
                    var emitter_config = ConfigData.BuffEmitter.getRow(config.BuffEmitter[i]);
                    emitter.buffs = new int[emitter_config.Buffs.Length];
                    for(int n = 0; n < emitter_config.Buffs.Length;++n)
                    {
                        emitter.buffs[n] =emitter_config.Buffs[n];
                    }
                    emitter.Caster =shooter;
                    emitter.filter = emitter_config.AoeFilter;
                    emitters[i] = emitter;

                }
                bullet.InitAoeFile(config.AoeRadius, config.AoeDuration, config.AoeInterval, emitters);
            }
            if (config.Buffs!=null && config.Buffs.Length > 0)
            {
                int[] buffs = new int[config.Buffs.Length];
                for (int n = 0; n < config.Buffs.Length; ++n)
                {
                    buffs[n] = config.Buffs[n];
                }
                bullet.InitBuff(buffs);
            }
            bullet.position = shooter.position;
            AddBullet(bullet);
            return bullet;
        }
        /// <summary>
        /// 找到血量最低的我方单位
        /// </summary>
        /// <param name="id"></param>
        /// <param name="campID"></param>
        /// <returns></returns>
        public UnitBase GetLowestHPAlly(int id,int campID)
        {
            UnitBase unit = null;
            var Group = mClampGroup[campID];
            for (int i = 0; i < Group.Count; ++i)
            {
                if (unit == null || unit.HP > Group[i].HP)
                {
                    unit = Group[i];
                }
            }
            return unit;
        }

        public UnitBase GetLowestHPEnemy(int campID)
        {
            UnitBase unit = null;
            for (int n = 0; n < mClampGroup.Length;++n)
            {
                if(n != campID)
                {
                    var Group = mClampGroup[n];
                    for (int i = 0; i < Group.Count; ++i)
                    {
                        if (unit == null || unit.HP > Group[i].HP)
                        {
                            unit = Group[i];
                        }
                    }
                }
            }
            return unit;
        }
        public UnitBase GetNearestAlly(float x, float y, int id, int campID)
        {

            UnitBase unit = null;
            float sqr_dis = 0;
            var Group = mClampGroup[campID];
            for (int i = 0; i < Group.Count; ++i)
            {  
                if (id != Group[i].ID)
                {
                    float dx = Group[i].position.x - x;
                    float dy = Group[i].position.y - y;
                    var _sqr_dis = dx * dx + dy * dy;
                    if (unit == null || _sqr_dis < sqr_dis)
                    {
                        unit = Group[i];
                        sqr_dis = _sqr_dis;
                    }
                }
            }
            return unit;
        }

        public UnitBase GetNearestEnemy(float x, float y, int id, int campID)
        {

            UnitBase unit = null;
            float sqr_dis = 0;
            for (int n = 0; n < mClampGroup.Length; ++n)
            {
                if (n != campID)
                {
                    var Group = mClampGroup[n];
                    for (int i = 0; i < Group.Count; ++i)
                    {
                        if (id != Group[i].ID)
                        {
                            float dx = Group[i].position.x - x;
                            float dy = Group[i].position.y - y;
                            var _sqr_dis = dx * dx + dy * dy;
                            if (unit == null || _sqr_dis < sqr_dis)
                            {
                                unit = Group[i];
                                sqr_dis = _sqr_dis;
                            }
                        }
                    }
                }
            }
            return unit;
        }

        public void Destroy()
        {
            if (world != null)
            {
                world.Destroy();
                world = null;
            }

            if (Instance == this)
                Instance = null;
        }

    }
}
