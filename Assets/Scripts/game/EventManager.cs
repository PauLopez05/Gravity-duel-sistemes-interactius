using System;
using UnityEngine;

public static class EventManager
{
    public static event Action<int> OnPlayerDeath;

    public static void TriggerPlayerDeath(int player)
    {
        OnPlayerDeath?.Invoke(player);
        Debug.Log("tuu puuta madrue.");
    }
}