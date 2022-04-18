using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FighterHitDetectionController : MonoBehaviour
{
    public List<Models.HitPointsData> hitPointsDatas = new List<Models.HitPointsData>();
    public List<Models.AttackPointData> attackPointDatas = new List<Models.AttackPointData>();

    public EnumClass.AttackPointTypes activeAttackType = EnumClass.AttackPointTypes.None;
    private bool isRunning = false;

    private void Start()
    {
        isRunning = true;
        if (hitPointsDatas.Count <= 0)
            return;

        for (int i = 0; i < hitPointsDatas.Count; i++)
        {
            hitPointsDatas[i].triggers.transform.parent = hitPointsDatas[i].hitPoint;
        }
    }

    private void OnValidate()
    {
        if (isRunning)
            return;

        if (hitPointsDatas.Count <= 0)
                return;

        for (int i = 0; i < hitPointsDatas.Count; i++)
        {
            hitPointsDatas[i].triggers.transform.position = hitPointsDatas[i].hitPoint.position + hitPointsDatas[i].offset;
            hitPointsDatas[i].triggers.transform.rotation = hitPointsDatas[i].hitPoint.rotation;
            hitPointsDatas[i].triggers.transform.localScale = hitPointsDatas[i].scale;
        }
    }

    public List<Vector3> GetDamageCircleSpawnPosition(EnumClass.AttackPointTypes attackType)
    {
        Models.AttackPointData[] _attackPointDatas = attackPointDatas.Where(x => x.attackPointType == attackType).ToArray();

        List<Vector3> points = new List<Vector3>();

        foreach (var data in _attackPointDatas)
        {
            points.Add(data.attackPoint.position);
        }

        return points;
    }

    public EnumClass.HitPointTypes GetHitPointType(Collider collider)
    {
        return hitPointsDatas.Where(x => x.triggers.name == collider.name).FirstOrDefault().hitPointType;
    }
}
