using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public int level;
    public float damage;
    public int count;
    public float speed;
    public float timer;
    Unit unit;
    UnitCombat unitCombat;
    Scanner scanner;

    public void Init(ItemData data)
    {
        unit = GameManager.instance.spawnUnit.GetComponent<Unit>();
        unitCombat = unit.GetComponent<UnitCombat>();
        scanner = unit.GetComponent<Scanner>();
        name = "Weapon " + data.itemId;
        transform.SetParent(unit.transform);
        transform.localPosition = new Vector3(0.5f,0,0);
        unit.SetWeapon(this);

        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for(int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            Debug.Log(data.prejectile.name);
            if(data.prejectile == GameManager.instance.pool.prefabs[index])
            {
                
                prefabId = index;
                break;
            }
        }

    }

    public void Fire()
    {
        if (!scanner.attackTarget)
            return;

        Vector3 targetPos = scanner.attackTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;

        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.rotation *= Quaternion.Euler(new Vector3(0, 0, 90));

        switch (id)
        {
            case 0:
                bullet.GetComponent<Bullet>().Init(id, damage, count, dir, 5 * unitCombat.bulletSpeed, "Enemy");
                break;
        }
    }
}
