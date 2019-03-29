/*
 * Based on video by Sebastian Lague: https://www.youtube.com/watch?v=n_RHttAaRCk
 * Complete project on his github
 * https://github.com/SebLague/Curve-Editor
 * 
 * 
 * ©2018 - Jaroslav Nejedlý
 * 
 */

 #define DEBUG


using UnityEngine;
using UnityEditor;

namespace Curves
{
    [CustomEditor(typeof(Bezier2DCreator))]
    public class Bezier2DEditor : Editor
    {
        Bezier2DCreator creator;
        Bezier2D path;
        bool useTransform = true;
        Transform t;

        const float AnchorHandleSize = 0.15f;
        const float SegmentSelectDistanceTreshold = 0.1f;

        int selectSegmentIndex = -1;
        int selectPointIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Create New"))
            {
                Undo.RecordObject(creator, "Create new");
                creator.CreatePath();
                path = creator.path;
            }

            bool isClosed = GUILayout.Toggle(path.IsClosed, "Is Closed");
            if (isClosed != path.IsClosed)
            {
                Undo.RecordObject(creator, "Toggle closed");
                path.IsClosed = isClosed;
            }

            bool autoSetPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
            if(autoSetPoints != path.AutoSetControlPoints)
            {
                Undo.RecordObject(creator, "Toggle autoset controls");
                path.AutoSetControlPoints = autoSetPoints;
            }
#if DEBUG
            useTransform = GUILayout.Toggle(useTransform, "Use transform");
#endif
            if (EditorGUI.EndChangeCheck())
            {
                SceneView.RepaintAll();
            }
        }

        private void OnSceneGUI()
        {
            Input();
            Draw();
        }

        void Input()
        {
            Event guiEvent = Event.current;
            Vector2 mousePos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                if (selectSegmentIndex != -1)
                {
                    Undo.RecordObject(creator, "Split segment");
                    path.SplitSegment(inverseTransformPoint(mousePos), selectSegmentIndex);
                }
                else if (!path.IsClosed)
                {
                    Undo.RecordObject(creator, "Add segment");
                    path.AddSegment(inverseTransformPoint(mousePos));
                }
            }

            float minDstToAnchor = AnchorHandleSize;
            int closestAnchorIndex = -1;
            for (int i = 0; i < path.PointCount; i += 3)
            {
                if (Vector2.Distance(mousePos, transformPoint(path[i])) < minDstToAnchor)
                {
                    closestAnchorIndex = i;
                }
            }
            selectPointIndex = closestAnchorIndex;

            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
            {
                if (selectPointIndex != -1)
                {
                    Undo.RecordObject(creator, "Delete segment");
                    path.DeleteSegment(selectPointIndex);
                }
            }

            if (guiEvent.type == EventType.mouseMove)
            {
                float minDstToSegment = SegmentSelectDistanceTreshold;
                int newSelectedSegmentIndex = -1;

                for (int i = 0; i < path.SegmentCount; i++)
                {
                    Vector2[] p = path.PointsInSegment(i);
                    transformPoints(p);
                    float dst = HandleUtility.DistancePointBezier(mousePos, p[0], p[3], p[1], p[2]);
                    if (dst < minDstToSegment)
                    {
                        minDstToSegment = dst;
                        newSelectedSegmentIndex = i;
                    }
                }

                if(newSelectedSegmentIndex != selectSegmentIndex)
                {
                    selectSegmentIndex = newSelectedSegmentIndex;
                }
                HandleUtility.Repaint();
            }
        }

        void Draw()
        {
            for(int i = 0; i < path.SegmentCount; i++)
            {
                Vector2[] points = path.PointsInSegment(i);

                if (useTransform) transformPoints(points);

                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Color segmentColor = selectSegmentIndex == i ? Color.yellow : Color.green;
                Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentColor, null, 2);
            }


            
            for(int i = 0; i < path.PointCount; i++)
            {
                Vector2 newPos;
                if (i % 3 == 0 && i != selectPointIndex)
                {
                    Handles.color = Color.red;
                    newPos = Handles.FreeMoveHandle(transformPoint(path[i]), Quaternion.identity, AnchorHandleSize, Vector2.zero, Handles.CylinderHandleCap);
                }
                else if(i != selectPointIndex)
                {
                    Handles.color = Color.white;
                    newPos = Handles.FreeMoveHandle(transformPoint(path[i]), Quaternion.identity, AnchorHandleSize * 0.75f, Vector2.zero, Handles.CylinderHandleCap);
                }
                else
                {
                    Handles.color = Color.yellow;
                    float sizeMod = i % 3 == 0 ? 1f : 0.75f;
                    newPos = Handles.FreeMoveHandle(transformPoint(path[i]), Quaternion.identity, AnchorHandleSize * sizeMod, Vector2.zero, Handles.CylinderHandleCap);
                }

                newPos = inverseTransformPoint(newPos);
                if (newPos != path[i])
                {
                    Undo.RecordObject(creator, "Move point");
                    path.MovePoint(i, newPos);
                }
            }
        }

        private void OnEnable()
        {
            creator = (Bezier2DCreator)target;
            t = creator.transform;
            if(creator.path == null)
            {
                creator.CreatePath();
            }
            path = creator.path;
        }

        Vector2 transformPoint(Vector2 point)
        {
            if (useTransform)
            {
                return t.TransformPoint(point);
            }
            return point;
        }

        Vector2 inverseTransformPoint(Vector2 point)
        {
            if (useTransform)
            {
                return t.InverseTransformPoint(point);
            }
            return point;
        }

        void transformPoints(Vector2[] points)
        {
            if (useTransform)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    points[i] = t.TransformPoint(points[i]);
                }
            }
        }
    }
}
