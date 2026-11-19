# Imperial Fortress Interiors and Weapons — Design

## Intent
Complete the existing `SampleScene` so players can enter larger fortress buildings, collect ammunition inside, pick up another weapon with E, switch weapons, and see the equipped weapon in the HUD.

## Goals
- Keep `Player`, camera, movement, shooting, ammo, HUD, checkpoints, route X=0–120, and `MissionEndPoint`.
- Reuse the existing `Gun`, `AmmoPickup`, `GameUI`, and New Input System.
- Add a compact weapon inventory: E picks up a nearby weapon; number keys and mouse wheel switch; HUD displays selected weapon and ammo.
- Replace the solid Area 2 bunker with an enterable, primitive-built building and place ammo inside. Add a larger enterable building in the map route if existing layout permits.

## Non-goals / anti-goals
- No ProBuilder, packages, new scene, replacement player/camera, or rewrite of shooting.
- No decorative systems or advanced weapon-stat framework.
- Do not destroy unrelated inactive map or existing gameplay objects.

## Constraints and calls
- Unity 6.3 project, New Input System, primitive GameObjects and existing URP materials.
- Existing input actions already bind `Interact` to E, `Previous` to 1, and `Next` to 2; preserve them. Mouse wheel can be read directly.
- Existing `AmmoPickup` is trigger-based; place pickups with triggers clear of walls and weapon collision.
- Use existing `Gun` instances as inventory entries; only equipped Gun is enabled. Keep current weapon equipped at spawn.
- A weapon pickup requires entering its small trigger and pressing E; number keys 1/2 and mouse wheel switch owned weapons.

## Approaches
- Add a separate weapon framework and replace current shooting: rejected because it duplicates `Gun` and risks breaking firing/ammo.
- Use existing `Gun` components with a small inventory/pickup bridge: selected because it satisfies pickup/switch/HUD while preserving combat code.
- Keep the current solid bunker and add a separate tiny room: rejected because the requested large enterable structures remain unmet.

## Design
Add a small `WeaponInventory` component to Player. It owns existing and picked-up `Gun` references, receives `Interact`/`Previous`/`Next` from PlayerInput Send Messages, handles mouse wheel, and raises a selected-weapon change event. A `WeaponPickup` trigger asks the Player inventory to collect its disabled Gun when E is pressed nearby; collection reparents the Gun under the Player, and switching enables exactly one Gun. Extend `GameUI` to display the selected Gun name and existing ammo counts. Keep the existing ammo pickup contract and place ammo crates inside the new building interiors.

Build larger enterable structures from Unity cubes with separate floor, ceiling, wall segments, and a clear doorway. Keep the route/collider layer and existing spawn links intact.

## Verification
Unity compile/console clean; tests or focused runtime assertions cover pickup, switching, exactly-one-enabled, and selected HUD text. Play Mode checks entry/exit, ammo pickup, route grounding, and screenshot. Save `Assets/Scenes/SampleScene.unity`.
