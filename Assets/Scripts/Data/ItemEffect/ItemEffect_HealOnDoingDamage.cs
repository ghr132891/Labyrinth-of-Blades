using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal On Doing Damage", fileName = "Item Effect data - Heal On Doing Damage")]
public class ItemEffect_HealOnDoingDamage : ItemEffect_DataSo
{
    [SerializeField] private float percentHealedOnAttack = .20f;


    public override void Subsribe(Player play)
    {
        base.Subsribe(play);
        play.combat.OnDoingPhysiclDamage += HealOnDoingDamage;
    }

    public override void Unsubribe()
    {
        player.combat.OnDoingPhysiclDamage -= HealOnDoingDamage;
        player = null;
    }
    private void HealOnDoingDamage(float damage)
    {
        player.health.IncreaseHealth(damage * percentHealedOnAttack);
    }


}
