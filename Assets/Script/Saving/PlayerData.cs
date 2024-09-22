using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable
{
    public FixedString32Bytes playerName;
    public float playerHealth;
    public float[] playerPosition;
    public float[] playerRotation;
    public int[] playerInventory;
    public int[] equipment;
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerHealth);

        int lengthP = 0;
        int lengthR = 0;
        int lengthI = 0;
        if (!serializer.IsReader)
        {
            lengthP = playerPosition.Length;
            lengthR = playerRotation.Length;
            lengthI = playerInventory.Length;
        }

        serializer.SerializeValue(ref lengthP);
        serializer.SerializeValue(ref lengthR);
        serializer.SerializeValue(ref lengthI);

        if (serializer.IsReader)
        {
            playerPosition = new float[lengthP];
            playerRotation = new float[lengthR];
            playerInventory = new int[lengthI];
        }

        for (int i = 0; i < lengthP; i++)
        {
            serializer.SerializeValue(ref playerPosition[i]);
            serializer.SerializeValue(ref playerRotation[i]);
        }

        for (int i = 0; i < lengthI; i++)
        {
            serializer.SerializeValue(ref playerInventory[i]);
        }
    }

    public static readonly PlayerData Empty = new PlayerData
    {
        playerName = "",
        playerHealth = 0,
        playerPosition = new float[0],
        playerRotation = new float[0],
        playerInventory = new int[0],
    };

    public bool IsEmpty()
    {
        return playerName == "";
    }
}


public struct FloatArray : INetworkSerializable
{
    public float[] value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        int length = 0;
        if (!serializer.IsReader)
        {
            length = value.Length;
        }

        serializer.SerializeValue(ref length);

        if (serializer.IsReader)
        {
            value = new float[length];
        }

        for (int i = 0; i < length; i++)
        {
            serializer.SerializeValue(ref value[i]);
        }
    }
}