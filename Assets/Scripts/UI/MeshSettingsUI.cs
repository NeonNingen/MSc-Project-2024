using UnityEngine;
using UnityEngine.UI;

public class MeshSettingsUI : MonoBehaviour {
    public Slider chunkSizeSlider;
    public MeshSettings meshSettings;
    public MapPreview mapPreview;

    void Start() {
        chunkSizeSlider.value = meshSettings.chunkSizeIndex;
        chunkSizeSlider.onValueChanged.AddListener(OnChunkSizeSliderChanged);
    }

    void OnChunkSizeSliderChanged(float value) {
        meshSettings.chunkSizeIndex = (int)value;

        if (mapPreview.autoUpdate) mapPreview.DrawMapInEditor();
    }
}
