//#define INTERPOLATOR1D_DO_CHECK
#define INTERPOLATOR1D_FORCE_CLAMP

using UnityEngine;
using System;

/*
 * Implements functions from this GDC talk: https://www.youtube.com/watch?v=mr5xkf6zSzk
 * Also uses same name convention 
 * 
 * © 2018 - Jaroslav Nejedlý
 * 
 */
namespace Interpolators
{
    /// <summary>
    /// Contains various interpolating functions for one variable.
    /// </summary>
    public static class Interpolator1D
    {
        /// <summary>
        /// Maps value t from range [min, max] to [0,1]
        /// </summary>
        /// <param name="t">variable to be mapped</param>
        /// <param name="min">value corresponding to 0</param>
        /// <param name="max">value corresponding to 1</param>
        /// <returns>Value between 0 and 1</returns>
        public static float RangeMap01(float t, float min, float max)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif

            return (t - min) / (max - min);
        }

        /// <summary>
        /// Maps value t from range [inMin, inMax] to [outMin, outMax]
        /// </summary>
        /// <param name="t">varable to be mapped</param>
        /// <param name="inMin">value corresponding to outMin</param>
        /// <param name="inMax">value corresponding to outMax</param>
        /// <param name="outMax">value corresponding to inMax</param>
        /// <param name="outMin">value corresponding to inMin</param>
        /// <returns>Value between outMin and outMax</returns>
        public static float RangeMap(float t, float inMin, float inMax, float outMin, float outMax)
        {
            return Lerp(RangeMap01(t, inMin, inMax), outMin, outMax);
        }

        /// <summary>
        /// Lineary interpolates between min and max using variable t. <para>Note: for t = 1 it is not assured that this function wll return max</para>
        /// </summary>
        /// <param name="t">Value to interpolate with</param>
        /// <param name="min">Value corresponding to 0</param>
        /// <param name="max">Value corresponding to 1</param>
        /// <returns>Value between min and max</returns>
        public static float Lerp(float t, float min, float max)
        {
            return min + t * (max - min);
        }

        /// <summary>
        /// Smooths value t around 0. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns>t * t</returns>
        public static float SmoothStart2(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif

            return t * t;
        }

        /// <summary>
        /// Smooths value t around 0 even more. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns>t * t * t</returns>
        public static float SmoothStart3(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif

            return t * t * t;
        }

        /// <summary>
        /// Smooths value t around 0 even more and more :D. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns>t * t * t * t</returns>
        public static float SmoothStart4(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif
            var t2 = t * t;
            return t2 * t2;
        }

        /// <summary>
        /// Smooths value t around 1. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns>t * (2 - t)</returns>
        public static float SmoothStop2(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif
            //1 - SmoothStart2(1 - t) = 1 - (1 - t) * (1 - t) = 1 - (1 - 2t + t2) = 2t - t2= t * (2 - t)
            return t * (2 - t);
        }

        /// <summary>
        /// Smooths value t around 1 even more. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns>t * (3 + t * (t - 3))</returns>
        public static float SmoothStop3(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif
            //1 - SmoothStart3(1 - t) = 1 - (1 - t)(1 - t)(1 - t) = 1 - (1 - 3*t + 3 * t * t - t * t *t)=3t-3t2 + t3 = t * (3 - 3 * t + t*t) = t * (3 + t * (t - 3))
            return t * (3 + t * (t - 3));
        }

        /// <summary>
        /// Smooths value t around 1 even more and more. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns>t * (4 + t * (t * (4 - t) - 6))</returns>
        public static float SmoothStop4(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif
            //1 - SmoothStart4(1 - t) = -t4 + 4t3 - 6t2 + 4t= t * (4 - 6 * t + 4 * t * t - t*t*t) = t * (4 + t * (t * (4 - t) - 6)
            return t * (4 + t * (t * (4 - t) - 6));
        }

        /// <summary>
        /// Smooths value t around 0 and around 1. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns></returns>
        public static float SmoothStep3(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif
            //t2 + t*(1 - (1 - 2t + t2) - t2) = t2 + 2t2 - 2t3 = 3t2 - 2t3
            // t * (3 * t - 2 * t2)= t * t *(3 - 2 * t)
            return t * t * (3 - 2 * t);
        }

        /// <summary>
        /// Smooths value t around 0 and around 1 even more. <para>Note: t MUST be between 0 and 1</para>
        /// </summary>
        /// <param name="t">Variable to be smoothed</param>
        /// <returns></returns>
        public static float SmoothStep5(float t)
        {
#if INTERPOLATOR1D_DO_CHECK
        if (t > max || t < min) throw new ArgumentOutOfRangeException("t must be between min and max");
#elif INTERPOLATOR1D_FORCE_CLAMP
            t = Saturate(t);
#endif
            return t * t * t * (10 + t * (6 * t - 15));
        }


        
        /// <summary>
        /// Clamps variable t between 0 and 1
        /// </summary>
        /// <param name="t">Variable to be clamped</param>
        /// <returns>Calue between 0 and 1</returns>
        public static float Saturate(float t)
        {
            return Mathf.Max(0, Mathf.Min(1, t));
        }
    }
}