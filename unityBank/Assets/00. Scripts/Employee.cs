using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Employee : MonoBehaviour
{
    // 내가 업무중인 카운터
    Counter myBankingCounter = null;

    // 직원 유형
    [SerializeField] EEmployeeType employeeType = EEmployeeType.Normal;

    // 싱글톤 활용.
    GameManager GM = GameManager.Instance;

    // 업무에 걸리는 시간.
    int jobTime = 0;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {

        // 내 담당 카운터 은행업무 성공시 이벤트 등록
        myBankingCounter.BankingDoneAction += () =>
        {
            // 은행업무중 아니라고 바꾸기.
            myBankingCounter.isInBanking = false;

            StartCoroutine(myBankingCounter.CoNextCall());

        };

        // 내 담당 카운터 은행업무 실패시 이벤트 등록
        myBankingCounter.BankingFailAction += () =>
        {

            // 은행업무중 아니라고 바꾸기.
            myBankingCounter.isInBanking = false;

            StartCoroutine(myBankingCounter.CoNextCall());

        };

        // 고객이 새로운 번호표 발행 받을시 받을 이벤트
        GM.IssuingNewNumberAction += () =>
        {
            // 내가 업무중이 아니라면 다음 번호 호출
            if (myBankingCounter.isInBanking == false)
            {
                StartCoroutine(myBankingCounter.CoNextCall());
            }
        };

        // 은행 업무가 시작 됐을 때 coStartBanking 코루틴 실행.
        GM.StartBankingAction += (Counter) =>
        {
            // 내 카운터라면?
            if (Counter == myBankingCounter)
            {
                // Debug.Log(myBankingCounter.gameObject.name + " 에서 " + myBankingCounter.NowBankingNum + "번 업무시작 from Employee!");

                // 업무 진행.
                StartCoroutine(coStartBanking());
            }
            else
            {
                return;
            }

        };

        // 상태 초기화
        init();

    }


    // 담당 카운터 설정.
    public void SetCounter(Counter counter)
    {
        myBankingCounter = counter;
    }


    void init()
    {
        switch (employeeType)
        {
            case EEmployeeType.Slow:
                jobTime = 25;
                break;
            case EEmployeeType.Normal:
                jobTime = 15;
                break;
            case EEmployeeType.Fast:
                jobTime = 5;
                break;
            default:
                Debug.LogError("타입이 선택되지 않았습니다!");
                break;
        }
    }


    IEnumerator coStartBanking()
    {
        // 은행업무 시작 코루틴.

        myBankingCounter.isInBanking = true;

        // 은행 업무 시간 측정용.
        float bankingTime = 0f;

        // 실제시간 1초를 시뮬레이션 1분으로 표현해서 활용예정.
        // 반복해서 사용할것이기때문에 캐싱.
        WaitForSeconds tick = new WaitForSeconds(1);

        // 업무가 끝날때까지 걸리는시간(jobTime)동안 반복
        while (bankingTime < jobTime)
        {
            // 만약 고객이 견디지 못하고 은행업무가 끝나버렸다면.
            // 코루틴 파괴
            if (myBankingCounter.isInBanking == false)
            {
                // 작업 실패 판정은 고객이 함.
                // Debug.LogWarning("직원 루틴 파괴");
                yield break;
            }

            yield return tick;
            bankingTime += 1;
        }

        // 작업 완료 이벤트 날리기.
        myBankingCounter.BankingDoneAction();
        // Debug.Log(myBankingCounter.name + "에서 성공적으로 은행업무를 끝마쳤습니다!");

        // 은행 업무 중 아니라고 바꿔주기.
        myBankingCounter.isInBanking = false;

    }

}

/// <summary>
/// 직원 유형
/// </summary>
public enum EEmployeeType { Fast = 0, Normal = 1, Slow = 2 }
