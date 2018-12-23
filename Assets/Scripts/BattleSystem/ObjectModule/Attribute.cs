using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace BattleSystem.ObjectModule
{
    public class Attribute
    {
        /// <summary>
        /// 基础值
        /// </summary>
        protected float mBaseValue;

        /// <summary>
        /// 基础值差量
        /// </summary>
        protected float mBaseDelta = 0;

        /// <summary>
        /// 当前值差量
        /// </summary>
        protected float mCurDelta = 0;

        /// <summary>
        /// 当前值百分比
        /// </summary>
        protected float mPercentage = 1.0f;

        /// <summary>
        /// 基础值百分比
        /// </summary>
        protected float mBasePercent = 0f;

        protected FloatDelegate OnChanged;

        protected float mLastValue = 0;


        protected bool isDirty = true;
        public Attribute()
        {
        }
        public Attribute(float v, FloatDelegate callBack = null)
        {
            mBaseValue = v;
            OnChanged = callBack;
        }

        public virtual void ModifyAll(float baseDelta,float curDelta,float basePer,float curPer)
        {
            mCurDelta += curDelta;
            mBaseDelta += baseDelta;
            mPercentage *= (1 + curPer);
            mBasePercent *= (1 + basePer);
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }

        public virtual void RemoveAll(float baseDelta, float curDelta, float basePer, float curPer)
        {
            mCurDelta -= curDelta;
            mBaseDelta -= baseDelta;
            mPercentage /= (1 + curPer);
            mBasePercent /= (1 + basePer);
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }

        public virtual void CurModify(float value)
        {
            mCurDelta += value;
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }

        public virtual void RemoveCur(float value)
        {
            mCurDelta -= value;
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }
        public virtual void BaseModify(float value)
        {
            mBaseDelta += value;
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }

        public virtual void RemoveBase(float value)
        {
            mBaseDelta -= value;
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }
        public virtual void BasePercentageModify(float value)
        {
            mBasePercent *= (1 + value);
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }

        public virtual void RemoveBasePercentage(float value)
        {
            mBasePercent /= (1 + value);
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }
        public virtual void CurPercentageModify(float value)
        {
            mPercentage *= (1 + value);
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }
        public virtual void RemoveCurPercentage(float value)
        {
            mPercentage /= (1 + value);
            isDirty = true;
            if (OnChanged != null)
                OnChanged(mLastValue);
        }
        public virtual float value
        {
            get
            {
                if (isDirty)
                {
                    mLastValue = (mBaseValue + mBaseDelta + mBaseValue * mBasePercent) * mPercentage + mCurDelta;
                    isDirty = false;
                }
                return mLastValue;
            }
        }


    }

    public class DistanceAttribute:Attribute
    {
        float mSqrValue;
        public DistanceAttribute(float v, FloatDelegate callBack = null):base(v,callBack)
        {

        }
        public override float value
        {
            get
            {
                if (isDirty)
                {
                    mLastValue = (mBaseValue + mBaseDelta + mBaseValue * mBasePercent) * mPercentage + mCurDelta;
                    mSqrValue = mLastValue * mLastValue;
                    isDirty = false;
                }
                return mLastValue;
            }
        }
        public virtual float sqrValue
        {
            get
            {
                if (isDirty)
                {
                    mLastValue = (mBaseValue + mBaseDelta + mBaseValue * mBasePercent) * mPercentage + mCurDelta;
                    mSqrValue = mLastValue * mLastValue;
                    isDirty = false;
                }
                return mSqrValue;
            }
        }
    }
    public class AttackDuration : Attribute
    {
        public AttackDuration(float v, FloatDelegate callBack = null)
            : base(v, callBack)
        {
            
        }
        public override float value
        {
            get
            {
                if (isDirty)
                {
                    float sp = 1.0f / (mBaseValue);
                    mLastValue = 1.0f / (sp * mPercentage);
                    isDirty = false;
                }
                return mLastValue;
            }
        }
    }
}
