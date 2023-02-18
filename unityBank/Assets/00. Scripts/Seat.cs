using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool isUsed = false;


    /// <summary>
    /// 마지막에 사용한 사람.
    /// </summary>
    public int LastUser = -1;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    private void Start()
    {
        isUsed = false;
    }
}
