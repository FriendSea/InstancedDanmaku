using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	public class BulletSource : MonoBehaviour
	{
		[SerializeField]
		BulletSpawner spawner = new BulletSpawner();
		[SerializeField]
		bool deleteBulletsWithSource;

		public BulletSpawner Spawner => spawner;

		private void Awake()
		{
			spawner.DanmakuInstance = DanmakuSettings.Instance.Danmaku;
		}

		private void OnEnable()
		{
            spawner.OwnerId = this.GetInstanceID();
            DanmakuSettings.Instance.Danmaku.CurrentSettings.updateMethod.OnPlayerLoop += UpdateSpawner;
			spawner.Reset();
		}

		private void OnDisable()
		{
			DanmakuSettings.Instance.Danmaku.CurrentSettings.updateMethod.OnPlayerLoop -= UpdateSpawner;
			if (deleteBulletsWithSource)
				spawner.DeleteSpawnedBullets();
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

  		public void ResetSpawner(){
    		spawner.Reset();
		}
	}
}
