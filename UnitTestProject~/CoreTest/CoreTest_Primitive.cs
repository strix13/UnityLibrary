using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class CoreTest_Primitive
	{
		[TestMethod]
		public void Test_CutDigitString_Number()
		{
			Test_PrimitiveHelper.Test_CutDigitString_Number();
		}

		[TestMethod]
		public void Test_CutDigitString()
		{
			Test_PrimitiveHelper.Test_CutDigitString();
		}

		[TestMethod]
        public void Test_CutDigitString_WithComma()
        {
			Test_PrimitiveHelper.Test_CutDigitString_WithComma();
		}
	}
}
