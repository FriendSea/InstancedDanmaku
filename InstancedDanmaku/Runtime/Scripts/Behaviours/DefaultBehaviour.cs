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
		[SerializeField]
		bool updateRotation;
		[SerializeReference, BulletBehaviourSelector]
		IBulletBehaviour[] behaviours;

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			foreach (var b in behaviours)
				b.UpdateBullet(ref bullet);
			if (updateRotation)
				bullet.rotation = Quaternion.LookRotation(bullet.velocity, Vector3.forward);
			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}
}
