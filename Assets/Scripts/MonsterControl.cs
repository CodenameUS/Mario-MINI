using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterControl : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D CapsuleCollider2D;

    public int nextBehaviour;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        CapsuleCollider2D = GetComponent<CapsuleCollider2D>();

        Invoke("nextMove", 3);   
    }

    void FixedUpdate()
    {
        //몬스터움직임
        rigid.velocity = new Vector2(nextBehaviour, rigid.velocity.y);

        //낭떠러지 확인
        Vector2 mosnterFront = new Vector2(rigid.position.x + nextBehaviour * 0.5f, rigid.position.y);
        Debug.DrawRay(mosnterFront, Vector3.down, new Color(1, 0, 0)); 
        RaycastHit2D rayHit = Physics2D.Raycast(mosnterFront, Vector3.down, 1, LayerMask.GetMask("Floor"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    //다음동작(랜덤하게)
    void nextMove()
    {
        //다음 동작
        nextBehaviour = Random.Range(-1, 2); 

        anim.SetInteger("isWalking", nextBehaviour);
        
        if (nextBehaviour != 0)
        {
            spriteRenderer.flipX = nextBehaviour == 1;  //방향전환
        }

        float nextTime = Random.Range(2.0f, 5.0f);
        Invoke("nextMove", nextTime);
    }

    void Turn()
    {
        nextBehaviour *= -1;        //낭떠러지가 되면 가는방향을 반대로
        spriteRenderer.flipX = nextBehaviour == 1;  //보는방향전환

        CancelInvoke();
        Invoke("nextMove", 3);
    }

    //몬스터죽음
    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        spriteRenderer.flipY = true;

        CapsuleCollider2D.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        Invoke("Destroy", 4);
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}
