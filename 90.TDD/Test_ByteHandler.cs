using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
public class Test_ByteHandler
{
	public enum ETest
	{
		ETest1
	}

	[Test]
	[Repeat( 10 )]
    [Category("StrixLibrary")]
	static public void Test_Byte_To_BitArray()
	{
		Assert.IsTrue(SCByteHandler.ConvertByte_To_Int( 1 ) == 1);
		Assert.IsTrue( SCByteHandler.ConvertByte_To_Int( 1, 8, 1 ) == 0 );
		Assert.IsTrue( SCByteHandler.ConvertByte_To_Int( 2, 8, 0 ) == 2 );
		Assert.IsTrue( SCByteHandler.ConvertByte_To_Int( 127, 8, 2 ) == (127 - 3));
	}
}
#endif