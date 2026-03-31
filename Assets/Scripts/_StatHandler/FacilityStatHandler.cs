using UnityEngine;

public class FacilityStatHandler : StatHandler
{
    [Header("Data")]
    [SerializeField] public FacilityData facilityData;

    private void Start()
    {
        OnDeath += Die;

        InitializeStats();
    }

    private void OnDestroy()
    {
        OnDeath -= Die;
    }

    public void InitializeStats()
    {
        if (facilityData != null)
        {
            MaxHP = facilityData.baseHP;
            CurrentHP = MaxHP;
            isDead = false;
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}