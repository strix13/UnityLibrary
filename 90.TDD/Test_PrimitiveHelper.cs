using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
public class Test_PrimitiveHelper
{
	[Test]
    [Category("StrixLibrary")]
    static public void Test_IntExtension_CutDigitString_Number([Random(1000, 10000, 10)] int iTestNum)
	{
		System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
		List<int> listTest = iTestNum.CutDigitString_Number();
		for (int i = 0; i < listTest.Count; i++)
			pStrBuilder.Append( listTest[i] );

		Assert.IsTrue( pStrBuilder.ToString() == iTestNum.ToString() );
	}

	[Test]
    [Category("StrixLibrary")]
    static public void Test_IntExtension_CutDigitString([Random(1000, 10000000, 10)] int iTestNum)
	{
		System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
		List<string> listTest = iTestNum.CutDigitString();
		for (int i = 0; i < listTest.Count; i++)
			pStrBuilder.Append( listTest[i] );

		Assert.IsTrue( pStrBuilder.ToString() == iTestNum.ToString() );
	}

	[Test]
    [Category("StrixLibrary")]
    static public void Test_IntExtension_CutDigitString_WithComma([Random(1000, 10000000, 10)] int iTestNum)
	{
		System.Text.StringBuilder pStrBuilder = new System.Text.StringBuilder();
		List<string> listTest = iTestNum.CutDigitString_WithComma();
		for (int i = 0; i < listTest.Count; i++)
			pStrBuilder.Append( listTest[i] );

		Assert.IsTrue( pStrBuilder.ToString() == iTestNum.CommaString() );
	}

	public enum ETestCase_FloatExtension
	{
		Similar,
		NotSimilar
	}

	[Test]
	[Repeat(10)]
    [Category("StrixLibrary")]
    static public void Test_FloatExtension_IsSimilar()
	{
		Random pRandom = new Random();
		float fTestNum = (float)pRandom.NextDouble();
		float fTestSimlarGap = (float)pRandom.NextDouble();

		ETestCase_FloatExtension eRandomTest = (ETestCase_FloatExtension)(pRandom.Next() % 2);
		if(eRandomTest == ETestCase_FloatExtension.NotSimilar)
		{
			float fSimilarValueNot = fTestNum + (fTestSimlarGap * 2f);
			Assert.IsFalse( fTestNum.IsSimilar( fSimilarValueNot, fTestSimlarGap ) );
		}
		else if(eRandomTest == ETestCase_FloatExtension.Similar)
		{
			float fSimilarValue = fTestNum + (fTestSimlarGap * 0.9f);
			Assert.IsTrue(fTestNum.IsSimilar( fSimilarValue, fTestSimlarGap ));
		}
	}
}
#endif