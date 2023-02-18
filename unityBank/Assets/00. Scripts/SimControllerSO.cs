using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "SimController", menuName = "Create SO/SimControllerSO", order = 1)]
public class SimControllerSO : ScriptableObject
{

    [Header("배속 설정")]
    // 게임이 실행되는 속도
    [SerializeField] float gameSpeed = 1.0f;

    public int TotalCustomerCount = 0;

    public int BankingDoneCount = 0;

    public int BankingFailCount = 0;


    /// <summary>
    /// 배속 설정
    /// </summary>
    public void _SetTimeScale()
    {
        // 시간 척도를 지정된 게임 속도로 설정
        Time.timeScale = gameSpeed;
    }

    public void AddBankingDoneCount()
    {
        BankingDoneCount += 1;
    }


    public void AddTotalCustomerCount()
    {
        TotalCustomerCount += 1;
    }

    public void AddBankingFailCount()
    {
        BankingFailCount += 1;
    }

    public void AllClear()
    {
        TotalCustomerCount = 0;
        BankingDoneCount = 0;
        BankingFailCount = 0;

    }


}
