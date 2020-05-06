using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Animator ani;
    Rigidbody2D rigid;
    public int nextAction;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Invoke() : 주어진 시간이 지난 뒤, 지정된 함수를 실행하는 함수
        Invoke("Think", 0.5f);
    }
    
    void FixedUpdate()
    {
        //이동방향 바라보게 하기
        if (nextAction != 0)
            spriteRenderer.flipX = nextAction == 1;

        rigid.velocity = new Vector2(nextAction, rigid.velocity.y);

        //추락 방지 소스
        Vector2 frontVec = new Vector2(rigid.position.x + (nextAction*0.4f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1/*단위벡터*/, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            nextAction *= -1;
            CancelInvoke(); // 현재 진행 중인 Invoke() 함수를 모두 정지
            Invoke("Think", 1);
        }
    }

    //어떤 행동을 취할지 랜덤 값 입력해주는 메소드
    void Think()
    {
        //Range()는 최소값, 최대값을 넣는데 max값에 넣는 숫자보다 작은 최대값이 나온다
        nextAction = Random.Range(-1, 2);

        ani.SetInteger("Walk", nextAction);

        float nextThinkTime = Random.Range(1f, 3f);
        Invoke("Think", nextThinkTime); // 재귀함수로 구현하여 계속 랜덤 값을 적용하도록 구현!
    }

}
