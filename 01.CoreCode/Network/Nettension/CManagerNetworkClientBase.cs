//#define Proud

#if Proud
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Nettention.Proud;
using C2C;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-11 오전 10:11:58
   Description : 
   Edit Log    : 
   ============================================ */

[RequireComponent(typeof(CCompoDontDestroyObj))]
abstract public class CManagerNetworkClientBase<CLASS> : CSingletonBase<CLASS>
    where CLASS : CManagerNetworkClientBase<CLASS>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */
    public List<EventDelegate> p_listOnConnectServer_Success = new List<EventDelegate>();
    public List<EventDelegate> p_listOnConnectServer_Fail = new List<EventDelegate>();

    public HostID p_hHostID { get { return _hHostID; } }

	/* protected - Variable declaration         */
	protected NetClient _pClient = new NetClient();
    protected HostID _hHostID;
    protected HostID _hGroupID;
    protected bool _bIsConnected;

    /* private - Variable declaration           */
    private NetConnectionParam _pNetConnectionParam = new NetConnectionParam();

    private bool _bIsConnectWaiting;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    public void DoConnectServer()
    {
        _bIsConnectWaiting = true;
        _pClient.Connect(_pNetConnectionParam);
    }

    public void DoConnectServer(EventDelegate.Callback OnConnectSuccess)
    {
        EventDelegate pEventDelegate = new EventDelegate(OnConnectSuccess);
        pEventDelegate.oneShot = true;
        p_listOnConnectServer_Success.Add(pEventDelegate);

        _bIsConnectWaiting = true;
        _pClient.Connect(_pNetConnectionParam);
    }

    public void DoConnectServer(EventDelegate.Callback OnConnectSucces, EventDelegate.Callback OnConnectFail)
    {
        EventDelegate pEventDelegate = new EventDelegate(OnConnectSucces);
        pEventDelegate.oneShot = true;
        p_listOnConnectServer_Success.Add(pEventDelegate);

        pEventDelegate = new EventDelegate(OnConnectFail);
        pEventDelegate.oneShot = true;
        p_listOnConnectServer_Fail.Add(pEventDelegate);

        _bIsConnectWaiting = true;
        _pClient.Connect(_pNetConnectionParam);
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    abstract protected void OnInitProxyAndStub(NetClient pClient, NetConnectionParam pNetConnectionParam);

    virtual protected void OnConnect_Server_Success(ByteArray arrReplyFromServer) { }
    virtual protected void OnConnect_Server_Failed(ErrorInfo info) { }
    virtual protected void OnDisConnect_Server() { }

    virtual protected void OnConnect_ClientOther(HostID memberHostID, ByteArray customField) { }
    virtual protected void OnDisConnect_ClientOther(HostID memberHostID, HostID groupHostID) { }
    
    virtual protected void OnError(ErrorInfo errorInfo)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ProudNet Network - Error : " + errorInfo.ToString(), this);
#endif
    }
    virtual protected void OnException(HostID remoteID, System.Exception e)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ProudNet Network - Exception : " + e.ToString(), this);
#endif
    }

    virtual protected void OnInformation(ErrorInfo errorInfo)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ProudNet Network - Information " + errorInfo.ToString());
#endif
    }

    virtual protected void OnNoRmiProcessed(RmiID rmiID)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ProudNet Network - NoRmiProcessed : " + rmiID);
#endif
    }

    virtual protected void OnWarning(ErrorInfo errorInfo)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ProudNet Network - Warinig :  " + errorInfo.ToString());
#endif
    }

    virtual protected void OnChangeServerUdp(ErrorType reason)
    {
#if UNITY_EDITOR
        if(reason != ErrorType.ErrorType_Ok)
            Debug.LogWarning("ProudNet Network - ChangeServerUdp " + reason, this);
#endif
    }

    virtual protected void OnReceiveUserMessage(HostID sender, RmiContext rmiContext, ByteArray payload)
    {
#if UNITY_EDITOR
        Debug.LogWarning("ProudNet Network - ReceiveUserMessage HostID : " + sender, this);
#endif
    }

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pClient.JoinServerCompleteHandler = OnConnectedServerComplete;
        _pClient.LeaveServerHandler = OnLeaveServer;
        _pClient.P2PMemberJoinHandler = OnP2PMemberJoin;
        _pClient.P2PMemberLeaveHandler = OnP2PMemberLeave;

        _pClient.ErrorHandler = OnError;
        _pClient.WarningHandler = OnWarning;
        _pClient.ExceptionHandler = OnException;
        _pClient.InformationHandler = OnInformation;

        _pClient.NoRmiProcessedHandler = OnNoRmiProcessed;
        _pClient.ChangeServerUdpStateHandler = OnChangeServerUdp;
        _pClient.ReceivedUserMessageHandler = OnReceiveUserMessage;

        OnInitProxyAndStub(_pClient, _pNetConnectionParam);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (_bIsConnected || _bIsConnectWaiting)
            _pClient.FrameMove();
    }

    void OnDestroy()
    {
        _pClient.Disconnect();
        _pClient.Dispose();
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void OnConnectedServerComplete(ErrorInfo info, ByteArray arrReplyFromServer)//
    {
        _bIsConnected = info.errorType == ErrorType.ErrorType_Ok;
        _bIsConnectWaiting = false;

		if (arrReplyFromServer == null)
			arrReplyFromServer = new ByteArray();

        if (_bIsConnected)
        {
            EventDelegate.Execute(p_listOnConnectServer_Success);
            OnConnect_Server_Success(arrReplyFromServer);
        }
        else
        {
            EventDelegate.Execute(p_listOnConnectServer_Fail);
            OnConnect_Server_Failed(info);
        }
    }

    private void OnLeaveServer(ErrorInfo errorInfo)
    {
        _bIsConnected = false;
        OnDisConnect_Server();
    }

    private void OnP2PMemberJoin(HostID memberHostID, HostID groupHostID, int memberCount, ByteArray customField)
    {
        if (memberHostID != _pClient.LocalHostID)
            OnConnect_ClientOther(memberHostID, customField);
    }

    private void OnP2PMemberLeave(HostID memberHostID, HostID groupHostID, int memberCount)
    {
        OnDisConnect_ClientOther(memberHostID, groupHostID);
    }

    /* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
#endif