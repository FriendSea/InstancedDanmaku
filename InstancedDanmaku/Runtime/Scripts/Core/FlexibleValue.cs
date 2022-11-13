using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InstancedDanmaku
{
	[System.Serializable]
	public struct FlexibleCurve
	{
		[SerializeField]
		bool difficultySwitch;

		[SerializeField]
		AnimationCurve easyCurve;
		[SerializeField]
		AnimationCurve normalCurve;
		[SerializeField]
		AnimationCurve hardCurve;
		[SerializeField]
		AnimationCurve lunaticCurve;

		public float GetValue(int frame)
		{
			if (difficultySwitch)
			{
				switch (DanmakuSettings.CurrentDifficulty)
				{
					case DanmakuSettings.Difficulty.Easy:
						return easyCurve.Evaluate(frame);
					case DanmakuSettings.Difficulty.Normal:
						return normalCurve.Evaluate(frame);
					case DanmakuSettings.Difficulty.Hard:
						return hardCurve.Evaluate(frame);
					case DanmakuSettings.Difficulty.Lunatic:
						return lunaticCurve.Evaluate(frame);
				}
			}
			return normalCurve.Evaluate(frame);
		}

		public float GetTangent(int frame)
		{
			return GetValue(frame) - GetValue(Mathf.Max(frame - 1, 0));
		}

		public int GetInt(int frame) => Mathf.RoundToInt(GetValue(frame));
	}

	[System.Serializable]
	public struct FlexibleValue
	{
		[SerializeField]
		bool difficultySwitch;
		[SerializeField]
		bool isCurve;

		[SerializeField]
		float easy;
		[SerializeField]
		float normal;
		[SerializeField]
		float hard;
		[SerializeField]
		float lunatic;

		[SerializeField]
		AnimationCurve easyCurve;
		[SerializeField]
		AnimationCurve normalCurve;
		[SerializeField]
		AnimationCurve hardCurve;
		[SerializeField]
		AnimationCurve lunaticCurve;

		public float GetValue(int frame)
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
			if (!isCurve) return 0;
			return GetValue(frame) - GetValue(Mathf.Max(frame - 1, 0));
		}

		public int GetInt(int frame) => Mathf.RoundToInt(GetValue(frame));

		public static FlexibleValue Curve => new FlexibleValue() { 
			isCurve = true,
			normalCurve = new AnimationCurve(new Keyframe(0,0), new Keyframe(100,1)),
		};
	}
}
