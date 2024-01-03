
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class IAPManager : SingletonMonoBehaviour<IAPManager>, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.

    public string diamon1 = "com.timny.diamon1";
    public string diamon2 = "com.timny.diamon2";
    public string diamon3 = "com.timny.diamon3";
    public string diamon4 = "com.timny.diamon4";

    public string diamon5 = "com.timny.diamon5";
    public string diamon6 = "com.timny.diamon6";
    
    //

    public int diamonamount1 = 50;
    public int diamonamount2 = 100;
    public int diamonamount3 = 200;
    public int diamonamount4 = 300;
    public int diamonamount5 = 500;
    public int diamonamount6 = 1000;
    
    // Apple App Store-specific product identifier for the subscription product.

    // Google Play Store-specific product identifier subscription product.


    void Start()
    {
        DontDestroyOnLoad(this);
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.
        //  builder.AddProduct(removeAds, ProductType.NonConsumable);
        // Continue adding the non-consumable product.
        //builder.AddProduct(special, ProductType.NonConsumable);
        //builder.AddProduct(special_fake, ProductType.NonConsumable);
        builder.AddProduct(diamon1, ProductType.NonConsumable);
        builder.AddProduct(diamon2, ProductType.NonConsumable);
        builder.AddProduct(diamon3, ProductType.NonConsumable);
        builder.AddProduct(diamon4, ProductType.NonConsumable);
        builder.AddProduct(diamon5, ProductType.NonConsumable);
        builder.AddProduct(diamon6, ProductType.NonConsumable);

        //builder.AddProduct(removeAdsCoins, ProductType.NonConsumable);
        //builder.AddProduct(removeAdsHeart, ProductType.NonConsumable);
        //builder.AddProduct(subscription, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }


    public bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }


    public void BuyDiamon1()
    {
        BuyProductID(diamon1);
    }  
    public void BuyDiamon2()
    {
        BuyProductID(diamon2);
    }  
    public void BuyDiamon3()
    {
        BuyProductID(diamon3);
    }
    public void BuyDiamon4()
    {
        BuyProductID(diamon4);
    }
    public void BuyDiamon5()
    {
        BuyProductID(diamon5);
    }
    public void BuyDiamon6()
    {
        BuyProductID(diamon6);
    }
    public string GetProducePriceFromStore(string id)
    {
        if (m_StoreController != null && m_StoreController.products != null)
        {
            return m_StoreController.products.WithID(id).metadata.localizedPriceString;
        }
        else
        {
            return "";
        }
    }
    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }


    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");
            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        var m_GooglePlayExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        var m_IOSExtensions = extensions.GetExtension<IAppleExtensions>();

#if UNITY_ANDROID

        //Dictionary<string, string> Dict = m_GooglePlayExtensions.GetProductJSONDictionary();
#endif
#if UNITY_IOS

        Dictionary<string, string> Dict = m_IOSExtensions.GetIntroductoryPriceDictionary();
#endif

        /*foreach (Product item in controller.products.all)
        {
            if (item.receipt != null)
            {
                if (item.definition.type == ProductType.Subscription)
                {
                    print("Subscription");

                    string json = (Dict == null || !Dict.ContainsKey(item.definition.storeSpecificId)) ? null : Dict[item.definition.storeSpecificId];
                    SubscriptionManager sup = new SubscriptionManager(item, json);
                    SubscriptionInfo info = sup.getSubscriptionInfo();

                    if (info.isSubscribed() == Result.False)
                    {

                    }
                    else if (info.isSubscribed() == Result.True)
                    {
                        if (info.isExpired() == Result.True)
                        {
                                
                         }
                         else
                         {
                                
                         }
                    }
                }
            }
        }*/
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (String.Equals(args.purchasedProduct.definition.id, diamon1, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            CommonData.player_online_info.Diamond +=(uint) diamonamount1;
            //U3dCmn.SendMessage("91Manager","BuyDiamond",diamonamount1);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, diamon2, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            CommonData.player_online_info.Diamond +=(uint) diamonamount2;
            //U3dCmn.SendMessage("91Manager","BuyDiamond",diamonamount2);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, diamon3, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            CommonData.player_online_info.Diamond +=(uint) diamonamount3;
            //U3dCmn.SendMessage("91Manager","BuyDiamond",diamonamount3);

        }
        else if (String.Equals(args.purchasedProduct.definition.id, diamon4, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            CommonData.player_online_info.Diamond +=(uint) diamonamount4;
            //U3dCmn.SendMessage("91Manager","BuyDiamond",diamonamount4);
        }
        else if (String.Equals(args.purchasedProduct.definition.id, diamon5, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            CommonData.player_online_info.Diamond +=(uint) diamonamount5;
            //U3dCmn.SendMessage("91Manager","BuyDiamond",diamonamount5);
            
        }
        else if (String.Equals(args.purchasedProduct.definition.id, diamon6, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            CommonData.player_online_info.Diamond +=(uint) diamonamount6;
            //U3dCmn.SendMessage("91Manager","BuyDiamond",diamonamount6);
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));

        }
        PlayerInfoManager.RefreshPlayerDataUI();

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }


    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}

