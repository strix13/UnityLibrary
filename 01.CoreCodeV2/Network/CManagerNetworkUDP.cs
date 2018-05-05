#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-26 오후 3:12:39
 *	기능 : 
 *	
 *	패킷 생성은 하단 링크의 구조체 마샬링 부분을 참조
 *	https://docs.microsoft.com/ko-kr/dotnet/framework/interop/marshaling-classes-structures-and-unions
 *	
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR

using NUnit.Framework;
using UnityEngine.TestTools;

#endif

abstract public class CManagerNetworkUDPBase<Class_Derived> : CSingletonMonoBase<Class_Derived>
    where Class_Derived : CManagerNetworkUDPBase<Class_Derived>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    UdpClient _pClientUDP_Recv;
    UdpClient _pClientUDP_Send = new UdpClient();

    Thread _ThreadReceive;
    bool _bIsListen;

    protected CircularBuffer<byte> _pRecvBuffer = new CircularBuffer<byte>(10240);

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSendPacket_UDP<Packet>(string strIP, int iPort, Packet pPacket)
    {
        IPEndPoint pSendIP = new IPEndPoint(IPAddress.Parse(strIP), iPort);
        byte[] arrData = ConvertByteArray(pPacket);

        _pClientUDP_Send.Send(arrData, arrData.Length, pSendIP);
    }

    public void DoStartListen_UDP(int iPort)
    {
        _pClientUDP_Recv = new UdpClient();
        _pClientUDP_Recv.Client.Bind(new IPEndPoint(IPAddress.Any, iPort));
        _pClientUDP_Recv.Client.SendTimeout = 10000;

        _bIsListen = true;

        if (_ThreadReceive != null)
            _ThreadReceive.Abort();

        _ThreadReceive = new Thread(new ThreadStart(ProcListenUDP));
        _ThreadReceive.IsBackground = true;
        _ThreadReceive.Start();
    }

    private void ProcListenUDP()
    {
        IPEndPoint pRecieveIP = new IPEndPoint(IPAddress.Any, 0);
        while (_bIsListen)
        {
            byte[] arrData = _pClientUDP_Recv.Receive(ref pRecieveIP);
            if(arrData != null && arrData.Length != 0)
            {
                // 정확한 패킷이 아니라면 일단 보류
                bool bIsStackPacket;
                CheckIsValidPacket(arrData, pRecieveIP.Address.ToString(), out bIsStackPacket);

                if (bIsStackPacket)
                {
                    bool bIsOver = _pRecvBuffer.Enqueue(arrData);
                }
            }
        }
    }

    public byte[] ConvertByteArray<Packet>(Packet pObject)
    {
        int iPacketSize = Marshal.SizeOf(pObject);
        IntPtr pBuffer = Marshal.AllocHGlobal(iPacketSize);
        Marshal.StructureToPtr(pObject, pBuffer, false);
        byte[] arrData = new byte[iPacketSize];
        Marshal.Copy(pBuffer, arrData, 0, iPacketSize);
        Marshal.FreeHGlobal(pBuffer);

        return arrData;
    }

    public bool ConvertPacket<Packet>(byte[] arrData, out Packet pObjectType)
    {
        pObjectType = default(Packet);
        int iPacketSize = Marshal.SizeOf(typeof(Packet));
        if (iPacketSize > arrData.Length)
            return false;

        IntPtr pBuffer = Marshal.AllocHGlobal(iPacketSize);
        Marshal.Copy(arrData, 0, pBuffer, iPacketSize);
        pObjectType = (Packet)Marshal.PtrToStructure(pBuffer, typeof(Packet));
        Marshal.FreeHGlobal(pBuffer);

        return true;
    }

    public byte[] DequeuePacket_OrNull(int iDataSize)
    {
        return _pRecvBuffer.Dequeue_OrNull(iDataSize);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */

    abstract protected void CheckIsValidPacket(byte[] arrRecieveData, string strIP, out bool bIsStackPacket);

    // ========================================================================== //

    #region Private

    #endregion Private

    // ========================================================================== //

}

#region Test
#if UNITY_EDITOR

public class CManagerNetworkUDPTest : CManagerNetworkUDPBase<CManagerNetworkUDPTest>
{
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public struct SPacketTest
    {
        public int iValue;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string strValue;

        static public SPacketTest Dummy()
        {
            return new SPacketTest(0, "");
        }

        public SPacketTest(int iValue, string strValue)
        {
            this.iValue = iValue;
            this.strValue = strValue;
        }
    }

    protected override void CheckIsValidPacket(byte[] arrRecieveData, string strIP, out bool bIsStackPacket)
    {
        bIsStackPacket = false;
        bIsRecievePacket = true;
        SPacketTest pPacket;
        CManagerNetworkUDPTest.instance.ConvertPacket(arrRecieveData, out pPacket);

        if (pPacket.iValue == 1)
        {
            pPacketCheckRecieve = pPacket;
            CManagerNetworkUDPTest.instance.DoSendPacket_UDP(strTestTargetIP, iTestPort, new SPacketTest(2, "받았다"));
        }
    }

    [UnityTest]
    [Category("StrixLibrary")]
    public IEnumerator 구조체에서_바이트변환_다시_구조체로변환_테스트()
    {
        SPacketTest pPacket1 = new SPacketTest(1, "테스트Test123!@#");
        SPacketTest pPacket2 = new SPacketTest();

        Assert.AreNotEqual(pPacket1, pPacket2);

        byte[] arrData = ConvertByteArray(pPacket1);
        ConvertPacket(arrData, out pPacket2);

        Assert.AreEqual(pPacket1, pPacket2);

        yield return null;
    }


    const string strTestTargetIP = "127.0.0.1";
    const int iTestPort = 9999;

    static SPacketTest pPacketCheckRecieve;
    static bool bIsRecievePacket;

    [UnityTest]
    [Category("StrixLibrary")]
    public IEnumerator 로컬_UDP송수신_기본테스트()
    {
        pPacketCheckRecieve = SPacketTest.Dummy();
        Assert.AreEqual(pPacketCheckRecieve, SPacketTest.Dummy());

        CManagerNetworkUDPTest.EventMakeSingleton();
        CManagerNetworkUDPTest.instance.DoStartListen_UDP(iTestPort);

        bIsRecievePacket = false;
        SPacketTest pSendPacket = new SPacketTest(1, "보냈다");

        // 보내기 전에는 체크용 패킷과 보낼 패킷과 일치하지 않는다.
        Assert.AreNotEqual(pPacketCheckRecieve.iValue, pSendPacket.iValue);
        Assert.AreNotEqual(pPacketCheckRecieve.strValue, pSendPacket.strValue);

        CManagerNetworkUDPTest.instance.DoSendPacket_UDP(strTestTargetIP, iTestPort, new SPacketTest(1, "보냈다"));
        while (bIsRecievePacket == false)
        {
            yield return null;
        }

        // 패킷을 받은 뒤에는 체크용 패킷과 보낼 패킷이 일치한다.
        Assert.AreEqual(pPacketCheckRecieve.iValue, pSendPacket.iValue);
        Assert.AreEqual(pPacketCheckRecieve.strValue, pSendPacket.strValue);
    }

    [Test]
    [Category("StrixLibrary")]
    public void 링버퍼_인큐_디큐_테스트()
    {
        SPacketTest pPacketTest = new SPacketTest(1, "인큐_디큐_테스트");
        SPacketTest pPacketTest2 = new SPacketTest(2, "더미데이터");

        byte[] arrPacketData = ConvertByteArray(pPacketTest);
        int iDataSize = arrPacketData.Length;

        _pRecvBuffer.Enqueue(arrPacketData);

        Assert.AreNotEqual(pPacketTest.iValue, pPacketTest2.iValue);
        Assert.AreNotEqual(pPacketTest.strValue, pPacketTest2.strValue);

        byte[] arrPacketData2 = _pRecvBuffer.Dequeue_OrNull(iDataSize);
        ConvertPacket(arrPacketData2, out pPacketTest2);

        Assert.AreEqual(pPacketTest.iValue, pPacketTest2.iValue);
        Assert.AreEqual(pPacketTest.strValue, pPacketTest2.strValue);
    }

    [Test]
    [Repeat(10)]
    [Category("StrixLibrary")]
    public void 링버퍼_인큐_디큐_랜덤_부하테스트()
    {
        Queue<bool> pTestQueue = new Queue<bool>();
        int iRandomEnqueueCount = UnityEngine.Random.Range(10000, 100000);
        for(int i = 0; i < iRandomEnqueueCount; i++)
        {
            bool bIsEnqeue = UnityEngine.Random.Range(0, 2) == 0;
            pTestQueue.Enqueue(bIsEnqeue);
        }
    }

    [UnityTest]
    [Category("StrixLibrary")]
    public IEnumerator 로컬_UDP송수신_패킷헤더및_패킷디큐_테스트()
    {
        pPacketCheckRecieve = SPacketTest.Dummy();
        Assert.AreEqual(pPacketCheckRecieve, SPacketTest.Dummy());

        CManagerNetworkUDPTest.EventMakeSingleton();
        CManagerNetworkUDPTest.instance.DoStartListen_UDP(iTestPort);

        bIsRecievePacket = false;
        SPacketTest pSendPacket = new SPacketTest(1, "보냈다");

        // 보내기 전에는 체크용 패킷과 보낼 패킷과 일치하지 않는다.
        Assert.AreNotEqual(pPacketCheckRecieve.iValue, pSendPacket.iValue);
        Assert.AreNotEqual(pPacketCheckRecieve.strValue, pSendPacket.strValue);

        CManagerNetworkUDPTest.instance.DoSendPacket_UDP(strTestTargetIP, iTestPort, new SPacketTest(1, "보냈다"));
        while (bIsRecievePacket == false)
        {
            yield return null;
        }

        // 패킷을 받은 뒤에는 체크용 패킷과 보낼 패킷이 일치한다.
        Assert.AreEqual(pPacketCheckRecieve.iValue, pSendPacket.iValue);
        Assert.AreEqual(pPacketCheckRecieve.strValue, pSendPacket.strValue);
    }
}

#endif
#endregion Test
