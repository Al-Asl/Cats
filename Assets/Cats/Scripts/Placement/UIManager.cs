using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public class UnitButton
    {
        public Button button;
        public int entityIndex;
        public bool isSelected;

        public void SetSelected(bool selected)
        {
            isSelected = selected;
            button.interactable = !selected;
        }
    }

    public TextMeshProUGUI goldText;
    public Button buttonTemplate;

    private List<UnitButton> unitButtons = new List<UnitButton>();
    private UnitButton currentSelected;

    private void Start()
    {
        var entities = PlacementManager.Instance.unitsRepo.entities;
        for (int i = 0; i < entities.Count; i++) {
            var entity = entities[i];

            var button = Instantiate(buttonTemplate, buttonTemplate.transform.parent, true);
            button.gameObject.SetActive(true);
            button.GetComponentInChildren<TextMeshProUGUI>().text = entity.name;

            var unitButton = new UnitButton()
            {
                button = button,
                entityIndex = i
            };

            unitButtons.Add(unitButton);
        }

        foreach (var ub in unitButtons)
        {
            UnitButton captured = ub;

            ub.button.onClick.AddListener(() => OnUnitButtonClicked(captured));
        }

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
        if (PlacementManager.Instance == null || goldText == null)
            return;

        goldText.text = "Gold: " + PlacementManager.Instance.GetBase().gold;
    }

    void OnUnitButtonClicked(UnitButton clicked)
    {
        SelectButton(clicked);
    }

    void SelectButton(UnitButton selected)
    {
        if (selected == null || PlacementManager.Instance == null)
            return;

        foreach (var ub in unitButtons)
        {
            ub.SetSelected(false);
        }

        selected.SetSelected(true);
        currentSelected = selected;

        PlacementManager.Instance.entityIndex = selected.entityIndex;
    }
}