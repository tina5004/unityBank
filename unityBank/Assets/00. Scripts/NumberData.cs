using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberData
{
    /// <summary>
    /// 호출 번호
    /// </summary>
    /// <value></value>
    public int CallNumber { get; private set; }

    /// <summary>
    /// 호출된 카운터 정보
    /// </summary>
    /// <value></value>
    public Counter CalledCounter { get; private set; }



    // 생성자.
    public NumberData(int callNum, Counter calledCounter)
    {
        CallNumber = callNum;
        CalledCounter = calledCounter;
    }
}
