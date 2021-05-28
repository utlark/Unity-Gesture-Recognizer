using UnityEngine;

public class MagicActivator : MonoBehaviour
{
    [Header("Components")]
    public GameObject glyphPrefab;
    public Texture2D[] glyphs;

    [Header("Particles")]
    public ParticleSystem firstGroup;
    public ParticleSystem secondGroup;
    public ParticleSystem thirdGroup;
    public ParticleSystem fourthGroup;
    public ParticleSystem notFind;

    [Header("Parameters")]
    public float shootForce = 1000f;

    private ParticleSystem _currentParticle;

    public void ActivateGlyph(ParticleSystem glyphGroupParticle, float particleStartSize, int glyphNumber)
    {
        _currentParticle = glyphGroupParticle;
        ParticleSystem.MainModule main = _currentParticle.main;
        main.startSize = particleStartSize;
        _currentParticle.gameObject.SetActive(true);

        GameObject glif = Instantiate(glyphPrefab, transform);

        Material material = glif.GetComponent<Renderer>().material;
        material.mainTexture = glyphs[glyphNumber - 1];
        material.SetTexture("_EmissiveColorMap", glyphs[glyphNumber - 1]);

        glif.AddComponent<LifeTime>().lifeTime = 1f;
    }

    public bool RecognizeGlyph(string glyphCode)
    {
        switch (glyphCode)
        {
            case "1":
            case "F1":
                ActivateGlyph(firstGroup, 0.1f, 1);
                return true;
            case "2":
            case "F2":
                ActivateGlyph(firstGroup, 0.25f, 2);
                return true;
            case "3":
            case "F3":
                ActivateGlyph(firstGroup, 0.5f, 3);
                return true;

            case "4":
            case "W1":
                ActivateGlyph(secondGroup, 0.1f, 4);
                return true;
            case "5":
            case "W2":
                ActivateGlyph(secondGroup, 0.25f, 5);
                return true;
            case "6":
            case "W3":
                ActivateGlyph(secondGroup, 0.5f, 6);
                return true;

            case "7":
            case "Wa1":
                ActivateGlyph(thirdGroup, 0.1f, 7);
                return true;
            case "8":
            case "Wa2":
                ActivateGlyph(thirdGroup, 0.25f, 8);
                return true;
            case "9":
            case "Wa3":
                ActivateGlyph(thirdGroup, 0.5f, 9);
                return true;

            case "E1":
                ActivateGlyph(fourthGroup, 0.1f, 10);
                return true;
            case "E2":
                ActivateGlyph(fourthGroup, 0.25f, 11);
                return true;
            case "E3":
                ActivateGlyph(fourthGroup, 0.5f, 12);
                return true;

            default:
                notFind.Play();
                return false;
        }
    }

    public void Shoot()
    {
        GameObject ball = Instantiate(_currentParticle.gameObject, _currentParticle.transform);
        ball.transform.parent = _currentParticle.gameObject.transform.parent;

        ball.AddComponent<Rigidbody>();
        ball.AddComponent<LifeTime>();
        ball.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * shootForce);

        _currentParticle.gameObject.SetActive(false);
        _currentParticle = null;
    }
}
