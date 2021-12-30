using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;

namespace InstancedDanmaku
{
	[CustomPropertyDrawer(typeof(BulletBehaviourSelector))]
	public class BulletBehaviourEditor : PropertyDrawer
	{
		static List<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assem => assem.GetTypes()).
						Where(t => typeof(IBulletBehaviour).IsAssignableFrom(t) && (t != typeof(IBulletBehaviour))).ToList();

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var index = types.Select(type => $"{type.Assembly.ToString().Split(',').FirstOrDefault()} {type.FullName}".Replace('+', '/')).ToList().IndexOf(property.managedReferenceFullTypename);

			var popUpRect = position;
			popUpRect.height = EditorGUIUtility.singleLineHeight + 2f;

			int newIndex;
			newIndex = EditorGUI.Popup(popUpRect, label.text, index, types.Select(type => type.Name).ToArray());

			if (index != newIndex)
				property.managedReferenceValue = Activator.CreateInstance(types[newIndex]);

			EditorGUI.PropertyField(position, property, true);
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, true);
		}
	}
}
