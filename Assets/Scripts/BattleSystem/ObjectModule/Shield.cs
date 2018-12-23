using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public class Shield
    {
        class TempShield
        {
            public int value;
            private float duration;

            public TempShield(int v,float t)
            {
                value = v;
                duration = t;
            }
            public bool Update(float dt)
            {
                duration -= dt;
                if(duration <= 0)
                {
                    value = 0;
                }
                return value > 0;
            }
        }
        class WeakenShield
        {
            public float value;
            private float drop;

            public WeakenShield(int v, float t)
            {
                value = v;
                drop = value / t;
            }
            public bool Update(float dt)
            {
                value -= drop * dt;
                return value > 0;
            }
        }
        List<TempShield> mTempShieldList = new List<TempShield>();
        List<WeakenShield> mWeakenShieldList = new List<WeakenShield>();


        private int mPermanentSheild = 0;

        /// <summary>
        /// 增加永久护盾
        /// </summary>
        /// <param name="value"></param>
        public void AddPermanentSheild(int value)
        {
            mPermanentSheild += value;
        }
        /// <summary>
        /// 增加维持一段时间的护盾
        /// </summary>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        public void AddTempShield(int value,float duration)
        {
            mTempShieldList.Add(new TempShield(value, duration));
        }

        /// <summary>
        /// 增加一个随时间衰减的护盾
        /// </summary>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        public void AddWeakenShield(int value, float duration)
        {
            mWeakenShieldList.Add(new WeakenShield(value, duration));
        }

        const float mUpdateInterval = 0.2f;
        private float mTimer = 0;
        private bool mIsDirty = false;
        public void Update(float dt)
        {
            for(int i = mWeakenShieldList.Count - 1;i >= 0;--i)
            {
                if(mWeakenShieldList[i].Update(dt))
                {
                    mIsDirty = true;
                    mWeakenShieldList.RemoveAt(i);
                }
            }
            mTimer += dt;
            //不需要每帧更新衰减护盾
            if (mTimer > mUpdateInterval)
            {
                float elapse = mTimer - mTimer % mUpdateInterval;
                for (int i = mTempShieldList.Count - 1; i >= 0; --i)
                {
                    if (mTempShieldList[i].Update(elapse))
                    {
                        mIsDirty = true;
                        mTempShieldList.RemoveAt(i);
                    }
                }
                mTimer -= elapse;
            }
            if(mIsDirty)
            {
                int dv = mValue - Value;
                //通知变化
            }
        }

        private int mValue = 0;
        public int Value
        {
            get
            {
                if(mIsDirty)
                {
                    mValue = mPermanentSheild;
                    for (int i = 0; i < mTempShieldList.Count;++i )
                    {
                        mValue += mTempShieldList[i].value;
                    }
                    for (int i = 0; i < mWeakenShieldList.Count; ++i)
                    {
                        mValue += (int)mWeakenShieldList[i].value;
                    }
                    mIsDirty = false;
                }
                return mValue;
            }
            
        }

        public int Consume(int value)
        {
            int cur = Value;
            if (cur < value)
            {
                mValue = 0;
                mWeakenShieldList.Clear();
                mTempShieldList.Clear();
                //通知变化cur
                return value - cur;
            }
            else
            {
                int consumeValue = value;
                mValue = cur - value;
                while (consumeValue > 0 && mTempShieldList.Count > 0)
                {
                    if (mTempShieldList[0].value > consumeValue)
                    {
                        mTempShieldList[0].value -= consumeValue;
                        consumeValue = 0;
                    }
                    else
                    {
                        consumeValue -= mTempShieldList[0].value;
                        mTempShieldList.RemoveAt(0);
                    }
                }
                while (consumeValue > 0 && mWeakenShieldList.Count > 0)
                {
                    if (mWeakenShieldList[0].value > consumeValue)
                    {
                        mWeakenShieldList[0].value -= consumeValue;
                        consumeValue = 0;
                    }
                    else
                    {
                        consumeValue -= (int)mWeakenShieldList[0].value;
                        mWeakenShieldList.RemoveAt(0);
                    }
                }
                if (consumeValue > 0)
                {
                    mPermanentSheild -= consumeValue;
                }
                //通知变化value
                return 0;
            }
        }
    }
}
