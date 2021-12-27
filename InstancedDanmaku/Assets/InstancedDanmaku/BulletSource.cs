using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
    public class BulletSource : MonoBehaviour
    {
		[SerializeField]
		BulletModel bulletModel;

		float angle;

		private void FixedUpdate()
		{
			angle += 5f;
			Danmaku.Instance.AddBullet(bulletModel, transform.position, Quaternion.Euler(0, 0, angle), Vector3.forward, Color.red);
		}
	}
}
