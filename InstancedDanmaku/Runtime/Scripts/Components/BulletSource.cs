using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class BulletSource : MonoBehaviour
	{
		[SerializeField]
		BulletSpawner spawner = new BulletSpawner();

		private void Awake()
		{
			spawner.DanmakuInstance = DanmakuSettings.Instance.Danmaku;
		}

		private void OnEnable()
		{
			DanmakuSettings.Instance.Danmaku.CurrentSettings.updateMethod.OnPlayerLoop += UpdateSpawner;
		}

		private void OnDisable()
		{
			DanmakuSettings.Instance.Danmaku.CurrentSettings.updateMethod.OnPlayerLoop -= UpdateSpawner;
		}

		void UpdateSpawner()
		{
			spawner.Position = transform.position;
			spawner.Rotation = transform.rotation;
			spawner.Update();
		}

		private void OnDrawGizmosSelected()
		{
			spawner.DrawGizmos();
		}
	}
}
