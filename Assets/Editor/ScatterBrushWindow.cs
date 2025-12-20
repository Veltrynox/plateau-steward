using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SubnauticaClone
{
    /// <summary>
    /// This tool allows users to select from a palette of prefabs and "paint" them onto the scene.
    /// </summary>
    public class ScatterBrushWindow : EditorWindow
    {
        public List<GameObject> palette = new List<GameObject>();
        public GameObject terrain;

        private int selectedIndex = 0;

        string toolName = "Scatter Brush";
        float brushRadius = 5f;
        bool isPainting = false;

        private SerializedObject serializedObj;

        [MenuItem("Tools/Scatter Brush")]
        public static void ShowWindow()
        {
            GetWindow<ScatterBrushWindow>("Scatter Brush");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            serializedObj = new SerializedObject(this);
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnGUI()
        {
            GUILayout.Label(toolName, EditorStyles.boldLabel);

            serializedObj.Update();
            SerializedProperty paletteProp = serializedObj.FindProperty("palette");
            SerializedProperty terrainProp = serializedObj.FindProperty("terrain");

            EditorGUILayout.PropertyField(paletteProp, new GUIContent("Brush Palette"), true);
            EditorGUILayout.PropertyField(terrainProp, new GUIContent("Terrain Parent"), true);
            serializedObj.ApplyModifiedProperties();

            GUILayout.Space(10);
            brushRadius = EditorGUILayout.Slider("Brush Radius", brushRadius, 0.5f, 10f);

            if (palette.Count > 0)
            {
                selectedIndex = Mathf.Clamp(selectedIndex, 0, palette.Count - 1);
                string currentName = palette[selectedIndex] != null ? palette[selectedIndex].name : "None";
                GUILayout.Label($"Selected [Key {selectedIndex + 1}]: {currentName}", EditorStyles.helpBox);
            }
            else
            {
                GUILayout.Label("Add items to Palette to start!", EditorStyles.label);
            }

            if (GUILayout.Button(isPainting ? "Stop Painting" : "Start Painting"))
            {
                isPainting = !isPainting;
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!isPainting || palette.Count == 0) return;

            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                int numberPressed = (int)e.keyCode - (int)KeyCode.Alpha1;

                if (numberPressed >= 0 && numberPressed < palette.Count)
                {
                    selectedIndex = numberPressed;
                    e.Use();
                    sceneView.Repaint();
                }
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Handles.color = Color.cyan;
                Handles.DrawWireDisc(hit.point, hit.normal, brushRadius);

                GameObject currentPrefab = palette[selectedIndex];
                if (currentPrefab != null)
                {
                    Handles.Label(hit.point + Vector3.up, $"Active: {currentPrefab.name}");
                }

                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    PaintObject(hit.point, hit.normal);
                    e.Use();
                }
            }

            sceneView.Repaint();
        }

        private void PaintObject(Vector3 position, Vector3 normal)
        {
            if (selectedIndex >= palette.Count) return;
            GameObject prefabToPaint = palette[selectedIndex];
            if (prefabToPaint == null) return;

            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToPaint);
            newObj.transform.parent = terrain.transform;
            newObj.transform.position = position;
            newObj.transform.up = normal;
            newObj.transform.Rotate(0, Random.Range(0, 360), 0);

            Undo.RegisterCreatedObjectUndo(newObj, "Paint Object");
        }
    }
}