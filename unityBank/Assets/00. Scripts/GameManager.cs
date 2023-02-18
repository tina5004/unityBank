using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class GameManager : MonoBehaviour
{
    [Header("직원 프리펩")]
    [SerializeField] GameObject[] Employees = new GameObject[3];

    [Header("고객 프리펩")]
    [SerializeField] GameObject[] Customers = new GameObject[3];

    [Header("카운터")]
    [SerializeField] Counter[] Counters = new Counter[5];

    [Header("고객 자리 위치")]
    public Transform[] TR_Seats = new Transform[16];

    [Header("고객 입장 위치")]
    [SerializeField] Transform TR_NewCustomerPos = null;

    [Header("고객 퇴장 위치")]
    public Transform TR_ExitCustomerPos = null;


    [Header("컨트롤러SO")]
    public SimControllerSO SimController = null;


    [Header("배속설정 버튼")]

    [SerializeField] Button SetTimeScaleBtn = null;

    [Header("배속상태 UI")]
    [SerializeField] TextMeshProUGUI TMP_TimeScale = null;

    [Header("배치할 직원 유형을 선택해 주세요. 순서대로 카운터에 배치됩니다.")]
    [SerializeField] EEmployeeType[] eEmployeeTypes = new EEmployeeType[5];

    // 싱글톤용 인스턴스 변수.
    static public GameManager Instance { get; private set; }

    // 모든 고객의 정보를 참조.
    public List<Customer> AllCustomerList = new List<Customer>();

    /// <summary>
    /// 현재 발행된 모든 번호표의 수
    /// </summary>
    public int TotalIssuingTicketCount { get; private set; } = 0;

    // 마지막으로 호출된 번호.
    public int LastCallNum { get; private set; } = 0;

    // 새로운 번호가 발급되었을 때 실행할 이벤트.
    public Action IssuingNewNumberAction;

    // 새로운 번호가 발급되었을 때 실행할 이벤트.
    public Action<Counter> StartBankingAction;

    // 고객 추가 루틴 중지용.
    private Coroutine addRandomCustomerCoroutine;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        setSingleton();
        TotalIssuingTicketCount = 0;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        // 배속설정 버튼과 컨트롤러 연결
        SetTimeScaleBtn.onClick.AddListener(
        () =>
            {
                SimController._SetTimeScale();

                TMP_TimeScale.text = "TimeScale : " + Time.timeScale.ToString();
            }
        );

        // 시뮬레이터 컨트롤러에 나오는 정보들 전부 0으로 초기화.
        SimController.AllClear();

        // 직원 배치
        placedEmployee();


        // 무작위로 손님 부르기 루틴 시작.
        addRandomCustomerCoroutine = StartCoroutine(CoAddRandomCustomer());


    }

    // 무작위로 손님 부르기
    IEnumerator CoAddRandomCustomer()
    {
        while (true)
        {
            // Code to be executed at a random time
            float waitTime = UnityEngine.Random.Range(1, 3);
            yield return new WaitForSeconds(waitTime);

            AddCustomer();
        }
    }


    // 고객 추가 루틴 중지.
    public void _StopAddRandomCustomerCoroutine()
    {
        StopCoroutine(addRandomCustomerCoroutine);
    }

    // 직원 배치
    void placedEmployee()
    {
        // 선택한 직원 타입 대로 직원을 
        for (int i = 0; i < eEmployeeTypes.Length; i++)
        {
            AddEmployee(employeeNum: (uint)eEmployeeTypes[i], counterNum: i);
        }


    }

    // 최종 호출번호 +1
    public void AddLastCallNumber()
    {
        LastCallNum += 1;
    }


    // 싱글톤 설정.
    private void setSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // 번호표 발권
    public int IssuingNumberTicket()
    {
        TotalIssuingTicketCount += 1;

        return TotalIssuingTicketCount;
    }


    // 고객 추가
    public void AddCustomer()
    {

        // 0,1,2 중 무작위 반환.
        int randomNum = UnityEngine.Random.Range(0, 3);

        // 고객 입장위치에 무작위 고객 소환.
        GameObject customerObj = Instantiate(Customers[randomNum], TR_NewCustomerPos);
        Customer customer = customerObj.GetComponent<Customer>();

        // 고객 명단에 고객 추가.
        AllCustomerList.Add(customer);

        // 번호표 발행.
        customer.SetTicketNumber(IssuingNumberTicket());

        // 번호표 발행 상황을 구독중인 직원들에게 이벤트 날리기.
        IssuingNewNumberAction();

        // 총 고객 데이터 수집
        SimController.AddTotalCustomerCount();
    }


    // 고객 추가
    public void AddEmployee(uint employeeNum, int counterNum)
    {
        // 0,1,2 중 무작위 반환.
        int randomNum = UnityEngine.Random.Range(0, 3);

        // 고객 입장위치에 무작위 고객 소환.
        GameObject employeeObj = Instantiate(Employees[employeeNum], Counters[counterNum].transform);
        Employee employee = employeeObj.GetComponent<Employee>();

        // 담당 카운터 설정.
        employee.SetCounter(Counters[counterNum]);
    }


    // 카운터로부터 번호가 떳을떄 고객들이 자기 번호인지 체크하도록 지시.
    public void ReciveNumberCallFromCounter(NumberData data)
    {
        foreach (Customer customer in AllCustomerList)
        {
            customer._MyNumberCheck(data);
        };
    }





}
