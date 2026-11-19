using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponentInParent<WeaponInventory>();
        var gun = inventory != null ? inventory.SelectedWeapon : other.GetComponent<Gun>();
        if (gun == null) gun = other.GetComponentInParent<Gun>();
        if (gun == null) gun = other.GetComponentInChildren<Gun>();
        if (gun == null) return;

        gun.AddReserveAmmo(ammoAmount);
        Destroy(gameObject);
    }
}
