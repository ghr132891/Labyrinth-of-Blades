using UnityEngine;

public class Object_BlackSmith : Object_NPC, IInteractable
{

    private Animator anim;
    public void Interact()
    {
        Debug.Log("Open craft or storage.");
    }

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("isBlackSmith",true);
    }
}
