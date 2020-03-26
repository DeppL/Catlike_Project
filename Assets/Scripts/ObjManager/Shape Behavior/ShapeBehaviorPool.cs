using System.Collections.Generic;
using UnityEngine;

public static class ShapeBehaviorPool<T> where T : ShapeBehavior, new() {
    static Stack<T> stack = new Stack<T>();
    public static T Get ()
    {
        if (stack.Count > 0) {
            T behaviour = stack.Pop();
#if UNITY_EDITOR
            behaviour.IsReclaimed = false;
#endif
            return behaviour;
            
        }
#if UNITY_EDITOR
        return ScriptableObject.CreateInstance<T>();
#else
        return new T();
#endif
    }

    public static void Reclaim (T behaviour)
    {
#if UNITY_EDITOR
        behaviour.IsReclaimed = true;
#endif
        stack.Push(behaviour);
    }
}
