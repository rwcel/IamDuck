using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    public System.Action onSuccess;
    public System.Action onFail;

    private static IStoreController m_StoreController;                  // ���� ������ �����ϴ� �Լ��� ����
    private static IExtensionProvider m_StoreExtensionProvider;     // ���� �÷����� ���� Ȯ�� ó���� ����

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
            Debug.Log("������� ����");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                Debug.Log($" ���� ���� �õ� ��� {result} ");
            });
        }
        {
            Debug.Log($" �������� �ʴ� �÷����Դϴ�. Current = {Application.platform} ");
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
            // Debug.Log("IAP - �ʱ�ȭ ����");
            onFail?.Invoke();
            return;
        }

        Product product = m_StoreController.products.WithID(productID);
        if(product != null && product.availableToPurchase)
        {
            Debug.Log($"IAP ���� - {product.definition.id}");

            m_StoreController.InitiatePurchase(productID);
        }
        else
        {
            Debug.Log("���� �Ұ�");
            onFail?.Invoke();
        }
    }


    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP - ����");

        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"IAP - �ʱ�ȭ���� : {error}");

        // onFail?.Invoke();
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"���Ž��� : {product} - {failureReason}");

        onFail?.Invoke();
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        Debug.Log($"IAP - ���� ��ǰ : {purchaseEvent.purchasedProduct.definition.id}");

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
