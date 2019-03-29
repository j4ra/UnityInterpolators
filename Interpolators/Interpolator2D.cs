using UnityEngine;

namespace Interpolators
{
    /// <summary>
    /// Contains various interpolating functions for 2D curves
    /// </summary>
    public static class Interpolator2D
    {
        /// <summary>
        /// Quadratic 2D Bezier Curve
        /// </summary>
        /// <param name="a">Control point A</param>
        /// <param name="b">Control point B</param>
        /// <param name="c">Control point C</param>
        /// <param name="t">Curve parameter should be between 0 and 1</param>
        /// <returns>Points on a 2D quadratic Bezier curve</returns>
        public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            Vector2 p0 = Vector2.Lerp(a, b, t);
            Vector2 p1 = Vector2.Lerp(b, c, t);
            return Vector2.Lerp(p0, p1, t);
        }

        /// <summary>
        /// Cubic 2D Bezier Curve
        /// </summary>
        /// <param name="a">Control point A</param>
        /// <param name="b">Control point B</param>
        /// <param name="c">Control point C</param>
        /// <param name="d">Control point D</param>
        /// <param name="t">Curve parameter should be between 0 and 1</param>
        /// <returns>Points on a 2D cubic Bezier curve</returns>
        public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            Vector2 p0 = EvaluateQuadratic(a, b, c, t);
            Vector2 p1 = EvaluateQuadratic(b, c, d, t);

            return Vector2.Lerp(p0, p1, t);
        }
    }
}