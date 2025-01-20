using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PathTextureSaver : MonoBehaviour
{
    [Header("PathTextrure")]
    [SerializeField]
    private RenderTexture pathTexture;
    [SerializeField]
    private string fileName = Application.dataPath + "/pathtexture.png";

    // ���� �ؽ��� PNG�� ����
    void SaveRenderTextureToPNG(RenderTexture renderTexture, string filePath)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // RenderTexture�� �о�ͼ� Texture2D�� ����
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // PNG �������� ���ڵ�
        byte[] pngData = texture.EncodeToPNG();

        // ���Ϸ� ����
        File.WriteAllBytes(filePath, pngData);
        // Debug.Log(filePath);
        
        // �ڿ� ����
        RenderTexture.active = null;
        Destroy(texture);
    }

    // ���α׷��� �������ᰡ �ɶ� Path Line UI���� ���̴� �ؽ��ĸ� ����
    private void OnApplicationQuit()
    {
        SaveRenderTextureToPNG(pathTexture, fileName);
    }
}
