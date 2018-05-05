using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject
{
    static class SCTestHelper
    {
        static public int GetRandomValue(int iRandomRange_Min, int iRandomRange_Max)
        {
            Random pRandom = new Random();
            return pRandom.Next(iRandomRange_Min, iRandomRange_Max);
        }
    }
}
