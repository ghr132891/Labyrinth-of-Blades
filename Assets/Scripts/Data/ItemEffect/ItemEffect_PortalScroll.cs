using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Protal Scroll", fileName = "Item Effect data - Portal Scroll")]
public class ItemEffect_PortalScroll : ItemEffect_DataSo
{
    public override void ExecuteEffect()
    {
        if(SceneManager.GetActiveScene().name == "Level_0")
        {
            Debug.Log("Cannot open portal in town !");
            return;
        }

        Player player = Player.instance;
        Vector3 portalPosition = player.transform.position + new Vector3(player.facingDir * 1.5f, 0);

        Object_Portal.instance.ActivatePortal(portalPosition,player.facingDir);

    }

}
