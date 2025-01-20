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

    // 렌더 텍스쳐 PNG로 저장
    void SaveRenderTextureToPNG(RenderTexture renderTexture, string filePath)
    {
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        // RenderTexture를 읽어와서 Texture2D에 저장
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        // PNG 형식으로 인코딩
        byte[] pngData = texture.EncodeToPNG();

        // 파일로 저장
        File.WriteAllBytes(filePath, pngData);
        // Debug.Log(filePath);
        
        // 자원 해제
        RenderTexture.active = null;
        Destroy(texture);
    }

    // 프로그램이 실행종료가 될때 Path Line UI에서 보이는 텍스쳐를 저장
    private void OnApplicationQuit()
    {
        SaveRenderTextureToPNG(pathTexture, fileName);
    }
}
