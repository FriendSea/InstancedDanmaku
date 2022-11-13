using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Burst;

namespace InstancedDanmaku
{
	public class AnimateSpeed : IBulletBehaviour
	{
		[SerializeField]
		FlexibleCurve curve;

		public void UpdateBullet(ref Bullet bullet)
		{
			bullet.velocity += bullet.rotation * Vector3.forward * curve.GetTangent(bullet.CurrentFrame);
		}
	}
}
