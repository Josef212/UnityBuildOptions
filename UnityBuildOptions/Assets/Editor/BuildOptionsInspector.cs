using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

[CustomEditor(typeof(BuildOptions))]
[CanEditMultipleObjects]
public class BuildOptionsInspector : Editor
{
    public override void OnInspectorGUI()
    {
        bTarget = (BuildOptions)target;

        for(int i = 0; i < bTarget.m_buildOptions.Count;)
        {
            EditorGUILayout.BeginHorizontal();

            BuildOptions.BuilOption bo = bTarget.m_buildOptions[i];

            bo.defines = EditorGUILayout.TextField(bo.group.ToString(), bo.defines);

            if(GUILayout.Button("Remove"))
            {
                bTarget.m_buildOptions.Remove(bo);
            }
            else
            {
                ++i;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();

        m_selectedGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup("New group", m_selectedGroup);
        if (GUILayout.Button("Add"))
        {
            if(AddBuildGroup(m_selectedGroup))
                m_selectedGroup = BuildTargetGroup.Unknown;
        }

        EditorGUILayout.EndHorizontal();

        string path = AssetDatabase.GetAssetPath(bTarget);
        bTarget.name = Path.GetFileNameWithoutExtension(path);

        EditorGUILayout.LabelField("Path: ", path);
        EditorGUILayout.LabelField("Name: ", bTarget.name);
    }

    private bool AddBuildGroup(BuildTargetGroup group)
    {
        if(bTarget.m_buildOptions.Find(g => g.group == group) == null)
        {
            bTarget.m_buildOptions.Add(new BuildOptions.BuilOption(m_selectedGroup));
            return true;
        }

        Debug.LogError("Can't add a BuildTargetGroup already in list!");
        return false;
    }

    private BuildOptions bTarget = null;
    private BuildTargetGroup m_selectedGroup = BuildTargetGroup.Unknown;
}
