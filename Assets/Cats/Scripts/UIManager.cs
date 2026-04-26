using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class UnitButton
{
    public Button button;
    public PlayerUnit unitPrefab;

    [HideInInspector] public bool isSelected;

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        // Disable interaction when selected
        button.interactable = !selected;
    }
}

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI goldText;

    [Header("Unit Buttons")]
    public List<UnitButton> unitButtons;

    private UnitButton currentSelected;

    private void Start()
    {
        // Assign listeners dynamically
        foreach (var ub in unitButtons)
        {
            UnitButton captured = ub;

            ub.button.onClick.AddListener(() => OnUnitButtonClicked(captured));
        }

        // Default selection (first one or pick manually)
        if (unitButtons.Count > 0)
        {
            SelectButton(unitButtons[0]);
        }
    }

    private void Update()
    {
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        if (GameManager.Instance == null || goldText == null)
            return;

        goldText.text = "Gold: " + GameManager.Instance.gold;
    }

    // =========================
    // BUTTON GROUP LOGIC
    // =========================

    void OnUnitButtonClicked(UnitButton clicked)
    {
        SelectButton(clicked);
    }

    void SelectButton(UnitButton selected)
    {
        if (selected == null || GameManager.Instance == null)
            return;

        // Deselect all
        foreach (var ub in unitButtons)
        {
            ub.SetSelected(false);
        }

        // Select this one
        selected.SetSelected(true);
        currentSelected = selected;

        // Gameplay logic
        if (!GameManager.Instance.CanAfford(selected.unitPrefab))
        {
            Debug.Log("Not enough gold for: " + selected.unitPrefab.name);
        }

        GameManager.Instance.selectedUnitPrefab = selected.unitPrefab;
    }
}