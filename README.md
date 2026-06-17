# Tiny RPG

A lightweight, 3rd-person Action RPG framework built in Unity, designed as a personal sandbox to master **software architecture, clean coding practices, and scalable game development patterns**. 

The gameplay mechanics are heavily inspired by *World of Warcraft*.

---

## 🎯 Project Purpose & Architectural Goals

The primary objective of this repository is not just to build a functional game, but to solve complex architectural challenges using solid software engineering practices. 

### Key Engineering Focus Areas:
* **Decoupling Systems:** Utilizing event-driven architecture to ensure features like combat, questing, and UI remain completely isolated from one another.
* **State Management:** Leveraging Finite State Machines (FSM) to drive character locomotion, combat postures, and AI behaviors cleanly.
* **Data-Driven Design:** Extensively using Unity's `ScriptableObjects` to separate raw gameplay data (item configurations, stats, ability variables) from executive execution logic.
* **UI Architecture:** Designing decoupled presentation layers using the Model-View-Controller (MVC) approach to manage inventory systems, loot grids, and actor nameplates.

---

## 🛠️ Feature Roadmap

This project is divided into several core gameplay subsystems, simulating a miniature vertical slice of a classic RPG layout:

### 🎮 Player Locomotion & Exploration
* [x] **3rd-Person Movement:** Kinematic player movement with fluid camera tracking.
* [ ] **Mount System:** Speed-modifying state shift allowing players to summon, mount, and dismount seamlessly.

### ⚔️ Combat & Gameplay Loop
* [x] **Tab-Targeting Engine:** Smart raycasting/distance utility system to select, cycle, and track hostile targets.
* [ ] **Abilities (Melee & Ranged):** Highly modular ability framework supporting cast times, instant strikes, and conditional requirements.
* [ ] **Aura & Status Effect System:** Flexible handler managing core debuff categories like Damage over Time (DoT), Slows, Roots, and crowd control.
* [ ] **Ability Resource Engine:** Managing secondary execution resources (e.g., Mana, Energy, or Rage) to govern mechanical costs.

### 📊 Character Progression & RPG Math
* [ ] **Character Stats Engine:** Live calculator tracking base attributes (Strength, Agility, Armor, Crit) dynamically scaled via equipment or active buff modifiers.
* [ ] **Leveling Pipeline:** Core experience curves governing level scaling and character attribute increases.

### 🎒 Inventories, Items & Interactions
* [x] **Dynamic Inventory Layout:** Grid-based inventory array utilizing event-driven drag-and-drop operations.
* [ ] **Equipment System:** Distinct equipment slots altering structural player math, stats, and visible geometry properties.
* [ ] **Contextual World Interactions:** Decoupled interactive environment structures handling distance-validated objects (e.g., world loot sources).

### 🤖 Non-Player Characters (NPCs) & World AI
* [ ] **Functional Vendors & Trainers:** Contextual shop-keepers and ability unlock interaction nodes.
* [ ] **Hostile NPCs & Pathfinding:** Modular AI threat engines equipped with state-driven chasing behaviors and return-to-spawn (leashing) mechanics.
* [ ] **Quest System:** Decoupled event listener listening for target kills and collection metrics to drive storyline progression.

---

## ⚙️ Tech Stack & Patterns Behind the Scenes

* **Engine:** Unity 6
* **Language:** C#
* **Input Infrastructure:** New Unity Input System Package
* **Patterns Used:**
  * Singleton Pattern (Central Managers)
  * Observer Pattern (Events & Actions)
  * State Pattern (Locomotion & Enemy AI States)
  * Strategy Pattern (Ability effect evaluation)

---
