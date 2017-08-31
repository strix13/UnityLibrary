using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Manager : CManagerUIBase<Inventory_Manager, Inventory_Manager.EFrame, Inventory_Manager.EInventoryPopup>
{
	public enum EFrame
	{

	}

	public enum EInventoryPopup
	{
		Inventory_TestOne,
		Inventory_TestTwo
	}
	
	protected override void OnUpdate()
	{
		base.OnUpdate();

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			DoShowHide_Popup(EInventoryPopup.Inventory_TestOne, true);
		}

		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			DoShowHide_Popup(EInventoryPopup.Inventory_TestTwo, true);
		}
	}

	protected override void OnDefaultFrameShow()
	{

	}
}
