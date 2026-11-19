# Manual Camera Collision Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Keep player facing under user control and prevent third-person camera wall clipping.

**Architecture:** Remove movement-driven `Rigidbody.MoveRotation` from `PlayerController`. In `ThirdPersonCamera`, sphere-cast from focus toward ideal camera position; clamp distance to hit distance minus padding and smoothly restore it when unobstructed.

**Tech Stack:** Unity 6, C#, built-in Physics.

**Spec:** `docs/superpowers/specs/2026-11-19-manual-camera-collision-design.md`

## Global Constraints
- Keep the existing New Input System and scene.
- Add no dependencies.
- Preserve mouse orbit and camera follow.

## Review Focus
- Obstacle directly between focus and camera: camera stays before the obstacle.
- No obstacle: camera returns smoothly to configured distance.
- Moving without mouse input: player rotation does not change.
- Obstacle collider starts close to focus: camera remains outside it using minimum padding.

---

### Task 1: Player facing and camera collision

**Files:**
- Modify: `Assets/_Project/Scripts/PlayerController.cs`
- Modify: `Assets/_Project/Scripts/ThirdPersonCamera.cs`
- Test: Unity MCP runtime assertions; no existing project gameplay test suite.

- [ ] Record baseline camera/player state and confirm no existing test assets cover these behaviors.
- [ ] Implement player-facing ownership by removing movement-driven rotation.
- [ ] Add sphere-cast obstruction handling and smooth restoration to camera distance.
- [ ] Refresh Unity, inspect compile errors, and run runtime assertions for facing and obstruction behavior.
- [ ] Save `Assets/Scenes/SampleScene.unity` only if scene changes are needed (expected none).
