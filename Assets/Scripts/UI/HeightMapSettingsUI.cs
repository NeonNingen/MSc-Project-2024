using UnityEngine;
using UnityEngine.UI;

public class HeightMapSettingsUI : MonoBehaviour {
    public Dropdown drawModeDropdown;
    public HeightMapSettings heightMapSettings;
    public HeightMapSettings defaultHeightMapSettings;
    public MapPreview mapPreview;
    public GameObject meshParentUI;

    public Slider lodSlider;
    public Slider curvePoint1SliderX;
    public Slider curvePoint1SliderY;
    public Slider curvePoint2SliderX;
    public Slider curvePoint2SliderY;
    
    private InputField scaleInput;
    private Slider octavesSlider;
    private Slider persistanceSlider;
    private InputField lacunarityInput;
    private InputField heightMultiplierInput;
    private InputField offsetXInput;
    private InputField offsetYInput;
    private Button resetButton;

    void Start() {
        // Find the UI elements under the meshParentUI
        scaleInput = meshParentUI.transform.Find("SeedInputField").GetComponent<InputField>();
        octavesSlider = meshParentUI.transform.Find("OctavesSlider").GetComponent<Slider>();
        persistanceSlider = meshParentUI.transform.Find("PersistenceSlider").GetComponent<Slider>();
        lacunarityInput = meshParentUI.transform.Find("LacunarityInputField").GetComponent<InputField>();
        heightMultiplierInput = meshParentUI.transform.Find("HeightInputField").GetComponent<InputField>();
        offsetXInput = meshParentUI.transform.Find("OffsetXInputField").GetComponent<InputField>();
        offsetYInput = meshParentUI.transform.Find("OffsetYInputField").GetComponent<InputField>();
        resetButton = meshParentUI.transform.Find("ResetButton").GetComponent<Button>();

        UpdateUI();

        resetButton.onClick.AddListener(ResetToDefault);
    }

    void ResetToDefault() {
        heightMapSettings.noiseSettings.scale = defaultHeightMapSettings.noiseSettings.scale;
        heightMapSettings.noiseSettings.octaves = defaultHeightMapSettings.noiseSettings.octaves;
        heightMapSettings.noiseSettings.persistance = defaultHeightMapSettings.noiseSettings.persistance;
        heightMapSettings.noiseSettings.lacunarity = defaultHeightMapSettings.noiseSettings.lacunarity;
        heightMapSettings.heightMultiplier = defaultHeightMapSettings.heightMultiplier;
        heightMapSettings.noiseSettings.offset = defaultHeightMapSettings.noiseSettings.offset;
        heightMapSettings.heightCurve = new AnimationCurve(defaultHeightMapSettings.heightCurve.keys);

        lodSlider.value = 0;

        UpdateUI();
        TryUpdateMap();
    }

    void UpdateUI() {
        scaleInput.text = heightMapSettings.noiseSettings.scale.ToString();
        scaleInput.onEndEdit.AddListener(OnScaleChanged);

        octavesSlider.value = heightMapSettings.noiseSettings.octaves;
        octavesSlider.onValueChanged.AddListener(OnOctavesChanged);

        persistanceSlider.value = heightMapSettings.noiseSettings.persistance;
        persistanceSlider.onValueChanged.AddListener(OnpersistanceChanged);

        lacunarityInput.text = heightMapSettings.noiseSettings.lacunarity.ToString();
        lacunarityInput.onEndEdit.AddListener(OnLacunarityChanged);

        heightMultiplierInput.text = heightMapSettings.heightMultiplier.ToString();
        heightMultiplierInput.onEndEdit.AddListener(OnHeightMultiplierChanged);

        offsetXInput.text = heightMapSettings.noiseSettings.offset.x.ToString();
        offsetXInput.onEndEdit.AddListener(OnOffsetXChanged);

        offsetYInput.text = heightMapSettings.noiseSettings.offset.y.ToString();
        offsetYInput.onEndEdit.AddListener(OnOffsetYChanged);

        lodSlider.value = mapPreview.LOD;
        lodSlider.onValueChanged.AddListener(OnLODSliderChanged);

        drawModeDropdown.options.Clear();
        drawModeDropdown.options.Add(new Dropdown.OptionData() { text = "Noise Map" });
        drawModeDropdown.options.Add(new Dropdown.OptionData() { text = "Mesh" });
        drawModeDropdown.options.Add(new Dropdown.OptionData() { text = "Falloff Map" });

        drawModeDropdown.onValueChanged.AddListener(delegate {
            OnDrawModeChanged(drawModeDropdown);
        });

        drawModeDropdown.value = (int)mapPreview.drawMode;
        drawModeDropdown.RefreshShownValue();

        if (heightMapSettings.heightCurve.keys.Length >= 2) {
            curvePoint1SliderX.value = heightMapSettings.heightCurve.keys[0].time;
            curvePoint1SliderY.value = heightMapSettings.heightCurve.keys[0].value;
            curvePoint2SliderX.value = heightMapSettings.heightCurve.keys[1].time;
            curvePoint2SliderY.value = heightMapSettings.heightCurve.keys[1].value;
        }

        curvePoint1SliderX.onValueChanged.AddListener(delegate { UpdateCurve(); });
        curvePoint1SliderY.onValueChanged.AddListener(delegate { UpdateCurve(); });
        curvePoint2SliderX.onValueChanged.AddListener(delegate { UpdateCurve(); });
        curvePoint2SliderY.onValueChanged.AddListener(delegate { UpdateCurve(); });
    }

    void OnScaleChanged(string value) {
        heightMapSettings.noiseSettings.scale = float.Parse(value);
        TryUpdateMap();
    }

    void OnOctavesChanged(float value) {
        heightMapSettings.noiseSettings.octaves = (int)value;
        TryUpdateMap();
    }

    void OnpersistanceChanged(float value) {
        heightMapSettings.noiseSettings.persistance = value;
        TryUpdateMap();
    }

    void OnLacunarityChanged(string value) {
        heightMapSettings.noiseSettings.lacunarity = float.Parse(value);
        TryUpdateMap();
    }

    void OnHeightMultiplierChanged(string value) {
        heightMapSettings.heightMultiplier = float.Parse(value);
        TryUpdateMap();
    }

    void OnOffsetXChanged(string value) {
        Vector2 offset = heightMapSettings.noiseSettings.offset;
        offset.x = float.Parse(value);
        heightMapSettings.noiseSettings.offset = offset;
        TryUpdateMap();
    }

    void OnOffsetYChanged(string value) {
        Vector2 offset = heightMapSettings.noiseSettings.offset;
        offset.y = float.Parse(value);
        heightMapSettings.noiseSettings.offset = offset;
        TryUpdateMap();
    }

    void OnLODSliderChanged(float value) {
        int lodValue = Mathf.RoundToInt(value);
        mapPreview.LOD = lodValue;
        TryUpdateMap();
    }

    void UpdateCurve() {
        if (heightMapSettings.heightCurve.keys.Length >= 2) {
            Keyframe[] keys = heightMapSettings.heightCurve.keys;

            // Update the positions of the keyframes based on slider values
            keys[0].time = curvePoint1SliderX.value;
            keys[0].value = curvePoint1SliderY.value;
            keys[1].time = curvePoint2SliderX.value;
            keys[1].value = curvePoint2SliderY.value;

            // Reassign the updated keys to the curve
            heightMapSettings.heightCurve.keys = keys;
            TryUpdateMap();
        }
    }

    void OnDrawModeChanged(Dropdown change) {
        MapPreview.DrawMode selectedMode = (MapPreview.DrawMode)change.value;
        mapPreview.drawMode = selectedMode;
        TryUpdateMap();

        ToggleUIElements(selectedMode);
    }

    void ToggleUIElements(MapPreview.DrawMode selectedMode) {

        if (selectedMode == MapPreview.DrawMode.NoiseMap || selectedMode == MapPreview.DrawMode.FalloffMap) {
            meshParentUI.SetActive(false);
        } else {
            meshParentUI.SetActive(true);
        }
    }

    void TryUpdateMap() {
        if (mapPreview.autoUpdate) mapPreview.DrawMapInEditor();
    }
}
