using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyEditor
{
    static bool _hierarchyHasFocus = false;
    static EditorWindow _hierarchyEditorWindow;

    static HierarchyEditor()
    {

        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        EditorApplication.update += OnEditorUpdate;

    }

    private static void OnEditorUpdate()
    {
        if (_hierarchyEditorWindow == null)
        {
            _hierarchyEditorWindow = Resources.FindObjectsOfTypeAll<EditorWindow>()
                .FirstOrDefault(window => window.GetType().Name == "SceneHierarchyWindow");
        }

        if (_hierarchyEditorWindow != null)
        {
            _hierarchyHasFocus = EditorWindow.focusedWindow == _hierarchyEditorWindow;
        }
        // if (!_hierarchyEditorWindow)
        // {
        //     _hierarchyEditorWindow = EditorWindow.GetWindow(Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor"));
        // }
        // _hierarchyHasFocus = EditorWindow.focusedWindow != null && EditorWindow.focusedWindow == _hierarchyEditorWindow;
    }

    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)

    {

        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (obj == null)
            return;

        if (PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj))
            return;

        Component[] components = obj.GetComponents<Component>();

        if (components == null || components.Length == 0)
            return;

        Component component = components.Length > 1 ? components[1] : components[0];

        Type type = component.GetType();



        GUIContent content = EditorGUIUtility.ObjectContent(component, type);

        content.text = null;

        content.tooltip = type.Name;

        if (!content.image)
        {
            return;
        }

        bool isSelected = Selection.instanceIDs.Contains(instanceID);
        bool isHovering = selectionRect.Contains(Event.current.mousePosition);

        Color color = UnityEditorBackgroundColor.Get(isSelected, isHovering, _hierarchyHasFocus);
        Rect backgroundRect = selectionRect;
        backgroundRect.width = 18.5f;
        EditorGUI.DrawRect(backgroundRect, color);

        EditorGUI.LabelField(selectionRect, content);

    }
}