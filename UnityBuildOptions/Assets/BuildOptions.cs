using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "BuildOptions", menuName = "BuildOptions")]
public class BuildOptions : ScriptableObject
{
    [System.Serializable]
    public class BuilOption
    {
        public BuildTargetGroup group = BuildTargetGroup.Unknown;
        public string defines = "";

        public BuilOption(BuildTargetGroup group)
        {
            this.group = group;
            defines = "";
        }
    }

    [HideInInspector] public List<BuilOption> m_buildOptions = new List<BuilOption>();

    public string name = "";
}
