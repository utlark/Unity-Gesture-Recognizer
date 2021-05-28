using UnityEngine;

public class MagicLight : MonoBehaviour
{
    private Material _material;

    private const string EmissiveColorLDR = "_EmissiveColorLDR";
    private const string EmissiveColor = "_EmissiveColor";
    private const string EmissiveIntensity = "_EmissiveIntensity";
    public void Activate(Color32 newColor, float intensity) 
    {
        _material = gameObject.GetComponent<Renderer>().material;

        _material.SetColor(EmissiveColorLDR, newColor);
        _material.SetFloat(EmissiveIntensity, intensity);

        UpdateEmissive();

        gameObject.GetComponent<Animation>().Play("MagicLightOn");
    }

    public void Deactivate()
    {
        gameObject.GetComponent<Animation>().Play("MagicLightOff");
    }

    private void UpdateEmissive()
    {        
        if (_material.HasProperty(EmissiveColorLDR) && _material.HasProperty(EmissiveIntensity) && _material.HasProperty(EmissiveColor))
        {
            Color emissiveColorLDR = _material.GetColor(EmissiveColorLDR);
            Color emissiveColorLDRLinear = new Color(Mathf.GammaToLinearSpace(emissiveColorLDR.r), Mathf.GammaToLinearSpace(emissiveColorLDR.g), Mathf.GammaToLinearSpace(emissiveColorLDR.b));
            _material.SetColor(EmissiveColor, emissiveColorLDRLinear * _material.GetFloat(EmissiveIntensity));
        }
    }
}
