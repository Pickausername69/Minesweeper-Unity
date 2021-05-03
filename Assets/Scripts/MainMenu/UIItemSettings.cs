using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIItemSettings : MonoBehaviour
{
    RectTransform rt;
    public Vector2 relativesizetoparent = new Vector2Int(1, 1);
    Vector2Int parentsize;

    [HideInInspector]
    public bool UseRelativePosition = false;
    [HideInInspector]
    public int relativepositionX = 0;
    [HideInInspector]
    public int relativepositionY = 0;

    public void Awake()
    {
        rt = gameObject.GetComponent<RectTransform>();
    }
    void Update()
    {
        if (UseRelativePosition)
        {
            
        }
    }

}
#if UNITY_EDITOR
[CustomEditor(typeof(UIItemSettings))]
public class CustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UIItemSettings settings = (UIItemSettings)target;
        settings.UseRelativePosition = EditorGUILayout.Toggle("Use Relative Position", settings.UseRelativePosition);
        if (settings.UseRelativePosition) // if bool is true, show other fields
        {
            settings.relativepositionX = EditorGUILayout.IntField("Relative Position on X axis",settings.relativepositionX, new GUILayoutOption[] { });
            settings.relativepositionY = EditorGUILayout.IntField("Relative Position on Y axis", settings.relativepositionY, new GUILayoutOption[] { });
        }
    }
}
#endif

