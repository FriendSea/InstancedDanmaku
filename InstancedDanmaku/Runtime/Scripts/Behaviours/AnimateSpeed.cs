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
			var delta = curve.Evaluate(bullet.CurrentFrame) - curve.Evaluate(Mathf.Max(bullet.CurrentFrame - 1, 0));
			bullet.velocity += bullet.rotation * Vector3.forward * delta;
			if (bullet.CurrentFrame >= curve[curve.length - 1].time)
				bullet.Destroy();
		}
	}
}
