using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.Linq;

namespace InstancedDanmaku
{
	public struct Bullet
	{
		public IBulletBehaviour behaviour;
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 velocity;
		public Vector4 color;
		public float scale;
		public int CurrentFrame { get; private set; }
		public bool Active { get; private set; }
		internal bool Used { get; set; }

		public delegate void BulletProcess(ref Bullet bullet);

		internal Bullet(Vector3 position, Quaternion rotation, Vector3 velocity, Vector4 color, IBulletBehaviour behaviour)
		{
			this.behaviour = behaviour;
			this.position = position;
			this.rotation = rotation;
			this.velocity = velocity;
			this.scale = 1f;
			this.color = color;
			Used = true;
			Active = true;
			CurrentFrame = 0;
		}

		internal void Update()
		{
			if (!Active) return;
			behaviour.UpdateBullet(ref this);
			position += velocity;
			CurrentFrame++;
		}
		public void Destroy() => Active = false;
	}

	public interface IBulletBehaviour
	{
		void UpdateBullet(ref Bullet bullet);
	}

	public interface IBulletCollider
	{
		void Collide(Bullet bullet);
		bool DeleteBullet { get; }
	}

	class BulletGroup : System.IDisposable
	{
		const int MAX_BULLETS = 1023;

		public BulletModel Model { get; }
		public Danmaku Parent { get; }

		Bullet[] bullets;
		Matrix4x4[] matricies;
		Vector4[] colors;
#if !BULLETS_DISABLE_COLLISON_JOB
		NativeArray<SpherecastCommand> raycastCommands;
#endif
		NativeArray<RaycastHit> raycastHits;
		internal Stack<int> Unused { get; }

		internal bool IsFull => Unused.Count <= 0;
		internal bool IsEmpty => Unused.Count >= MAX_BULLETS;

		internal BulletGroup(BulletModel model, Danmaku parent)
		{
			this.Model = model;
			this.Parent = parent;

			bullets = new Bullet[MAX_BULLETS];
			matricies = new Matrix4x4[MAX_BULLETS];
			colors = new Vector4[MAX_BULLETS];

#if !BULLETS_DISABLE_COLLISON_JOB
			raycastCommands = new NativeArray<SpherecastCommand>(MAX_BULLETS, Allocator.Persistent);
#endif
			raycastHits = new NativeArray<RaycastHit>(MAX_BULLETS, Allocator.Persistent);

			Unused = new Stack<int>(Enumerable.Range(0, MAX_BULLETS));
		}

		internal void AddNewBullet(Vector3 position, Quaternion rotation, Color color, IBulletBehaviour behaviour, Vector3 velocity = default)
		{
			var index = Unused.Pop();
			bullets[index] = new Bullet(position, rotation, velocity, new Vector4(color.r, color.g, color.b, color.a), behaviour);
		}

		List<IBulletCollider> collisionTargets = new List<IBulletCollider>();
		internal void UpdateBullets()
		{
			var camDir = Camera.main.transform.forward.normalized;

			for (int i = 0; i < bullets.Length; i++)
			{
				if (!bullets[i].Active && bullets[i].Used)
				{
					bullets[i].Used = false;
					Unused.Push(i);

					if (Model.VanishEffect)
						Parent.AddBullet(Parent.CurrentSettings.vanishEffect, bullets[i].position, Quaternion.identity, bullets[i].color, Parent.CurrentSettings.vanishBulletBehaviour);
				}

				bullets[i].Update();
				colors[i] = bullets[i].color;
				matricies[i] = bullets[i].Active ? Matrix4x4.TRS(bullets[i].position, bullets[i].rotation, Model.Scale * bullets[i].scale) : Matrix4x4.zero;
#if !BULLETS_DISABLE_COLLISON_JOB
				raycastCommands[i] = bullets[i].Used ? new SpherecastCommand(bullets[i].position - camDir * Parent.CurrentSettings.collisionDepth, Model.Radius, camDir, Parent.CurrentSettings.collisionDepth * 2f, Parent.CurrentSettings.collisionMask) : new SpherecastCommand();
#endif
			}
		}


#if BULLETS_DISABLE_COLLISON_JOB
		Collider[] colliders = new Collider[4];
#endif
		public void CollisionBullets()
		{
#if BULLETS_DISABLE_COLLISON_JOB
			if (Model.Radius <= 0) return;
			for (int i = 0; i < bullets.Length; i++)
			{
				if (!bullets[i].Used) continue;
				var count = Physics.OverlapSphereNonAlloc(bullets[i].position, Model.Radius, colliders, Parent.CurrentSettings.collisionMask, QueryTriggerInteraction.Ignore);
				if (count <= 0) continue;
				DoCollision(colliders[0], ref bullets[i]);
			}
#else
			SpherecastCommand.ScheduleBatch(raycastCommands, raycastHits, 20).Complete();
			for (int i = 0; i < bullets.Length; i++)
				if (raycastCommands[i].radius > 0)
					if (raycastHits[i].collider != null)
						DoCollision(raycastHits[i].collider, ref bullets[i]);
#endif
		}

		void DoCollision(Collider collider, ref Bullet bullet)
		{
			var delete = false;
			collider.GetComponentsInChildren<IBulletCollider>(collisionTargets);
			if (collisionTargets.Count <= 0)
				delete = true;
			else foreach (var target in collisionTargets)
				{
					target.Collide(bullet);
					delete |= target.DeleteBullet;
				}
			if (delete)
				bullet.Destroy();
		}

		MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
		internal void Render()
		{
			propertyBlock.Clear();
			propertyBlock.SetVectorArray("_Color", colors);
			propertyBlock.SetTexture("_MainTex", Model.Texture);
			Graphics.DrawMeshInstanced(Model.Mesh, 0, Model.Material, matricies, MAX_BULLETS, propertyBlock);
		}

		internal void DrawGizmos()
		{
			Gizmos.color = Color.green;
			foreach (var bul in bullets)
				if (bul.Active)
					Gizmos.DrawWireSphere(bul.position, Model.Radius);
		}

		public void Dispose()
		{
#if !BULLETS_DISABLE_COLLISON_JOB
			raycastCommands.Dispose();
#endif
			raycastHits.Dispose();
		}
	}

	[System.Serializable]
	class VanishEffectBehaviour : IBulletBehaviour
	{
		[SerializeField]
		int lifeTime = 30;

		public bool VanishEffect => false;

		public void UpdateBullet(ref Bullet bullet)
		{
			var col = bullet.color;
			col.w = 0.99f - (float)bullet.CurrentFrame / lifeTime;
			bullet.color = col;

			if (bullet.CurrentFrame > lifeTime)
				bullet.Destroy();
		}
	}

	public class Danmaku : System.IDisposable
	{
		[System.Serializable]
		public class Settings
		{
			[SerializeField]
			public int collisionMask = 1;
			[SerializeField]
			public float collisionDepth = 1000f;
			[SerializeField]
			public BulletModel vanishEffect;
			[SerializeReference, BulletBehaviourSelector]
			public IBulletBehaviour vanishBulletBehaviour = new VanishEffectBehaviour();
			[SerializeField]
			public SerializablePlayerLoop updateMethod = new SerializablePlayerLoop();
		}

		public Settings CurrentSettings { get; } = null;

		public Danmaku(Settings settings)
		{
			CurrentSettings = settings;
			settings.updateMethod.OnPlayerLoop += UpdateAndCollision;
			PlayerLoopInjector.AddAction(typeof(UnityEngine.PlayerLoop.PostLateUpdate.UpdateAllRenderers), Render);
#if UNITY_EDITOR
			UnityEditor.EditorApplication.playModeStateChanged += change =>
			{
				if (change == UnityEditor.PlayModeStateChange.EnteredEditMode)
					Dispose();
			};
#endif
		}

		void UpdateAndCollision()
		{
			if (Mathf.Approximately(Time.timeScale, 0f)) return;
			Update();
			Colission();
		}

		List<BulletGroup> groups = new List<BulletGroup>();

		public void AddBullet(BulletModel model, Vector3 position, Quaternion rotation, Color color, IBulletBehaviour behaviour, Vector3 velocity = default)
		{
			BulletGroup group = null;
			foreach (var g in groups)
			{
				if (g.Model != model) continue;
				if (g.IsFull) continue;
				group = g;
				break;
			}
			if (group == null)
			{
				group = new BulletGroup(model, this);
				groups.Add(group);
			}

			group.AddNewBullet(position, rotation, color, behaviour, velocity);
		}

		public void Update()
		{
			for (int i = 0; i < groups.Count; i++)
				groups[i].UpdateBullets();
			for (int i = groups.Count - 1; i >= 0; i--)
			{
				if (groups[i].IsEmpty)
				{
					groups[i].Dispose();
					groups.RemoveAt(i);
				}
			}
		}

		public void Colission()
		{
			foreach (var g in groups)
				g.CollisionBullets();
		}

		public void Render()
		{
			foreach (var g in groups)
				g.Render();
		}

		public void DrawGizmos()
		{
#if UNITY_EDITOR
			foreach (var g in groups)
				g.DrawGizmos();
#endif
		}

		public void Clear()
		{
			foreach (var group in groups)
				group.Dispose();
			groups.Clear();
		}

		bool disposed = false;
		public void Dispose()
		{
			if (disposed) return;
			disposed = true;
			Clear();

			CurrentSettings.updateMethod.OnPlayerLoop -= UpdateAndCollision;
			PlayerLoopInjector.RemoveAction(typeof(UnityEngine.PlayerLoop.PostLateUpdate.UpdateAllRenderers), Render);
		}

		~Danmaku()
		{
			Dispose();
		}
	}
}
