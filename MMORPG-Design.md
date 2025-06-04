# Etherial Haven — Official MMORPG Project Guide

## 🎯 Project Overview

**Project Name:** Etherial Haven (codename)  
**Genre:** Fantasy MMORPG with open-world sandbox and instanced content  
**Style:** Mix of classic WoW + sandbox features from ArcheAge  
**Engine:** Unity 6 with uMMORPG (Mirror networking), enhanced via GPU Instancer, World Streamer, Odin Inspector, and DLSS

---

## 🧠 Design Philosophy

| Principle               | Description                                                                |
|------------------------|----------------------------------------------------------------------------|
| Player Agency          | Empower players through land ownership, economy, class builds              |
| Seamless Exploration   | Encourage exploration via an open world, naval systems, and flying/gliding |
| Community Driven       | Emphasize player interaction: guilds, trade, PvP, alliances                |
| Respect Time Investment| Progression matters: dungeons, crafting, mounts, housing                   |
| Scalable & Sustainable | Tech must scale and allow modular deployment                               |

---

## ⚒️ Core Feature Breakdown

### 🔹 Open World & Land System
- Seamless continent with zones
- Player housing, farms, and land claims
- Territory tax, building upgrades, world events

### 🔹 PvE System
- WoW-style quest chains, dungeons, and elite areas
- Instanced content: solo, group, and raid

### 🔹 PvP System
- Tab-target skill combat
- Faction-based or lawless PvP zones
- Instanced battlegrounds & arenas
- Naval PvP

### 🔹 Economy
- Player-driven market
- Raw resources → crafting → gear/packs
- Marketplace, auctions, caravans

### 🔹 Vehicles & Mounts
- Land vehicles (carts)
- Naval ships
- Gliders and flying mounts

### 🔹 Character Progression
- Leveling, stats, talents
- Class paths
- Gear enhancement

---

## 🏗️ Architecture Overview

```
[Unity Clients (uMMORPG, Mirror, GPU Instancer, World Streamer, DLSS)]
        |
  [Global Load Balancer]
        |
 ┌────────────┬────────────┐
 |            |            |
[NA Auth]   [EU Auth]     ← Region-based Auth Clusters
        |
 ┌────┬────┐
 |    |    |
[World 1][World 2]     ← Each WorldServer handles an entire open world
  |         |
[InstanceMgr][InstanceMgr]   ← Dynamic dungeons/BGs
  |         |
[Microservices] ← Chat, Mail, Guild, PvP, Market
        |
┌───────┬────────┬──────────┐
| MySQL | Redis  | FastAPI  | ← Persistence, Sessions, Live APIs
```

---

## 📁 Folder/Repo Structure

```
/mmorpg-project
├── /client (Unity)
│   ├── uMMORPG
│   ├── GPUInstancer
│   ├── WorldStreamer
│   ├── DLSSIntegration
│   ├── Scripts
│   ├── UI
│   └── Scenes
├── /server
│   ├── /auth
│   ├── /world
│   ├── /instance-manager
│   ├── /microservices
│   │   ├── chat
│   │   ├── guild
│   │   ├── mail
│   │   ├── pvp
│   │   └── marketplace
├── /database
│   ├── mysql-schema.sql
│   ├── redis-config
│   └── migrate_sqlite_to_mysql.py
├── /infra
│   ├── docker-compose.yml
│   ├── k8s/
│   ├── nginx.conf
│   └── prometheus/grafana
└── README.md
```

---

## 📅 Development Roadmap

### 🧩 Phase 1: Foundation
- Setup client + backend scaffolding
- Implement region-aware auth routing
- Replace SQLite with MySQL
- Create base WorldServer instance (combat + quests)

### 🧩 Phase 2: Instancing & Partying
- Build Instance Manager
- Add dungeon prefab spawning via Z-stacking
- Add matchmaking logic

### 🧩 Phase 3: Economy + Market
- Create craft → sell → profit system
- Integrate auction house, caravans

### 🧩 Phase 4: PvP
- Faction, arenas, battlegrounds
- Leaderboards (Redis-backed)

### 🧩 Phase 5: Housing & Naval
- Land ownership, taxation
- Boats, gliders, housing system

### 🧩 Phase 6: Optimization + Launch Prep
- DLSS, GPU LOD tuning
- Load testing with Mirror stress tools
- Add dashboards and admin tools

---

## 🧠 Tech Stack

| Layer         | Stack                                             |
|---------------|--------------------------------------------------|
| Engine        | Unity 6 + uMMORPG                                |
| Netcode       | Mirror Networking + KCP Transport                |
| Backend       | FastAPI, Docker, NGINX                           |
| Persistence   | MySQL                                            |
| Cache         | Redis                                            |
| Monitoring    | Prometheus + Grafana                             |
| Tooling       | Odin Inspector, World Streamer, GPU Instancer    |

---

## 🛠️ Tooling & Workflow

- CI/CD: GitHub Actions or GitLab CI
- DevOps: Docker for local, K8s for production
- Versioning: Git Flow or trunk-based workflow
- Issue Tracking: Jira, Trello, or GitHub Projects

---

## ✅ Summary

You’re building a **modern, scalable MMORPG** that fuses:
- WoW’s proven instanced content model
- ArcheAge’s land, vehicle, and economy sandbox
- A full-stack Unity ecosystem ready to deploy regionally

This is a practical, production-level plan with modular growth and a clean development path.

Let me know when you’re ready to:
- Generate Unity/Mirror scripts
- Build CI pipelines
- Scaffold the next microservice
