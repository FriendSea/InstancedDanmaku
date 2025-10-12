using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class BulletCancel : MonoBehaviour, IBulletCollider
	{
		public bool DeleteBullet => true;

		public void Collide(ref Bullet bullet)
		{
			
		}
	}
}
