using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;

public class PathImporter : EditorWindow
{
    private static List<GameObject> _selectedObjects = new List<GameObject>();
    private readonly List<Bezier2D> _importedBeziers = new List<Bezier2D>();
    private float _duration;

    private string _label = "Please import a path.";
    private Vector2 _scrollPosition;
    
    [MenuItem("Window/Path Importer")]
    public static void ShowWindow()
    {
        _selectedObjects = Selection.gameObjects.Where(go => go.GetComponent<PathFollower>() != null).ToList();
        GetWindow(typeof (PathImporter));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Import path", GUILayout.Width(150)))
        {
            var path = EditorUtility.OpenFilePanel("Open path", "", "path");
            if (string.IsNullOrEmpty(path)) return;

            using (var sr = new StreamReader(path))
            {
                _importedBeziers.Clear();
                string line;
                var readFirstLine = false;

                while ((line = sr.ReadLine()) != null)
                {
                    // Read in the duration
                    if (!readFirstLine)
                    {
                        try
                        {
                            _duration = float.Parse(line, CultureInfo.InvariantCulture);
                            readFirstLine = true;
                            continue;
                        }
                        catch (Exception)
                        {
                            Debug.LogError("Invalid path file. Could not read animation length.");
                            _label = "Invalid path.";
                            continue;
                        }                       
                    }



                    // Read in all the points
                    var points = line.Split(' ');

                    if (string.IsNullOrEmpty(points[0])) continue;
                    try
                    {
                        _importedBeziers.Add(Bezier2D.ParseBezier(points[0], points[1], points[2]));
                        _label = "Path has been imported.";
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                        Debug.LogError("Invalid path file. Please select a valid file that has been exported by the PathingTool.");
                        _label = "Invalid path.";
                    }
                }
            }
        }

        GUILayout.Label(_label);
        EditorGUILayout.EndHorizontal();

        if (_importedBeziers == null || _importedBeziers.Count == 0) return;
        EditorGUILayout.Space();  
        GUILayout.Label("Select objects with a PathFollower component.");

        EditorGUILayout.Space();  

        
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        foreach (var go in _selectedObjects)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(go.name);
                if (GUILayout.Button("Apply path", GUILayout.Width(100)))
                {
                    var follower = go.GetComponent<PathFollower>();
                    follower.Path.Clear();
                    follower.Path.Add(_importedBeziers);
                    follower.AnimationTime = _duration;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    void OnSelectionChange()
    {
        _selectedObjects = Selection.gameObjects.Where(go => go.GetComponent<PathFollower>() != null).ToList();
        Repaint();
    }
}