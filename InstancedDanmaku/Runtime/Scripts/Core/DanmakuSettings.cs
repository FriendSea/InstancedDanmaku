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
		[SerializeField]
		internal bool useFixedUpdate;

		static DanmakuSettings _current;
		public static DanmakuSettings Current {
			get => _current;
			set
			{
				_current = value;
				Danmaku.InstanceSetting = value.settings;
			}
		}
	}
}
