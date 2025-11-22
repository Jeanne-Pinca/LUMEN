using UnityEngine;
using TMPro; // Required for TextMeshPro

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float destroyTime = 2f;
    public Vector3 offset = new Vector3(0, 2, 0); // Height above the player

    private TextMeshPro textMesh;
    private Color textColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            // Fallback if using UI Text inside a Canvas, 
            // but usually we put this script directly on a World Space TMP object
            textMesh = GetComponentInChildren<TextMeshPro>();
        }
        
        textColor = textMesh.color;
    }

    void Start()
    {
        // Destroy this object automatically after 'destroyTime' seconds
        Destroy(gameObject, destroyTime);
    }

    void Update()
    {
        // 1. Float Upwards
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);

        // 2. Fade Out
        // We calculate how much time is left and fade the alpha
        float fadeSpeed = 1f / destroyTime;
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;
    }

    public void SetText(string message)
    {
        if (textMesh != null)
        {
            textMesh.text = message;
        }
    }
}