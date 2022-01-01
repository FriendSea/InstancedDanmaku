using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
	class CruvedBullet : IBulletBehaviour
	{
		[SerializeField]
		float speed = 1f;
		[SerializeField]
		AnimationCurve curve;

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			bullet.rotation *= Quaternion.Euler(curve.Evaluate(bullet.CurrentFrame), 0, 0);
			bullet.velocity = bullet.rotation * Vector3.forward * speed;
			if (bullet.CurrentFrame >= curve[curve.length - 1].time)
				bullet.Destroy();
		}
	}
}
