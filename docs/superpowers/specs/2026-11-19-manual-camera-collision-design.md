# Manual Camera and Wall Collision Design

- Intent: camera angle and player facing remain controlled by the player, while the third-person camera follows the player position and avoids clipping through walls.
- Goals: remove movement-driven player rotation; preserve mouse orbit and existing camera follow; use a sphere cast to shorten camera distance near obstacles and smoothly restore distance afterward.
- Non-goals: add a camera package, change input mappings, redesign camera controls, or alter movement direction.
- Constraints: use Unity's built-in Physics API and existing `ThirdPersonCamera`/`PlayerController`; keep the current scene and input setup.
- Approaches: retain current movement-driven facing and only collision-adjust the camera (does not meet manual facing); add a camera package (unneeded dependency); remove movement-driven facing and add sphere-cast camera collision (selected as the smallest complete fix).
- Validation: Unity compile/console check, assert movement does not rotate the player, and assert the camera shortens its distance when an obstacle intersects the camera path. If Unity's test runner is unavailable, disclose that and report runtime checks separately.
