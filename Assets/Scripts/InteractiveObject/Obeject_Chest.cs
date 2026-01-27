using UnityEngine;

public class Obeject_Chest : MonoBehaviour, IDamagable
{
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx =>GetComponent<Entity_VFX>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [Header("Open Details")]
    [SerializeField] private Vector2 openSence;
    [SerializeField] private bool canDropItems = true;
    public bool TakeDamage(float damage, float elementalDamage,ElementType element , Transform damageDealer)
    {
        if (canDropItems == false)
            return false;
            
        dropManager.DropItems();
        vfx.PlayerOnDamageVfx();
        anim.SetBool("chestOpen",true);
        rb.linearVelocity = openSence;
        rb.angularVelocity = Random.Range(-200f,200f);

        return true;
    }


}
