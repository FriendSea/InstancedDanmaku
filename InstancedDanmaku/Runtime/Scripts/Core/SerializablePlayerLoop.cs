using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using System.Linq;

namespace InstancedDanmaku
{
    static class PlayerLoopInjector
	{
        static Dictionary<System.Type, System.Action> actions = new Dictionary<System.Type, System.Action>();

#if UNITY_EDITOR
        static PlayerLoopInjector()
		{
            UnityEditor.EditorApplication.playModeStateChanged += change => {
                if (change == UnityEditor.PlayModeStateChange.EnteredEditMode)
                    actions.Clear();
            };
		}
#endif

        public static void AddAction(System.Type timing, System.Action action)
        {
            if (!actions.ContainsKey(timing))
            {
                var loops = PlayerLoop.GetCurrentPlayerLoop();
                loops.subSystemList = loops.subSystemList.Select(set =>
                {
                    var index = set.subSystemList.Select(s => s.type).ToList().IndexOf(timing);
                    if (index < 0) return set;
                    set.subSystemList = set.subSystemList.Take(index + 1).Append(new PlayerLoopSystem()
                    {
                        type = typeof(SerializablePlayerLoop),
                        updateDelegate = () => {
                            if (actions.ContainsKey(timing))
                                actions[timing]?.Invoke();
                        }
                    }).Concat(set.subSystemList.Skip(index + 1)).ToArray();
                    return set;
                }).ToArray();
                PlayerLoop.SetPlayerLoop(loops);
                actions[timing] = null;
            }
            actions[timing] += action;
        }

        public static void RemoveAction(System.Type timing, System.Action action) {
            if (actions.ContainsKey(timing))
                actions[timing] -= action;
        }
    }

    [System.Serializable]
    public class SerializablePlayerLoop
    {
        [SerializeReference]
        object timing = new UnityEngine.PlayerLoop.Update.ScriptRunBehaviourUpdate();

        public event System.Action OnPlayerLoop
		{
            add => PlayerLoopInjector.AddAction(timing.GetType(), value);
            remove => PlayerLoopInjector.RemoveAction(timing.GetType(), value);
		}
    }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(SerializablePlayerLoop))]
    class SerializablePlayerLoopDrawer : UnityEditor.PropertyDrawer
	{
		public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
		{
            var prop = property.FindPropertyRelative("timing");
            var types = PlayerLoop.GetDefaultPlayerLoop().subSystemList.SelectMany(s =>s.subSystemList.Select(m => new {group = s.type, item = m.type })).Prepend(null).ToList();
            var currentIndex = types.Select(p => p?.item).ToList().IndexOf(prop.managedReferenceValue?.GetType());
            var newIndex = UnityEditor.EditorGUI.Popup(position, label.text, currentIndex, types.Select(t => t == null ? "<none>" : $"{t?.group?.Name }/{t?.item?.Name}").ToArray());
            if (newIndex != currentIndex)
                prop.managedReferenceValue = types[newIndex] == null ?
                    null :
                    System.Activator.CreateInstance(types[newIndex]?.item);
		}
	}
#endif
}
