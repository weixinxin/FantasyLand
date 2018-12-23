using BattleSystem.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleSystem.ObjectModule
{
    public enum AnimationState
    {
        kStop,
        kPlaying,
        kPause,
        kComplete,
    }
    public class AnimatorController
    {

        private Animator mAnimator;
        private AnimationState State = AnimationState.kStop;
        public AnimatorController(int animatorID)
        {
            mAnimator = ConfigData.Animator.getRow(animatorID);
            if(mAnimator == null)
            {
                Logger.LogErrorFormat("init animatorController failed! animator id = {0}", animatorID);
            }
        }
        public float speed
        {
            get
            {
                return mAnimationSpeedScale;
            }
            set
            {
                if (value < 0)
                {
                    mAnimationSpeedScale = 0;
                }
                else
                {
                    mAnimationSpeedScale = value;
                }
            }
        }

        public delegate void AnimationEvent(string @event);
        public AnimationEvent OnAnimationEvent;

        public string mAnimationName { get; private set; }

        private float mAnimationSpeedScale = 1.0f;

        private float mElapseTime = 0;

        private int mPrevFrameIndex;
        private int mAnimationTotalFrames;
        private Animator.Animation mAnimation;



        public void Pause()
        {
            if(State == AnimationState.kPlaying)
                State = AnimationState.kPause;
        }

        public void Resume()
        {
            if(State == AnimationState.kPause)
                State = AnimationState.kPlaying;
        }

        public void Stop()
        {
            State = AnimationState.kStop;
        }

        public void OnAbort()
        {
            if (this.OnAnimationEvent != null && (State == AnimationState.kPlaying || State == AnimationState.kPause))
            {
                this.OnAnimationEvent("abort");
            }
        }

        public bool PlayAnimation(string animationName)
        {
            OnAbort();
            mAnimation = mAnimator.getAnimation(animationName);
            if(mAnimation == null)
            {
                Logger.LogErrorFormat("can not find animation {0}", animationName);
                return false;
            }
            mAnimationName = animationName;
            mPrevFrameIndex = 0;
            mAnimationTotalFrames = mAnimation.duration;
            mElapseTime = 0;
            State = AnimationState.kPlaying;
            return true;
        }
        public readonly static int FrameSampe = 60;
        public readonly static float FrameRate = 1.0f / FrameSampe;


        public float getAnimationDurration(string animationName)
        {
            var animation = mAnimator.getAnimation(animationName);
            if (animation == null)
            {
                Logger.LogErrorFormat("can not find animation {0}", animationName);
                return 0;
            }
            return animation.duration * FrameRate;
        }

        public void Update(float dt)
        {
            if (State != AnimationState.kPlaying)
            {
                return;
            }
            mElapseTime += dt;
            if(mAnimation.Events.Length > 0)
            {
                int curFrameIndex = (int)(mElapseTime * FrameSampe);
                if (!mAnimation.loop && curFrameIndex >= mAnimationTotalFrames)
                {
                    curFrameIndex = mAnimationTotalFrames - 1;
                    State = AnimationState.kComplete;
                }
                for (int i = mPrevFrameIndex + 1; i <= curFrameIndex; ++i)
                {
                    int frameIndex = i % mAnimationTotalFrames;
                    for(int n = 0;n < mAnimation.Events.Length;++n)
                    {
                        var @event = mAnimation.Events[n];
                        if(@event.frame == frameIndex)
                        {
                            this.OnAnimationEvent(@event.name);
                        }
                    }
                }
                mPrevFrameIndex = curFrameIndex;
            }

        }
    }
}
