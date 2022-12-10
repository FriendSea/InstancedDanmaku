using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[CreateAssetMenu(fileName = "New Bullet Settings", menuName = "FriendSea/DanmakuSetting")]
	public class DanmakuSettings : ScriptableObject
	{
		[System.Serializable]
		public enum Difficulty
		{
			Easy,
			Normal,
			Hard,
			Lunatic,
		}

		public static Difficulty CurrentDifficulty { get; set; }

		[SerializeField]
		internal Danmaku.Settings settings;

		static DanmakuSettings _instance = null;
		public static DanmakuSettings Instance {
			get => _instance ?? (_instance = Resources.FindObjectsOfTypeAll<DanmakuSettings>()[0]);
		}

		Danmaku _danmaku = null;
		public Danmaku Danmaku => _danmaku ?? (_danmaku = new Danmaku(settings));
	}
}
