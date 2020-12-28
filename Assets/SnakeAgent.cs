using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leguar.TotalJSON;
using System.IO;

public class SnakeAgent : MonoBehaviour
{
    public string qTableName;
    public float stepReward;
    public float cumReward;
    public float stepPenalty;
    public float cumPenalty;
    [Range(0, 0.999999f)]
    public float epsilon = 0.99f;
    public float epsilonDecrease;
    public float learningRate = 0.6f;
    public int action = 0;
    public float[] qValues;
    public float[] newQValues;
    public string state;
    public bool changeQTable;
    public bool clearQTable;

    void Start()
    {
        if (clearQTable)
        {
            PlayerPrefs.DeleteAll();
        }
        else 
        {
            StreamReader reader = new StreamReader("Assets/Resources/" + qTableName + ".json");
            reader.Close();
        }
    }
    public int RequestInput(string DangerTiles, Vector2 GoalDirection)
    {
        stepPenalty = 0;
        stepReward = 0;

        state = GoalDirection.x + "," + GoalDirection.y + " " + DangerTiles;

        if (PlayerPrefs.HasKey(state))
        {
            if (Random.Range(0, 1) < epsilon)
            {
                action = Random.Range(0, 4);
                epsilon -= epsilonDecrease;
            }
            else
            {
                qValues = RetrieveQValues(state);
                action = System.Array.IndexOf(qValues, qValues.Max());
            }
        }
        else
        {
            action = Random.Range(0, 4);
            epsilon -= epsilonDecrease;
        }

        qValues = RetrieveQValues(state);
        return action;
    }

    public void UpdateQTable(string NewDangerTiles, Vector2 NewGoalDirection) 
    {
        if (changeQTable)
        {
            string newState = NewGoalDirection.x + "," + NewGoalDirection.y + " " + NewDangerTiles;

            newQValues = RetrieveQValues(newState);

            string encodedValues = "";
            for (int i = 0; i < qValues.Length; i++)
            {
                if (i == action)
                    encodedValues += qValues[i] + learningRate * (stepReward + stepPenalty * System.Array.IndexOf(newQValues, newQValues.Max()) - qValues[i]) + ",";
                else
                    encodedValues += qValues[i] + ",";
            }
            encodedValues = encodedValues.Substring(0, encodedValues.Length - 1);
            PlayerPrefs.SetString(state, encodedValues);
        }
    }

    float[] RetrieveQValues(string state)
    {
        string rawQString = PlayerPrefs.GetString(state, "0,0,0,0");
        string[] qString = rawQString.Split(',');
        float[] qValues = new float[qString.Length];
        for (int i = 0; i < qString.Length; i++)
        {
            qValues[i] = float.Parse(qString[i]);
        }
        return qValues;
    }

    public void Reward(float amount) 
    { 
        stepReward += amount;
        cumReward += amount;
    }
    public void Penalize(float amount) 
    { 
        stepPenalty += amount;
        cumPenalty += amount;
    }

    public void ResetScores() 
    {
        cumPenalty = 0;
        cumReward = 0;
    }
}
