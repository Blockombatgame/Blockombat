using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = Constants.Editor_Menu_Prefix + "/Items/" + nameof(Item))]
public class Item : ScriptableObject
{
    public EnumClass.ItemID itemID;
    public EnumClass.ItemType itemType;
    public EnumClass.ItemPurchaseState itemPurchaseState;
    public Sprite iconImage;
    public int price;
    public string itemTagName;

    [Header("Character Stats")]
    public List<Models.CharacterStat> characterStats;
}
