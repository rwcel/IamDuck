using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gaa;

public class OnestoreManager : Singleton<OnestoreManager>
{
    enum PurchaseButtonState
    {
        NONE, ACKNOWLEDGE, CONSUME, REACTIVE, CANCEL
    };

    enum ERecurringState
    {
        Force = -1,
        Consume = 0
    }

    [HideInInspector]
    public Dictionary<string, ProductDetail> productDetails = new Dictionary<string, ProductDetail>();
    //public List<ProductDetail> productDetails;

    private Dictionary<string, PurchaseData> purchaseMap = new Dictionary<string, PurchaseData>();
    private Dictionary<string, string> signatureMap = new Dictionary<string, string>();

    private System.Action OnSuccess;
    private System.Action OnFail;

    private const string storePublicKey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCNojg0dpFAqv0kP+HU6/WqGeCvdLu+eMSEcare+5GdXw+IBWkGOZczDH3LrKGrF+e8P2I2Mzel2uOv6IEn5vRRFsGG2kC4zOqPY+5QtxLISxPKKzH728VUZT2IR65458fL6gsrV/NUxZQ3EWMJMY1rez5omAX6yvft3nps2UDYfwIDAQAB";

    private static readonly string _Tag = "Onestore";

    private string forceProductID = "";
    public string ForceProductID => forceProductID;

    public List<string> onestore_All_Products;
    // public string[] onestore_All_Products;

    List<FGetItem> getItems;
    public List<FGetItem> GetItems => getItems;
    public void ClearItems() => getItems = null;


    protected override void AwakeInstance()
    {
        getItems = new List<FGetItem>();

        GaaIapResultListener.OnLoadingVisibility += OnLoadingVisibility;

        GaaIapResultListener.PurchaseClientStateResponse += PurchaseClientStateResponse;
        GaaIapResultListener.OnPurchaseUpdatedResponse += OnPurchaseUpdatedResponse;
        GaaIapResultListener.OnQueryPurchasesResponse += OnQueryPurchasesResponse;
        GaaIapResultListener.OnProductDetailsResponse += OnProductDetailsResponse;

        GaaIapResultListener.OnConsumeSuccessResponse += OnConsumeSuccessResponse;

        GaaIapResultListener.SendLog += SendLog;
    }

    protected override void DestroyInstance()
    {
        GaaIapResultListener.OnLoadingVisibility -= OnLoadingVisibility;

        GaaIapResultListener.PurchaseClientStateResponse -= PurchaseClientStateResponse;
        GaaIapResultListener.OnPurchaseUpdatedResponse -= OnPurchaseUpdatedResponse;
        GaaIapResultListener.OnQueryPurchasesResponse -= OnQueryPurchasesResponse;
        GaaIapResultListener.OnProductDetailsResponse -= OnProductDetailsResponse;

        GaaIapResultListener.OnConsumeSuccessResponse -= OnConsumeSuccessResponse;

        GaaIapResultListener.SendLog -= SendLog;

        GaaIapCallManager.Destroy();
    }

    public PurchaseData GetPurchaseData(string productId)
    {
        PurchaseData pData = null;
        foreach (KeyValuePair<string, PurchaseData> pair in purchaseMap)
        {
            if (productId.Equals(pair.Key))
            {
                pData = pair.Value;
                break;
            }
        }

        return pData;
    }

    private void SendLog(string tag, string message)
    {
        Debug.Log($"{tag} : {message}");
    }

    // ======================================================================================
    // Request
    // ======================================================================================

    void Start()
    {
        if (GameData.Instance.Store != EStore.Onestore)
            return;

#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(StartConnectService());
#endif
    }

    IEnumerator StartConnectService()
    {
        GetProducts();
        yield return Values.Delay1;
        StartConnection();
    }

    void GetProducts()
    {
        onestore_All_Products = new List<string>();
        foreach (var shopData in GameData.Instance.ShopDatas)
        {
            if (shopData.productID != "")
            {
                onestore_All_Products.Add(shopData.productID);
                // onestore_All_Products[idx++] = 
            }
        }

        Debug.Log($"???????? ???? ???? : {onestore_All_Products.Count}");
    }

    void StartConnection()
    {
        if (GaaIapCallManager.IsServiceAvailable() == false)
        {
            SendLog(_Tag, "StartConnection");
            OnLoadingVisibility(true);
            GaaIapCallManager.StartConnection(storePublicKey);
        }
        else
        {
            SendLog(_Tag, "Fail Connection.");
        }
    }

    void QueryPurchases()
    {
        OnLoadingVisibility(true);
        purchaseMap.Clear();
        signatureMap.Clear();

        GaaIapCallManager.QueryPurchases(ProductType.INAPP);
    }

    // ???? ????
    void ConsumePurchase(string productId)
    {
        SendLog(_Tag, "???? ?????? key " + productId);
        PurchaseData purchaseData = GetPurchaseData(productId);
        if (purchaseData != null)
        {
            OnLoadingVisibility(true);
            GaaIapCallManager.Consume(purchaseData, /*developerPayload*/null);
        }
        else
        {
            SendLog(_Tag, "key???? ???????? ???????? ????????.");
            OnFail?.Invoke();
        }
    }

    // ======================================================================================
    // Response
    // ======================================================================================

    private void OnLoadingVisibility(bool obj)
    {
        Debug.Log($"??????... : {obj}");
    }

    private void PurchaseClientStateResponse(IapResult iapResult)
    {
        SendLog(_Tag, "????????:\n\t\t-> " + iapResult.ToString());
        if (iapResult.IsSuccess())
        {
            SendLog(_Tag, "???? ????");
            QueryPurchases();
            GaaIapCallManager.QueryProductDetails(onestore_All_Products.ToArray(), Gaa.ProductType.ALL);
        }
        else
        {
            GaaIapResultListener.HandleError("PurchaseClientStateResponse", iapResult);
        }
    }

    void OnPurchaseUpdatedResponse(List<PurchaseData> purchases, List<string> signatures)
    {
        ParsePurchaseData("OnPurchaseUpdatedResponse", purchases, signatures);
    }

    void OnQueryPurchasesResponse(List<PurchaseData> purchases, List<string> signatures)
    {
        ParsePurchaseData("OnQueryPurchasesResponse", purchases, signatures);
    }

    private void ParsePurchaseData(string func, List<PurchaseData> purchases, List<string> signatures)
    {
        SendLog(_Tag, func);
        for (int i = 0; i < purchases.Count; i++)
        {
            PurchaseData p = purchases[i];
            string s = signatures[i];

            purchaseMap.Add(p.productId, p);
            signatureMap.Add(p.productId, s);

            PurchaseButtonState state = PurchaseButtonState.NONE;
            state = PurchaseButtonState.CONSUME;

            string id = p.productId;
            // ????, ???? ?????? ?? Response
            OnPurchaseItemClick(id, state);

            SendLog(_Tag, "PurchaseData[" + i + "]: " + p.productId);
        }
    }

    void OnPurchaseItemClick(string productId, PurchaseButtonState state)
    {
        SendLog(_Tag, $"?????????? ??????:\n\t\t-> {productId} - {state}");

        ConsumePurchase(productId);     // ?????? ?????? ????
    }

    void OnProductDetailsResponse(List<ProductDetail> products)
    {
        SendLog(_Tag, "??????");
        foreach (var product in products)
        {
            Debug.Log(product.productId);
            productDetails.Add(product.productId, product);
        }

        //productDetails = products;
    }

    void OnConsumeSuccessResponse(PurchaseData purchaseData)
    {
        // ???? React
        if (purchaseData != null)
        {
            SendLog(_Tag, $"???? ????:\n\t\t-> {purchaseData.productId} : {(ERecurringState)purchaseData.recurringState}");

            purchaseMap.Remove(purchaseData.productId);
            signatureMap.Remove(purchaseData.productId);

            if (purchaseData.recurringState == (int)ERecurringState.Consume)
            {
                OnSuccess?.Invoke();
            }
            else if (purchaseData.recurringState == (int)ERecurringState.Force)
            {
                // ???? ?? ?????? ????. ?????? ?????? ????????
                forceProductID = purchaseData.productId;
            }
        }
        else
        {
            OnFail?.Invoke();
        }
    }

    public void ForceConsumeProduct()
    {
        if (forceProductID == "")
            return;

        foreach (var shopData in GameData.Instance.ShopDatas)
        {
            if (shopData.productID == forceProductID)
            {
                foreach (var item in shopData.items)
                {
                    getItems.Add(item);
                }
            }
        }

        //if (getItems.Count > 0)
        //{
        //    SystemUI.Instance.OpenReward(getItems);
        //}
    }

    // ======================================================================================
    // ???????? ????
    // ======================================================================================

#if UNITY_EDITOR
    // ???? ???? ???? ??
    public void BuyProductID(string productID, System.Action successAction, System.Action failAction)
    {
        OnSuccess?.Invoke();
    }
#else
    // ???? ???? ???? ??
    public void BuyProductID(string productID, System.Action successAction, System.Action failAction)
    {
        OnSuccess = successAction;
        OnFail = failAction;

        if(productDetails.ContainsKey(productID))
        {
            BuyProduct(productDetails[productID]);
        }
        else
        {
            SendLog(_Tag, "???????? -> ????????");
            OnFail?.Invoke();
        }
    }

    void BuyProduct(ProductDetail detail)
    {
        SendLog(_Tag, $"???? ????: {detail.productId} / ???? : {detail.type}");

        PurchaseFlowParams param = new PurchaseFlowParams();
        param.productId = detail.productId;
        param.productType = detail.type;
        //param.productName = "";
        //param.devPayload = "your Developer Payload";
        //param.gameUserId = "";
        //param.promotionApplicable = false;

        GaaIapCallManager.LaunchPurchaseFlow(param);
    }
#endif
}
