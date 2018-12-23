using System;
using UnityEngine;

namespace Framework
{
    public static class GameManager
    {
        /// <summary>
        /// The time in seconds it took to complete the last frame (Read Only)
        /// </summary>
        public static float deltaTime {get { return Time.deltaTime; }}

        /// <summary>
        ///  The interval in seconds at which physics and other fixed frame rate updates
        ///  (like MonoBehaviour's MonoBehaviour.FixedUpdate) are performed.
        /// </summary>
        public static float fixedDeltaTime { get { return Time.fixedDeltaTime; } }

        /// <summary>
        /// The time the latest MonoBehaviour.FixedUpdate has started (Read Only). This
        /// is the time in seconds since the start of the game.
        /// </summary>
        public static float fixedTime { get { return Time.fixedTime; } }

        /// <summary>
        /// The total number of frames that have passed (Read Only).
        /// </summary>
        public static int frameCount { get { return Time.frameCount; } }
        /// <summary>
        /// The maximum time a frame can take. Physics and other fixed frame rate updates
        /// (like MonoBehaviour's MonoBehaviour.FixedUpdate).
        /// </summary>
        public static float maximumDeltaTime { get { return Time.maximumDeltaTime; } }
       

        /// <summary>
        /// The real time in seconds since the game started (Read Only).
        /// </summary>
        public static float realtimeSinceStartup { get { return Time.realtimeSinceStartup; } }


        public static int renderedFrameCount { get { return Time.renderedFrameCount; } }
        
        /// <summary>
        /// A smoothed out Time.deltaTime (Read Only).
        /// </summary>
        public static float smoothDeltaTime { get { return Time.smoothDeltaTime; } }
        /// <summary>
        /// The time at the beginning of this frame (Read Only). 
        /// This is the time in seconds since the start of the game.
        /// </summary>
        public static float time { get { return Time.time; } }
        
  
        /// <summary>
        /// The scale at which the time is passing.
        /// This can be used for slow motion effects.
        /// </summary>
        public static float timeScale { 
            get { return Time.unscaledDeltaTime; }
            set { Time.timeScale = value; }
        }
        //
        // 摘要: 
        //     The timeScale-independent time in seconds it took to complete the last frame
        //     (Read Only).
        public static float unscaledDeltaTime { get { return Time.unscaledDeltaTime; } }
        //
        // 摘要: 
        //     The timeScale-independant time at the beginning of this frame (Read Only).
        //     This is the time in seconds since the start of the game.
        public static float unscaledTime { get { return Time.unscaledTime; } }
    }
}
