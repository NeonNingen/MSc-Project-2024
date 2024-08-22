using UnityEngine;

public class CameraSwitcher : MonoBehaviour {
    // Terrain Camera
    public Camera mainCamera; 
    public Camera aiStillCamera;
    public Camera aiMovingCamera;
    public GameObject meshParentUI;
    public GameObject drawModeUI;
    public GameObject aiModeUI;
    public GameObject aiParentObj;

    private AudioListener mainCameraAudioListener;
    private AudioListener aiStillCameraAudioListener;
    private AudioListener aiMovingCameraAudioListener;

    void Start() {
        aiModeUI.gameObject.SetActive(true);

        mainCameraAudioListener = mainCamera.GetComponent<AudioListener>();
        aiStillCameraAudioListener = aiStillCamera.GetComponent<AudioListener>();
        aiMovingCameraAudioListener = aiMovingCamera.GetComponent<AudioListener>();
        
        ActivateMainCamera();
    }

    public void ActivateMainCamera() {
        mainCamera.enabled = true;
        aiStillCamera.enabled = false;
        aiMovingCamera.enabled = false;

        // Enable the main camera's AudioListener and disable the AI camera's AudioListener
        if (mainCameraAudioListener != null) mainCameraAudioListener.enabled = true;
        if (aiStillCamera != null) aiStillCamera.enabled = false;
        if (aiMovingCamera != null) aiMovingCamera.enabled = false;

        aiParentObj.gameObject.SetActive(false);

        ShowUI(true);
    }

    public void ActivateAIStillCamera() {
        mainCamera.enabled = false;
        aiStillCamera.enabled = true;
        aiMovingCamera.enabled = false;

        // Enable the AI Perspective camera's AudioListener and disable the main camera's AudioListener
        if (mainCameraAudioListener != null) mainCameraAudioListener.enabled = false;
        if (aiStillCamera != null) aiStillCamera.enabled = true;
        if (aiMovingCamera != null) aiMovingCamera.enabled = false;

        aiParentObj.gameObject.SetActive(true);

        ShowUI(false);
    }

    public void ActivateAIMovingCamera() {
        mainCamera.enabled = false;
        aiStillCamera.enabled = false;
        aiMovingCamera.enabled = true;

        // Enable the AI Moving camera's AudioListener and disable the main camera's AudioListener
        if (mainCameraAudioListener != null) mainCameraAudioListener.enabled = false;
        if (aiStillCamera != null) aiStillCamera.enabled = false;
        if (aiMovingCamera != null) aiMovingCamera.enabled = true;

        ShowUI(false);
    }

    void ShowUI(bool show) {
        foreach (Transform child in meshParentUI.transform) {
            child.gameObject.SetActive(show);
        }
        
        foreach (Transform child in drawModeUI.transform) {
            child.gameObject.SetActive(show);
        }

        foreach (Transform child in aiModeUI.transform) {
            child.gameObject.SetActive(!show);
            if (aiMovingCamera.enabled == false && child.gameObject.name == "CameraControlsText") child.gameObject.SetActive(false);
            if (aiMovingCamera.enabled == false && child.gameObject.name == "CameraControlsSubText") child.gameObject.SetActive(false);
            if (aiMovingCamera.enabled == true && child.gameObject.name == "CameraControlsText") child.gameObject.SetActive(true);
            if (aiMovingCamera.enabled == true && child.gameObject.name == "CameraControlsSubText") child.gameObject.SetActive(true);
        }
    }
}