using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class DataThreadRequester : MonoBehaviour
{
    static DataThreadRequester instance;

    Queue<DataThreadInfo> dataThreadInfoQueue = new Queue<DataThreadInfo>();

    private void Awake()
    {
        instance = FindObjectOfType<DataThreadRequester>();
    }

    private void Update()
    {
        lock (dataThreadInfoQueue)
        {
            while (dataThreadInfoQueue.Count > 0)
            {
                DataThreadInfo dataThreadInfo = dataThreadInfoQueue.Dequeue();
                dataThreadInfo.callback(dataThreadInfo.parameter);
            }
        }
       
    }
    public static void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart threadStart = delegate
        {
            instance.DataThread(generateData, callback);
        };

        new Thread(threadStart).Start();
    }

    public void DataThread(Func<object> generateData, Action<object> callback)
    {
        object data = generateData();
        lock (dataThreadInfoQueue)
        {
            dataThreadInfoQueue.Enqueue(new DataThreadInfo(callback, data));
        }
    }
}
public struct DataThreadInfo
{
    public readonly Action<object> callback;
    public readonly object parameter;

    public DataThreadInfo(Action<object> callback, object parameter)
    {
        this.callback = callback;
        this.parameter = parameter;
    }
}