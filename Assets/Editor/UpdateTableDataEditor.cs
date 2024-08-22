using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UpdateTableData), true)]
public class UpdateTableDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UpdateTableData data = (UpdateTableData)target;

        if (GUILayout.Button("Update")) {
            data.NotifyOfUpdatedValues();
            EditorUtility.SetDirty(target);
        }
    }
}
