using NUnit.Framework;
using System;
using System.Collections.Generic;

public class Test_ByteHandler
{
	[Test]
	[Repeat( 10 )]
	static public void Test_Byte_To_BitArray()
	{
		Assert.IsTrue(SCByteHandler.ConvertByte_To_Int( 1 ) == 1);
		Assert.IsTrue( SCByteHandler.ConvertByte_To_Int( 1, 8, 1 ) == 0 );
		Assert.IsTrue( SCByteHandler.ConvertByte_To_Int( 2, 8, 0 ) == 2 );
		Assert.IsTrue( SCByteHandler.ConvertByte_To_Int( 127, 8, 2 ) == (127 - 3));
	}
}
