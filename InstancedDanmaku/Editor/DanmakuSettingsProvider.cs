using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Linq;

namespace InstancedDanmaku
{
	public class DanmakuSettingsProvider
	{
		static Editor cachedEditor = null;

		[SettingsProvider]
		public static SettingsProvider CreateSettingProvider()
		{
			return new SettingsProvider("Project/Instanced Danmaku", SettingsScope.Project)
			{
				label = "Instanced Danmaku",
				guiHandler = ctx => {
					var current = PlayerSettings.GetPreloadedAssets().FirstOrDefault(a => a is DanmakuSettings) as DanmakuSettings;
					var newObj = EditorGUILayout.ObjectField(new GUIContent("Setting Asset"), current, typeof(DanmakuSettings), false);
					if(newObj == null || newObj == DanmakuSettings.DefaultForEditor)
					{
						if(GUILayout.Button("Create Danmaku Settings Asset"))
						{
							newObj = Object.Instantiate(DanmakuSettings.DefaultForEditor);
							ProjectWindowUtil.CreateAsset(newObj, "New Danmaku Settings.asset");
						}
					}
					if (current != newObj)
					{
						var assets = PlayerSettings.GetPreloadedAssets().Where(a => !(a is DanmakuSettings)).ToList();
						if (newObj != null)
							assets.Add(newObj);
						PlayerSettings.SetPreloadedAssets(assets.ToArray());
					}

					EditorGUILayout.Space();

					if (newObj == null) return;
					using var disabled = new EditorGUI.DisabledGroupScope(newObj == DanmakuSettings.DefaultForEditor);
					Editor.CreateCachedEditor(newObj, null, ref cachedEditor);
					EditorGUI.indentLevel++;
					cachedEditor.OnInspectorGUI();
					EditorGUI.indentLevel--;
				},
			};
		}
	}

	class DanmakuSettingPreprocessor : IPreprocessBuildWithReport
	{
		public int callbackOrder => 0;
		public void OnPreprocessBuild(BuildReport report)
		{
			var assets = PlayerSettings.GetPreloadedAssets();
			if (assets.Any(a => a is DanmakuSettings)) return;

			assets = assets.Append(DanmakuSettings.DefaultForEditor).ToArray();
			PlayerSettings.SetPreloadedAssets(assets);
		}
	}
}