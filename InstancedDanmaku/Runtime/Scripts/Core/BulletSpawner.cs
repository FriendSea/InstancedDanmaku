using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
    [System.Serializable]
    public class BulletSpawner
    {
        [SerializeField]
        int startFrame = 0;
        [SerializeField]
        public int loopLength = 0;
        [SerializeField]
        public int span = 5;
        [SerializeField]
        public int count = 0;

        [SerializeField]
        public FlexibleValue startSpeed;
        [SerializeField]
        public FlexibleValue ways;
        [SerializeField]
        public FlexibleValue angle;
        [SerializeField]
        public FlexibleValue subtendAngle;
        [SerializeField]
        public FlexibleValue positionOffset;
        [SerializeReference, BulletBehaviourSelector]
        IBulletBehaviour behaviour = new DefaultBehaviour();

        [SerializeField]
        BulletModel bulletModel;
        [SerializeField]
        public Color[] colors = new Color[] { Color.red };
        [SerializeField]
        UnityEngine.Events.UnityEvent onFire;

        [field: System.NonSerialized]
        public Vector3 Position { get; set; }
        [field: System.NonSerialized]
        public Quaternion Rotation { get; set; } = Quaternion.identity;

        public BulletModel Model => bulletModel;
        public IBulletBehaviour Behaviour => behaviour;

        public Danmaku DanmakuInstance { get; set; } = null;
        public int OwnerId { get; set; } = 0;

        int currentFrame;
        public void Update(int currentFrame)
        {
            if (currentFrame < startFrame) return;
            if (span <= 1)
                Fire(currentFrame - startFrame);
            else if ((currentFrame - startFrame) % span == 0)
                Fire(currentFrame - startFrame);
        }

        public void Update()
        {
            Update(currentFrame);
            currentFrame++;
            if (currentFrame == loopLength)
            {
                currentFrame = 0;
            }
        }

        void Fire(int currentFrame)
        {
            void AddBullet(Vector3 position, Quaternion rotation) =>
                DanmakuInstance.AddBullet(bulletModel, position + rotation * Vector3.forward * positionOffset.GetValue(currentFrame), rotation, colors[currentFrame / Mathf.Max(span, 1) % colors.Length], behaviour, OwnerId, rotation * Vector3.forward * startSpeed.GetValue(currentFrame));

            if (count > 0 && currentFrame >= Mathf.Max(span, 1) * count) return;

            for (int i = 0; i < ways.GetInt(currentFrame); i++)
            {
                float totalWidth = subtendAngle.GetValue(currentFrame) * (ways.GetInt(currentFrame) - 1);
                float ang = angle.GetValue(currentFrame) - totalWidth / 2f + i * subtendAngle.GetValue(currentFrame);
                var rot = Rotation * Quaternion.Euler(ang, 90f, 0);
                AddBullet(Position, rot);
            }

            onFire?.Invoke();
        }

        public void Reset() => currentFrame = 0;

        public void DeleteSpawnedBullets()
        {
            DanmakuInstance.DeleteWithOwner(OwnerId);
        }

        /*
		IEnumerable<Quaternion> GetRotations()
		{
			for (int i = 0; i < ways.GetInt(currentFrame); i++)
			{
				float totalWidth = angleDelta * (ways.GetInt(currentFrame) - 1);
				float ang = angle - totalWidth / 2f + i * angleDelta;
				yield return Rotation * Quaternion.Euler(ang, 90f, 0);
			}
		}
		*/

        public void DrawGizmos()
        {
#if UNITY_EDITOR
            /*
			Gizmos.color = Color.red;

			foreach(var ang in GetRotations())
			{
				var bullet = new Bullet(Position, ang, ang * Vector3.forward * startSpeed.GetValue(0), Vector4.zero, Behaviour);
				var beforepos = Position;
				for (int i = 0; i < 1000; i++)
				{
					bullet.Update();
					if (!bullet.Active) break;

					Gizmos.DrawLine(beforepos, bullet.position);
					beforepos = bullet.position;
				}
			}
			*/
#endif
        }
    }
}
