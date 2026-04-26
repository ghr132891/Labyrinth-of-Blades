using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [Header("Counter Attack Details")]
    [SerializeField] private float counterRecovery = .1f;
    [SerializeField] private LayerMask whatisCounterable;

    public bool  CounterAttackPerformed()
    {
        bool hasCounteredSomeone = false;

        foreach(var target in GetDetectedColliders(whatisCounterable))
        {
            ICounterable counterable = target.GetComponent<ICounterable>();

            if (counterable == null)
                continue;

            if (counterable.CanBeCountered)
            {
                counterable.HandleCounter();
                hasCounteredSomeone = true;
            }
        }

        return hasCounteredSomeone;
    }

    public float GetCounterRecoveryDuration() => counterRecovery;
}
