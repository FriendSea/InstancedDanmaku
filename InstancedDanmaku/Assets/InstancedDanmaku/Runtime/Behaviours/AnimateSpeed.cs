using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class AnimateSpeed : IBulletBehaviour
	{
		[SerializeField]
		AnimationCurve curve;

		public void UpdateBullet(ref Bullet bullet)
		{
			bullet.velocity = bullet.rotation * Vector3.forward * curve.Evaluate(bullet.CurrentFrame);
			if (bullet.CurrentFrame >= curve[curve.length - 1].time)
				bullet.Destroy();
		}
	}
}
