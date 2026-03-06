using UnityEngine;
using UnityEngine.Events;

// skillEvent는 So라 Listener필요
public class SkillEventListener : MonoBehaviour
{
    [SerializeField] private SkillEvent skillEvent;

    // 인스펙터에서 연결할 응답 함수
    public UnityEvent<int, SkillData> response;

    private void OnEnable() => skillEvent.Register(this);
    private void OnDisable() => skillEvent.Unregister(this);

    public void OnEventRaised(int slotIndex, SkillData skill) => response.Invoke(slotIndex, skill);
}
