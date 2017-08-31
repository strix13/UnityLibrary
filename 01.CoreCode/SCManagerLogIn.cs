using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Convert = System.Convert;
using System.IO;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-06-19 오후 1:53:44
   Description : 
   Edit Log    : 
   ============================================ */

public class SCManagerLogIn : CSingletonBase_Not_UnityComponent<SCManagerLogIn>
{
    /* const & readonly declaration             */

    public const int const_iLimitLength_Email = 50;
    public const int const_iLimitLength_Password = 20;

    private const string const_strLocalPath_INI = "/INI";
    private const string const_strLogInFileName = "Account";
	private const string const_strPasswordKey = "Strix";

    /// <summary>
    /// Regular expression, which is used to validate an E-Mail address.
    /// </summary>
    public const string const_strMatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
          + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    /* enum & struct declaration                */

    public enum EResult_Login
    {
        Login_Success,
        Login_Fail_Wrong_Account,
        Login_Fail_NotExist_LocalFile,
    }

    public enum EResult_RegistAccount
    {
        RegistAccount_Success,
        RegistAccount_Fail_ExistEmail,
        RegistAccount_Fail_EmailLength,
        RegistAccount_Fail_PasswordLength,
        RegistAccount_Fail_Not_EmailFormat,
        RegistAccount_Fail_Other,
    }

	public enum EResult_ChangePassword
	{
		ChangePassword_Success,
		ChangePassword_Fail,
	}

	public enum EResult_FindPassword
	{
		FindPassword_Success,
		FindPassword_Fail
	}

	private enum ERandomCharType
    {
        LowerCase,
        UpperCase,
        Number
    }

    [System.Serializable]
    public class SINI_Account
    {
        public string strEmailAddress;
        public string strPassword;
		public string strNick;
    }

	/* public - Field declaration            */

	static public event System.Action p_EVENT_OnSuccessLogin;

    /* protected - Field declaration         */

    /* private - Field declaration           */
	
    static private System.Action<bool> _OnResult_DB;
    static private System.Action<EResult_Login> _OnResult_AutoLogin;
    static private System.Action<EResult_RegistAccount> _OnResult_RegistAccount;

    static private SCManagerParserJson _pManagerJson;
    static private StringBuilder _pStrBuilder = new StringBuilder();
    static private CCompoTemp _pObjectDummy_ForCoroutine;

    static private SINI_Account _pLoginInfo;	static public SINI_Account p_pLoginInfo {  get { return _pLoginInfo; } }

    static private string _strEmailAddress;
    static private string _strPassword;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    static public void DoCheckAutoLogin(System.Action<SCManagerLogIn.EResult_Login> OnFinishAutoLogin)
    {
        Check_And_InitManagerLogin();

        if (PrimitiveHelper.CheckIsValidString(_strEmailAddress) == false ||
            PrimitiveHelper.CheckIsValidString(_strPassword) == false)
        {
            // 저장되있는 로그인 정보가 유효하지 않다.
            OnFinishAutoLogin(EResult_Login.Login_Fail_NotExist_LocalFile);
        }
        else
        {
            _OnResult_AutoLogin = OnFinishAutoLogin;
            _pObjectDummy_ForCoroutine.StartCoroutine(CManagerNetworkDB_Common.instance.CoExcutePHP(_strEmailAddress, EPHPName.Check_Value, CDefineMap.const_strTableName_Account, OnResult_LoginAuto, new StringPair("password", _strPassword)));
        }
    }

    static public void DoLogin(string strEmailAddress, string strPassword, System.Action<bool> OnFinishCheckID)
    {
		strPassword = ProcEncrypt_AES256(strPassword);
		Check_And_InitManagerLogin();

        _OnResult_DB = OnFinishCheckID;
        _strEmailAddress = strEmailAddress;
        _strPassword = strPassword;

        _pObjectDummy_ForCoroutine.StartCoroutine(CManagerNetworkDB_Common.instance.CoExcutePHP(strEmailAddress, EPHPName.Check_Value, CDefineMap.const_strTableName_Account, OnResult_Login, new StringPair("password", strPassword)));
    }

    static public void DoRegistAccount(string strEmailAddress, string strPassword, System.Action<SCManagerLogIn.EResult_RegistAccount> OnResult_RegistAccount)
    {
        Check_And_InitManagerLogin();

        if (strEmailAddress.Length >= const_iLimitLength_Email)
        {
            OnResult_RegistAccount(EResult_RegistAccount.RegistAccount_Fail_EmailLength);
            return;
        }
        else if(strPassword.Length >= const_iLimitLength_Password)
        {
            OnResult_RegistAccount(EResult_RegistAccount.RegistAccount_Fail_PasswordLength);
            return;
        }
        else if (IsEmail(strEmailAddress) == false)
        {
            OnResult_RegistAccount(EResult_RegistAccount.RegistAccount_Fail_Not_EmailFormat);
            return;
        }

		strPassword = ProcEncrypt_AES256(strPassword);
		_strEmailAddress = strEmailAddress;
        _strPassword = strPassword;
        _OnResult_RegistAccount = OnResult_RegistAccount;

        _pObjectDummy_ForCoroutine.StartCoroutine(CManagerNetworkDB_Common.instance.CoExcutePHP(strEmailAddress, EPHPName.Get, CDefineMap.const_strTableName_Account, OnResult_ExistEmail, new StringPair("id", _strEmailAddress)));
    }

    static public void DoChangePassword(string strEmailAddress, string strPassword_Old, string strPassword_New, System.Action<bool> OnFinishChangePassword)
    {
        Check_And_InitManagerLogin();

		strPassword_Old = ProcEncrypt_AES256(strPassword_Old);
		strPassword_New = ProcEncrypt_AES256(strPassword_New);

		_strEmailAddress = strEmailAddress;
        _strPassword = strPassword_New;
        _OnResult_DB = OnFinishChangePassword;
        _pObjectDummy_ForCoroutine.StartCoroutine(CManagerNetworkDB_Common.instance.CoExcutePHP(strEmailAddress, EPHPName.CheckValue_AndUpdateSet, CDefineMap.const_strTableName_Account, OnResult_ChangePassword, new StringPair("password", strPassword_Old), new StringPair("passwordNew", strPassword_New)));
    }

    static public void DoResetPassword(string strEmailAddress)
    {
        Check_And_InitManagerLogin();

        _strEmailAddress = strEmailAddress;
        _strPassword = CalculateHashPassword(10);
        _pObjectDummy_ForCoroutine.StartCoroutine(CManagerNetworkDB_Common.instance.CoExcutePHP(strEmailAddress, EPHPName.Update_Set, CDefineMap.const_strTableName_Account, OnResult_ResetPassword, new StringPair("password", _strPassword)));
    }

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    static private void Check_And_InitManagerLogin()
    {
        if (_pObjectDummy_ForCoroutine != null) return;

        GameObject pObjectDummy = new GameObject("ManagerLogInDummy_ForCoroutine");
        _pObjectDummy_ForCoroutine = pObjectDummy.AddComponent<CCompoTemp>();

        _pManagerJson = SCManagerParserJson.DoMakeClass(_pObjectDummy_ForCoroutine, const_strLocalPath_INI, SCManagerResourceBase<SCManagerParserJson, string, TextAsset>.EResourcePath.PersistentDataPath);
        if (_pManagerJson.DoReadJson<string, SINI_Account>(const_strLogInFileName, out _pLoginInfo) == false)
        {
            _pLoginInfo = new SINI_Account();
            _pManagerJson.DoWriteJson<string>(const_strLogInFileName, _pLoginInfo);
        }
        else
        {
            _strEmailAddress = _pLoginInfo.strEmailAddress;
            _strPassword = _pLoginInfo.strPassword;
        }
    }


	static private void OnResult_Login(bool bResult)
	{
		_OnResult_DB(bResult);
		if (bResult)
		{
			if (p_EVENT_OnSuccessLogin != null)
				p_EVENT_OnSuccessLogin();

			_pLoginInfo.strEmailAddress = _strEmailAddress;
			_pLoginInfo.strPassword = _strPassword;
			_pManagerJson.DoWriteJson(const_strLogInFileName, _pLoginInfo);
		}
	}

	static private void OnResult_LoginAuto(bool bResult)
    {
        if (bResult)
		{
			_OnResult_AutoLogin(EResult_Login.Login_Success);

			if (p_EVENT_OnSuccessLogin != null)
				p_EVENT_OnSuccessLogin();

			_pLoginInfo.strEmailAddress = _strEmailAddress;
			_pLoginInfo.strPassword = _strPassword;
			_pManagerJson.DoWriteJson(const_strLogInFileName, _pLoginInfo);
		}
		else
            _OnResult_AutoLogin(EResult_Login.Login_Fail_Wrong_Account);
    }
	
    static private void OnResult_ExistEmail(bool bResult)
    {
        if (bResult)
            _OnResult_RegistAccount(EResult_RegistAccount.RegistAccount_Fail_ExistEmail);
        else
            _pObjectDummy_ForCoroutine.StartCoroutine(CManagerNetworkDB_Common.instance.CoExcutePHP(_strEmailAddress, EPHPName.Insert, CDefineMap.const_strTableName_Account, OnResult_RegistAccount, new StringPair("id", _strEmailAddress), new StringPair("password", _strPassword)));
    }

    static private void OnResult_RegistAccount(bool bResult)
    {
        if (bResult)
        {
            _pLoginInfo.strEmailAddress = _strEmailAddress;
            _pLoginInfo.strPassword = _strPassword;
            _pManagerJson.DoWriteJson(const_strLogInFileName, _pLoginInfo);
            _OnResult_RegistAccount(EResult_RegistAccount.RegistAccount_Success);
        }
        else
        {
            _OnResult_RegistAccount(EResult_RegistAccount.RegistAccount_Fail_Other);
        }
    }

    static private void OnResult_ChangePassword(bool bResult)
    {
        _OnResult_DB(bResult);
        if(bResult)
        {
            _pLoginInfo.strEmailAddress = _strEmailAddress;
            _pLoginInfo.strPassword = _strPassword;
            _pManagerJson.DoWriteJson(const_strLogInFileName, _pLoginInfo);
        }
    }

    static private void OnResult_ResetPassword(bool bResult)
    {
        SCManagerSMTP.DoSendEmail(_strEmailAddress, "Noonbaram Password Change", string.Format("New Password : {0}", _strPassword));
    }

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

    static public string CalculateHashPassword(int iLength)
    {
        _pStrBuilder.Length = 0;
        for (int i = 0; i < iLength; i++)
        {
            ERandomCharType eRandomType = (ERandomCharType)Random.Range(0, PrimitiveHelper.GetEnumLength<ERandomCharType>());
            int iRandomChar = 0;
            switch (eRandomType)
            {
                case ERandomCharType.LowerCase:
                    iRandomChar = Random.Range(Convert.ToInt32('a'), Convert.ToInt32('z'));
                    _pStrBuilder.Append(Convert.ToChar(iRandomChar));
                    break;

                case ERandomCharType.UpperCase:
                    iRandomChar = Random.Range(Convert.ToInt32('A'), Convert.ToInt32('Z'));
                    _pStrBuilder.Append(Convert.ToChar(iRandomChar));
                    break;

                case ERandomCharType.Number:
                    iRandomChar = Random.Range(0, 10);
                    _pStrBuilder.Append(iRandomChar.ToString());
                    break;
            }
        }

        return _pStrBuilder.ToString();
    }



    /// <summary>
    /// Checks whether the given Email-Parameter is a valid E-Mail address.
    /// </summary>
    /// <param name="email">Parameter-string that contains an E-Mail address.</param>
    /// <returns>True, wenn Parameter-string is not null and contains a valid E-Mail address;
    /// otherwise false.</returns>
    public static bool IsEmail(string email)
    {
        if (email != null) return System.Text.RegularExpressions.Regex.IsMatch(email, const_strMatchEmailPattern);
        else return false;
    }


	static private string ProcEncrypt_AES256(string InputText)
	{
		RijndaelManaged RijndaelCipher = new RijndaelManaged();

		// 입력받은 문자열을 바이트 배열로 변환  
		byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(InputText);

		// 딕셔너리 공격을 대비해서 키를 더 풀기 어렵게 만들기 위해서   
		// Salt를 사용한다.  
		byte[] Salt = Encoding.ASCII.GetBytes(const_strPasswordKey.Length.ToString());

		PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(const_strPasswordKey, Salt);

		// Create a encryptor from the existing SecretKey bytes.  
		// encryptor 객체를 SecretKey로부터 만든다.  
		// Secret Key에는 32바이트  
		// Initialization Vector로 16바이트를 사용  
		ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(16), SecretKey.GetBytes(16));

		MemoryStream memoryStream = new MemoryStream();

		// CryptoStream객체를 암호화된 데이터를 쓰기 위한 용도로 선언  
		CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

		cryptoStream.Write(PlainText, 0, PlainText.Length);

		cryptoStream.FlushFinalBlock();

		byte[] CipherBytes = memoryStream.ToArray();

		memoryStream.Close();
		cryptoStream.Close();

		string EncryptedData = Convert.ToBase64String(CipherBytes);

		return EncryptedData;
	}

	//AES_256 복호화  
	static private string ProcDecrypt_AES256(string InputText)
	{
		RijndaelManaged RijndaelCipher = new RijndaelManaged();

		byte[] EncryptedData = Convert.FromBase64String(InputText);
		byte[] Salt = Encoding.ASCII.GetBytes(const_strPasswordKey.Length.ToString());

		PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(const_strPasswordKey, Salt);

		// Decryptor 객체를 만든다.  
		ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(16), SecretKey.GetBytes(16));

		MemoryStream memoryStream = new MemoryStream(EncryptedData);

		// 데이터 읽기 용도의 cryptoStream객체  
		CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

		// 복호화된 데이터를 담을 바이트 배열을 선언한다.  
		byte[] PlainText = new byte[EncryptedData.Length];

		int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

		memoryStream.Close();
		cryptoStream.Close();

		string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

		return DecryptedData;
	}
}
