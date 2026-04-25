using UnityEngine;

public enum ActiveSkill { Woodcutting, Mining }

public class GameManager : MonoBehaviour
{

    public ActiveSkill currentSkill = ActiveSkill.Woodcutting;

    // Listener for UIManager tracking current skill
    public event System.Action OnSkillSwapped;
    public static GameManager Instance { get; private set; }

    void Awake(){
        Instance = this;
    }

    // Called directly from skill swap button, invokes UI manager flag
    public void SwapSkill()
    {
        currentSkill = currentSkill == ActiveSkill.Woodcutting ? ActiveSkill.Mining : ActiveSkill.Woodcutting;
        OnSkillSwapped?.Invoke();
    }
   
}
