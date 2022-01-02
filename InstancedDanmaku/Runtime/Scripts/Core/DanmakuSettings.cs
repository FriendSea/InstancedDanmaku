using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[CreateAssetMenu(fileName = "New Bullet Settings", menuName = "FriendSea/DanmakuSetting")]
	public class DanmakuSettings : ScriptableObject
	{
		[SerializeField]
		internal int collisionMask = 1;
		[SerializeField]
		internal bool useFixedUpdate;
		[SerializeField]
		internal float collisionDepth = 1f;
		[SerializeField]
		internal BulletModel vanishEffect;
		[SerializeReference, BulletBehaviourSelector]
		internal IBulletBehaviour vanishBulletBehaviour = new VanishEffectBehaviour();

		public static DanmakuSettings Current { get; set; }
	}
}
