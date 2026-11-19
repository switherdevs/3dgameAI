using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    private Gun gun;

    void Awake() => gun = GetComponentInChildren<Gun>(true);
    public bool CanCollect(WeaponInventory inventory, Vector3 playerPosition)
    {
        if (gun == null) gun = GetComponentInChildren<Gun>(true);
        return gun != null && inventory != null && Vector3.Distance(transform.position, playerPosition) <= 2f;
    }
    public void Collect(WeaponInventory inventory)
    {
        if (!CanCollect(inventory, inventory.transform.position)) return;
        if (!inventory.TryPickup(gun)) return;
        Destroy(gameObject);
    }
}
