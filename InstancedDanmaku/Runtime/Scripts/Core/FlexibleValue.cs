using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("InstancedDanmaku.Editor")]

namespace InstancedDanmaku
{
	[System.Serializable]
	public class FlexibleValueWithoutModfier
	{
		[SerializeField]
		protected bool difficultySwitch;
		[SerializeField]
		protected bool isCurve;

		[SerializeField]
		protected float easy;
		[SerializeField]
		protected float normal;
		[SerializeField]
		protected float hard;
		[SerializeField]
		protected float lunatic;

		[SerializeField]
		protected AnimationCurve easyCurve;
		[SerializeField]
		protected AnimationCurve normalCurve;
		[SerializeField]
		protected AnimationCurve hardCurve;
		[SerializeField]
		protected AnimationCurve lunaticCurve;

		public virtual float GetValue(int frame)
		{
			if (difficultySwitch)
			{
				switch (DanmakuSettings.CurrentDifficulty)
				{
					case DanmakuSettings.Difficulty.Easy:
						return isCurve ? easyCurve.Evaluate(frame) : easy;
					case DanmakuSettings.Difficulty.Normal:
						return isCurve ? normalCurve.Evaluate(frame) : normal;
					case DanmakuSettings.Difficulty.Hard:
						return isCurve ? hardCurve.Evaluate(frame) : hard;
					case DanmakuSettings.Difficulty.Lunatic:
						return isCurve ? lunaticCurve.Evaluate(frame) : lunatic;
				}
			}
			return isCurve ? normalCurve.Evaluate(frame) : normal;
		}

		public float GetTangent(int frame)
		{
			// AddDeltaの都合で先に評価してほしい
			var before = GetValue(Mathf.Max(frame - 1, 0));
			return GetValue(frame) - before;
		}

		public int GetInt(int frame) => Mathf.RoundToInt(GetValue(frame));
	}

	[System.Serializable]
	public class AddRandom : FlexibleValueWithoutModfier, FlexibleValue.IModifier
	{
		public float ModifyValue(float original, int frame)
		{
			var val = GetValue(frame);
			return original + Random.Range(-val, +val);
		}
	}

	[System.Serializable]
	public class AddDelta : FlexibleValueWithoutModfier, FlexibleValue.IModifier
	{
		int cacheFrame;
		float cacheValue;

		public float ModifyValue(float original, int frame)
		{
			if (frame == 0)
			{
				cacheFrame = 0;
				cacheValue = GetValue(0);
				return original + cacheValue;
			}

			if(frame < cacheFrame)
				throw new System.NotImplementedException();
			
			while(cacheFrame < frame)
			{
				cacheFrame++;
				cacheValue += GetValue(cacheFrame);
			}
			return original + cacheValue;
		}
	}

	[System.Serializable]
	public class FlexibleValue : FlexibleValueWithoutModfier
	{
		public interface IModifier
		{
			public float ModifyValue(float original, int frame);
		}

		[SerializeReference]
		internal IModifier[] modifiers;

		public override float GetValue(int frame)
		{
			var val = base.GetValue(frame);
			foreach (var mod in modifiers)
				val = mod.ModifyValue(val, frame);
			return val;
		}

		public FlexibleValue(float value, int frame = 100)
		{
			isCurve = difficultySwitch = false;
			easy = normal = hard = lunatic = value;
			easyCurve = normalCurve = hardCurve = lunaticCurve = AnimationCurve.Linear(0, 0, frame, value);
		}

		public static FlexibleValue Curve(float value) =>
			new FlexibleValue(value, 100) { isCurve = true };
	}
}
