using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Counter : MonoBehaviour
{
    // 게임매니저 인스턴스
    GameManager GM = null;

    // 은행업무 성공했을 때 이벤트.
    public Action BankingDoneAction = null;

    // 은행업무 실패했을 때 이벤트.
    public Action BankingFailAction = null;

    // 고객이 호출을 받았을 때 실행 할 이벤트
    public Action GetCallAction = null;

    public int NowBankingNum { get; private set; }

    /// <summary>
    /// 카운트 번호 표기용 텍스트
    /// </summary>
    TextMeshPro CountText;

    // 현재 은행업무 중인가?
    // flag
    public bool isInBanking = false;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        // 초기화.
        GM = GameManager.Instance;
        CountText = transform.GetComponentInChildren<TextMeshPro>();

        BankingDoneAction += GM.SimController.AddBankingDoneCount;
        BankingFailAction += GM.SimController.AddBankingFailCount;

        GetCallAction += () =>
        {
            // 호출받은 고객이 있으면, isInBanking을 true로 변경.
            isInBanking = true;
        };
    }

    // 다음 번호 호출.
    public IEnumerator CoNextCall()
    {

        // 은행 업무가 다 끝나고 나서 번호를 호출할 수 있도록 한프레임을 건너뛰어서
        // 순서를 맞추기 위해 코루틴 활용,
        yield return null;

        // 업무 중이면 루틴 중지.
        if (isInBanking == true)
        {
            yield break;
        }

        WaitForSeconds tick = new WaitForSeconds(1);

        // 업무가 시작 될때까지 1초 간격으로 계속 다음 번호 호출.
        // GetCallAction에 의해서 호출을 받으면 isInBanking 변수가 true로 바뀜.
        while (isInBanking == false)
        {
            // 전체 발행한 티켓 수가 마지막 호출번호와 같거나 작으면 루틴 중지.
            if (GM.TotalIssuingTicketCount <= GM.LastCallNum)
            {
                // Debug.Log(this.gameObject.name + " 호출가능한 번호가 없습니다.");

                yield break;
            }

            // 최종 호출 번호 +1
            GM.AddLastCallNumber();

            // 카운터에 번호 기록 및 표시하기
            NowBankingNum = GM.LastCallNum;
            CountText.text = NowBankingNum.ToString();

            // 공지할 데이터 생성.
            NumberData ND = new NumberData(callNum: GM.LastCallNum, calledCounter: this);

            // 생성한 데이터 공지.
            noticeNumberData(ND);

            // 1초 대기.
            yield return tick;
        }


    }

    // 번호표 공지.
    void noticeNumberData(NumberData data)
    {
        // 전체 공지
        GM.ReciveNumberCallFromCounter(data);
    }



}