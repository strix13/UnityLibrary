using System;
using System.Collections.Generic;

public class CList<T> : List<T>
{
    public void Remove(Predicate<T> matchCallBack)
    {
        for (int i = 0; i < Count; i++)
        {
            if(matchCallBack(this[i]))
            {
                Remove(this[i]);
                break;
            }
        }
    }

    public int CalculateMatchCount(T value)
    {
        int iCount = 0;
        for(int i = 0; i < Count; i++)
            if (this[i].Equals(value)) iCount++;

        return iCount;
    }

    public int CalculateMatchCount(Predicate<T> matchCallBack)
    {
        int iCount = 0;
        for (int i = 0; i < Count; i++)
            if (matchCallBack(this[i])) iCount++;

        return iCount;
    }

    public void Find(Predicate<T> matchCallBack, List<T> listOutput)
    {
        for (int i = 0; i < Count; i++)
        {
            if (matchCallBack(this[i]))
            {
                listOutput.Add(this[i]);
                break;
            }
        }
    }

    public void FindAll(Predicate<T> matchCallBack, List<T> listOutput)
    {
        for (int i = 0; i < Count; i++)
            if (matchCallBack(this[i])) listOutput.Add(this[i]);
    }

    public void Add_NotOverlap(T pItem)
    {
        if (pItem == null)
            UnityEngine.Debug.LogWarning("List에 null을 넣었습니다.");

        if (IndexOf(pItem) == -1)
            Add(pItem);
    }

    public void AddRange_NotOverlap(List<T> list)
    {
        for(int i = 0; i < list.Count; i++)
            Add_NotOverlap(list[i]);
    }

    public void Add_Intersection(List<T> listA, List<T> listB, bool bClearOrigin)
    {
        if(bClearOrigin) Clear();
        for (int i = 0; i < listA.Count; i++)
        {
            for (int j = 0; j < listB.Count; j++)
            {
                if (listA[i].Equals(listB[j]))
                {
                    Add(listA[i]);
                    break;
                }
            }
        }
    }

    public bool Contains(Predicate<T> matchCallBack)
    {
        bool bIsContain = false;
        for(int i = 0; i < Count; i++)
        {
            if(matchCallBack(this[i]))
            {
                bIsContain = true;
                break;
            }
        }

        return bIsContain;
    }
}
