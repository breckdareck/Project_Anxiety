using System.Collections;
using System.Collections.Generic;
using Project_Anxiety.Game.Units;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(JobSystem))]
public class JobInfoCustomEditor : OdinEditor
{
    private Vector2 scroll;
    
    // public override void OnInspectorGUI()
    // {
    //     base.OnInspectorGUI();
    //     
    //     EditorGUILayout.Space();
    //
    //     JobSystem data = (JobSystem)target;
    //     
    //     scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.MaxHeight(200));
    //     
    //     for (int i = 1; i < data.CurrentJob.MaxJobLevel; i++)
    //     {
    //         EditorGUILayout.BeginHorizontal("box");
    //         EditorGUILayout.LabelField("Level " + (i) + ":", GUILayout.MaxWidth(60));
    //         EditorGUILayout.LabelField("Requires: " + (int)data.CurrentJob.ExpCurve.Evaluate(i) + " EXP To Level " + (i+1));
    //         EditorGUILayout.EndHorizontal();
    //     }
    //     
    //     EditorGUILayout.EndScrollView();
    // }
}
