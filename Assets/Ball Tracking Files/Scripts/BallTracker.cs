using System.Collections;
using PassthroughCameraSamples;
using UnityEngine;
using UnityEngine.UI;
using System;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
// using OpenCVForUnityExample;
using TMPro;
using System.Threading;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.XR;

public class BallTracker : MonoBehaviour
{
    private Camera _mainCamera;
    private WebCamTexture _webcamTexture;
    private WebCamTextureManager _cameraManager;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private RawImage _rawImage2;
    [SerializeField] private TMPro.TextMeshProUGUI _fpsText;

    private Scalar orangeHSVMin = new Scalar(0, 229, 49);   // Untere Grenze f�r Orange im HSV-Farbraum
    private Scalar orangeHSVMax = new Scalar(5, 255, 255);  // Obere Grenze f�r Orange im HSV-Farbraum

    private bool isChecking = false;
    private int frameCount = 0;
    private float elapsedTime = 0f;
    private float fps = 0f;

    [SerializeField] private Slider _sliderHueMin;
    [SerializeField] private Slider _sliderHueMax;
    [SerializeField] private Slider _sliderSatMin;
    [SerializeField] private Slider _sliderSatMax;
    [SerializeField] private Slider _sliderValMin;
    [SerializeField] private Slider _sliderValMax;

    [SerializeField] private TextMeshProUGUI _sliderMinText;
    [SerializeField] private TextMeshProUGUI _sliderMaxText;

    [SerializeField] private Transform leftEyeAnchor;
    [SerializeField] private GameObject virtualBall;
    private float realBallSize = 0.17f;

    private Mat imgMat = null, hsvMat = null;
    private Texture2D texture = null, texture2 = null;
    private Mat erodeElement = null, dilateElement = null;

    [SerializeField] private Slider yOffsetslider;
    [SerializeField] private TextMeshProUGUI yOffsetText;
    private float yOffset = -0.2f;

    [SerializeField] private GameObject GUI_Spawn_Elements;
    [SerializeField] private GameObject UI_Interactors_For_Menu;

    [SerializeField] private GameObject leftHandController;
    [SerializeField] private GameObject rightHandController;

    [SerializeField] private GameObject targetField;

    [SerializeField] private GameObject fieldMoverPrefab;
    private GameObject activeFieldMover = null;

    private void Start()
    {
        XRSettings.useOcclusionMesh = false; // prevent fisheye effect when taking video...

        yOffsetslider.value = yOffset;
        isChecking = true;

        _mainCamera = Camera.main;
        _cameraManager = FindAnyObjectByType<WebCamTextureManager>();

        erodeElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(8, 8));
        dilateElement = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(8, 8));

        if (!_mainCamera || !_cameraManager)
        {
            Debug.LogError("BallTracker: Missing required references.");
            return;
        }

        StartCoroutine(WaitForWebCam());

        yOffsetslider.onValueChanged.AddListener(value =>
        {
            yOffset = value;
            yOffsetText.SetText($"Y Offset: {value:F2}");
        });

        _sliderHueMin.value = (float)orangeHSVMin.val[0];
        _sliderHueMax.value = (float)orangeHSVMax.val[0];
        _sliderSatMin.value = (float)orangeHSVMin.val[1];
        _sliderSatMax.value = (float)orangeHSVMax.val[1];
        _sliderValMin.value = (float)orangeHSVMin.val[2];
        _sliderValMax.value = (float)orangeHSVMax.val[2];

        _sliderHueMin.onValueChanged.AddListener(value =>
        {
            orangeHSVMin.val[0] = value;
            _sliderMinText.SetText($"Min (H:{orangeHSVMin.val[0]:F0}, S:{orangeHSVMin.val[1]:F0}, V:{orangeHSVMin.val[2]:F0})");
        });
        _sliderHueMax.onValueChanged.AddListener(value =>
        {
            orangeHSVMax.val[0] = value;
            _sliderMaxText.SetText($"Max (H:{orangeHSVMax.val[0]:F0}, S:{orangeHSVMax.val[1]:F0}, V:{orangeHSVMax.val[2]:F0})");
        });
        _sliderSatMin.onValueChanged.AddListener(value =>
        {
            orangeHSVMin.val[1] = value;
            _sliderMinText.SetText($"Min (H:{orangeHSVMin.val[0]:F0}, S:{orangeHSVMin.val[1]:F0}, V:{orangeHSVMin.val[2]:F0})");
        });
        _sliderSatMax.onValueChanged.AddListener(value =>
        {
            orangeHSVMax.val[1] = value;
            _sliderMaxText.SetText($"Max (H:{orangeHSVMax.val[0]:F0}, S:{orangeHSVMax.val[1]:F0}, V:{orangeHSVMax.val[2]:F0})");
        });
        _sliderValMin.onValueChanged.AddListener(value =>
        {
            orangeHSVMin.val[2] = value;
            _sliderMinText.SetText($"Min (H:{orangeHSVMin.val[0]:F0}, S:{orangeHSVMin.val[1]:F0}, V:{orangeHSVMin.val[2]:F0})");
        });
        _sliderValMax.onValueChanged.AddListener(value =>
        {
            orangeHSVMax.val[2] = value;
            _sliderMaxText.SetText($"Max (H:{orangeHSVMax.val[0]:F0}, S:{orangeHSVMax.val[1]:F0}, V:{orangeHSVMax.val[2]:F0})");
        });
    }

    private IEnumerator WaitForWebCam()
    {
        while (!_cameraManager.WebCamTexture || !_cameraManager.WebCamTexture.isPlaying)
        {
            yield return null;
        }

        _webcamTexture = _cameraManager.WebCamTexture;
        if (imgMat == null) {
            imgMat = new Mat(_webcamTexture.height, _webcamTexture.width, CvType.CV_8UC4);
            hsvMat = imgMat.clone();
            texture = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);
            texture2 = new Texture2D(imgMat.cols(), imgMat.rows(), TextureFormat.RGBA32, false);
        }
    }

    public void toggleFieldMover() {
        if(activeFieldMover != null)
        {
            Destroy(activeFieldMover);
            activeFieldMover = null;
        }
        else
        {
            activeFieldMover = Instantiate(fieldMoverPrefab, leftEyeAnchor.position + leftEyeAnchor.forward * 0.5f, Quaternion.identity);
            activeFieldMover.GetComponent<Mover>().target = targetField.transform;
            activeFieldMover.GetComponent<Mover>().Init();
            GUI_Spawn_Elements.SetActive(false); // Deactivate GUI when field mover is active
            
            if (UI_Interactors_For_Menu != null)
            {
                UI_Interactors_For_Menu.SetActive(false);
            }
        }
    }

    private void Update()
    {
        countFPS();
        if (isChecking) {
            CheckForBall();
        }
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (!GUI_Spawn_Elements.activeSelf)
            {
                if (UI_Interactors_For_Menu != null)
                {
                    UI_Interactors_For_Menu.SetActive(true);
                }
                
                // Setze das GUI doppelt so weit von der linken Auge-Position entfernt wie der Controller
                GUI_Spawn_Elements.transform.position = leftEyeAnchor.transform.position + leftEyeAnchor.forward * 1f;
                GUI_Spawn_Elements.transform.rotation = Quaternion.LookRotation(GUI_Spawn_Elements.transform.position - leftEyeAnchor.transform.position);
                GUI_Spawn_Elements.SetActive(true);
            }
            else
            {
                if (UI_Interactors_For_Menu != null)
                {
                    UI_Interactors_For_Menu.SetActive(false);
                }
                
                GUI_Spawn_Elements.SetActive(false);
            }
        }
    }

    private void countFPS()
    {
        // FPS-Z�hler
        frameCount++;
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 1f)
        {
            fps = frameCount / elapsedTime;
            _fpsText.text = $"FPS: {fps:F2}";
            frameCount = 0;
            elapsedTime = 0f;
        }
    }

    public void ToggleTracking()
    {
        if (_webcamTexture && _webcamTexture.isPlaying)
        {
            if(isChecking)
            {
                isChecking = false;
            }
            else
            {
                isChecking = true;
            }
            Debug.Log("BallTracker: Tracking started.");
        }
        else
        {
            Debug.LogWarning("BallTracker: Webcam texture not ready.");
        }
    }

    public void toggleBallRendering()
    {
        virtualBall.GetComponent<Renderer>().enabled = !virtualBall.GetComponent<Renderer>().enabled;
    }

    private void CheckForBall()
    {
            if (!_webcamTexture || !_webcamTexture.isPlaying)
            {
                Debug.LogWarning("BallTracker: Webcam texture not ready.");
                 return;
            }
            Utils.webCamTextureToMat(_webcamTexture, imgMat, false, 0);

            Imgproc.cvtColor(imgMat, hsvMat, Imgproc.COLOR_RGB2HSV);
            Core.inRange(hsvMat, orangeHSVMin, orangeHSVMax, hsvMat);

            Imgproc.erode(hsvMat, hsvMat, erodeElement);
            Imgproc.erode(hsvMat, hsvMat, erodeElement);

            Imgproc.dilate(hsvMat, hsvMat, dilateElement);
            Imgproc.dilate(hsvMat, hsvMat, dilateElement);

            // --- NEU: Konturen finden und gr��te Kontur bestimmen ---
            List<MatOfPoint> contours = new List<MatOfPoint>();
            Mat hierarchy = new Mat();
            Imgproc.findContours(hsvMat, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
            Debug.Log($"BallTracker: Found {contours.Count} contours.");
            if (contours.Count > 0)
            {
                // Gr��te Kontur finden
                double maxArea = 0;
                int maxAreaIdx = 0;
                for (int i = 0; i < contours.Count; i++)
                {
                    double area = Imgproc.contourArea(contours[i]);
                    if (area > maxArea)
                    {
                        maxArea = area;
                        maxAreaIdx = i;
                    }
                }
                Debug.Log($"BallTracker: Largest contour area: {maxArea} with id {maxAreaIdx}");

                // Bounding Box berechnen
                OpenCVForUnity.CoreModule.Rect boundingRect = Imgproc.boundingRect(contours[maxAreaIdx]);
                int centerX = boundingRect.x + boundingRect.width / 2;
                int centerY = boundingRect.y + boundingRect.height / 2;
                int width = boundingRect.width;
                int height = boundingRect.height;

                Debug.Log($"Ball Center: ({centerX}, {centerY}), Width: {width}, Height: {height}");
                // Optional: Rechteck im Bild einzeichnen
                Imgproc.rectangle(imgMat, boundingRect, new Scalar(0, 255, 0, 255), 5);

                // 3D -Koordinaten berechnen
                // Step 1: Set up camera parameters for tracking
                // These intrinsic parameters are essential for accurate marker pose estimation
                var intrinsics = PassthroughCameraUtils.GetCameraIntrinsics(_cameraManager.Eye);
                var cx = intrinsics.PrincipalPoint.x;  // Principal point X (optical center)
                var cy = intrinsics.PrincipalPoint.y;  // Principal point Y (optical center)
                var fx = intrinsics.FocalLength.x;     // Focal length X
                var fy = intrinsics.FocalLength.y;     // Focal length Y
                var imgWidth = intrinsics.Resolution.x;   // Image width
                var imgHeight = intrinsics.Resolution.y;  // Image height

                // Use average of width and height as the apparent diameter in pixels
                float d = (width + height) / 2f;

                // Avoid division by zero
                if (d < 1e-5f)
                {
                    Debug.LogWarning("Ball diameter in pixels is too small.");
                    return;
                }

                // Estimate Z distance (depth) from the camera to the ball
                float Z = (fx * realBallSize) / d; // in meters

                // Back-project to 3D coordinates in camera space
                float X = (centerX - cx) * Z / fx;
                //float Y = (v - cy) * Z / fy;
                float Y = ((centerY - cy) * Z / fy) + (yOffset * Z);

                // Create camera-space 3D position
                Vector3 cameraSpaceBallPosition = new Vector3(X, Y, Z); // Y inverted to match Unity's coord system

                // Convert camera space to world space
                Vector3 worldPosition = leftEyeAnchor.TransformPoint(cameraSpaceBallPosition);

                // Move the virtual ball
                virtualBall.transform.position = worldPosition;
            }

            Utils.matToTexture2D(hsvMat, texture, false, 0);
            _rawImage.texture = texture;
            _rawImage.material.mainTexture = _rawImage.texture;
            Utils.matToTexture2D(imgMat, texture2, false, 0);
            _rawImage2.texture = texture2;
            _rawImage2.material.mainTexture = _rawImage2.texture;
        }
}