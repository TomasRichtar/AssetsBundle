using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterial : MonoBehaviour
{
    [SerializeField] private Material _objectMaterial;
    [SerializeField] private float _fadeInSpeed =0.2f;
    [SerializeField] private float _fadeOutSpeed = 0.2f;
    [SerializeField] private bool _isFadingIn = false;
    [SerializeField] private bool _isFadingOut = false;
    [SerializeField] private float _currentAlpha;
     private float _startedAlpha;
    private Color _objectColor;
    public bool IsFadingIn { get => _isFadingIn; set => _isFadingIn = value; }
    public bool IsFadingOut { get => _isFadingOut; set => _isFadingOut = value; }
    [SerializeField] private int _myRenderingQue;

    private void Awake()
    {
        _objectMaterial = GetComponent<Renderer>().material;
    }
    private void Start()
    {
        _objectColor = _objectMaterial.color;
        _startedAlpha = _objectMaterial.color.a-0.001f;
        _myRenderingQue = _objectMaterial.renderQueue;
        if (_isFadingIn)
        {
            _objectMaterial.color = new Color(_objectColor.r, _objectColor.g, _objectColor.b,0);
            ChangeRenderMode.ChangeRenderModeMaterial(_objectMaterial, BlendMode.Fade);
        }
        //objectColor = _objectMaterial.color;
    }
    // Update is called once per frame
    void Update()
    {
        _objectColor = _objectMaterial.color;

        _currentAlpha = _objectColor.a;
        if (_isFadingIn)
        {
            FadeIn(_fadeInSpeed);

        }
        if (_isFadingOut)
        {
            FadeOut(_fadeOutSpeed);

        }
    }

    void FadeIn(float fadeTime)
    {

        
        // Calculate the alpha value to fade to.
        float targetAlpha = _startedAlpha;
        float fadeAmount = Mathf.Clamp01(Time.deltaTime / fadeTime);

        float newAlpha = Mathf.Lerp(_currentAlpha, targetAlpha, fadeAmount);

        // Set the color on the material using Color.Lerp().
        _objectMaterial.color = Color.Lerp(_objectColor, new Color(_objectColor.r, _objectColor.g, _objectColor.b, targetAlpha), fadeAmount);
        if (_objectMaterial.color.a >= _startedAlpha-0.001f)
        {
           // _objectMaterial.color =new Color(_objectColor.r, _objectColor.g, _objectColor.b, 1);
            if (_myRenderingQue<2450)
            {
                ChangeRenderMode.ChangeRenderModeMaterial(_objectMaterial, BlendMode.Opaque);
            }
            else
            {
                ChangeRenderMode.ChangeRenderModeMaterial(_objectMaterial, BlendMode.Transparent);
            }


            IsFadingIn = false;
        }
    }
    void FadeOut(float fadeTime)
    {
   
        ChangeRenderMode.ChangeRenderModeMaterial(_objectMaterial,BlendMode.Fade);
        // Calculate the alpha value to fade to.
        float targetAlpha = 0f;
        float fadeAmount = Mathf.Clamp01(Time.deltaTime / fadeTime);
        
        float newAlpha = Mathf.Lerp(_currentAlpha, targetAlpha, fadeAmount);

        // Set the color on the material using Color.Lerp().
        _objectMaterial.color = Color.Lerp(_objectColor, new Color(_objectColor.r, _objectColor.g, _objectColor.b, targetAlpha), fadeAmount);
        if (_currentAlpha <= 0.0015f)
        {
            IsFadingOut = false;
            gameObject.SetActive(false);
        }
    }

}
