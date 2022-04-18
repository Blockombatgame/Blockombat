using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodIdentity : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(RemoveBlood(gameObject));
    }

    IEnumerator RemoveBlood(GameObject blood)
    {
        yield return new WaitForSeconds(2.5f);
        //Remove blood
        FactoryManager.Instance.prefabsFactory.Recycle(blood.GetComponent<BloodIdentity>());
    }
}
