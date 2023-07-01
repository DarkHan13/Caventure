using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FIeldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView Fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(Fow.transform.position, Vector3.back, Vector3.up, 360, Fow.viewRadius);
        Vector3 viewAngleA = Fow.DirFromAngle(-Fow.viewAngle / 2, false);
        Vector3 viewAngleB = Fow.DirFromAngle(Fow.viewAngle / 2, false);

        Handles.DrawLine(Fow.transform.position, Fow.transform.position + viewAngleA * Fow.viewRadius);
        Handles.DrawLine(Fow.transform.position, Fow.transform.position + viewAngleB * Fow.viewRadius);

        foreach (var visibleTarget in Fow.visibleTargets)
        {
            Handles.DrawLine(Fow.transform.position, visibleTarget.position);
        }
    }
}
