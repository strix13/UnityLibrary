using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_TestOne : CUIPopupInventoryBase<Inventory_TestOne.SDataItem>
{
	public class SDataItem : IInventoryData
	{
		string strSpriteName;

		public SDataItem(string strSpriteName)
		{
			this.strSpriteName = strSpriteName;
		}

		public string IInventoryData_GetSpriteName()
		{
			return strSpriteName;
		}
	}


	protected override void OnShow(int iSortOrder)
	{
		base.OnShow(iSortOrder);

		List<SDataItem> listData = new List<SDataItem>();
		listData.Add(new SDataItem("button_gray"));
		listData.Add(new SDataItem("button_orange"));

		DoInitInventory(listData);
	}

	protected override EInventoryOption OnInitInventory()
	{
		return EInventoryOption.None;
	}
}
