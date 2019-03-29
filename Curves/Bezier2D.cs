/*
 * Based on video by Sebastian Lague: https://www.youtube.com/watch?v=n_RHttAaRCk
 * Complete project on his github
 * https://github.com/SebLague/Curve-Editor
 * 
 * 
 * ©2018 - Jaroslav Nejedlý
 * 
 */


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpolators;

namespace Curves
{
    /// <summary>
    /// Bezier path object in 2D
    /// </summary>
    [Serializable]
    public class Bezier2D
    {
        [HideInInspector, SerializeField]
        List<Vector2> points;

        [HideInInspector, SerializeField]
        bool isClosed;

        [HideInInspector, SerializeField]
        bool autoSetPoints;

        /// <summary>
        /// Should control points be set automatically?
        /// </summary>
        public bool AutoSetControlPoints
        {
            get { return autoSetPoints; }
            set
            {
                if (value != autoSetPoints)
                {
                    autoSetPoints = value;
                    if(autoSetPoints)
                    {
                        AutoSetAllControlPoints();
                    }
                }
            }
        }

        /// <summary>
        /// Is this a closed path?
        /// </summary>
        public bool IsClosed { get { return isClosed; }
            set
            {
                if(isClosed != value)
                {
                    isClosed = value;

                    if (isClosed)
                    {
                        points.Add(points[points.Count - 1] + points[points.Count - 1] - points[points.Count - 2]);
                        points.Add(points[0] + points[0] - points[1]);
                        if (autoSetPoints)
                        {
                            AutoSetAnchorControlPoints(0);
                            AutoSetAnchorControlPoints(points.Count - 3);
                        }
                    }
                    else
                    {
                        points.RemoveAt(points.Count - 1);
                        points.RemoveAt(points.Count - 1);
                        if (autoSetPoints)
                        {
                            AutoSetStartAndEndControls();
                        }
                    }
                }
            }
        } 

        /// <summary>
        /// Number of points in a path
        /// </summary>
        public int PointCount { get { return points.Count; } }

        /// <summary>
        /// Number of Bezier segments in a path
        /// </summary>
        public int SegmentCount { get { return points.Count / 3; } }

        /// <summary>
        /// Creates path with 2 anchor points and 2 control points.
        /// </summary>
        /// <param name="center">Center of a new curve</param>
        public Bezier2D(Vector2 center)
        {
            points = new List<Vector2>()
            {
                center + Vector2.left,
                center + (Vector2.left + Vector2.up) * 0.5f,
                center + (Vector2.right + Vector2.down) * 0.5f,
                center + Vector2.right
            };
        }

        /// <summary>
        /// Gets the point on a path with a given index
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Vector2 this[int i]
        {
            get { return points[i]; }
        }

        /// <summary>
        /// Adds new segment to path. Automatically generates control points with respect to anchor.
        /// </summary>
        /// <param name="anchor">Anchor point (end of new segment)</param>
        public void AddSegment(Vector2 anchor)
        {
            if (!isClosed)
            {
                points.Add(points[points.Count - 1] + points[points.Count - 1] - points[points.Count - 2]);
                points.Add((points[points.Count - 1] + anchor) * 0.5f);
                points.Add(anchor);

                if (autoSetPoints)
                {
                    AutoSetAffectedControlPoints(points.Count - 1);
                }
            }
        }

        /// <summary>
        /// Points in a given segment. Always returns array that has 4 elements.
        /// </summary>
        /// <param name="segment">Segment index</param>
        /// <returns></returns>
        public Vector2[] PointsInSegment(int segment)
        {
            if (segment < 0 || segment >= SegmentCount) throw new ArgumentOutOfRangeException("Segment index invalid.");

            int offset = segment * 3;
            return new Vector2[] { points[offset], points[offset + 1], points[offset + 2], points[LoopIndex(offset + 3)] };
        }

        /// <summary>
        /// Moves point with a given index onto newPos.
        /// </summary>
        /// <param name="index">Which point is being edited.</param>
        /// <param name="newPos">New position of a edited point.</param>
        public void MovePoint(int index, Vector2 newPos)
        {
            if (index < 0 || index >= PointCount) throw new ArgumentOutOfRangeException("Point index invalid.");

            Vector2 deltaMove = newPos - points[index];

            if (!autoSetPoints || index % 3 == 0)
            {
                points[index] = newPos;

            if(autoSetPoints)
            {
                AutoSetAffectedControlPoints(index);
                return;
            }

                if (index % 3 == 0)
                {
                    if (index + 1 < points.Count || isClosed)
                    {
                        points[LoopIndex(index + 1)] += deltaMove;
                    }
                    if (index - 1 >= 0 || isClosed)
                    {
                        points[LoopIndex(index - 1)] += deltaMove;
                    }
                }
                else
                {
                    bool isNextAnchor = (index + 1) % 3 == 0;
                    int correspondingControlPoint = isNextAnchor ? index + 2 : index - 2;

                    if (correspondingControlPoint >= 0 && correspondingControlPoint < points.Count || isClosed)
                    {
                        int anchorIndex = isNextAnchor ? index + 1 : index - 1;
                        anchorIndex = LoopIndex(anchorIndex);
                        correspondingControlPoint = LoopIndex(correspondingControlPoint);
                        float dst = (points[anchorIndex] - points[correspondingControlPoint]).magnitude;
                        Vector2 dir = (points[anchorIndex] - newPos).normalized;
                        points[correspondingControlPoint] = points[anchorIndex] + dir * dst;
                    }
                }
            }
        }

        /// <summary>
        /// Adds control point with position <paramref name="anchorPos"/> into the middle of segment with <paramref name="segmentIndex"/>
        /// </summary>
        /// <param name="anchorPos">Position of new anchor point</param>
        /// <param name="segmentIndex">Segment index of affected segment</param>
        public void SplitSegment(Vector2 anchorPos, int segmentIndex)
        {
            if (segmentIndex < 0 || segmentIndex >= SegmentCount) throw new ArgumentOutOfRangeException("Segment index invalid");
            points.InsertRange(segmentIndex * 3 + 2, new Vector2[] { Vector2.zero, anchorPos, Vector2.zero });
            if(autoSetPoints)
            {
                AutoSetAffectedControlPoints(segmentIndex * 3 + 3);
            }
            else
            {
                AutoSetAnchorControlPoints(segmentIndex * 3 + 3);
            }

        }

        /// <summary>
        /// Deletes segment coresponding to <paramref name="anchorIndex"/>
        /// </summary>
        /// <param name="anchorIndex">Index of anchor point that is being deleted</param>
        public void DeleteSegment(int anchorIndex)
        {
            if (anchorIndex < 0 || anchorIndex >= PointCount) throw new ArgumentOutOfRangeException("Point index invalid.");

            if (SegmentCount > 2 || (!isClosed && SegmentCount > 1))
            {

                if (anchorIndex == 0)
                {
                    if (isClosed)
                    {
                        points[points.Count - 1] = points[2];
                    }
                    points.RemoveRange(0, 3);
                }
                else if (anchorIndex == points.Count - 1 && !isClosed)
                {
                    points.RemoveRange(anchorIndex - 2, 3);
                }
                else
                {
                    points.RemoveRange(anchorIndex - 1, 3);
                }
            }
        }

        /// <summary>
        /// Calculates evenly spaced points on a curve and stores them into array
        /// </summary>
        /// <param name="spacing">How far appart the points should be</param>
        /// <param name="resolution">Higher values for more precision and slower compute time</param>
        /// <returns>Array of evenly spaced points</returns>
        public Vector2[] CalculateEvenlySpacedPoints(float spacing, float resolution = 1)
        {
            List<Vector2> evenlySpacedPoints = new List<Vector2>();
            evenlySpacedPoints.Add(points[0]);
            Vector2 previousPoint = points[0];
            float distanceSinceLastEvenPoint = 0;

            for (int segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
            {
                Vector2[] p = PointsInSegment(segmentIndex);
                float controlNetLength = Vector2.Distance(p[0], p[1]) + Vector2.Distance(p[1], p[2]) + Vector2.Distance(p[2], p[3]);
                float estimatedCurveLength = Vector2.Distance(p[0], p[3]) + 0.5f * controlNetLength;
                float dt = 1f / (estimatedCurveLength * resolution * 10);
                float t = 0;
                while(t <= 1)
                {
                    t += dt;
                    Vector2 pointOnCurve = Interpolator2D.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                    distanceSinceLastEvenPoint += Vector2.Distance(previousPoint, pointOnCurve);

                    while(distanceSinceLastEvenPoint >= spacing)
                    {
                        float overshootDistance = distanceSinceLastEvenPoint - spacing;
                        Vector2 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDistance;
                        evenlySpacedPoints.Add(newEvenlySpacedPoint);
                        distanceSinceLastEvenPoint = overshootDistance;
                        previousPoint = newEvenlySpacedPoint;
                    }

                    previousPoint = pointOnCurve;
                }
            }

            return evenlySpacedPoints.ToArray();
        }

        /// <summary>
        /// Wraps around index so it is never outside the range
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private int LoopIndex(int i)
        {
            return (i + points.Count) % points.Count;
        }


        /// <summary>
        /// Automatically sets control points around given anchor point
        /// </summary>
        /// <param name="anchorIndex"></param>
        private void AutoSetAnchorControlPoints(int anchorIndex)
        {
            Vector2 anchorPos = points[anchorIndex];
            Vector2 dir = Vector2.zero;
            float[] neighbourDistances = new float[2];
            if(anchorIndex - 3 >= 0 || isClosed)
            {
                Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
                dir += offset.normalized;
                neighbourDistances[0] = offset.magnitude;
            }
            if (anchorIndex + 3 >= 0 || isClosed)
            {
                Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
                dir -= offset.normalized;
                neighbourDistances[1] = -offset.magnitude;
            }

            dir.Normalize();

            for (int i = 0; i < 2; i++)
            {
                int controlIndex = anchorIndex + i * 2 - 1;
                if(controlIndex >= 0 && controlIndex < points.Count || isClosed)
                {
                    points[LoopIndex(controlIndex)] = anchorPos + dir * neighbourDistances[i] * 0.5f;
                }
            }
        }

        /// <summary>
        /// Auto sets control points on first and last segment
        /// </summary>
        private void AutoSetStartAndEndControls()
        {
            if(!isClosed)
            {
                points[1] = (points[0] + points[2]) * 0.5f;
                points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * 0.5f;
            }
        }

        /// <summary>
        /// Auto sets all control points on a path
        /// </summary>
        private void AutoSetAllControlPoints()
        {
            for (int i = 0; i < points.Count; i+= 3)
            {
                AutoSetAnchorControlPoints(i);
            }
            AutoSetStartAndEndControls();
        }


        /// <summary>
        /// Autosets control points around anchor point with index <paramref name="updateAnchorIndex"/>
        /// </summary>
        /// <param name="updateAnchorIndex">Index of affected anchor point</param>
        private void AutoSetAffectedControlPoints(int updateAnchorIndex)
        {
            for (int i = updateAnchorIndex - 3; i <= updateAnchorIndex + 3; i += 3)
            {
                if(i >= 0 && i < points.Count || isClosed)
                {
                    AutoSetAnchorControlPoints(LoopIndex(i));
                }
            }

            AutoSetStartAndEndControls();
        }
    }
}
