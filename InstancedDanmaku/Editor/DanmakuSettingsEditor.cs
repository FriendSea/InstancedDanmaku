using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InstancedDanmaku
{
	[CustomEditor(typeof(DanmakuSettings))]
	public class DanmakuSettingsEditor : Editor
	{
        bool isOpen = false;
        public override void OnInspectorGUI()
		{
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings.updateMethod"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings.collisionDepth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings.vanishEffect"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings.vanishBulletBehaviour"));

            var maskproperty = serializedObject.FindProperty("settings.collisionMask");
            isOpen = EditorGUILayout.Foldout(isOpen, "CollisionMask : " + maskproperty.intValue.ToString());
            if (isOpen)
            {
                bool[] masklist = decodeMask(maskproperty.intValue);
                for (int i = 0; i < 32; i++)
                {
                    var name = LayerMask.LayerToName(i);
                    if (string.IsNullOrEmpty(name)) continue;
                    var value = EditorGUILayout.ToggleLeft(name, masklist[i]);
                    masklist[i] = value;
                }
                maskproperty.intValue = encodeMask(masklist);
            }

            serializedObject.ApplyModifiedProperties();
        }

        bool[] decodeMask(int mask)
        {
            bool[] list = new bool[32];
            for (int i = 0; i < 32; i++)
                list[i] = ((mask >> i) & 1) > 0;
            return list;
        }

        int encodeMask(bool[] list)
        {
            int mask = 0;
            for (int i = 0; i < 32; i++)
                if (list[i])
                    mask += (1 << i);
            return mask;
        }
    }
}
