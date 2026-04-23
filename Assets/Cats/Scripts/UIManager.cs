using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI goldText;

    [Header("Unit Toggles")]
    public Toggle workerToggle;
    public Toggle rangeToggle;

    [Header("Unit Prefabs")]
    public PlayerUnit workerPrefab;
    public PlayerUnit rangePrefab;

    private void Start()
    {
        workerToggle.onValueChanged.AddListener(OnWorkerToggle);
        rangeToggle.onValueChanged.AddListener(OnRangeToggle);

        rangeToggle.isOn = true;
        SelectUnit(rangePrefab);
    }

    private void Update()
    {
        UpdateGoldUI();
    }

    void UpdateGoldUI()
    {
        if (GameManager.Instance == null || goldText == null)
            return;

        goldText.text = "Gold: " + BaseUnit.Instance.storedGold;
    }

    // =========================
    // TOGGLE EVENTS
    // =========================

    void OnWorkerToggle(bool isOn)
    {
        if (!isOn) return;

        SelectUnit(workerPrefab);
    }

    void OnRangeToggle(bool isOn)
    {
        if (!isOn) return;

        SelectUnit(rangePrefab);
    }

    // =========================
    // UNIT SELECTION
    // =========================

    void SelectUnit(PlayerUnit unit)
    {
        if (GameManager.Instance == null || unit == null)
            return;

        if (!GameManager.Instance.CanAfford(unit))
            Debug.Log("Not enough gold for: " + unit.name);

        GameManager.Instance.selectedUnitPrefab = unit;

    }
}