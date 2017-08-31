#if UNITY_ADS

using System;
using UnityEngine;
using UnityEngine.Purchasing;

/* ============================================ 
   Editor      : KJH                               
   Date        : 2017-02-14 오후 5:50:01
   Description : 
   Edit Log    : 
   ============================================ */
public class SCUnityIAPService<TENUM> : IStoreListener
	where TENUM : IConvertible, IComparable
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */
	private static event Action<TENUM, bool> _EVENT_OnPurchased = null;

	private static IStoreController _pStoreCtrl = null;
	private static IExtensionProvider _pExtProvider = null;

	private static bool _bInit = false;
	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */
	public static void DoInit(Action<TENUM, bool> onPurchased)
	{
		if (_bInit) return;

		int iLen = PrimitiveHelper.GetEnumMax<TENUM>();
		if (iLen == 0) { Debug.LogWarning("Unity IAP : 상품을 등록해주세요!"); return; }

		StandardPurchasingModule pModule = StandardPurchasingModule.Instance();
		ConfigurationBuilder pBuilder = ConfigurationBuilder.Instance(pModule);

		TENUM[] eEnum = PrimitiveHelper.DoGetEnumType<TENUM>();

		for (int i = 0; i < iLen; i++)
		{
			string strProductID = eEnum[i].ToString();

			Debug.Log(strProductID);

			pBuilder.AddProduct(strProductID, ProductType.Consumable, new IDs {
				{strProductID, AppleAppStore.Name},
				{strProductID, GooglePlay.Name}
			});
		}

		IStoreListener pStoreListener = new SCUnityIAPService<TENUM>();

		UnityPurchasing.Initialize(pStoreListener, pBuilder);
		_EVENT_OnPurchased = onPurchased;
	}

	public static void DoPurchase(TENUM eProductID)
	{
		if (_bInit == false) { Debug.LogWarning("Unity IAP : 초기화 안되있음."); return; }

		string strProductID = eProductID.ToString();

		try
		{
			Product pProduct = _pStoreCtrl.products.WithID(strProductID);
			if (pProduct != null && pProduct.availableToPurchase)
			{
				Debug.Log(string.Format("Unity IAP : {0} 상품을 구입 중...", pProduct.definition.id));
				_pStoreCtrl.InitiatePurchase(pProduct);
			}
			else
			{
				Debug.LogWarning("Unity IAP : 등록 된 상품이 없거나 알수없는 에러가 발생했습니다.");
			}
		}
		catch(Exception pExc)
		{
			Debug.LogWarning("Unity IAP : 구입 중에 알수없는 에러 발생! : " + pExc);
		}
	}

	/* public - [Event] Function             
       프랜드 객체가 호출                       */
	public void OnInitialized(IStoreController pStoreCtrl, IExtensionProvider pExtProvider)
	{
		if (_bInit) return;

		_pStoreCtrl = pStoreCtrl;
		_pExtProvider = pExtProvider;

		Debug.Log("Unity IAP : 초기화 성공!");

		_bInit = true;
	}

	public void OnInitializeFailed(InitializationFailureReason eReason)
	{
		Debug.LogWarning("Unity IAP : 초기화 실패! 사유 : " + eReason);

		_bInit = false;
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs pPurchaseEvent)
	{
		string strProductID = pPurchaseEvent.purchasedProduct.definition.id;
		Debug.Log(string.Format("Unity IAP : {0} 상품 구입에 성공했습니다!", strProductID));

		_EVENT_OnPurchased(strProductID.ConvertEnum<TENUM>(), true);

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product pProduct, PurchaseFailureReason eResult)
	{
        string strProductID = pProduct.definition.storeSpecificId;

        _EVENT_OnPurchased(strProductID.ConvertEnum<TENUM>(), false);
        Debug.LogWarning(string.Format("Unity IAP : 상품 구입 실패... 상품명 : {0}\n구입불가 사유 : {1}", strProductID, eResult));
	}

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */
}

#endif