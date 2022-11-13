using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
    public class BulletSource : MonoBehaviour
    {
		[SerializeField]
		BulletSpawner spawner = new BulletSpawner();

		private void FixedUpdate()
		{
			if (DanmakuSettings.Current.useFixedUpdate)
			{
				spawner.Position = transform.position;
				spawner.Rotation = transform.rotation;
				spawner.Update();
			}
		}

		private void Update()
		{
			if (!DanmakuSettings.Current.useFixedUpdate)
			{
				spawner.Position = transform.position;
				spawner.Rotation = transform.rotation;
				spawner.Update();
			}
		}

		private void OnDrawGizmosSelected()
		{
			spawner.DrawGizmos();
		}
	}
}
