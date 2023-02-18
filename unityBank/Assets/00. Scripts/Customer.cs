using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 고객이 생성된 직후 번호표를 수령한 상황임.
public class Customer : MonoBehaviour
{
    // 내가 업무중인 카운터
    Counter myBankingCounter = null;

    // -1일경우 아무대도 앉아있지 않는다.
    int mySeatNum = -1;

    // 고객의 성격.
    [Header("고객이 참고 견디는 시간")]
    public float PersonalityTime = 10;

    private Transform target; // 목표의 Transform

    [Header("속도")]
    [SerializeField] private float speed = 1f; // 목표물을 향해 이동하는 속도

    [Header("목표물과의 정지 거리")]
    [SerializeField] private float stopDistance = 0.1f; // 정지거리 설정

    private Transform cachedTransform; // 개체의 Transform 캐싱된 참조
    private Rigidbody2D cachedRigidbody; // 객체의 Rigidbody2D 캐싱된 참조

    private GameManager GM;



    // 퇴장하는 중인가?
    // flag
    bool isExiting = false;

    // 수령한 번호표 번호
    // 0일 경우 수령하지 않은 상태이거나 폐기한 상태로 간주
    int myTicketNumber = 0;


    void Start()
    {
        GM = GameManager.Instance;

        // transform 및 Rigidbody2D 캐싱
        cachedTransform = this.transform;
        cachedRigidbody = GetComponent<Rigidbody2D>();


        // 카운터가 배정이 안되었다면.
        if (myBankingCounter == null)
        {
            // 입장과 동시에 일단 대기석으로 간다.
            StartCoroutine(CoRandomSit());
        }


    }

    public void SetTicketNumber(int num)
    {
        myTicketNumber = num;
    }


    // 타겟이 바뀌면 실시간으로 이동.
    void FixedUpdate()
    {
        goToTarget();
    }

    // 매 프레임마다 체크하면 메모리가 성능하락이 있을 수 있으므로 간격을 둔다.
    IEnumerator CoRandomSit()
    {
        WaitForSeconds tick = new WaitForSeconds(1);

        // 0~15사이의 수가 나옴.
        int randomNum = Random.Range(0, 16);

        // 빈자리를 찾을 때 까지 1초마다 반복
        // == 누가 자리를 사용 중이면 반복.
        while (GM.TR_Seats[randomNum].GetComponent<Seat>().isUsed == true)
        {
            // 빈자리를 찾는 도중 카운터에 배정 되었다면?
            if (myBankingCounter != null)
            {
                // 대기석 찾아가는 루틴 종료.
                yield break;
            }

            // 1초 대기.
            yield return tick;

            // 다른 랜덤 번호 생성.
            randomNum = Random.Range(0, 16);
        }

        // 빈자리를 찾으면 가서 앉는다.
        target = GM.TR_Seats[randomNum];

        // 사용중인 좌석 번호를 기록.
        // 사용중으로 기록.
        mySeatNum = randomNum;

        Seat mySeat = GM.TR_Seats[mySeatNum].GetComponent<Seat>();

        mySeat.isUsed = true;
        mySeat.LastUser = myTicketNumber;

    }


    // 이동로직.
    void goToTarget()
    {
        // 목표물이 없으면 함수정지.
        if (target == null)
        {
            return;
        }

        // 목표까지의 거리 계산
        float distance = Vector2.Distance(cachedTransform.position, target.position);

        // 목표까지의 거리가 정지 거리보다 크면 목표를 향해 이동
        if (distance > stopDistance)
        {
            Vector2 targetPosition = Vector2.MoveTowards(cachedTransform.position, target.position, speed * Time.deltaTime);
            cachedRigidbody.MovePosition(targetPosition);
        }
        else// 도착.
        {
            // 목표위치 초기화.
            // 정지.
            TargetClear();

            // 나가는 중이었다면?
            if (isExiting == true)
            {
                // 고객 삭제
                Destroy(this.gameObject);

                return;
            }
            else
            {
                // 카운터가 배정되어 있다면
                if (myBankingCounter != null)
                {
                    // 업무 시작.
                    StartCoroutine(CoStartBanking());

                    // 카운터에 공지하기.
                    GM.StartBankingAction(myBankingCounter);
                }
                else // 나가는 중도 아니고 도착을 했는데 카운터가 배정되어있지 않다 == 대기석에 앉아 있다.
                {
                    // 대기루틴 시작.
                    StartCoroutine(CoStartWaiting());
                }
            }

        }
    }


    // 은행업무 시작시 코루틴.
    IEnumerator CoStartBanking()
    {
        // 은행 업무 시간 측정용.
        float bankingTime = 0f;

        // 실제시간 1초를 시뮬레이션 1분으로 표현해서 활용예정.
        // 반복해서 사용할것이기때문에 캐싱.
        WaitForSeconds tick = new WaitForSeconds(1);

        // 은행 업무 시간이 고객성격에 따른 한계 시간을 넘어가면 작업 종료.
        while (bankingTime < PersonalityTime)
        {
            // 만약 시간 추가를 판정 뒤에하면
            // 판정 후 시간이 지나가는 동안 myBankingCounter가 배정될수도 있기때문에
            // 해당 로직의 경우 이후에 if (myBankingCounter != null)체크를 한번 더 해줘야함.
            yield return tick;
            bankingTime += 1;

            // 만약 기다릴 수 있는 시간보다 먼저 은행업무가 끝나버렸다면.
            // 코루틴 파괴
            if (myBankingCounter.isInBanking == false)
            {
                // 업무 성공 판정은 직원이 함.
                //Debug.Log("고객 업무 루틴 파괴 - 업무 성공");
                yield break;
            }

        }

        // 작업 실패 이벤트 날리기.
        myBankingCounter.BankingFailAction();
        //Debug.Log(myTicketNumber + "번 고객이 느린업무를 견디지 못하고 이탈하였습니다.");
    }

    // 대기 시 오래걸리면 나가버리는 루틴
    IEnumerator CoStartWaiting()
    {

        // 은행 업무 시간 측정용.
        float WaitngTime = 0f;

        // 실제시간 1초를 시뮬레이션 1분으로 표현해서 활용예정.
        // 반복해서 사용할것이기때문에 캐싱.
        WaitForSeconds tick = new WaitForSeconds(1);

        // 대기 시간이 고객성격에 따른 한계 시간을 넘어가면 작업 종료.
        while (WaitngTime < PersonalityTime)
        {

            // 만약 시간 추가를 판정 뒤에하면
            // 판정 후 시간이 지나가는 동안 myBankingCounter가 배정될수도 있기때문에
            // 해당 로직의 경우 이후에 if (myBankingCounter != null)체크를 한번 더 해줘야함.
            yield return tick;
            WaitngTime += 1;

            // 대기 중 만약 카운터가 배정되면
            if (myBankingCounter != null)
            {
                // Debug.LogWarning("고객 대기루틴 파괴");

                yield break;
            }

        }

        // 작업 실패 이벤트 날리기.
        doBankingFail();

        // 이탈 카운트 올려주기.
        GM.SimController.AddBankingFailCount();

        // Debug.Log(myTicketNumber + "번 고객이 오랜 대기를 견디지 못하고 이탈하였습니다.");
    }


    /// <summary>
    /// 타겟 없애기.
    /// </summary>
    public void TargetClear()
    {
        target = null;
    }

    /// <summary>
    /// 타겟 설정.
    /// </summary>
    /// <param name="_target"></param>
    public void SetTargetFromTR(Transform _target)
    {
        target = _target;
    }


    // 외부에서 번호 공지 될때 내번호 맞는지 체크.
    public void _MyNumberCheck(NumberData data)
    {
        // 업무가 끝나고 나가는 중이면 번호를 체크 하지 않는다.
        if (isExiting)
        {
            return;
        }

        // 내가 수령한 번호가 맞지 않으면 체크 중지
        if (myTicketNumber != data.CallNumber)
        {
            return;
        }

        // 이 로직 밑으로는 내 번호가 맞을 때 할 행동.

        // 대기석에 앉아 있을 경우.
        if (mySeatNum != -1)
        {
            // 자리 사용안함으로 변경.
            GM.TR_Seats[mySeatNum].GetComponent<Seat>().isUsed = false;

            // 자리 정보 폐기.
            mySeatNum = -1;

        }


        // 업무 중인 카운터 캐싱.
        myBankingCounter = data.CalledCounter;

        // 카운터를 타겟으로 설정.
        // 설정 즉시 update문에서 타겟을 향해 달려감.
        SetTargetFromTR(myBankingCounter.transform);

        // 카운터 배정 후 은행업무 성공시 이벤트 등록
        myBankingCounter.BankingDoneAction += doBankingDone;

        // 카운터 배정 후 은행업무 실패시 이벤트 등록
        myBankingCounter.BankingFailAction += doBankingFail;

        // 카운터에 내가 호출을 받았다고 알려주기.
        myBankingCounter.GetCallAction();


    }





    void doBankingDone()
    {
        // 퇴장.
        isExiting = true;

        // 퇴장 위치로 이동.
        target = GM.TR_ExitCustomerPos;

        // Debug.Log(myTicketNumber + "번 고객 : " + "업무 완료.");
    }

    void doBankingFail()
    {

        // 의자에 앉아 있었으면 의자 자리 비웠다고 알려주기.
        if (mySeatNum != -1)
        {
            // Debug.LogWarning("내 번호 : " + myTicketNumber);
            // Debug.LogWarning("앉아있던 좌석 번호 : " + mySeatNum);

            // 자리 사용안함으로 변경.
            GM.TR_Seats[mySeatNum].GetComponent<Seat>().isUsed = false;

            // 자리 정보 폐기.
            mySeatNum = -1;
        }


        // 퇴장.
        isExiting = true;

        // 퇴장 위치로 이동.
        target = GM.TR_ExitCustomerPos;

        // Debug.Log(myTicketNumber + "번 고객 : " + "업무 실패.");

    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        // 배정된 카운터가 있을 경우. 등록한 이벤트 해제.
        // 이벤트 등록의 경우 카운터가 배정될 때 하기 때문.
        if (myBankingCounter != null)
        {
            // 이벤트 해제
            myBankingCounter.BankingDoneAction -= doBankingDone;
            myBankingCounter.BankingFailAction -= doBankingFail;
        }


        // 전체 리스트에서 고객 삭제.
        GM.AllCustomerList.Remove(this);

    }


}
