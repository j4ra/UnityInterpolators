/*
 * Based on video by Sebastian Lague: https://www.youtube.com/watch?v=n_RHttAaRCk
 * Complete project on his github
 * https://github.com/SebLague/Curve-Editor
 * 
 * 
 * ©2018 - Jaroslav Nejedlý
 * 
 */



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Curves
{
    [CustomEditor(typeof(Bezier2DCreator))]
    public class Bezier2DEditor : Editor
    {
        Bezier2DCreator creator;
        Bezier2D path;

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

            if (GUILayout.Button("Toggle Closed"))
            {
                Undo.RecordObject(creator, "Toggle closed");
                path.ToggleClosed();
            }

            bool autoSetPoints = GUILayout.Toggle(path.AutoSetControlPoints, "Auto Set Control Points");
            if(autoSetPoints != path.AutoSetControlPoints)
            {
                Undo.RecordObject(creator, "Toggle autoset controls");
                path.AutoSetControlPoints = autoSetPoints;
            }

            if(EditorGUI.EndChangeCheck())
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

            if(guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                Undo.RecordObject(creator, "Add segment");
                path.AddSegment(mousePos);
            }
        }

        void Draw()
        {
            for(int i = 0; i < path.SegmentCount; i++)
            {
                Vector2[] points = path.PointsInSegment(i);

                Handles.color = Color.black;
                Handles.DrawLine(points[1], points[0]);
                Handles.DrawLine(points[2], points[3]);
                Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);
            }


            
            for(int i = 0; i < path.PointCount; i++)
            {
                Vector2 newPos;
                if (i % 3 == 0)
                {
                    Handles.color = Color.red;
                    newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 0.15f, Vector2.zero, Handles.CylinderHandleCap);
                }
                else
                {
                    Handles.color = Color.white;
                    newPos = Handles.FreeMoveHandle(path[i], Quaternion.identity, 0.1f, Vector2.zero, Handles.CylinderHandleCap);
                }

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
            if(creator.path == null)
            {
                creator.CreatePath();
            }
            path = creator.path;
        }
    }
}
