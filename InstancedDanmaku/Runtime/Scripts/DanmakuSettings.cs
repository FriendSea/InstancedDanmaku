using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace InstancedDanmaku
{
	[CreateAssetMenu(fileName = "New Bullet Settings", menuName = "FriendSea/DanmakuSetting")]
	public class DanmakuSettings : ScriptableObject
	{
		[SerializeField]
		internal Danmaku.Settings settings = new Danmaku.Settings();

		static DanmakuSettings _instance = null;
		public static DanmakuSettings Instance {
#if UNITY_EDITOR
			get => _instance ?? (_instance =
				(PlayerSettings.GetPreloadedAssets().FirstOrDefault(a => a is DanmakuSettings) as DanmakuSettings) ??
				DefaultForEditor);
#else
			get => _instance ?? (_instance = Resources.FindObjectsOfTypeAll<DanmakuSettings>()[0]);
#endif
		}

#if UNITY_EDITOR
		static DanmakuSettings _defaultForEditor = null;
		internal static DanmakuSettings DefaultForEditor => _defaultForEditor ?? (_defaultForEditor =  AssetDatabase.LoadAssetAtPath<DanmakuSettings>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"DefaultDanmakuSetting t:{nameof(DanmakuSettings)}")[0])));
#endif

		Danmaku _danmaku = null;
		public Danmaku Danmaku => _danmaku ?? (_danmaku = new Danmaku(settings));
	}
}
