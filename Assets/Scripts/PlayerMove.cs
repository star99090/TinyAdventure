using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator ani;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }
    //Rigidbody2D에 z축 얼리는게 있는데 그거 해줘야 캐릭터가 안굴러간다
    void Update() // 단발적인, 한 번의 키 입력은 Update()
    {
        //이동 멈추는 속도 조절
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
                // normalized : 벡터의 크기를 1로 만들어버린 상태(+ - 부호는 그대로 유지해줌) 방향구할때 씀
            /* 키에서 손 떼면 바로 멈추는 소스코드
            rigid.velocity = new Vector2(0, rigid.velocity.y);
            */
        }

        //점프
        if (Input.GetButtonDown("Jump") && !ani.GetBool("isJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            ani.SetBool("isJumping", true);
        }
        
        //방향전환
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //완전히 멈췄을 때
        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            ani.SetBool("isWalking", false);
        else
            ani.SetBool("isWalking", true);

    }
    void FixedUpdate() // 지속적인 키 입력은 FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h * 3, ForceMode2D.Impulse);

        //오른쪽 이동 Max Speed
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        //왼쪽 이동 Max Speed
        else if (rigid.velocity.x < maxSpeed*(-1))
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);

        //Raycast : 오브젝트 검색을 위해 ray를 쏘는 방식(3D에서는 OnColliderEnter를 썼다)
        //Landing platform, 점프했을 때 행동 한 번하고 멈추는거를 수정하기위해 만들어진 부분
        //DrawRay() : 에디터 상에서만 Ray를 그려주는 함수(시작위치,쏘는방향)
        //rayHit : 우리가 쏜 빔에 맞은 오브젝트의 정보를 받아온다
        //y축이 감소할 때(중력받고 내려가는 중) 그때만 레이를 쏜다
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1/*단위벡터*/, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                    ani.SetBool("isJumping", false);
            }
        } 
    }
    //충돌 효과는 OnCollisionEnter
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            //몬스터를 위에서 밟았을 때, Attack
            if(rigid.velocity.y < 0 && collision.transform.position.y < transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else
                OnDamaged(collision.transform.position);
        }
        
    }
    
    void OnAttack(Transform enemy)
    {
        //내 캐릭터 리액션
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        //몬스터 데미지 리액션
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();
    }
    void OnDamaged(Vector2 targetPos)
    {
        //맞으면 데미지 맞은 레이어로 이동
        gameObject.layer = 11;

        //맞은 이펙트
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        //맞아서 생기는 이동
        int dirc = transform.position.x-targetPos.x > 0? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse);

        //애니메이션
        ani.SetTrigger("doDamaged");

        //원상태로 복귀
        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
