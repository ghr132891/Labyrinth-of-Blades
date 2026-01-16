
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Ice Blast", fileName = "Item Effect data - Ice Blast On Taking Damage")]
public class ItemEffect_IceBlastOnTakingDamage : ItemEffect_DataSo
{
    [SerializeField] private ElementalEffectData effectData;
    [SerializeField] private float iceDamage;
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [SerializeField] private float healthPercentTrigger = .25f;
    [SerializeField] private float cooldown;
    [NonSerialized] private float lastTimeUesed = -999;

    [Header("Vfx Objects")]
    [SerializeField] private GameObject iceBlastVfx;
    [SerializeField] private GameObject onHitVfx;
    public override void ExecuteEffect()
    {
        bool noCoolDown = Time.time >= lastTimeUesed + cooldown;
        bool reachedThresold = player.health.GetHealthPercent() <= healthPercentTrigger;

        if (noCoolDown && reachedThresold)
        {

            player.vfx.CreateEffectOf(iceBlastVfx, player.transform);
            lastTimeUesed = Time.time;
            DamageEnemiesWithIce();
        }

    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(player.transform.position, 1.5f, whatIsEnemy);

        foreach (var target in enemies)
        {
            IDamagable damagable = target.GetComponent<IDamagable>();

            if (damagable == null) continue;

            bool targetGotHit = damagable.TakeDamage(0, iceDamage, ElementType.Ice, player.transform);

            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, effectData);

            if (targetGotHit)
                player.vfx.CreateEffectOf(onHitVfx, target.transform);

        }

    }

    public override void Subsribe(Player play)
    {
        base.Subsribe(play);
        player.health.OnTakingDamage += ExecuteEffect;
    }

    public override void Unsubribe()
    {
        base.Unsubribe();
        player.health.OnTakingDamage -= ExecuteEffect;
        player = null;
    }
}
