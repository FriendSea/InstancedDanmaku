using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class BulletCancel : MonoBehaviour, IBulletCollider
	{
		public bool DeleteBullet(BulletModel model) => true;

		public void Collide(ref Bullet bullet, BulletModel model)
		{
			
		}
	}
}
