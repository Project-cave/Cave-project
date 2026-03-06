using System.Collections.Generic;
using UnityEngine;

// 이벤트 관리자 느낌?
[CreateAssetMenu(menuName = "Event/SkillEvent")]
public class SkillEvent : ScriptableObject
{
    private readonly List<SkillEventListener> listeners = new();

    public void Raise(int slotIndex, SkillData skill)
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(slotIndex, skill);
    }

    public void Register(SkillEventListener listener) => listeners.Add(listener);
    public void Unregister(SkillEventListener listener) => listeners.Remove(listener);
}
