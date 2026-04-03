using UnityEngine;
using System.Collections;

public class Facility : MonoBehaviour
{
    private FacilityStatHandler statHandler;
    private SpriteRenderer sr;
    private Rigidbody2D rigid;

    private void Awake()
    {
        statHandler = GetComponent<FacilityStatHandler>();
        sr = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();

        if (statHandler != null)
        {
            statHandler.OnDeath += Death;
        }
    }

    protected virtual void OnDestroy()
    {
        if (statHandler != null)
        {
            statHandler.OnDeath -= Death;
        }
    }

    private void OnDisable()
    {
        if (UnitManager.instance != null)
        {
            UnitManager.instance.UnRegisterUnit(gameObject);
        }
    }

    public void Death()
    {

        GetComponent<Collider2D>().enabled = false;
        rigid.linearVelocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Kinematic;

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        float fadeTime = 1.0f;
        float startAlpha = sr.color.a;
        float time = 0;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0, time / fadeTime);

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);

            yield return null;
        }

        gameObject.SetActive(false);
    }
}
