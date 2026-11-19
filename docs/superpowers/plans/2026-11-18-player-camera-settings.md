# Player Camera Settings Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Face the player toward movement while keeping camera orbit independent and making the existing Settings UI control camera sensitivity.

**Architecture:** PlayerController derives movement from the camera's horizontal axes and rotates only the player. ThirdPersonCamera owns its own orbit and sensitivity. Existing SettingsMenu controls pause/cursor and synchronizes its slider with camera sensitivity. Existing UI GameObjects are grouped under Canvas.

**Tech Stack:** Unity 6, C#, New Input System, uGUI, TextMeshPro, Unity Test Framework.

**Spec:** `docs/superpowers/specs/2026-11-18-player-camera-settings-design.md`

## Global Constraints
- Reuse existing dependencies and UI objects.
- Do not overwrite unrelated dirty scene/assets or reset user changes.
- Do not add camera rotation writes in PlayerController.
- Keep camera-based aiming intact.

## Review Focus
- Zero movement: player rotation must remain stable; verify in Play Mode.
- Diagonal movement: facing matches actual normalized travel direction; verify in Play Mode.
- Pause open/close: timeScale and cursor restore correctly; verify with Esc and Resume.
- Slider edge values: camera sensitivity updates and displayed value remains accurate; test in Play Mode.
- Existing 19 console errors: compare against initial cursor; report only new errors as regressions.

---

### Task 1: Movement-facing and independent camera orbit

**Files:**
- Modify: `Assets/_Project/Scripts/PlayerController.cs`
- Modify: `Assets/_Project/Scripts/ThirdPersonCamera.cs`
- Test: Unity Test Framework Play Mode tests if existing test assembly supports these components; otherwise run explicit Play Mode verification.

**Interfaces:** Existing input handlers and serialized settings remain unchanged. Player rotation follows horizontal movement; camera sensitivity is exposed for SettingsMenu.

- [ ] Verify current input and camera orientation in the open scene; capture baseline console cursor.
- [ ] Add a failing test for movement-driven player facing and unchanged camera rotation, if test assembly available.
- [ ] Implement movement vector-based player yaw; remove player mouse-yaw mutation.
- [ ] Run test; enter Play Mode and verify forward/side/diagonal movement plus independent mouse orbit.

### Task 2: Settings and hierarchy

**Files:**
- Modify: `Assets/_Project/Scripts/ThirdPersonCamera.cs`
- Modify: `Assets/_Project/Scripts/SettingsMenu.cs`
- Modify: `Assets/Scenes/SampleScene.unity` via connected Unity Editor.

**Interfaces:** SettingsMenu references existing panel, slider, label and resume button. ThirdPersonCamera exposes Get/Set sensitivity.

- [ ] Add failing test for slider-driven camera sensitivity if test assembly supports scene UI.
- [ ] Wire slider to ThirdPersonCamera; retain existing PlayerController compatibility only if other callers require it.
- [ ] Ensure Esc and Resume open/close panel, pause/resume, and lock/show cursor consistently.
- [ ] Parent the four settings objects under `Other/Canvas` without rebuilding the UI; make panel initially hidden.
- [ ] Save scene and verify slider, Esc, resume, and hierarchy in Play Mode.

### Task 3: Verification

- [ ] Compile scripts; inspect Console entries since baseline cursor.
- [ ] Confirm movement, camera orbit, Settings pause/resume, and sensitivity in Play Mode.
- [ ] Report baseline errors separately; do not clear Console or revert unrelated modifications.
