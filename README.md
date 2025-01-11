# Tron Binary Drift

By **Arnau González**, **Biel Rubio**, and **Pau Argiz**.

Step into a futuristic arena where two robotic contenders face off in a fast-paced battle for victory! In this 2-player competitive game, each player controls a sleek, trail-leaving robot in a glowing, Tron-inspired environment. The objective is simple: grab the ball and score in your opponent's goal while outmaneuvering their defenses. Strategic movement, quick reflexes, and precise timing are key to dominating the arena. With stunning neon visuals and intense gameplay, this game delivers an electrifying experience for players looking to test their skills and claim the title of ultimate robo-champion!

**GitHub Link:** [https://github.com/arinWald/TBD-Xarxes-Games](https://github.com/arinWald/TBD-Xarxes-Games)
**Github Release:** [https://github.com/arinWald/TBD-Xarxes-Games/releases/tag/1.0_FinalRelease](https://github.com/arinWald/TBD-Xarxes-Games/releases/tag/1.0_FinalRelease)

---

## Controls
- **WASD** → Movement  
- **SPACE** → Charge ball throw (up to 1.5 seconds)  
- **SPACE RELEASE** → Throw ball  
- **Automatic Pickup** → Ball is picked up automatically if it is not being grabbed by any player  
- **SPACE (next to ball)** → Steal ball from the enemy  
- **SHIFT** → Dash in the facing direction  

---

## Network Aspects

### P2P Connection
Since the game is for only 2 players, we’ve decided that the best method to create the network is Peer-To-Peer. This way, each player can choose between being the host of the game or the client who joins, eliminating the need for an external server and making the multiplayer network easier to manage.

### World State Replication
To avoid desynchronization between each player’s game, we’ve implemented a **Master-Slave system** where the server dictates the state of the game. We periodically send information such as:
- Both player positions
- Ball transformations
- Who has possession of the ball

### Input Data Package
To send the data between players, we’ve decided to send the inputs. This approach avoids the need for position interpolation since the receiver recreates them locally.  
Additionally, due to the small size of the variables for each input, the package sent is lightweight and can be transmitted more frequently.

### Ping
We’ve calculated the time between package transmission in milliseconds.

### Package Send Optimization
Thanks to the input send system:
- Small-sized packages with simple variables are sent, making them easy to manage.
- We ensure to stop package transmission when unnecessary, e.g., between rounds where no player input is required.

---

## Improvements from Previous Versions

### Bug Fixes
- **Ball Reset Issue**: Fixed a bug where the ball would not restart properly after scoring a goal due to hierarchy issues.
- **World State Replication Jitter**: Resolved an issue where the ball would occasionally return to the player after kicking it, causing a state desync. The fix involved replicating the ball rotation, velocity, and possession state.

### New Features & Improvements
- **Countdown Between Rounds**: A countdown is displayed, and each player’s input is disabled during this time.
- **Ping Display**: Added a visual ping indicator.
- **Performance Optimization**: Improved game performance by tweaking scene values.

---

## Known Bugs
- **Desync on Scoring**: While replicating the World State from server to client, if one player scores during a World State update, the goal may count only for one player. This locks the players in the round countdown and displays the wrong score.

---

## Tasks and Responsibilities

| **Task**                                      | **Members**         |
|-----------------------------------------------|---------------------|
| Gameplay video                                | Arnau               |
| Report document                               | Arnau, Biel, Pau    |
| GitHub release                                | Biel                |
| Code clean-up                                 | Biel, Pau           |
| Bug fix: Ball hierarchy issues                | Arnau, Biel         |
| Ping display                                  | Arnau, Pau          |
| Countdown between rounds                      | Biel, Pau           |
| Improve game performance (scene values)       | Arnau               |
| Bug fix: World State Replication jitter       | Arnau, Biel, Pau    |
| Stop package transmission when unnecessary    | Biel, Pau           |
