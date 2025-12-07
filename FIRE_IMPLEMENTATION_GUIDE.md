# Fire Katana Implementation Guide
## Precise Setup for CuttingSceneWithController Scene

---

## Your Current Setup

**Controller Object:**
- Position: X: 0.03, Y: -0.2, Z: -0.2
- Rotation: X: 180, Y: 83.55, Z: 48.7
- Scale: X: 0.12, Y: 0.12, Z: 0.12

**Katana Object:**
- Position: X: 0.362, Y: 0.037, Z: 0.06
- Rotation: X: 40.181, Y: -7184, Z: 6.967
- Scale: X: 0.12, Y: 0.12, Z: 0.12

---

## Goal

Add fire particle systems **only to the blade portion** of the katana, ensuring:
- Fire does NOT appear on the handle (where controller overlaps)
- Fire reacts to controller movement velocity
- Fire trail stays in the air as you swing
- Maintains 90 FPS on Quest 3

---

## Step 1: Open Your Scene and Locate the Katana

1. **Open Unity Editor**
2. **Open Scene**: `Assets/Edgelord Samurai/Scenes/CuttingSceneWithController.unity`
3. **In Hierarchy panel**, find your katana GameObject (likely named "KatanaSamuraiGameplay" or similar)
4. **Click on it** to select it
5. **In Inspector**, verify the Position/Rotation/Scale match what you provided

---

## Step 2: Understand Your Katana Structure

Before we add fire, we need to understand the katana's local coordinate system:

1. **With the katana selected**, look at the **Scene view**
2. **Press F** to frame/focus on the katana
3. **In the Scene view toolbar**, click the **"Local/Global" toggle** and set it to **Local**
4. **Observe the colored axes** (Transform Gizmo):
   - **Red arrow (X-axis)**: Side to side
   - **Green arrow (Y-axis)**: Up/down
   - **Blue arrow (Z-axis)**: Along the blade length (THIS IS KEY!)

**Question to verify:**
- Which direction does the **blue Z-axis** point?
  - If it points from handle → blade tip, the blade extends in **+Z direction**
  - If it points from blade tip → handle, the blade extends in **-Z direction**

**For this guide, I'm assuming the blade extends in +Z direction (standard Unity convention).**

---

## Step 3: Determine Fire Start Position (Blade Only, Not Handle)

We need to position fire particles to start **after the handle ends**.

### Method A: Visual Estimation (Quick)

1. **Select the katana** in Hierarchy
2. **In Scene view**, visually estimate where the handle ends and blade begins
3. **Measure the distance** from the katana's pivot point to that location
   - If the pivot is at the handle base, this might be ~0.3 to 0.4 units
   - If the pivot is at the center, it might be 0.0 to 0.2 units

### Method B: Precise Measurement (Recommended)

1. **Right-click on the katana** in Hierarchy
2. **Create Empty Child**: 3D Object > Create Empty
3. **Name it**: "BladeStartMarker"
4. **In Scene view**, use the **Move tool (W key)** to position this empty GameObject at the exact point where the handle ends and blade begins
5. **Note the Z position** in the Transform component (this is your `BLADE_START_Z` value)
6. **Delete the marker** after noting the value

**Typical values:**
- If katana pivot is at handle base: `BLADE_START_Z = 0.3` to `0.4`
- If katana pivot is at center: `BLADE_START_Z = -0.2` to `0.0`
- If katana pivot is at blade tip: `BLADE_START_Z = -0.8` to `-0.5`

**For this guide, let's use `BLADE_START_Z = 0.35`** (you'll adjust this based on your model)

---

## Step 4: Create Katana Hierarchy with Fire Children

Now we'll organize the katana to have fire as child objects.

### 4.1 Current Structure
Your hierarchy likely looks like:
```
KatanaSamuraiGameplay (or your katana name)
├── [Mesh/Model of the katana]
└── [Possibly other children]
```

### 4.2 Create Fire Parent Container

We'll create an empty GameObject to hold both fire particle systems:

1. **Right-click on your katana** in Hierarchy
2. **Create Empty Child**: Right-click > Create Empty
3. **Name it**: `FireEffects`
4. **Set its Transform**:
   - **Position**: X: 0, Y: 0, Z: 0.35 ← Use your `BLADE_START_Z` value here
   - **Rotation**: X: 0, Y: 0, Z: 0
   - **Scale**: X: 1, Y: 1, Z: 1

This positions the fire container at the start of the blade.

---

## Step 5: Create Core Fire Particle System

### 5.1 Create the GameObject

1. **Right-click on `FireEffects`** in Hierarchy
2. **Effects > Particle System**
3. **Name it**: `CoreFire`
4. **Set Transform**:
   - **Position**: X: 0, Y: 0, Z: 0.3 ← This is relative to FireEffects, positions fire mid-blade
   - **Rotation**: X: 0, Y: 0, Z: 0
   - **Scale**: X: 1, Y: 1, Z: 1

### 5.2 Configure Core Fire - Main Module

**Select `CoreFire`**, then in Inspector configure:

**Main Module:**
- ☑ **Looping**: Checked
- **Duration**: 5.00
- **Start Lifetime**: Random Between Two Constants
  - Min: 0.3
  - Max: 0.8
- **Start Speed**: Random Between Two Constants
  - Min: 0.5
  - Max: 1.5
- **Start Size**: Random Between Two Constants
  - Min: 0.05
  - Max: 0.15
- **Start Rotation**: Random Between Two Constants
  - Min: 0
  - Max: 360
- **Start Color**: Gradient or solid orange/yellow (use your fire asset colors)
- **Gravity Modifier**: 0
- **Simulation Space**: ⚠️ **Local** (CRITICAL - particles move with blade)
- **Max Particles**: 50

### 5.3 Configure Core Fire - Emission Module

Click **Emission** to expand:
- ☑ **Enabled**: Checked
- **Rate over Time**: 10 (this will be controlled dynamically by script)
- **Rate over Distance**: 0

### 5.4 Configure Core Fire - Shape Module

Click **Shape** to expand:
- ☑ **Enabled**: Checked
- **Shape**: Box
- **Position**: X: 0, Y: 0, Z: 0
- **Rotation**: X: 0, Y: 0, Z: 0
- **Scale**: X: 0.02, Y: 0.02, Z: 0.6 ← This creates a thin box along the blade
- ☑ **Randomize Direction**: Checked

### 5.5 Configure Core Fire - Color over Lifetime

Click **Color over Lifetime** to expand:
- ☑ **Enabled**: Checked
- Click the **Color** box to open Gradient Editor
- Create gradient: **Yellow → Orange → Red → Transparent**
  - **Left key (time 0%)**: Bright Yellow/White (RGB: 255, 255, 200), Alpha: 255
  - **Middle-left key (time 30%)**: Orange (RGB: 255, 128, 0), Alpha: 255
  - **Middle-right key (time 70%)**: Dark Red (RGB: 128, 0, 0), Alpha: 128
  - **Right key (time 100%)**: Any color, Alpha: 0

### 5.6 Configure Core Fire - Size over Lifetime

Click **Size over Lifetime** to expand:
- ☑ **Enabled**: Checked
- Click the **Size** curve
- Create curve that shrinks over time:
  - **Left point (time 0)**: Value 1.0
  - **Right point (time 1)**: Value 0.2

### 5.7 Configure Core Fire - Renderer

Scroll to **Renderer** module:
- **Render Mode**: Billboard
- **Material**: Assign your fire material from the asset store
  - Click the circle icon → search for your fire material
- **Sorting Fudge**: 0

---

## Step 6: Create Trail Fire Particle System

### 6.1 Create the GameObject

1. **Right-click on `FireEffects`** in Hierarchy
2. **Effects > Particle System**
3. **Name it**: `TrailFire`
4. **Set Transform**:
   - **Position**: X: 0, Y: 0, Z: 0.3 ← Same as CoreFire
   - **Rotation**: X: 0, Y: 0, Z: 0
   - **Scale**: X: 1, Y: 1, Z: 1

### 6.2 Configure Trail Fire - Main Module

**Select `TrailFire`**, then in Inspector configure:

**Main Module:**
- ☑ **Looping**: Checked
- **Duration**: 5.00
- **Start Lifetime**: Random Between Two Constants
  - Min: 0.5
  - Max: 1.2
- **Start Speed**: Random Between Two Constants
  - Min: 0.2
  - Max: 0.8
- **Start Size**: Random Between Two Constants
  - Min: 0.08
  - Max: 0.2
- **Start Rotation**: Random Between Two Constants
  - Min: 0
  - Max: 360
- **Start Color**: Orange/Yellow (slightly brighter than CoreFire)
- **Gravity Modifier**: -0.3 ← Slight upward drift like real fire
- **Simulation Space**: ⚠️ **World** (CRITICAL - particles stay in air!)
- **Max Particles**: 200

### 6.3 Configure Trail Fire - Emission Module

Click **Emission** to expand:
- ☑ **Enabled**: Checked
- **Rate over Time**: 0 ← We use distance instead
- **Rate over Distance**: 20 ← Particles spawn based on movement

### 6.4 Configure Trail Fire - Shape Module

Click **Shape** to expand:
- ☑ **Enabled**: Checked
- **Shape**: Sphere
- **Radius**: 0.05
- **Randomize Direction**: ☑ Checked

### 6.5 Configure Trail Fire - Velocity over Lifetime

Click **Velocity over Lifetime** to expand:
- ☑ **Enabled**: Checked
- **Linear**:
  - X: 0
  - Y: 0.5 ← Slight upward drift
  - Z: 0
- **Space**: World

### 6.6 Configure Trail Fire - Color over Lifetime

Same gradient as CoreFire (Yellow → Orange → Red → Transparent)

### 6.7 Configure Trail Fire - Size over Lifetime

Click **Size over Lifetime** to expand:
- ☑ **Enabled**: Checked
- Click the **Size** curve
- Create curve that grows then shrinks:
  - **Left point (time 0)**: Value 1.0
  - **Middle point (time 0.3)**: Value 1.2 ← Particles grow slightly
  - **Right point (time 1)**: Value 0.0 ← Fade out

### 6.8 Configure Trail Fire - Renderer

Scroll to **Renderer** module:
- **Render Mode**: ⚠️ **Stretched Billboard** (CRITICAL for swipe trail effect)
  - **Camera Scale**: 0
  - **Speed Scale**: 0.05 ← Stretches particles based on velocity
  - **Length Scale**: 2.0 ← Makes particles longer
- **Material**: Same fire material as CoreFire
- **Sorting Fudge**: 0

---

## Step 7: Add FireKatanaController Script

### 7.1 Attach Script to Katana

1. **Select the katana GameObject** (the parent, NOT the FireEffects child)
2. **In Inspector**, click **Add Component**
3. **Search for**: `Fire Katana Controller`
4. **Click** to add it

### 7.2 Configure Script Parameters

**With the katana still selected**, configure the FireKatanaController component:

#### Configuration Section:
- **Max Velocity**: 5.0
- **Smooth Time**: 0.1

#### Visuals - Particle Systems Section:
1. **Core Fire Particles**:
   - Click the circle icon → Find `CoreFire` → Select it
   - OR drag `CoreFire` from Hierarchy to this field
2. **Trail Fire Particles**:
   - Click the circle icon → Find `TrailFire` → Select it
   - OR drag `TrailFire` from Hierarchy to this field
3. **Min Emission**: 10
4. **Max Emission**: 100

#### Audio Section:
We'll add audio sources next, leave these empty for now.

---

## Step 8: Add Audio Sources

### 8.1 Create Swing Whoosh Audio

1. **Select the katana GameObject** in Hierarchy
2. **Add Component > Audio > Audio Source**
3. **In the Audio Source component**:
   - Click the **gear icon (⚙)** in the top-right → **Rename** → Type: `SwingWhoosh`
   - **AudioClip**: Assign a whoosh/swish sound from your assets
   - **Play On Awake**: ☐ Unchecked
   - **Loop**: ☑ Checked
   - **Volume**: 0 (script controls this)
   - **Pitch**: 1.0 (script controls this)
   - **Spatial Blend**: 1.0 (full 3D sound)
   - **Doppler Level**: 0.5
   - **Min Distance**: 0.3
   - **Max Distance**: 10

### 8.2 Create Idle Crackle Audio (Optional)

1. **Select the katana GameObject** in Hierarchy
2. **Add Component > Audio > Audio Source** (yes, add a second one)
3. **In the second Audio Source component**:
   - Click the **gear icon (⚙)** in the top-right → **Rename** → Type: `IdleCrackle`
   - **AudioClip**: Assign a fire crackling loop from your assets
   - **Play On Awake**: ☑ Checked
   - **Loop**: ☑ Checked
   - **Volume**: 0.1 (subtle constant background)
   - **Pitch**: 1.0
   - **Spatial Blend**: 1.0 (full 3D sound)
   - **Min Distance**: 0.3
   - **Max Distance**: 5

### 8.3 Link Audio to FireKatanaController

1. **Select the katana GameObject**
2. **Find the FireKatanaController component** in Inspector
3. **In the Audio Section**:
   - **Swing Audio Source**: Drag the `SwingWhoosh` Audio Source component here
     - (Click and drag the component header, not the katana GameObject)
   - **Min Pitch**: 0.8
   - **Max Pitch**: 1.3
   - **Idle Crackle Source**: Drag the `IdleCrackle` Audio Source component here (optional)

---

## Step 9: Verify Hierarchy

Your final hierarchy should look like this:

```
KatanaSamuraiGameplay (or your katana name)
├── [Your katana mesh/model]
├── FireEffects (Transform: Position Z = 0.35)
│   ├── CoreFire (Particle System, Local space)
│   └── TrailFire (Particle System, World space)
├── [Other existing children if any]
└── Components:
    ├── FireKatanaController (Script)
    ├── SwingWhoosh (Audio Source)
    └── IdleCrackle (Audio Source)
```

---

## Step 10: Adjust Fire Position (Fine Tuning)

Now we need to ensure fire appears ONLY on the blade, not the handle:

### 10.1 Test Fire Position Visually

1. **Press Play** in Unity Editor
2. **In Hierarchy**, select `CoreFire` and `TrailFire`
3. **Check if particles are visible** in Scene view
4. **Look at where particles are spawning**:
   - ✅ **Correct**: Particles appear along the blade, starting after the handle
   - ❌ **Wrong**: Particles appear on the handle or too far from the blade

### 10.2 Adjust if Needed

If fire appears in the wrong location:

**If fire is on the handle (too close to pivot):**
1. **Select `FireEffects`** in Hierarchy
2. **Increase Z position**: Try 0.4, 0.45, 0.5, etc.
3. **Test again** until fire starts where blade begins

**If fire is too far down the blade (missing the base):**
1. **Select `FireEffects`** in Hierarchy
2. **Decrease Z position**: Try 0.3, 0.25, 0.2, etc.
3. **Test again** until fire covers the whole blade

**If fire extends beyond blade tip:**
1. **Select `CoreFire`**
2. **In Shape module**, decrease **Scale Z** from 0.6 to 0.4 or 0.5
3. **Select `TrailFire`**
4. **Same adjustment**

### 10.3 Test with Controller Movement

1. **With Play mode still running**
2. **In Scene view**, click on the **katana GameObject**
3. **Use the Move tool (W)** to manually move it around
4. **Watch the fire particles**:
   - ✅ CoreFire should move with the blade (orange glow)
   - ✅ TrailFire should stay in the air (orange trail behind)
   - ✅ Moving faster should create more particles

---

## Step 11: Save and Test in VR

### 11.1 Save Everything

1. **Stop Play mode** (click Play button again)
2. **File > Save** (Ctrl+S) to save the scene
3. **File > Save Project** to save all changes

### 11.2 Build for Quest 3

1. **File > Build Profiles**
2. **Select Android** platform
3. **Ensure scene is added**: Check that `CuttingSceneWithController` is in the build list
4. **Click "Build and Run"**
5. **Choose a location** to save the APK (or use previous location)
6. **Wait for build** (5-10 minutes)
7. **APK auto-installs** to Quest 3

### 11.3 VR Testing Checklist

Put on your Quest 3 and check:

- ☐ Can you see the katana in your hand?
- ☐ Does the katana have fire on the blade?
- ☐ Is the handle (where you grip) fire-free?
- ☐ When you swing slow, is the fire dim?
- ☐ When you swing fast, does the fire intensify?
- ☐ Does the fire trail stay in the air behind your swing?
- ☐ Can you hear the whoosh sound getting louder/higher pitched?
- ☐ Is the frame rate smooth (90 FPS)?

---

## Troubleshooting

### Fire appears on the handle

**Solution**: Increase `FireEffects` Z position
1. Select `FireEffects` in Hierarchy
2. Increase Position Z by 0.05 increments until fire starts after handle

### Fire doesn't respond to movement

**Solution**: Verify script is on the correct object
1. FireKatanaController must be on the katana GameObject (the one that moves)
2. NOT on FireEffects or the particle systems themselves

### Trail fire moves with blade (doesn't stay in air)

**Solution**: Check simulation space
1. Select `TrailFire`
2. In Particle System > Main module
3. **Simulation Space MUST be World**, not Local

### No fire visible at all

**Solution**: Check particle systems are playing
1. Select `CoreFire` and `TrailFire`
2. In Inspector, ensure they're enabled (checkbox at top)
3. Check that firewall material is assigned in Renderer module

### Fire is too intense/too weak

**Solution**: Adjust emission rates
1. Select katana GameObject
2. In FireKatanaController component:
   - Decrease Max Emission to 50 (if too intense)
   - Increase Max Emission to 150 (if too weak)

---

## Quick Reference: Key Values

| Object | Property | Value | Note |
|--------|----------|-------|------|
| FireEffects | Position Z | 0.35 | **Adjust to match your blade start point** |
| CoreFire | Simulation Space | Local | Moves with blade |
| CoreFire | Shape Scale Z | 0.6 | Length of fire along blade |
| TrailFire | Simulation Space | World | Stays in air |
| TrailFire | Render Mode | Stretched Billboard | Creates swipe effect |
| SwingWhoosh | Spatial Blend | 1.0 | Full 3D audio |

---

## Next Steps

Once fire is working:
1. **Integrate with slicing mechanics** - trigger fire burst on successful cut
2. **Add haptic feedback** - vibrate controller on impact
3. **Create combo effects** - fire changes color on combo streak
4. **Optimize further** - reduce particles if frame rate drops

Enjoy your flaming katana! 🔥⚔️
