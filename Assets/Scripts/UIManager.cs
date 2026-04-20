using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject equipPanel;


    void Start()
    {
        ShowPanel(mainPanel);
    }

    public void ShowPanel(GameObject panel) {
        mainPanel.SetActive(false);
        shopPanel.SetActive(false);
        equipPanel.SetActive(false);
        panel.SetActive(true);
    }

    public void OnShopButtonClicked(){
        ShowPanel(shopPanel);
    }

    public void OnEquipButtonClicked(){
        ShowPanel(equipPanel);
    }

    public void OnBackButtonClicked(){
        ShowPanel(mainPanel);
    }

    
    void Update()
    {
        
    }
}
