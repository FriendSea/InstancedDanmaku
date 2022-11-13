using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
	class DefaultBehaviour : IBulletBehaviour
	{
		[SerializeField]
		int lifeTime = 1000;
		[SerializeReference, BulletBehaviourSelector]
		IBulletBehaviour[] behaviours;

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			foreach (var b in behaviours)
				b.UpdateBullet(ref bullet);
			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}
}
