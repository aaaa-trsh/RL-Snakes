using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Leguar.TotalJSON;
using System.IO;

public class SnakeAgent2 : MonoBehaviour
{
    [Header("Config")]
    public JSON qJson;
    public string qTableName;
    public bool clearQTable;
    public bool changeQTable;

    [Header("Settings")]
    [Range(0, 0.2f)]
    public float epsilon;
    public float epsilonDecay;
    public float learningRate;

    float[] qValues;
    float[] newQValues;
    float stepReward, stepPenalty;

    string state, newState;
    void Start()
    {
        if (clearQTable)
        {
            qJson = new JSON();
        }
        else
        {
            var textFile = Resources.Load<TextAsset>(qTableName);
            qJson = JSON.ParseString(textFile.text);
        }
    }

    public int RequestInput(string dangerTiles, string goalDir)
    {
        stepPenalty = 0;
        stepReward = 0;

        int action = Random.Range(0, 4);
        state = dangerTiles + goalDir;
        if (!qJson.ContainsKey(state))
            NewQEntry(state);

        qValues = qJson.GetJArray(state).AsFloatArray();

        if (Random.Range(0, 1) < epsilon)
        {
            action = System.Array.IndexOf(qValues, qValues.Max());
        }

        return action;
    }

    public void UpdateQTable(string newDangerTiles, string newGoalDir, int action)
    {
        if (changeQTable)
        {
            newState = newDangerTiles + newGoalDir;
            if (!qJson.ContainsKey(newState))
                NewQEntry(newState);
            newQValues = qJson.GetJArray(newState).AsFloatArray();
            
            float actionScore = qValues[action] + learningRate * (stepReward + stepPenalty * System.Array.IndexOf(newQValues, newQValues.Max()) - qValues[action]);

            qJson.GetJArray(state)[action] = new JNumber(actionScore);
        }
    }

    public void SaveTable()
    {
        string jsonString = qJson.CreatePrettyString();
        StreamWriter writer = new StreamWriter(new FileStream("Assets/Resources/" + qTableName + ".json", FileMode.Truncate));
        writer.Write(jsonString);
        writer.Close();
    }
    void NewQEntry(string state) 
    {
        qJson.Add(state, new JArray());
        for (int i = 0; i < 4; i++)
            qJson.GetJArray(state).Add(new JNumber(0));
    }
    public void Reward(float amount)
    {
        stepReward += amount;
    }
    public void Penalize(float amount)
    {
        stepPenalty += amount;
    }
}
