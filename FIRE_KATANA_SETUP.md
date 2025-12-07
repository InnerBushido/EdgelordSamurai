# Dynamic Pyrotechnic Katana System - Setup Guide

## Overview
This guide will help you set up velocity-reactive fire VFX and audio for your katana in VR. The fire will respond dynamically to how fast you swing the sword, creating premium "juice" for immersive gameplay.

## What You'll Create
- **Core Fire**: Flames attached to the blade that move with it (Local space)
- **Trail Fire**: Flames that stay in the air as you swing (World space)
- **Dynamic Audio**: Whoosh sound that gets louder and higher-pitched as you swing faster
- **Optimized Performance**: Maintains 90 FPS on Quest 3

---

## Step 1: Prepare Your Katana GameObject

1. **Locate your katana in the Hierarchy** (or create one if needed)
2. **Ensure it has these children**:
   - `Blade` (the cutting part)
   - `Handle` (the grip)

Your hierarchy should look like:
```
Katana (parent - follows XR controller)
├── Handle
├── Blade
└── [We'll add fire particle systems here]
```

---

## Step 2: Create Core Fire Particle System

### 2.1 Create the Core Fire GameObject
1. **Right-click on Katana** in Hierarchy
2. Select **Effects > Particle System**
3. Rename it to `CoreFire`
4. **Position**: Set to (0, 0, 0.5) - halfway down the blade
5. **Rotation**: (0, 0, 0)

### 2.2 Configure Core Fire - Main Module
1. Select `CoreFire` in Hierarchy
2. In Inspector, configure **Particle System Main Module**:
   - **Duration**: 5.00 (looping)
   - **Looping**: ✓ Checked
   - **Start Lifetime**: Random Between Two Constants → 0.3 - 0.8
   - **Start Speed**: Random Between Two Constants → 0.5 - 1.5
   - **Start Size**: Random Between Two Constants → 0.05 - 0.15
   - **Start Color**: Orange/Yellow gradient (you can use your fire asset colors)
   - **Gravity Modifier**: 0
   - **Simulation Space**: **Local** ⚠️ CRITICAL - particles move with blade
   - **Max Particles**: 50

### 2.3 Configure Core Fire - Emission Module
1. **Emission**:
   - **Rate over Time**: 10 (this will be controlled by script)
   - **Rate over Distance**: 0

### 2.4 Configure Core Fire - Shape Module
1. **Shape**:
   - **Shape**: Box
   - **Scale**: X=0.02, Y=0.02, Z=0.8 (thin box along blade length)
   - **Random Direction**: ✓ Checked

### 2.5 Configure Core Fire - Color over Lifetime
1. **Color over Lifetime**: ✓ Enable
2. Create gradient: Yellow → Orange → Red → Transparent
   - Time 0.0: Bright Yellow/White (RGB: 1, 1, 0.8, Alpha: 1)
   - Time 0.3: Orange (RGB: 1, 0.5, 0, Alpha: 1)
   - Time 0.7: Dark Red (RGB: 0.5, 0, 0, Alpha: 0.5)
   - Time 1.0: Transparent (Alpha: 0)

### 2.6 Configure Core Fire - Size over Lifetime
1. **Size over Lifetime**: ✓ Enable
2. Create curve that shrinks particles:
   - Start: 1.0
   - End: 0.2

### 2.7 Configure Core Fire - Renderer Module
1. **Render Mode**: Billboard
2. **Material**: Use your fire material from the asset store
3. **Sorting Fudge**: 0
4. **Min Particle Size**: 0
5. **Max Particle Size**: 0.5

---

## Step 3: Create Trail Fire Particle System

### 3.1 Create the Trail Fire GameObject
1. **Right-click on Katana** in Hierarchy
2. Select **Effects > Particle System**
3. Rename it to `TrailFire`
4. **Position**: Set to (0, 0, 0.5) - same as CoreFire
5. **Rotation**: (0, 0, 0)

### 3.2 Configure Trail Fire - Main Module
1. Select `TrailFire` in Hierarchy
2. In Inspector, configure **Particle System Main Module**:
   - **Duration**: 5.00 (looping)
   - **Looping**: ✓ Checked
   - **Start Lifetime**: Random Between Two Constants → 0.5 - 1.2
   - **Start Speed**: Random Between Two Constants → 0.2 - 0.8
   - **Start Size**: Random Between Two Constants → 0.08 - 0.2
   - **Start Color**: Orange/Yellow (brighter than core)
   - **Gravity Modifier**: -0.3 (slight upward drift)
   - **Simulation Space**: **World** ⚠️ CRITICAL - particles stay in air!
   - **Max Particles**: 200

### 3.3 Configure Trail Fire - Emission Module
1. **Emission**:
   - **Rate over Time**: 0 (we use distance instead)
   - **Rate over Distance**: 20 (this will be controlled by script)

### 3.4 Configure Trail Fire - Shape Module
1. **Shape**:
   - **Shape**: Sphere
   - **Radius**: 0.05
   - **Randomize Direction**: ✓ Checked

### 3.5 Configure Trail Fire - Velocity over Lifetime
1. **Velocity over Lifetime**: ✓ Enable
2. **Linear**: X=0, Y=0.5, Z=0 (slight upward drift)
3. **Space**: World

### 3.6 Configure Trail Fire - Color over Lifetime
Same gradient as CoreFire (Yellow → Orange → Red → Transparent)

### 3.7 Configure Trail Fire - Size over Lifetime
1. **Size over Lifetime**: ✓ Enable
2. Create curve:
   - Start: 1.0
   - Middle: 1.2 (particles grow slightly)
   - End: 0.0 (fade out)

### 3.8 Configure Trail Fire - Renderer Module
1. **Render Mode**: **Stretched Billboard** ⚠️ CRITICAL for swipe trail effect
   - **Camera Scale**: 0
   - **Speed Scale**: 0.05 (stretches particles based on velocity)
   - **Length Scale**: 2.0 (makes particles longer)
2. **Material**: Use your fire material from the asset store
3. **Sorting Fudge**: 0

---

## Step 4: Set Up Audio

### 4.1 Create Swing Whoosh Audio Source
1. **Select Katana** in Hierarchy
2. **Add Component > Audio > Audio Source**
3. Rename component to "SwingWhoosh" (use the gear icon)
4. Configure:
   - **AudioClip**: Assign your whoosh/swish sound from assets
   - **Play On Awake**: ✗ Unchecked
   - **Loop**: ✓ Checked
   - **Volume**: 0 (script will control this)
   - **Pitch**: 1.0 (script will control this)
   - **Spatial Blend**: 1.0 (full 3D sound)
   - **Doppler Level**: 0.5
   - **Min Distance**: 0.3
   - **Max Distance**: 10

### 4.2 Create Idle Crackle Audio Source (Optional)
1. **Select Katana** in Hierarchy
2. **Add Component > Audio > Audio Source**
3. Rename component to "IdleCrackle"
4. Configure:
   - **AudioClip**: Assign fire crackling sound loop
   - **Play On Awake**: ✓ Checked
   - **Loop**: ✓ Checked
   - **Volume**: 0.1 (subtle constant background)
   - **Pitch**: 1.0
   - **Spatial Blend**: 1.0 (full 3D sound)
   - **Min Distance**: 0.3
   - **Max Distance**: 5

---

## Step 5: Attach and Configure FireKatanaController Script

### 5.1 Add the Script
1. **Select Katana** in Hierarchy
2. **Add Component > Scripts > Fire Katana Controller**

### 5.2 Configure Inspector Fields

#### Configuration Section:
- **Max Velocity**: 5.0 (m/s - Quest controller can reach ~4-6 m/s)
- **Smooth Time**: 0.1 (lower = more responsive, higher = smoother)

#### Visuals - Particle Systems Section:
1. **Core Fire Particles**: Drag `CoreFire` GameObject here
2. **Trail Fire Particles**: Drag `TrailFire` GameObject here
3. **Min Emission**: 10 (particles/sec at rest)
4. **Max Emission**: 100 (particles/sec at max velocity)

#### Audio Section:
1. **Swing Audio Source**: Drag the "SwingWhoosh" Audio Source component here
2. **Min Pitch**: 0.8 (low pitch at rest)
3. **Max Pitch**: 1.3 (high pitch at max velocity)
4. **Idle Crackle Source**: Drag the "IdleCrackle" Audio Source component here (optional)

---

## Step 6: Test in Unity Editor

### 6.1 Play Mode Test
1. **Press Play** in Unity
2. **Select Katana** in Hierarchy
3. **Use the Transform tools** to move it around in the Scene view
4. **Watch the Inspector** - FireKatanaController shows current velocity

### 6.2 What to Look For:
- ✓ CoreFire particles should move with the blade
- ✓ TrailFire particles should stay in the air
- ✓ Moving faster = more particles
- ✓ Console warnings if something is misconfigured

---

## Step 7: Optimize for Quest 3

### 7.1 Performance Checklist
- ✓ **Total Max Particles**: 50 (core) + 200 (trail) = 250 total
- ✓ **Simulation Space**: Local for core, World for trail
- ✓ **Texture Sheet Animation**: If using sprite sheets, enable this
- ✓ **Disable Soft Particles**: Better performance on Quest
- ✓ **Use GPU Instancing**: Check material settings

### 7.2 If You Experience Lag:
1. **Reduce Max Particles**:
   - CoreFire: Max Particles → 30
   - TrailFire: Max Particles → 100

2. **Lower Emission Rates**:
   - Min Emission: 10 → 5
   - Max Emission: 100 → 50

3. **Simplify Fire Material**:
   - Use simpler shaders (unlit instead of lit)
   - Reduce texture resolution to 256x256 or 512x512

---

## Step 8: Build and Test on Quest 3

### 8.1 Save Everything
1. **File > Save** (Ctrl+S)
2. **File > Save Project**

### 8.2 Build for Quest 3
1. **File > Build Profiles**
2. **Select Android** platform
3. **Click "Build and Run"**
4. **Wait for build** (5-10 minutes)
5. **APK will auto-install** to your Quest 3

### 8.3 In VR Testing Checklist:
- ✓ Can you see fire on the katana?
- ✓ Does fire intensity increase when you swing fast?
- ✓ Does fire trail stay in the air behind your swing?
- ✓ Can you hear the whoosh sound getting louder/higher pitched?
- ✓ Is the frame rate smooth (90 FPS)?

---

## Troubleshooting

### Fire Not Appearing:
- ✗ Check if particle systems are enabled (checkbox in Inspector)
- ✗ Verify CoreFire uses **Local** simulation space
- ✗ Verify TrailFire uses **World** simulation space
- ✗ Check Console for FireKatanaController warnings

### Fire Not Responding to Movement:
- ✗ Ensure FireKatanaController script is attached to moving GameObject
- ✗ Check that particle system references are assigned in Inspector
- ✗ Verify Max Velocity is reasonable (try 5.0)

### Audio Not Working:
- ✗ Check that AudioClips are assigned
- ✗ Verify Spatial Blend is set to 1.0 (3D sound)
- ✗ Make sure audio isn't muted in Unity/Quest

### Trail Fire Moves with Blade (Not Staying in Air):
- ✗ **CRITICAL**: TrailFire must use **World** simulation space, not Local!

### Performance Issues (Frame Drops):
- ✗ Reduce Max Particles (see Step 7.2)
- ✗ Lower emission rates
- ✗ Use simpler fire material
- ✗ Check Quest 3 performance overlay (Meta Quest Developer Hub)

---

## Advanced: Customization Tips

### Make Fire More Aggressive:
- Increase Start Speed → 2.0 - 3.0
- Increase emission rates
- Use brighter colors (more white in the gradient)

### Make Fire Subtle:
- Decrease Start Size → 0.03 - 0.08
- Lower emission rates
- Use darker colors (more red/orange, less yellow)

### Add Spark Particles:
1. Create third particle system
2. Use small, fast, bright particles
3. Emit from blade tip only
4. World space, high velocity

### Synchronize with Music/Beats:
- Expose `currentVelocity` from FireKatanaController
- Use it to drive other effects (screen shake, haptics, etc.)

---

## Quick Reference: Critical Settings

| Component | Property | Value |
|-----------|----------|-------|
| CoreFire Main | Simulation Space | **Local** |
| TrailFire Main | Simulation Space | **World** |
| TrailFire Emission | Rate over Distance | 20 |
| TrailFire Renderer | Render Mode | **Stretched Billboard** |
| SwingAudio | Spatial Blend | 1.0 (3D) |
| SwingAudio | Play On Awake | Unchecked |

---

## Next Steps

Once you have the fire working:
1. **Integrate with DecartXR API** - Add score multipliers for fast swings
2. **Add Haptic Feedback** - Vibrate controller on impact
3. **Create Multiple Fire Colors** - Different elements (blue ice, purple lightning)
4. **Sync with Music** - Make fire pulse to the beat

Good luck, samurai! 🔥⚔️
