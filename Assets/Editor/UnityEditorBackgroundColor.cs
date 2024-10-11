using UnityEditor;
using UnityEngine;

public static class UnityEditorBackgroundColor
{
    static readonly Color k_defaultColor = new Color(0.7843f, 0.7843f, 0.7843f);
    static readonly Color k_defaultProColor = new Color(0.2196f, 0.2196f, 0.2196f);

    static readonly Color k_selectedColor = new Color(0.22745f, 0.447f, 0.6902f);
    static readonly Color k_selectedProColor = new Color(0.1725f, 0.3647f, 0.5294f);

    static readonly Color k_selectedUnFocusedColor = new Color(0.68f, 0.68f, 0.68f);
    static readonly Color k_selectedUnFocusedProColor = new Color(0.3f, 0.3f, 0.3f);

    static readonly Color k_hoveredColor = new Color(0.698f, 0.698f, 0.698f);
    static readonly Color k_hoveredProColor = new Color(0.2706f, 0.2706f, 0.2706f);


    public static Color Get(bool isSelected, bool isHovered, bool iswindowFocused)

    {

        if (isSelected)

        {

            if (iswindowFocused)

            {

                return EditorGUIUtility.isProSkin ? k_selectedProColor : k_selectedColor;

            }

            else

            {

                return EditorGUIUtility.isProSkin ? k_selectedUnFocusedProColor : k_selectedUnFocusedColor;

            }

        }

        else if (isHovered)

        {

            return EditorGUIUtility.isProSkin ? k_hoveredProColor : k_hoveredColor;

        }

        else

        {

            return EditorGUIUtility.isProSkin ? k_defaultProColor : k_defaultColor;

        }
    }
}
