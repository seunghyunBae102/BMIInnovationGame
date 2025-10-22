using System;
using System.IO;
using UnityEngine;

// 간단한 CSV/Console 로거. 필요하면 확장하세요.
public class SimulationLogger : MonoBehaviour
{
    public bool writeCsv = false;
    public string csvFilePath = "simulation_events.csv";
    private StreamWriter writer;

    private void Awake()
    {
        if (writeCsv)
        {
            try
            {
                writer = new StreamWriter(csvFilePath, false);
                writer.WriteLine("time,user,media,sessionMinutes,revenue,deltaHabit,deltaBrand");
            }
            catch (Exception ex)
            {
                Debug.LogWarning("SimulationLogger: CSV open failed: " + ex.Message);
                writeCsv = false;
            }
        }
    }

    public void LogEvent(string userId, string mediaId, float sessionMinutes, float revenue, float deltaHabit, float deltaBrand)
    {
        string line = $"{Time.time},{userId},{mediaId},{sessionMinutes},{revenue},{deltaHabit},{deltaBrand}";
        Debug.Log("[SimEvent] " + line);
        if (writeCsv && writer != null)
        {
            writer.WriteLine(line);
        }
    }

    private void OnDestroy()
    {
        if (writer != null) writer.Close();
    }
}