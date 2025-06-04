# Project-EtherialHaven
# Scalable MMORPG Architecture

This document outlines a scalable, modular architecture for building a modern MMORPG using **uMMORPG**, **Mirror Networking**, and a variety of microservices to handle key gameplay and backend systems. The architecture is designed for extensibility, performance, and high player concurrency.

---

## Overview Diagram

![Architecture Diagram](path/to/your/diagram.png)  
*Replace the image path with your diagram if hosted.*

---

## Components

### 1. **Client (Unity + uMMORPG)**
- Front-end game client built with Unity.
- Networking via Mirror.
- Connects to:
  - Load Balancer (for login & world routing)
  - Microservices (via HTTP/WebSocket)

---

### 2. **Load Balancer**
- Examples: **HAProxy**, **NGINX**, **AWS ALB**
- Routes traffic to:
  - Auth Server (for login)
  - World Servers (based on realm/zone)
- Handles:
  - Session stickiness
  - Failover and redundancy
  - TLS termination

---

### 3. **Authentication Server**
- Manages account login, token generation, and session tracking.
- Connects to:
  - Database (accounts, credentials)
  - Redis or SpacetimeDB (session cache)
- Returns:
  - JWT or custom session token
  - Available realms/worlds to client

---

### 4. **Database Layer**
- Options: **MySQL**, **PostgreSQL**, or **SpacetimeDB**
- Stores:
  - Account data
  - Character stats, gear, inventories
  - Guilds, mail, progression
- Accessed by:
  - Auth Server
  - World Servers
  - Microservices

---

### 5. **World Server(s)**
- One instance per zone, realm, or dungeon.
- Built on uMMORPG + Mirror.
- Responsible for:
  - Combat, movement, AI
  - Real-time sync with client
- Connects to:
  - Database (for persistent character data)
  - Microservices (chat, events, mail)
  - Redis or SpacetimeDB (shared state)

---

## Microservices

All services are **stateless**, containerized, and horizontally scalable.

### a. **Chat Service**
- Handles global/zone/private chats.
- Real-time via WebSocket (e.g., Socket.IO).
- Optional moderation/filters.

### b. **Matchmaking Service**
- Queues players for PvP, dungeons, etc.
- Assigns groups to instance servers.

### c. **Guild Service**
- Handles guild creation, rosters, banks.
- Reads/writes to the database.

### d. **Friends & Presence Service**
- Manages online/offline status, friend lists.
- Can broadcast status changes.

### e. **Leaderboard Service**
- Tracks and displays top players.
- Uses Redis for fast leaderboard lookup.

### f. **Mail Service**
- In-game mail handling (with attachments).
- Supports expiration and offline delivery.

### g. **Notification/Event Service**
- Sends alerts, event timers, announcements.
- Pushes to client UI.

### h. **Instance Manager**
- Spins up or shuts down World Server instances on demand.
- Can be based on Docker or Kubernetes.

---

## Bundling vs Splitting Microservices

You **do not need** to deploy each microservice independently. In fact, it’s often best to bundle related microservices when starting out. Later, they can be split for performance, fault isolation, or scaling needs.

### When to Bundle:
- Low traffic or early development
- Shared domain logic or database
- Fewer deployment pipelines needed

**Examples:**
- `player-services`: Chat, Mail, Friends, Guild
- `combat-services`: Matchmaking, Instance Manager, Leaderboards
- `utility-services`: Notifications, Logging

### When to Split:
- Services have different scaling needs
- Different storage/tech requirements
- You need fault isolation or separate deployments

**Recommendation:**  
Structure code into clear, isolated modules so you can bundle now and split later with minimal refactoring.

---

## Shared Layers

### **Redis / Memcached**
- In-memory cache used for:
  - Session tokens
  - Online player tracking
  - Chat or friend presence
- Improves response time and reduces DB load.

### **API Gateway (Optional)**
- Centralizes access to all REST microservices.
- Provides rate limiting, routing, and metrics.

---

## Monitoring / Logging

Recommended stack:
- **Prometheus + Grafana** for metrics
- **ELK Stack (Elasticsearch, Logstash, Kibana)** for logs
- **AlertManager** for real-time failure alerts

---

## Example Workflow: Login & Game Entry

1. Client connects to **Load Balancer**
2. Load Balancer forwards to **Auth Server**
3. Auth Server verifies credentials → returns session token & realm list
4. Client selects realm → Load Balancer routes to appropriate **World Server**
5. World Server loads player data from DB
6. Microservices handle live chat, mail, etc.

---

## Future Enhancements

- Distributed CDN for patcher/assets
- Cloud autoscaling of microservices
- Real-time analytics with Kafka

---

## Tech Stack (Suggested)

| Component             | Tech Options                             |
|----------------------|-------------------------------------------|
| Client               | Unity + Mirror                            |
| Auth Server          | FastAPI / Node.js                         |
| DB                   | MySQL / PostgreSQL / SpacetimeDB          |
| Cache                | Redis                                     |
| WebSockets           | Socket.IO / uWebSockets.js                |
| Load Balancer        | NGINX / HAProxy / AWS ALB                 |
| Microservices        | Node.js / Golang / Python (FastAPI)       |
| Containerization     | Docker + Kubernetes                       |
| Monitoring           | Prometheus, Grafana, ELK Stack            |

---

## License
MIT (or your custom license here)

---

## Contact
For questions or contributions, open an issue or contact the project maintainer.
