# RL-Snakes
The classic snake game with agents trained with Q-learning

Each snake has a 3 x 3 vision box around its head that detect if that cell is a wall, and is also aware of what direction it has to travel to get to the goal. Every game tick, it chooses the direction that has the highest probability of giving it points.

A snake gets 1 point when it approaches its goal, and is penalized 1 point when it does not. It gets 10 points if it reaches its goal, and is penalized 20 points if it dies.
