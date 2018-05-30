using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example_HandleCard : MonoBehaviour
{
    public GameObject _pObjectTestCard;

    public int iTestCard = 5;

    private void OnEnable()
    {
        CManagerHandleCard.instance._iLimitHandleCount = iTestCard;
        for(int i = 0; i < iTestCard; i++)
        {
            CManagerHandleCard.instance.DoInsertHandle(_pObjectTestCard.transform);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            CManagerHandleCard.instance.DoInsertHandle(_pObjectTestCard.transform);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CManagerHandleCard.instance.DoInsertHandle(_pObjectTestCard.transform);
        }
    }
}
