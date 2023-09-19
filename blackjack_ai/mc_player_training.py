import numpy as np
import gym
import copy
from collections import defaultdict
import sys
import json
import matplotlib
import matplotlib.pyplot as plt
import timeit
import json

def remap_keys(mapping):
    return [{'key':k, 'value': int(v)} for k, v in mapping.items()]

if __name__ == "__main__":
    env = gym.make('Blackjack-v1')
    EPS = 0.05
    GAMMA = 1.0

    Q = {}
    agentSumSpace = [i for i in range(4, 22)]
    dealerShowCardSpace = [i + 1 for i in range(10)]
    agentAceSpace = [False, True]
    actionSpace = [0, 1] #stick or hit

    stateSpace = []
    returns = {}
    pairsVisited = {}

    for total in agentSumSpace:
        for card in dealerShowCardSpace:
            for ace in agentAceSpace:
                for action in actionSpace:
                    Q[((total, card, ace), action)] = 0
                    returns[((total, card, ace), action)] = 0
                    pairsVisited[((total, card, ace), action)] = 0
                stateSpace.append((total, card, ace))

    policy = {}
    for state in stateSpace:
        policy[state] = np.random.choice([0])


    start = timeit.default_timer()

    numEpisodes = 5000000
    for i in range(numEpisodes):
        stateActionReturns = []
        memory = []
        if i % 1000000 == 0:
            print('start episode: ', i)
        observation = env.reset()
        observation = observation[0]
        done = False
        while not done:
            action = policy[observation]
            observation_, reward, done, info, _ = env.step(action)
            memory.append((observation[0], observation[1], observation[2], action, reward))
            observation = observation_
        memory.append((observation[0], observation[1], observation[2], action, reward))

        G = 0
        last = True
        for playerSum, dealerCard, usableAce, action, reward in reversed(memory):
            if last:
                last = not last
            else:
                stateActionReturns.append((playerSum, dealerCard, usableAce, action, G))

            G = GAMMA * G + reward

        stateActionReturns.reverse()
        stateActionVisited = []

        for playerSum, dealerCard, usableAce, action, G in stateActionReturns:
            sa = ((playerSum, dealerCard, usableAce), action)
            if sa not in stateActionVisited:
                pairsVisited[sa] += 1
                returns[(sa)] += (1 / pairsVisited[(sa)]) * (G - returns[(sa)])
                Q[sa] = returns[sa]

                rand = np.random.random()
                if rand < 1 - EPS:
                    state = (playerSum, dealerCard, usableAce)
                    values = np.array([Q[(state, a)] for a in actionSpace])
                    best = np.random.choice(np.where(values == values.max())[0])
                    policy[state] = actionSpace[best]
                else:
                    policy[state] = np.random.choice(actionSpace)
                stateActionVisited.append(sa)
        if EPS - 1e-7 > 0:
            EPS -= 1e-7
        else:
            EPS = 0

    stop = timeit.default_timer()

    print('Time: ', stop - start)

    numEpisodes = 1000
    rewards = np.zeros(numEpisodes)
    totalReward = 0
    wins = 0
    losses = 0
    draws = 0
    print('getting ready to test policy')   
    for i in range(numEpisodes):
        observation, _ = env.reset()
        done = False
        while not done:
            action = policy[observation]
            observation_, reward, done, info, _ = env.step(action)            
            observation = observation_
        totalReward += reward
        rewards[i] = totalReward

        if reward >= 1:
            wins += 1
        elif reward == 0:
            draws += 1
        elif reward == -1:
            losses += 1
    
    wins /= numEpisodes
    losses /= numEpisodes
    draws /= numEpisodes
    print('win rate', wins, 'loss rate', losses, 'draw rate', draws)
    plt.plot(rewards)
    plt.show()    

    table = []
    for total in agentSumSpace:
        row = []
        print(total)
        for card in dealerShowCardSpace:
            state = (total, card, True)
            row.append(policy[state])
        print(len(table))
        table.append(copy.deepcopy(row))
    
    plt.imshow( table , cmap = 'magma' )
  
    # Adding details to the plot
    plt.title( "2-D Heat Map" )
    plt.xlabel('x-axis')
    plt.ylabel('y-axis')

    # Adding a color bar to the plot
    plt.colorbar()

    # Displaying the plot
    plt.show()
    with open("mc_policy.json", "w") as f:
        json.dump(remap_keys(policy), f)
        f.close()