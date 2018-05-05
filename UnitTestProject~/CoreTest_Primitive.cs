using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestClass]
    public class CoreTest_Primitive
	{
		[TestMethod]
		public void Test_CutDigitString_Number()
		{
            for(int i = 0; i < 10; i++)
                Test_PrimitiveHelper.Test_IntExtension_CutDigitString_Number(SCTestHelper.GetRandomValue(1000, 10000));
		}

		[TestMethod]
		public void Test_CutDigitString()
		{
            for (int i = 0; i < 10; i++)
                Test_PrimitiveHelper.Test_IntExtension_CutDigitString(SCTestHelper.GetRandomValue(1000, 10000000));
		}

		[TestMethod]
        public void Test_CutDigitString_WithComma()
        {
            for (int i = 0; i < 10; i++)
                Test_PrimitiveHelper.Test_IntExtension_CutDigitString_WithComma(SCTestHelper.GetRandomValue(1000, 10000000));
		}

		[TestMethod]
		public void Test_IsSimilar()
		{
			Test_PrimitiveHelper.Test_FloatExtension_IsSimilar();
		}
	}
}
