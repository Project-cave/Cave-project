using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public static SlotManager instance;

    private readonly Dictionary<int, Slot> listSlots = new();
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RegisterListSlot(int skillId, Slot slot) => listSlots[skillId] = slot;
    public Slot GetListSlot(int index) => listSlots.TryGetValue(index, out var slot) ? slot : null;
    public void ClearListSlots() => listSlots.Clear();
}
