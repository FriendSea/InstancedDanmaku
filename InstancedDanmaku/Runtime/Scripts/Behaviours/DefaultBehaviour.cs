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
		int spawnFrame = 10;
		[SerializeField]
		bool updateRotation;
		[SerializeReference, BulletBehaviourSelector]
		IBulletBehaviour[] behaviours;

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			if (bullet.CurrentFrame <= spawnFrame && spawnFrame != 0)
			{
				bullet.scale = Mathf.Lerp(3f, 1f, (float)bullet.CurrentFrame / spawnFrame);
				var col = bullet.color;
				col.w = (float)bullet.CurrentFrame / spawnFrame;
				bullet.color = col;
			}
			foreach (var b in behaviours)
				b.UpdateBullet(ref bullet);
			if (updateRotation)
				bullet.rotation = Quaternion.LookRotation(bullet.velocity, Vector3.forward);
			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}
}
