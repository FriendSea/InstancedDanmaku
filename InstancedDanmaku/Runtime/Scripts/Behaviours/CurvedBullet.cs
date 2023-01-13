using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
	class CruvedBullet : IBulletBehaviour
	{
		[SerializeField]
		FlexibleValue curve;// = FlexibleValue.Curve(180);

		public bool VanishEffect => true;

		public void UpdateBullet(ref Bullet bullet)
		{
			var speed = bullet.velocity.magnitude;
			bullet.rotation = Quaternion.Normalize(bullet.rotation * Quaternion.Euler(curve.GetTangent(bullet.CurrentFrame), 0, 0));
			bullet.velocity = bullet.rotation * Vector3.forward * speed;
		}
	}
}
