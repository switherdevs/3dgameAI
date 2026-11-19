# Imperial Fortress Interiors and Weapons Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make fortress buildings enterable with ammo inside, add E-based weapon pickup, switching, and selected-weapon HUD in the existing scene.

**Architecture:** Reuse `Gun`, `AmmoPickup`, `GameUI`, and `PlayerInput` Send Messages. Add a small inventory and pickup component; compose walkable buildings from Unity primitives in `SampleScene`.

**Tech Stack:** Unity 6.3, C#, New Input System, URP, Unity primitives.

**Spec:** `docs/superpowers/specs/2026-11-19-imperial-fortress-interiors-weapons-design.md`

## Global Constraints
- Keep `Assets/Scenes/SampleScene.unity`; do not create a scene.
- Keep Player, camera, movement, shooting, ammo, HUD, checkpoints, route X=0–120, and `MissionEndPoint`.
- Do not use ProBuilder or add packages.
- Retain existing New Input System bindings: Interact=E, Previous=1, Next=2.
- Keep pickup colliders clear of building collisions and movement route layer `grond`.

## Review Focus
- Pickup out of range must not collect weapon; cover with trigger/range assertion.
- Switching must enable exactly one owned Gun and retain ammo values; cover with inventory assertions.
- Existing ammo pickup must still affect equipped weapon; cover with Play Mode check.
- Door openings and interior floors must have non-blocking passage and `grond` collision; cover with bounds/raycast checks.
- HUD must follow current selected weapon after pickup/switch; cover with text assertion.

---

### Task 1: Weapon pickup and inventory behavior

**Files:**
- Create: `Assets/_Project/Scripts/WeaponInventory.cs`
- Create: `Assets/_Project/Scripts/WeaponPickup.cs`
- Create: `Assets/_Project/Tests/PlayMode/WeaponInventoryTests.cs` (only if test assembly setup permits; otherwise use focused Play Mode assertions through Unity MCP)
- Modify: `Assets/_Project/Scripts/GameUI.cs`

**Interfaces:**
- `WeaponInventory.TryPickup(Gun gun) -> bool`
- `WeaponInventory.SelectNext(int direction) -> void`
- `WeaponInventory.SelectedWeapon -> Gun`
- `WeaponInventory.OnSelectedWeaponChanged -> System.Action<Gun>`
- `WeaponPickup` receives E while Player is in its trigger and passes its child `Gun` to inventory.

- [ ] Write failing tests/assertions for accepting a new Gun once, rejecting duplicates, switching selection, and exactly one active Gun.
- [ ] Run tests/assertions and observe failure against missing inventory behavior.
- [ ] Implement the inventory/pickup components and selected weapon HUD with existing input bindings.
- [ ] Verify tests/assertions pass; check existing ammo HUD follows selected Gun.

### Task 2: Enterable buildings and interior pickups

**Files:**
- Modify: `Assets/Scenes/SampleScene.unity` via Unity MCP; no scene scripts unless required.

**Interfaces:**
- Interior floors use `grond` layer; walls/roof colliders form a doorway at least 2.2m high and 1.5m wide.
- Weapon pickups and existing `AmmoPickup` triggers sit inside clear walkable interior areas.

- [ ] Add/assert a large Area 1 building and replace the solid Area 2 bunker with segmented walls, roof, floor, and a clear doorway.
- [ ] Place a second weapon pickup and ammo pickup inside interiors; avoid player spawn and mission markers.
- [ ] Verify door clearance, interior floor raycasts, weapon/ammo pickup reachability, and existing route continuity.

### Task 3: Unity verification and save

**Files:**
- Modify: `Assets/Scenes/SampleScene.unity`

- [ ] Wait for Unity compilation and inspect console errors/warnings.
- [ ] Run available Play Mode tests; otherwise run focused MCP assertions for pickup/switch/ammo/HUD.
- [ ] Enter both buildings in Play Mode and confirm collision, ammo collection, weapon switching, and HUD name.
- [ ] Save scene, validate scene, and capture Game View screenshot.
