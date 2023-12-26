using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    [SerializeField] private MeshRenderer _bgRenderer;
    [SerializeField] private float _speed = 0.01f;

    private Vector2 _startOffset;
    private int _mainTextureId = Shader.PropertyToID("_MainTex");
    private float _tempYOffset;

    void Start()
    {
        _startOffset = _bgRenderer.sharedMaterial.GetTextureOffset(_mainTextureId);

    }

    void Update()
    {
        _tempYOffset = Mathf.Repeat(_tempYOffset + Time.deltaTime * _speed, 1);
        Vector2 offset = new Vector2(_startOffset.x, _tempYOffset);
        _bgRenderer.sharedMaterial.SetTextureOffset(_mainTextureId, offset);
    }
}
