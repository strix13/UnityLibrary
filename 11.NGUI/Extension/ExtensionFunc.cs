using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NGUIExtensionHelper
{
	public static void UpdateInputLabel( this UIInput pInput, UIInput.InputType eInputType )
	{
		pInput.inputType = eInputType;
		pInput.UpdateLabel();
	}
}