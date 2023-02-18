using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BarGraph : MonoBehaviour
{
    [SerializeField] RawImage[] bars;

    // 데이터가 들어있는 SO파일.
    [SerializeField] SimControllerSO simControllerSO;

    [SerializeField] float BarHeight = 1;

    void Start()
    {
        StartCoroutine(coUpdateBars());
    }

    // 업데이트 주기를 늘리기위해 코루틴으로 작성.
    IEnumerator coUpdateBars()
    {
        // 업데이트 간격 캐싱.
        WaitForSeconds sec = new WaitForSeconds(0.2f);

        RectTransform BankingDoneCountBar = bars[0].rectTransform;
        RectTransform BankingFailCountBar = bars[1].rectTransform;


        while (true)
        {
            float doneHeight = 50f;
            float failHeight = 50f;

            // TotalCustomerCount 값이 0일 경우 높이를 50으로 설정.
            // 0을 나눌수는 없으므로 위와같은 이유로 체크.
            if (simControllerSO.TotalCustomerCount == 0)
            {

                // 그래프 반반
                doneHeight = 50f;
                failHeight = 50f;

            }
            else
            {

                // 번호표를 뽑은 총 고객의 수가 0이 아닌데 BankingDoneCount나 BankingFailCount가 동시에 0이라면?
                if (simControllerSO.BankingDoneCount == 0 && simControllerSO.BankingFailCount == 0)
                {
                    // 그래프 반반
                    doneHeight = 50f;
                    failHeight = 50f;
                }
                else
                {
                    // 둘중 하나만 0이라면?
                    // 그래프 한쪽으로 몰아주기.
                    if (simControllerSO.BankingDoneCount == 0)
                    {
                        doneHeight = 99f;
                        failHeight = 1f;
                    }
                    else if (simControllerSO.BankingFailCount == 0)
                    {
                        doneHeight = 1f;
                        failHeight = 99f;
                    }
                    else
                    {
                        // simControllerSO.BankingDoneCount는 int라 나눗셈 후 소수점으로 값이 안나오므로 반드시 float으로 캐스팅

                        float doneFailSum = (float)simControllerSO.BankingDoneCount + (float)simControllerSO.BankingFailCount;

                        // 업무가 끝난 고객 중 성공 실패 비율을 알기 위해 아래와 같이 연산.
                        doneHeight = 100f * ((float)simControllerSO.BankingDoneCount / doneFailSum);
                        failHeight = 100f * ((float)simControllerSO.BankingFailCount / doneFailSum);
                    }

                }

            }


            // BankingDoneCountBar 높이 적용.
            BankingDoneCountBar.sizeDelta = new Vector2(BankingDoneCountBar.sizeDelta.x, doneHeight * BarHeight);

            // BankingFailCountBar 높이 적용.
            BankingFailCountBar.sizeDelta = new Vector2(BankingFailCountBar.sizeDelta.x, failHeight * BarHeight);

            yield return sec;

        }
    }
}