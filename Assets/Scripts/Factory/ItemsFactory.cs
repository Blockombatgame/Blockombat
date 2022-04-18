using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = Constants.Editor_Menu_Prefix + "/Factories/" + nameof(ItemsFactory))]
public class ItemsFactory : ScriptableObject
{
    public List<Item> items = new List<Item>();

    public List<Item> GetItems(EnumClass.ItemType itemType)
    {
        return items.Where(x => x.itemType == itemType).ToList();
    }

    public List<Item> GetBoughtCharacters()
    {
        return GetItems(EnumClass.ItemType.Character).Where(x => x.itemPurchaseState == EnumClass.ItemPurchaseState.Bought).ToList();
    }

    public EnumClass.ItemPurchaseState IsItemBought(string itemTagName)
    {
        return items.Where(x => x.itemTagName == itemTagName).FirstOrDefault().itemPurchaseState;
    }

    public Item GetItem(string itemTagName)
    {
        return items.Where(x => x.itemTagName == itemTagName).FirstOrDefault();
    }
}
