using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
	class DefaultBehaviour : IBulletBehaviour
	{
		[SerializeField]
		float speed = 1f;
		[SerializeField]
		int lifeTime = 600;

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			if (bullet.CurrentFrame == 0)
				bullet.velocity = bullet.rotation * Vector3.forward * speed;
			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}
}
