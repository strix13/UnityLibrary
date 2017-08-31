using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NETWORK.Protocol
{
    enum REQ
    {
        _REQ_LOGIN = 0,   // 로그인 요청
        _REQ_CREATE_ACCOUNT,   // 아이디 생성 요청
    };
    public struct REQ_LOGIN
    {
        public string strNickname;
        public REQ_LOGIN(bool bAlloc)
        {
            strNickname = "";
        }
    }
    public struct REQ_CREATE_ACCOUNT
    {
        public string strNickname;
        public REQ_CREATE_ACCOUNT(bool bAlloc)
        {
            strNickname = "";
        }
    }
}