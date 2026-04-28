using System.Collections;

using UnityEngine;

public class Player_VFX : Entity_VFX
{
    
    public void CreateEffectOf(GameObject effect, Transform target)
    {
        Instantiate(effect, target.position, Quaternion.identity);
    }

}
