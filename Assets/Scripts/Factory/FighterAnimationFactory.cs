using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//stores all fighters animations
[CreateAssetMenu(menuName = Constants.Editor_Menu_Prefix + "/Factories/" + nameof(FighterAnimationFactory))]
public class FighterAnimationFactory : ScriptableObject
{
    public List<Models.AnimationSet> animationSets;

    public Models.AnimationSet? GetAnimation(EnumClass.FighterAnimations animationID)
    {
        return animationSets.Where(x => x.animationID == animationID).FirstOrDefault();
    }
}
