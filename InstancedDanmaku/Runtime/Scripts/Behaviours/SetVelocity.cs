using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class SetVelocity : IBulletBehaviour
	{
		[SerializeField]
		FlexibleValue x;
		[SerializeField]
		FlexibleValue y;

		public void UpdateBullet(ref Bullet bullet)
		{
			bullet.velocity = new Vector3(x.GetValue(bullet.CurrentFrame), y.GetValue(bullet.CurrentFrame));
		}
	}
}
