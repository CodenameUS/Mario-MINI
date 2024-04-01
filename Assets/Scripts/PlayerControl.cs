using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public AudioClip audioJump;
    public AudioClip audioItem;
    public AudioClip audioFinish;
    public AudioClip audioAttack;
    public AudioClip audioDie;
    public GameDirector gameDirector;
    public float maxSpeed;
    public float jumpPower;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    CapsuleCollider2D CapsuleCollider2D;
    AudioSource audioSource;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    
    void Update() 
    {
        //멈출때 속도
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //방향전환
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //애니메이션 바꿈
        if (Mathf.Abs(rigid.velocity.x) < 0.3f)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);

        //점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("isJumping",true);
            audioSource.clip = audioJump;
            audioSource.Play();
        }

    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        //좌우이동
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //최대이동속도조정
        if(rigid.velocity.x > maxSpeed) 
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y); 
        }
        else if(rigid.velocity.x < maxSpeed * (-1))
        {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        //착지확인
        if (rigid.velocity.y < 0)   
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(1, 0, 0)); 
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Floor"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance <= 0.45f) 
                    anim.SetBool("isJumping", false);
            }
        }
    }

    //점수, 도착
    void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.gameObject.tag == "Items")
        {
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameDirector.stagePoint += 10;
            else if (isSilver)
                gameDirector.stagePoint += 50;
            else if (isGold)
                gameDirector.stagePoint += 100;

            audioSource.clip = audioItem;
            audioSource.Play();

            collision.gameObject.SetActive(false);
        }       
        else if(collision.gameObject.tag == "Finish")
        {
            gameDirector.NextStage();
            audioSource.clip = audioFinish;
            audioSource.Play();
        }
    }

    //충돌판정
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Traps")
        {
            //몬스터밟기
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                audioSource.clip = audioAttack;
                audioSource.Play();

                Attack(collision.transform);
                                
                gameDirector.stagePoint += 100;               
            }
            else 
                Damaged(collision.transform.position);
        }
    }

    void Attack(Transform monster)
    {
        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
        MonsterControl monsterControl = monster.GetComponent<MonsterControl>();
        monsterControl.OnDamaged();
    }

    //무적판정
    void Damaged(Vector2 targetPos)
    {
        gameDirector.hpDown();

        gameObject.layer = 11;      

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        Invoke("OffDamaged", 2);
    }

    //무적풀림
    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    //플레이어죽음
    public void Die()
    {
        audioSource.clip = audioDie;
        audioSource.Play();

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        spriteRenderer.flipY = true;

        CapsuleCollider2D.enabled = false;

        rigid.AddForce(Vector2.up * 7, ForceMode2D.Impulse);
    }
}
