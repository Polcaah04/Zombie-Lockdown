using UnityEngine;

public class AmmoItem : Item
{
    public int m_AddAmmoCount = 10;

    public override void Pick(PlayerController player)
    {
        base.Pick(player);
        player.AddAmmo(m_AddAmmoCount);

    }
}
