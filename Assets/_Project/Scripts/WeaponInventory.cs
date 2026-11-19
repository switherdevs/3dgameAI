using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInventory : MonoBehaviour
{
    private readonly List<Gun> weapons = new();
    private int selectedIndex = -1;
    public Gun SelectedWeapon => weapons.Count == 0 ? null : weapons[selectedIndex];
    public System.Action<Gun> OnSelectedWeaponChanged;

    void Awake()
    {
        foreach (var gun in GetComponentsInChildren<Gun>(true))
        {
            weapons.Add(gun);
            gun.enabled = false;
        }
        if (weapons.Count > 0) Select(0);
    }

    void Update()
    {
        if (Mouse.current == null) return;
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll > 0) SelectNext(1);
        else if (scroll < 0) SelectNext(-1);
    }

    public void OnInteract()
    {
        foreach (var pickup in FindObjectsByType<WeaponPickup>(FindObjectsSortMode.None))
            if (pickup.CanCollect(this, transform.position)) { pickup.Collect(this); break; }
    }

    public void OnNext() => SelectNext(1);
    public void OnPrevious() => SelectNext(-1);

    public bool TryPickup(Gun gun)
    {
        if (gun == null || weapons.Contains(gun)) return false;
        foreach (var owned in weapons) owned.enabled = false;
        gun.transform.SetParent(transform, false);
        gun.enabled = false;
        gun.name = gun.name.Replace(" (Pickup)", "");
        weapons.Add(gun);
        Select(weapons.Count - 1);
        return true;
    }

    public void SelectNext(int direction)
    {
        if (weapons.Count < 2 || direction == 0) return;
        Select((selectedIndex + direction + weapons.Count) % weapons.Count);
    }

    void Select(int index)
    {
        selectedIndex = index;
        for (int i = 0; i < weapons.Count; i++) weapons[i].enabled = i == selectedIndex;
        OnSelectedWeaponChanged?.Invoke(SelectedWeapon);
    }
}
