using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InstancedDanmaku
{
	[CustomPropertyDrawer(typeof(FlexibleCurve))]
	public class FlexibleCurveEditor : PropertyDrawer
	{
		const float dropDownWidth = 20f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var switchProp = property.FindPropertyRelative("difficultySwitch");

			EditorGUI.LabelField(position, label);

			position.x += EditorGUIUtility.labelWidth;
			position.width -= EditorGUIUtility.labelWidth;
			position.width -= dropDownWidth;

			var count = (switchProp.boolValue ? 4 : 1);
			position.width /= count;
			if (switchProp.boolValue)
			{
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative("easyCurve"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative("normalCurve"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative("hardCurve"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative("lunaticCurve"), GUIContent.none);
				position.x += position.width;
			}
			else
			{
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative("normalCurve"), GUIContent.none);
				position.x += position.width;
			}

			position.width = dropDownWidth;

			if (GUI.Button(position, new GUIContent("«")))
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("DifficultySwitch"), switchProp.boolValue, () =>
				{
					switchProp.serializedObject.Update();
					switchProp.boolValue = !switchProp.boolValue;
					switchProp.serializedObject.ApplyModifiedProperties();
				});
				menu.DropDown(position);
			}
		}

		Rect MakeMichiMichiWidth(Rect position)
		{
			position.x -= 10f;
			position.width += 20f;
			return position;
		}
	}

	[CustomPropertyDrawer(typeof(FlexibleValue))]
	public class FlexibleValueEditor : PropertyDrawer
	{
		const float dropDownWidth = 20f;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var switchProp = property.FindPropertyRelative("difficultySwitch");
			var iscurveProp = property.FindPropertyRelative("isCurve");

			EditorGUI.LabelField(position, label);

			position.x += EditorGUIUtility.labelWidth;
			position.width -= EditorGUIUtility.labelWidth;
			position.width -= dropDownWidth;

			var count = (switchProp.boolValue ? 4 : 1);
			position.width /= count;
			if (switchProp.boolValue)
			{
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(iscurveProp.boolValue ? "easyCurve" : "easy"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(iscurveProp.boolValue ? "normalCurve" : "normal"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(iscurveProp.boolValue ? "hardCurve" : "hard"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(iscurveProp.boolValue ? "lunaticCurve" : "lunatic"), GUIContent.none);
				position.x += position.width;
			}
			else
			{
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(iscurveProp.boolValue ? "normalCurve" : "normal"), GUIContent.none);
				position.x += position.width;
			}

			position.width = dropDownWidth;

			if (GUI.Button(position, new GUIContent("«")))
			{
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("DifficultySwitch"), switchProp.boolValue, () =>
				{
					switchProp.serializedObject.Update();
					switchProp.boolValue = !switchProp.boolValue;
					switchProp.serializedObject.ApplyModifiedProperties();
				});
				menu.AddItem(new GUIContent("Curve"), iscurveProp.boolValue, () =>
				{
					iscurveProp.serializedObject.Update();
					iscurveProp.boolValue = !iscurveProp.boolValue;
					iscurveProp.serializedObject.ApplyModifiedProperties();
				});
				menu.DropDown(position);
			}
		}

		Rect MakeMichiMichiWidth(Rect position)
		{
			position.x -= 10f;
			position.width += 20f;
			return position;
		}
	}
}
