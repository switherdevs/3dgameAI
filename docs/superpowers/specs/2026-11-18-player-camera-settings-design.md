# Player Rotation and Camera Settings Design

- Intent: Make the player face the WASD movement direction while keeping camera orbit independent; expose the existing camera sensitivity through the existing Esc settings UI.
- Goals: Player movement never writes camera rotation; camera mouse sensitivity is adjustable; Esc opens/closes settings and pauses/resumes; group settings objects under the existing `Other/Canvas` hierarchy.
- Non-goals: New UI redesign, new plugin/dependency, aim rotation changes, or unrelated scene cleanup.
- Constraints: Keep existing New Input System, uGUI, scripts, serialized scene setup, and existing user changes. Camera aiming/fire stays centered on camera.
- Approaches: (1) Rotate player from movement vector, recommended because it separates movement from camera yaw. (2) Rotate toward camera aim, rejected because movement would not determine facing. (3) Add a separate camera pivot/controller, rejected as unnecessary restructuring.
- Implementation call: Use camera yaw to calculate movement basis, but do not mutate camera from PlayerController; smoothly face nonzero movement. SettingsMenu updates ThirdPersonCamera sensitivity and restores cursor/time state on Esc. Reuse existing scene UI objects and group under Canvas.
- Validation: Unity compile and console delta; verify player facing and camera orbit independently, Esc open/close, slider sensitivity in Play Mode. Existing Console errors are baseline and must be distinguished from new errors.
