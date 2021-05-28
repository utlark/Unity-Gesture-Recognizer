using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineDrawer))]
[RequireComponent(typeof(QDollar))]
[RequireComponent(typeof(MagicActivator))]
[RequireComponent(typeof(TensorFlowRecognizer))]
public class MagicWand : MonoBehaviour
{
    [Header("Components")]
    public Animation playerController;
    public MagicLight magicLight;

    [Header("Parameters")]
    [Tooltip("The speed of returning the wand to start position.")]
    public float animationSpeed = 5f;    

    [Header("Recognition algorithm")]
    [Tooltip("Slower and training for Digits[1-9].")]
    public bool useTensorFlow = true;
    [Tooltip("Faster and training for runes.")]
    public bool useQDollar = false;

    private bool _canShoot;
    private bool _isDrawMod = false;
    private bool _isDrawing = false;
    
    private QDollar _qDollar;
    private LineDrawer _lineDrawer;
    private Vector3 _wandStartPosition;
    private MagicActivator _magicActivator;
    private TensorFlowRecognizer _tensorFlow;   

    private void Start()
    {
        Cursor.visible = false;

        if (!useQDollar && !useTensorFlow || useQDollar && useTensorFlow)
            Debug.LogException(new Exception("You must choose Tensorflow or QDollar recognition algorithm."));

        _lineDrawer = gameObject.GetComponent<LineDrawer>();
        _magicActivator = gameObject.GetComponent<MagicActivator>();
        _tensorFlow = gameObject.GetComponent<TensorFlowRecognizer>();
        _qDollar = gameObject.GetComponent<QDollar>();
        _wandStartPosition = transform.localPosition;   
    }

    async void Update()
    {
        if (_canShoot && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
        {
            _magicActivator.Shoot();
            _canShoot = false;
        }
        else if (!_isDrawMod && !_isDrawing && Input.GetMouseButtonDown(0))
        {
            StateChange(_isDrawMod);
        }
        else if (_isDrawMod)
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _wandStartPosition.z));

            if (!_isDrawing && Input.GetMouseButtonDown(0))
            {
                _isDrawing = true;
                magicLight.Activate(new Color32(0, 255, 255, 255), 2500);
                _lineDrawer.CreateLine(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _wandStartPosition.z)));
            }
            else if (_isDrawing && Input.GetMouseButton(0))
            {
                _lineDrawer.UpdateLine(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _wandStartPosition.z)));
            }
            else if (_isDrawing && Input.GetMouseButtonUp(0))
            {
                magicLight.Deactivate();

                StateChange(_isDrawMod);
                StartCoroutine(SmoothLocalMove(_wandStartPosition));
                _lineDrawer.DeleteLine();

                if (useTensorFlow)
                {
                    Texture2D texture = await _lineDrawer.GetLineTexture(28, 28);
                    _canShoot = _magicActivator.RecognizeGlyph(await _tensorFlow.Evaluate(texture));
                }
                else if (useQDollar)
                {
                    string answer = await _qDollar.Evaluate(_lineDrawer.GetLinePoints());
                    _canShoot = _magicActivator.RecognizeGlyph(answer);
                }
                _isDrawing = false;
            }
        }
    }

    IEnumerator SmoothLocalMove(Vector3 endPosition)
    {
        while (transform.localPosition != endPosition)
        {
            yield return null;
            float step = animationSpeed * Time.deltaTime;
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, endPosition, step);
        }
    }

    private void StateChange(bool state) 
    {        
        _isDrawMod = !state;

        if (state)
        {
            playerController.Play("DrawModOff");
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            playerController.Play("DrawModOn");
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
