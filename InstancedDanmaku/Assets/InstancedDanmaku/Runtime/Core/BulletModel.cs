using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[CreateAssetMenu(fileName = "New Bullet Model", menuName = "FriendSea/BulletModel")]
	public class BulletModel : ScriptableObject
	{
		[SerializeField]
		internal Mesh mesh;
		[SerializeField]
		internal Texture2D texture;
		[SerializeField]
		internal Vector3 scale = Vector3.one;
		[SerializeField]
		internal Material material;
		[SerializeField]
		internal float radius = 0.1f;
		[SerializeField]
		internal bool vanishEffect = true;
	}
}
