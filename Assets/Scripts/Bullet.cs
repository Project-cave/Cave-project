using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public enum BulletType { Normal };
    public BulletType type;
    public float Damage;
    public int per;
    public int id;

    public float BulletTime;

    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        BulletTime += Time.deltaTime;

        if(BulletTime > 5 && per != -1)
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            BulletTime = 0;
        }
    }

    public void Init(int id, float Damage, int per, Vector3 dir, int bulletVelocity)
    {
        this.Damage = Damage;
        this.per = per;
        this.id = id;      
        rigid.linearVelocity = dir * bulletVelocity;        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision.CompareTag("Enemy") || collision.CompareTag("Wall")) || per == -1)
            return;
        

        per--;
        

        if(per == -1 || collision.CompareTag("Wall"))
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
