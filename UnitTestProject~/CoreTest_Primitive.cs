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
			Test_PrimitiveHelper.Test_IntExtension_CutDigitString_Number();
		}

		[TestMethod]
		public void Test_CutDigitString()
		{
			Test_PrimitiveHelper.Test_IntExtension_CutDigitString();
		}

		[TestMethod]
        public void Test_CutDigitString_WithComma()
        {
			Test_PrimitiveHelper.Test_IntExtension_CutDigitString_WithComma();
		}

		[TestMethod]
		public void Test_IsSimilar()
		{
			Test_PrimitiveHelper.Test_FloatExtension_IsSimilar();
		}
	}
}
