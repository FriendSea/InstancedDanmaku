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
		internal Vector3 scale;
		[SerializeField]
		internal Material material;
	}
}
