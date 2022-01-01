using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InstancedDanmaku
{
    [CustomEditor(typeof(BulletModel))]
    public class BulletModelEditor : Editor
    {
		BulletModel Model => target as BulletModel;

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("mesh"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("texture"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("material"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("scale"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("radius"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("vanishEffect"));
			serializedObject.ApplyModifiedProperties();
		}

		public override bool HasPreviewGUI() => true;

		Mesh _quad = null;
		Mesh Quad { get
			{
				if (_quad == null)
				{
					var prim = GameObject.CreatePrimitive(PrimitiveType.Quad);
					_quad = prim.GetComponent<MeshFilter>().sharedMesh;
					GameObject.DestroyImmediate(prim);
				}
				return _quad;
			} }
		PreviewRenderUtility renderer = null;
		MaterialPropertyBlock propertyBlock;
		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (renderer == null)
				renderer = new PreviewRenderUtility();
			if (propertyBlock == null)
				propertyBlock = new MaterialPropertyBlock();

			renderer.BeginPreview(r, GUIStyle.none);

			renderer.camera.farClipPlane = 100;
			renderer.camera.transform.position = new Vector3(0, 0, -10);
			renderer.camera.transform.rotation = Quaternion.identity;
			renderer.camera.clearFlags = CameraClearFlags.SolidColor;

			propertyBlock.Clear();
			propertyBlock.SetTexture("_MainTex", Model.Texture);
			propertyBlock.SetVectorArray("_Color", new Vector4[] { new Vector4(1, 0, 0, 1) });
			renderer.DrawMesh(Model.Mesh, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(90, 0, 0), Model.Scale), Model.Material, 0, propertyBlock);

			var mat = new Material(Shader.Find("Hidden/Bullets/CollisionPreview"));
			renderer.DrawMesh(Quad, Matrix4x4.Scale(Vector3.one * Model.Radius * 2f), mat, 0);

			renderer.camera.Render();
			renderer.EndAndDrawPreview(r);
		}

		private void OnDisable()
		{
			renderer?.Cleanup();
		}
	}
}
