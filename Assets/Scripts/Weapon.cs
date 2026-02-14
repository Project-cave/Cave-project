using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    PlayerMovement player;
    Scanner scanner;

    public void Init(ItemData data)
    {
        scanner = GameManager.instance.spawnUnit.GetComponent<Scanner>();
        player = GameManager.instance.spawnUnit.GetComponent<PlayerMovement>();
        unit = player.gameObject.GetComponent<Unit>();
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0.5f,0,0);
        player.SetWeapon(this);

        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        for(int index = 0; index < GameManager.instance.pool.prefebs.Length; index++)
        {
            Debug.Log(data.prejectile.name);
            if(data.prejectile == GameManager.instance.pool.prefebs[index])
            {
                
                prefabId = index;
                break;
            }
        }

    }

    public void Fire()
    {
        if (!scanner.AttackTarget)
            return;

        Vector3 targetPos = scanner.AttackTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir = dir.normalized;

        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;

        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        bullet.rotation *= Quaternion.Euler(new Vector3(0, 0, 90));

        switch (id)
        {
            case 0:
                bullet.GetComponent<Bullet>().Init(id, damage, count, dir, 5 * unit.bulletSpeed);
                break;
        }
    }
}
