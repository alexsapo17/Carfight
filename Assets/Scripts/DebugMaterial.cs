using UnityEngine;

public class DebugMaterial : MonoBehaviour
{
    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            Debug.Log("Material Name: " + mat.name);
            Debug.Log("Shader Name: " + mat.shader.name);
            foreach (var texturePropertyName in mat.GetTexturePropertyNames())
            {
                Texture tex = mat.GetTexture(texturePropertyName);
                Debug.Log("Texture Property: " + texturePropertyName + ", Texture Name: " + (tex != null ? tex.name : "NULL"));
            }
        }
    }
}
