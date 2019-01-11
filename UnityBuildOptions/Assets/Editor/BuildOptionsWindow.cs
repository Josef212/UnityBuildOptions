using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildOptionsWindow : EditorWindow
{
    [MenuItem("BuildOptions", menuItem = "Window/BuildOptions")]
    public static void Display()
    {
        EditorWindow.GetWindow(typeof(BuildOptionsWindow), false, "Build options");
    }

    private void OnEnable()
    {
        m_nameCounter = EditorPrefs.GetInt("NameCounter");
        Refresh();
    }

    private void OnDisable()
    {
        m_presets.Clear();
        m_presetsFolds = null;
        EditorPrefs.SetInt("NameCounter", m_nameCounter);
    }

    private void Refresh()
    {
        m_presets.Clear();

        string[] assets = AssetDatabase.FindAssets("t: BuildOptions");
        m_presetsFolds = new bool[assets.Length];

        for (int i = 0; i < assets.Length; ++i)
        {
            string asset = assets[i];

            string path = AssetDatabase.GUIDToAssetPath(asset);
            m_presets.Add((BuildOptions)AssetDatabase.LoadAssetAtPath(path, typeof(BuildOptions)));
            m_presetsFolds[i] = false;
        }
    }

    private void OnGUI()
    {
        // Presets

        EditorGUILayout.LabelField("Presets", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if(m_presets.Count == 0)
        {
            EditorGUILayout.HelpBox("No BuildOptions assets found. Create one or refresh.", MessageType.Warning, true);
        }

        for(int i = 0; i < m_presets.Count; ++i)
        {
            BuildOptions preset = m_presets[i];

            m_presetsFolds[i] = EditorGUILayout.Foldout(m_presetsFolds[i], preset.m_name, true);

            if(m_presetsFolds[i])
            {
                foreach (var bo in preset.m_buildOptions)
                {
                    EditorGUILayout.BeginHorizontal();

                    bo.defines = EditorGUILayout.TextField(bo.group.ToString(), bo.defines);
                    if(GUILayout.Button("Set single build target group"))
                    {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(bo.group, bo.defines);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("Set preset BuildOptions"))
                {
                    SetBuildOptionsPreset(preset);
                }
            }
        }

        // ====================================

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Single define
        EditorGUILayout.LabelField("Single target group", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        m_buildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(m_buildTargetGroup);
        m_defines = EditorGUILayout.TextField(m_defines);

        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Set build options"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(m_buildTargetGroup, m_defines);
            m_buildTargetGroup = BuildTargetGroup.Unknown;
            m_defines = "";
        }
        
        // ====================================

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.Space();

        // Refresh / Create

        EditorGUILayout.BeginHorizontal();

        if(GUILayout.Button("Refresh"))
        {
            Refresh();
        }

        if (GUILayout.Button("Create"))
        {
            var bo = new BuildOptions();
            AssetDatabase.CreateAsset(bo, string.Format("Assets/BuildOptions{0}.asset", m_nameCounter));
            bo.m_name = Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(bo));
            ++m_nameCounter;

            Refresh();
        }

        EditorGUILayout.EndHorizontal();
    }

    // ======================

    private void SetBuildOptionsPreset(BuildOptions preset)
    {
        foreach (var bo in preset.m_buildOptions)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(bo.group, bo.defines);
        }
    }

    private List<BuildOptions> m_presets = new List<BuildOptions>();
    private bool[] m_presetsFolds = new bool[1];

    private BuildTargetGroup m_buildTargetGroup = BuildTargetGroup.Unknown;
    private string m_defines = "";

    private int m_nameCounter = 1;
}
