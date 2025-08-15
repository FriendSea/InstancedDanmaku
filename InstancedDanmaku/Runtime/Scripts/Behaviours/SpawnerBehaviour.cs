using UnityEngine;

namespace InstancedDanmaku {

    public class SpawnerBehaviour : IBulletBehaviour
    {
        [SerializeField]
        BulletSpawner spawner;

        public void UpdateBullet(ref Bullet bullet)
        {
            spawner.DanmakuInstance = DanmakuSettings.Instance.Danmaku;
            spawner.Position = bullet.position;
            spawner.Rotation = bullet.rotation * Quaternion.Euler(0, 90, 0);
            spawner.Update(bullet.CurrentFrame);
        }
    }
}
