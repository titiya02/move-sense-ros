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

    // RenderTexture�� Texture2D�� �̸� �����Ͽ� ����
    private RenderTexture   renderTexture;
    private Texture2D       imageTexture;

    void Awake()
    {
        rosConnection = GetComponentInParent<ROSConnection>();
        if(!string.IsNullOrEmpty(topicName))
            rosConnection.RegisterPublisher<ImageMsg>(topicName);

        // ���� �ɷ��� �ػ󵵸� ���̱�(������ ���� ����)
        if (sensorCamera == null) return;
        int width = sensorCamera.pixelWidth / 2;
        int height = sensorCamera.pixelHeight / 2;

        renderTexture = new RenderTexture(width, height, 24);
        imageTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
    }

    // ī�޶� ������ ����
    void PublishMsg()
    {
        CaptureCameraImage(sensorCamera);

        // RGB ������ ��������
        Color32[] pixels = imageTexture.GetPixels32();
        byte[] imageData = new byte[pixels.Length * 3]; // RGB�� 3����Ʈ

        // �� �ȼ��� RGB ������ ����
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
        imageMsg.step = (uint)(imageTexture.width * 3); // RGB�� 3����Ʈ
        imageMsg.encoding = "rgb8"; // ���ڵ��� rgb8�� ����

        // Publish �޽���
        rosConnection.Publish(topicName, imageMsg);
    }

    // ī�޶� ������ ����
    void CaptureCameraImage(Camera camera)
    {
        camera.targetTexture = renderTexture;
        camera.Render();
        RenderTexture.active = renderTexture;
    
        // ������ Texture2D�� ����
        imageTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
    
        // ROS���� ������ �Ǳ� ������ ���� ���� ó��
        Color32[] pixels = imageTexture.GetPixels32();
        for (int y = 0; y < imageTexture.height / 2; y++)
        {
            for (int x = 0; x < imageTexture.width; x++)
            {
                // �ȼ� ����
                Color32 temp = pixels[y * imageTexture.width + x];
                pixels[y * imageTexture.width + x] = pixels[(imageTexture.height - 1 - y) * imageTexture.width + x];
                pixels[(imageTexture.height - 1 - y) * imageTexture.width + x] = temp;
            }
        }
        imageTexture.SetPixels32(pixels);
        imageTexture.Apply();
    
        // RenderTexture�� active�� ���� ���·� ����
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
