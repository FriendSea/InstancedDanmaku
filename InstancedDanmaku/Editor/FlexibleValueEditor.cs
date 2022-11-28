using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace InstancedDanmaku
{
	[CustomPropertyDrawer(typeof(FlexibleValueWithoutModfier), true)]
	public class FlexibleValueEditor : PropertyDrawer
	{
		const float dropDownWidth = 30f;

		System.Type[] _modTypes = null;
		System.Type[] ModTypes => _modTypes ?? (_modTypes = typeof(FlexibleValue).Assembly.GetTypes().Where(t => typeof(FlexibleValue.IModifier).IsAssignableFrom(t) && !t.IsInterface).ToArray());

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var switchProp = property.FindPropertyRelative("difficultySwitch");
			var isCurveProp = property.FindPropertyRelative("isCurve");
			var modifierProp = property.FindPropertyRelative("modifiers");
			var isCurve = isCurveProp?.boolValue ?? true;

			position.height = EditorGUIUtility.singleLineHeight;

			var originalPosition = position;

			EditorGUI.LabelField(position, label);

			position.x += EditorGUIUtility.labelWidth;
			position.width -= EditorGUIUtility.labelWidth;
			position.width -= dropDownWidth;

			var count = (switchProp.boolValue ? 4 : 1);
			position.width /= count;
			if (switchProp.boolValue)
			{
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(isCurve ? "easyCurve" : "easy"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(isCurve ? "normalCurve" : "normal"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(isCurve ? "hardCurve" : "hard"), GUIContent.none);
				position.x += position.width;
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(isCurve ? "lunaticCurve" : "lunatic"), GUIContent.none);
				position.x += position.width;
			}
			else
			{
				EditorGUI.PropertyField(MakeMichiMichiWidth(position), property.FindPropertyRelative(isCurve ? "normalCurve" : "normal"), GUIContent.none);
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
				if (isCurveProp != null)
					menu.AddItem(new GUIContent("Curve"), isCurveProp.boolValue, () =>
					{
						isCurveProp.serializedObject.Update();
						isCurveProp.boolValue = !isCurveProp.boolValue;
						isCurveProp.serializedObject.ApplyModifiedProperties();
					});
				if(modifierProp!=null)
					foreach(var type in ModTypes)
					{
						var list = Enumerable.Range(0, modifierProp.arraySize).Select(index => modifierProp.GetArrayElementAtIndex(index).managedReferenceValue.GetType()).ToList();
						var hasType = list.Contains(type);
						menu.AddItem(new GUIContent(type.Name), hasType, () =>
						{
							property.serializedObject.Update();
							if (hasType)
							{
								modifierProp.DeleteArrayElementAtIndex(list.IndexOf(type));
							}
							else
							{
								modifierProp.arraySize++;
								modifierProp.GetArrayElementAtIndex(modifierProp.arraySize - 1).managedReferenceValue = System.Activator.CreateInstance(type);
							}
							property.serializedObject.ApplyModifiedProperties();
						});
					}
				menu.DropDown(position);
			}

			if (modifierProp != null)
			{
				EditorGUI.indentLevel++;
				foreach(var mod in Enumerable.Range(0, modifierProp.arraySize).Select(index => modifierProp.GetArrayElementAtIndex(index)))
				{
					originalPosition.y += EditorGUIUtility.singleLineHeight + 2;
					EditorGUI.PropertyField(originalPosition, mod, new GUIContent(mod.managedReferenceValue.GetType().Name));
				}
				EditorGUI.indentLevel--;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label) * (1 + property.FindPropertyRelative("modifiers").arraySize);
		}

		Rect MakeMichiMichiWidth(Rect position)
		{
			position.x -= 7f;
			position.width += 14f;
			return position;
		}
	}
}
