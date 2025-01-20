using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosMessageTypes.Sensor;
using Unity.Robotics.ROSTCPConnector;

public class CameraPublisher : MonoBehaviour
{
    [Header("Unity Publish Setting")]
    [SerializeField]
    private ROSConnection   rosConnection;
    [SerializeField]
    private Camera          sensorCamera;
    [SerializeField]
    private string          topicName;
    [SerializeField]
    private float           publishInterval;

    private float           lastPublishTime = 0f;

    // RenderTexture와 Texture2D를 미리 생성하여 재사용
    private RenderTexture   renderTexture;
    private Texture2D       imageTexture;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if(!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<ImageMsg>(topicName);

        // 렉이 걸려서 해상도를 줄이기(기존의 절반 으로)
        if (sensorCamera == null) return;
        int width = sensorCamera.pixelWidth / 2;
        int height = sensorCamera.pixelHeight / 2;

        renderTexture = new RenderTexture(width, height, 24);
        imageTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
    }

    // 카메라 데이터 발행
    void PublishMsg()
    {
        CaptureCameraImage(sensorCamera);

        // RGB 데이터 가져오기
        Color32[] pixels = imageTexture.GetPixels32();
        byte[] imageData = new byte[pixels.Length * 3]; // RGB는 3바이트

        // 각 픽셀의 RGB 데이터 저장
        for (int i = 0; i < pixels.Length; i++)
        {
            imageData[i * 3] = pixels[i].r;   // Red
            imageData[i * 3 + 1] = pixels[i].g; // Green
            imageData[i * 3 + 2] = pixels[i].b; // Blue
        }

        ImageMsg imageMsg = new ImageMsg();
        imageMsg.data = imageData;
        imageMsg.height = (uint)imageTexture.height;
        imageMsg.width = (uint)imageTexture.width;
        imageMsg.step = (uint)(imageTexture.width * 3); // RGB는 3바이트
        imageMsg.encoding = "rgb8"; // 인코딩을 rgb8로 설정

        // Publish 메시지
        rosConnection.Publish(topicName, imageMsg);
    }

    // 카메라 데이터 추출
    void CaptureCameraImage(Camera camera)
    {
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;
    
        // 기존의 Texture2D를 재사용
        imageTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
    
        // ROS에서 반전이 되기 때문에 상하 반전 처리
        Color32[] pixels = imageTexture.GetPixels32();
        for (int y = 0; y < imageTexture.height / 2; y++)
        {
            for (int x = 0; x < imageTexture.width; x++)
            {
                // 픽셀 스왑
                Color32 temp = pixels[y * imageTexture.width + x];
                pixels[y * imageTexture.width + x] = pixels[(imageTexture.height - 1 - y) * imageTexture.width + x];
                pixels[(imageTexture.height - 1 - y) * imageTexture.width + x] = temp;
            }
        }
        imageTexture.SetPixels32(pixels);
        imageTexture.Apply();
    
        // RenderTexture와 active를 원래 상태로 복원
        camera.targetTexture = null;
        RenderTexture.active = null;
    }

    void FixedUpdate()
    {
        if (rosConnection == null || string.IsNullOrEmpty(topicName) || sensorCamera == null) return;
        if (Time.time - lastPublishTime >= publishInterval)
        {
            PublishMsg();
            lastPublishTime = Time.time;
        }
    }

    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderTexture.Release();
            Destroy(renderTexture);
        }
        if (imageTexture != null)
        {
            Destroy(imageTexture);
        }
    }
}
