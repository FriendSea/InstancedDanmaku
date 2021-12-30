using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class DanmakuSettings : ScriptableObject
	{
		[SerializeField]
		internal int collisionMask = -1;
		[SerializeField]
		internal bool useFixedUpdate;
		[SerializeField]
		internal BulletModel vanishEffect;
		[SerializeReference, BulletBehaviourSelector]
		internal IBulletBehaviour vanishBulletBehaviour = new VanishEffectBehaviour();

		static DanmakuSettings _instance = null;
		internal static DanmakuSettings Instance => (_instance == null) ? (_instance = Resources.Load<DanmakuSettings>("InstancedDanmaku/Setting")) : _instance;
	}
}
