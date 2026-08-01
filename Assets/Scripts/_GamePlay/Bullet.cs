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
    SpriteRenderer sr;
    private string targetTag;
    private Transform shooter;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        BulletTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        BulletTime += Time.deltaTime;

        if(BulletTime > 5 && per != -1)
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        sr.sortingOrder = Mathf.RoundToInt(1000 + (-27 - transform.position.y) * 100);
    }

    public void Init(int id, float Damage, int per, Vector3 dir, float bulletVelocity, string targetTag, Transform shooter)
    {
        this.Damage = Damage;
        this.per = per;
        this.id = id;
        this.targetTag = targetTag;
        this.shooter = shooter;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        rigid.linearVelocity = dir * bulletVelocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!(collision.CompareTag(targetTag) || collision.CompareTag("Wall")) || per == -1)
            return;
        

        if(per == -1 || collision.CompareTag("Wall"))
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            return;
        }

        // Enemy에게 데미지 주는 로직
        if (collision.CompareTag(targetTag))
        {
            StatHandler stat = collision.GetComponent<StatHandler>();

            if (stat != null)
            {
                stat.TakeDamage((int)Damage, shooter);
            }

            per--;

            if (per < 0)
            {
                rigid.linearVelocity = Vector2.zero;
                gameObject.SetActive(false);
            }
        }
    }
}
