using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    public System.Action onSuccess;
    public System.Action onFail;

    private static IStoreController m_StoreController;                  // 구매 과정을 제어하는 함수를 제공
    private static IExtensionProvider m_StoreExtensionProvider;     // 여러 플랫폼을 위한 확장 처리를 제공

    public bool IsInitialized => m_StoreController != null && m_StoreExtensionProvider != null;


    protected override void AwakeInstance()
    {
        InitializePurchasing();
    }

    protected override void DestroyInstance()
    {
    }

    void InitializePurchasing()
    {
        if (IsInitialized)
            return;

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var shopData in GameData.Instance.ShopDatas)
        {
            // shopData.type
            if (shopData.productID != "")
            {
                builder.AddProduct(shopData.productID, ProductType.Consumable);
                //shopData.type == EShopType.Limit ? ProductType.NonConsumable : ProductType.Consumable);
            }
        }

        UnityPurchasing.Initialize(this, builder);

        // Debug.Log("IAP - Build");
    }

    public void RestorePurchases()
    {
        if (!IsInitialized) return;

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("리스토어 시작");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                Debug.Log($" 구매 복구 시도 결과 {result} ");
            });
        }
        {
            Debug.Log($" 지원하지 않는 플랫폼입니다. Current = {Application.platform} ");
        }

    }


    public bool HadPurchased(string productID)
    {
        if (!IsInitialized) 
            return false;

        // if (CodelessIAPStoreListener.Instance.StoreController.products.WithID(productID).hasReceipt)

        var product = m_StoreController.products.WithID(productID);
        if (product != null)
        {
            return product.hasReceipt;
        }

        return false;
    }

    public void BuyProductID(string productID, System.Action _onSuccess, System.Action _onFail)
    {
        onSuccess = _onSuccess;
        onFail = _onFail;

        if (!IsInitialized)
        {
            // Debug.Log("IAP - 초기화 실패");
            onFail?.Invoke();
            return;
        }

        Product product = m_StoreController.products.WithID(productID);
        if(product != null && product.availableToPurchase)
        {
            Debug.Log($"IAP 구매 - {product.definition.id}");

            m_StoreController.InitiatePurchase(productID);
        }
        else
        {
            Debug.Log("구매 불가");
            onFail?.Invoke();
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP - 시작");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"IAP - 초기화실패 : {error}");

        // onFail?.Invoke();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"구매실패 : {product} - {failureReason}");

        onFail?.Invoke();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"IAP - 구매 상품 : {purchaseEvent.purchasedProduct.definition.id}");

        // BackendManager.Instance.PurchaseReceipt(purchaseEvent.purchasedProduct);

        onSuccess?.Invoke();

        return PurchaseProcessingResult.Complete;
    }

    public Product GetProduct(string _productId)
    {
        return m_StoreController.products.WithID(_productId);
    }

    public string GetPrice(string _productId)
    {
        // Debug.Log(_productId);
        var product = GetProduct(_productId);
        if(product.metadata.isoCurrencyCode == "KRW")
        {
            return $"\\{((int)product.metadata.localizedPrice).CommaThousands()}";
        }
        else
        {
            return $"{product.metadata.localizedPriceString}";
        }
    }

    public decimal GetPriceToDecimal(string _productId)
    {
        return GetProduct(_productId).metadata.localizedPrice;
    }


}
