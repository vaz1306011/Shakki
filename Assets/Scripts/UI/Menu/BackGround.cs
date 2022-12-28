using UnityEngine;
using UnityEngine.UI;

public class BackGround : MonoBehaviour
{
    [SerializeField] Vector2 _moveSpeed;

    RawImage _image;
    Texture[] _chessSprite;
    float _rotateSpeed;
    float _roateInterval = 1.65f;
    float _time = 0;

    void Start()
    {
        _image = GetComponent<RawImage>();
        _chessSprite = Resources.LoadAll<Texture>("Sprites/Chesses");
        _rotateSpeed = Random.Range(-180, 180);

        var index = Random.Range(0, _chessSprite.Length);
        _image.texture = _chessSprite[index];
    }

    void Update()
    {
        _image.uvRect = new Rect(_image.uvRect.position + _moveSpeed * Time.deltaTime, _image.uvRect.size);

        _time += Time.deltaTime;
        if (_time >= _roateInterval)
        {
            _time = 0;

            _roateInterval = 3.7f;

            //if (_roateInterval == 3.8f)
            //    _roateInterval = 3.7f;
            //else if (_roateInterval == 3.7f)
            //    _roateInterval = 3.8f;

            _rotateSpeed = Random.Range(-180, 180);
        }
        _image.rectTransform.Rotate(new Vector3(0, 0, _rotateSpeed * Time.deltaTime));
    }
}
