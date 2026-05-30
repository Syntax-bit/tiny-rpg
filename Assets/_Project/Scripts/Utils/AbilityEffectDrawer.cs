using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using TinyRPG.Gameplay;

[CustomPropertyDrawer(typeof(SelectableEffectAttribute))]
public class AbilityEffectDrawer : PropertyDrawer
{
    static Dictionary<string, Type> typeMap;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (typeMap == null) BuildTypeMap();

        EditorGUI.BeginProperty(position, label, property);

        // 1. Draw the clean standard prefix label (e.g., "Element 0")
        Rect dropdownRect = EditorGUI.PrefixLabel(position, label);
        dropdownRect.height = EditorGUIUtility.singleLineHeight;

        var typeName = property.managedReferenceFullTypename;
        var displayName = GetShortTypeName(typeName);

        // 2. Draw the Type Dropdown Button right next to the label
        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(displayName ?? "Select Effect Type"), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            if (typeMap == null || typeMap.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No Effects found implementing IEffect<Unit>"));
                menu.ShowAsContext();
                return;
            }

            foreach (var kvp in typeMap)
            {
                var name = kvp.Key;
                var type = kvp.Value;
                menu.AddItem(new GUIContent(name), type.FullName == typeName, () => {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        // 3. Draw the exposed properties under the dropdown selector safely
        if (property.managedReferenceValue != null)
        {
            // Move our positioning down past the row height of the dropdown button
            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Subtract the dropdown height from the total height block passed to children fields
            position.height -= EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            EditorGUI.indentLevel++;
            // Draw the structural tree layout natively; passing true ensures fields are fully editable!
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;

        if (property.managedReferenceValue != null)
        {
            // Ask Unity exactly how tall the concrete class's variables are, and add spacing
            totalHeight += EditorGUI.GetPropertyHeight(property, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;
    }

    static void BuildTypeMap()
    {
        var baseType = typeof(IEffectFactory<Unit>);

        typeMap = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => {
                try { return asm.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(t => !t.IsAbstract && !t.IsInterface && baseType.IsAssignableFrom(t))
            .ToDictionary(t => ObjectNames.NicifyVariableName(t.Name).Replace(" Factory", ""), t => t);
    }

    static string GetShortTypeName(string fullTypeName)
    {
        if (string.IsNullOrEmpty(fullTypeName)) return null;
        var parts = fullTypeName.Split(' ');
        return parts.Length > 1 ? parts[1].Split('.').Last() : fullTypeName;
    }
}


[CustomPropertyDrawer(typeof(SelectableStrategyAttribute))]
public class StrategySelectorDrawer : PropertyDrawer
{
    static Dictionary<string, Type> strategyTypes;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (strategyTypes == null) BuildStrategyMap();

        EditorGUI.BeginProperty(position, label, property);

        // Draw a clean dropdown button row boundary block
        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        dropdownRect = EditorGUI.PrefixLabel(dropdownRect, label);

        var typeName = property.managedReferenceFullTypename;
        var displayName = GetShortTypeName(typeName);

        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent(displayName ?? "Select Targeting Strategy"), FocusType.Keyboard))
        {
            var menu = new GenericMenu();
            if (strategyTypes == null || strategyTypes.Count == 0)
            {
                menu.AddDisabledItem(new GUIContent("No strategies found inheriting TargetingStrategy"));
                menu.ShowAsContext();
                return;
            }

            foreach (var kvp in strategyTypes)
            {
                var name = kvp.Key;
                var type = kvp.Value;
                menu.AddItem(new GUIContent(name), type.FullName == typeName, () => {
                    property.managedReferenceValue = Activator.CreateInstance(type);
                    property.serializedObject.ApplyModifiedProperties();
                });
            }
            menu.ShowAsContext();
        }

        // Render exposed sub-fields (like AOE Radius, Ground Layer Masks, etc.)
        if (property.managedReferenceValue != null)
        {
            Rect fieldsRect = new Rect(
                position.x,
                position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                position.height - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)
            );

            EditorGUI.indentLevel++;
            EditorGUI.PropertyField(fieldsRect, property, GUIContent.none, true);
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = EditorGUIUtility.singleLineHeight;

        if (property.managedReferenceValue != null)
        {
            totalHeight += EditorGUI.GetPropertyHeight(property, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;
    }

    static void BuildStrategyMap()
    {
        var baseType = typeof(TargetingStrategy);
        strategyTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(asm => {
                try { return asm.GetTypes(); }
                catch { return Type.EmptyTypes; }
            })
            .Where(t => !t.IsAbstract && !t.IsInterface && baseType.IsAssignableFrom(t))
            .ToDictionary(t => ObjectNames.NicifyVariableName(t.Name).Replace(" Strategy", ""), t => t);
    }

    static string GetShortTypeName(string fullTypeName)
    {
        if (string.IsNullOrEmpty(fullTypeName)) return null;
        var parts = fullTypeName.Split(' ');
        string finalName = parts.Length > 1 ? parts[1].Split('.').Last() : fullTypeName;
        if (finalName.EndsWith("Strategy")) finalName = finalName.Substring(0, finalName.Length - 8);
        return finalName;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class SelectableEffectAttribute : PropertyAttribute { }

[AttributeUsage(AttributeTargets.Field)]
public class SelectableStrategyAttribute : PropertyAttribute { }