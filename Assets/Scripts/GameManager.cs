using UnityEngine;
using System;

public enum ActiveSkill { Woodcutting, Mining }

public class GameManager : MonoBehaviour
{

    // Gamestate Trackers
    public ActiveSkill currentSkill = ActiveSkill.Woodcutting;
    public long coins = 0;


    // Listeners for invoking UIManager methods
    public event System.Action OnSkillSwapped;
    public event System.Action OnCoinsChanged;

    // Init GameManager instance
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        coins = 0;
    }

    // Called directly from skill swap button, invokes UI manager flag
    public void SwapSkill()
    {
        currentSkill = currentSkill == ActiveSkill.Woodcutting ? ActiveSkill.Mining : ActiveSkill.Woodcutting;
        OnSkillSwapped?.Invoke();

        // DELETE ME ONCE CLICKER IS IMPLEMENTED
        coins *= 2; AddCoins(1);
        
    }

    // Called when a skill objective completes (tree felled, ore mined)
    void AddCoins(long amount)
    {
        coins += amount;
        OnCoinsChanged?.Invoke();
    }
   
}
