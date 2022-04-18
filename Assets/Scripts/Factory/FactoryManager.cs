using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance;

    public PrefabsFactory prefabsFactory;
    public ItemsFactory itemsFactory;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        EventManager.Instance.OnFactoryReset += ClearAllPools;
        SetData();
    }

    public void ClearAllPools()
    {
        prefabsFactory.ResetPool();
    }

    public void SetData()
    {
        for (int i = 0; i < itemsFactory.items.Count; i++)
        {
            if (PlayerPrefs.GetString(itemsFactory.items[i].itemID.ToString()) == "Bought")
            {
                itemsFactory.items[i].itemPurchaseState = EnumClass.ItemPurchaseState.Bought;
            }
        }
    }
}