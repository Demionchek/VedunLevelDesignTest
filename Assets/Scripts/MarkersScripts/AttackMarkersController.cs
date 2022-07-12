using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.VFX;

public class AttackMarkersController : MonoBehaviour
{
    [SerializeField] private AttackMarkersConfigs _markersConfigs;
    [SerializeField] private LayerMask _targetMask;

    [SerializeField] private float _alfaValue = 0.2f;
    [Header("Marker prefabs")]
    [SerializeField] private GameObject _pondMarkerPrefab;
    [SerializeField] private GameObject _rayMarkerPrefab;
    [SerializeField] private GameObject _coneMarkerPrefab;

    [Header("Boss Effects")]
    [SerializeField] private VisualEffect _yagaSingleConeEffect;
    [SerializeField] private Transform _conesTransform;
    [SerializeField] private VisualEffect[] _yagaMultyConeEffects;
    [SerializeField] private VisualEffect _BigAOEEffect;
    [SerializeField] private GameObject _ClowdEffect;
    [SerializeField] private VisualEffect _rocketsEffect;

    private const float _yPosCorrection = 0.4f;
    private const int _multyConesCount = 3;
    private const float k_Angle = 120f;

    private void Awake()
    {
        LikhoChargingState.CreateMarker += CreateEnemyPondMarker;
        GiantChargeState.CreateMarker += CreateEnemyRayMarker;
    }

    public void CreateRocketMarker(Vector3 pos, float timeToDel)
    => StartCoroutine(RocketsCorutine(pos, timeToDel));
    public void CreateEnemyPondMarker(Vector3 pos, float timeToDel) 
        => StartCoroutine(PondCorutine(pos, timeToDel));
    public void CreateEnemyRayMarker(Vector3 pos, Vector3 target, float timeToDel) 
        => StartCoroutine(RayCorutine(pos, target, timeToDel));
    public void CreateSingeConeMarker(Vector3 pos, Vector3 target, float timeToDel)
        => StartCoroutine(ConeCorutine(pos, target, timeToDel));
    public void CreateMultyCones(Vector3 pos, float timeToDel) 
        => StartCoroutine(MultyConeCorutine(pos, timeToDel));
    public void CreateClowdSpell(ThirdPersonController controller, Vector3 pos)
        => StartCoroutine(ClowdSpellCorutine(controller, pos));
    public void CreateBossPondSpell(Vector3 pos, float timeToDel)
        => StartCoroutine(BossPondCorutine(pos, timeToDel));
    public void CreateHutRaySpell(Vector3 pos, Vector3 target, float timeToDel)
        => StartCoroutine(BossRayCorutine(pos, target, timeToDel));

    private Material MaterialSetAlfa(Material material, Color color)
    {
        Color newAlfa = new Color(0, 0, 0, _alfaValue);
        material.color = color - newAlfa;
        return material;
    }

    private IEnumerator PondCorutine(Vector3 pos, float timeToDel)
    {
        pos.y += _yPosCorrection;
        GameObject pond = Instantiate(_pondMarkerPrefab, pos, Quaternion.identity);
        float delay = timeToDel / 3;
        var pondScript = pond.GetComponent<MarkerDamageScript>();
        var pondMaterial = pond.GetComponent<Renderer>().material;
        pondScript.PondResize(_markersConfigs.jumpMarkerSize);
        MaterialSetAlfa(pondMaterial, Color.green);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(pondMaterial, Color.yellow);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(pondMaterial, Color.red);
        yield return new WaitForSeconds(delay);
        pondScript.TryToHit(_markersConfigs.jumpMarkerDamage, _targetMask);
        Destroy(pond);
    }

    private IEnumerator RocketsCorutine(Vector3 pos, float timeToDel)
    {
        pos.y += _yPosCorrection;
        GameObject pond = Instantiate(_pondMarkerPrefab, pos, Quaternion.identity);

        float delay = timeToDel / 3;
        var pondScript = pond.GetComponent<MarkerDamageScript>();
        var pondMaterial = pond.GetComponent<Renderer>().material;
        pondScript.PondResize(_markersConfigs.jumpMarkerSize);
        MaterialSetAlfa(pondMaterial, Color.green);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(pondMaterial, Color.yellow);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(pondMaterial, Color.red);
        yield return new WaitForSeconds(delay);
        _rocketsEffect.transform.position = pos;
        _rocketsEffect.Play();
        pondScript.TryToHit(_markersConfigs.jumpMarkerDamage, _targetMask);
        Destroy(pond);
    }

    private IEnumerator RayCorutine(Vector3 pos, Vector3 target, float timeToDel)
    {
        pos.y += _yPosCorrection;
        target.y = pos.y;
        GameObject rayMark = Instantiate(_rayMarkerPrefab, pos, Quaternion.identity);
        rayMark.transform.LookAt(target);
        rayMark.transform.Rotate(90, rayMark.transform.rotation.y, rayMark.transform.rotation.z);
        float delay = timeToDel / 3;
        var rayMarkScript = rayMark.GetComponent<MarkerDamageScript>();
        var rayMaterial = rayMark.transform.GetChild(0).GetComponent<Renderer>().material;
        rayMarkScript.RayResize(_markersConfigs.rayMarkerWidth, _markersConfigs.rayMarkerLength);
        rayMaterial.color = Color.green;
        yield return new WaitForSeconds(delay);
        rayMaterial.color = Color.yellow;
        yield return new WaitForSeconds(delay);
        rayMaterial.color = Color.red;
        yield return new WaitForSeconds(delay);
        rayMarkScript.TryToHit(_markersConfigs.rayMarkerDamage, _targetMask);
        Destroy(rayMark);
    }

    private IEnumerator BossRayCorutine(Vector3 pos, Vector3 target, float timeToDel)
    {
        pos.y += _yPosCorrection;
        target.y = pos.y;
        GameObject rayMark = Instantiate(_rayMarkerPrefab, pos, Quaternion.identity);
        rayMark.transform.LookAt(target);
        rayMark.transform.Rotate(90, rayMark.transform.rotation.y, rayMark.transform.rotation.z);
        float delay = timeToDel / 3;
        var rayMarkScript = rayMark.GetComponent<MarkerDamageScript>();
        var rayMaterial = rayMark.transform.GetChild(0).GetComponent<Renderer>().material;
        rayMarkScript.RayResize(_markersConfigs.bossRayMarkerWidth, _markersConfigs.bossRayMarkerLength);
        MaterialSetAlfa(rayMaterial, Color.green);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(rayMaterial, Color.yellow);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(rayMaterial, Color.red);
        yield return new WaitForSeconds(delay);
        rayMarkScript.TryToHit(_markersConfigs.bossRayMarkerDamage, _targetMask);
        Destroy(rayMark);
    }

    private IEnumerator ConeCorutine(Vector3 pos, Vector3 target, float timeToDel)
    {
        GameObject cone = Instantiate(_coneMarkerPrefab, pos, Quaternion.identity);
        target.y = pos.y;
        cone.transform.LookAt(target);
        cone.transform.Rotate(0, 0, 180);
        if (_yagaSingleConeEffect != null)
        {
            pos.y += 1;
            _yagaSingleConeEffect.transform.position = pos;
            target.y = pos.y;
            _yagaSingleConeEffect.transform.LookAt(target);
        }
        var coneScript = cone.GetComponent<MarkerDamageScript>();
        var coneMaterial = cone.GetComponent<Renderer>().material;
        coneScript.ConeResize(_markersConfigs.bossSingleConeMarkerWidth, _markersConfigs.bossSingleConeMarkerLength);
        float delay = timeToDel / 3;
        MaterialSetAlfa(coneMaterial, Color.green);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(coneMaterial, Color.yellow);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(coneMaterial, Color.red);
        yield return new WaitForSeconds(delay);
        _yagaSingleConeEffect.Play();
        coneScript.TryToHit(_markersConfigs.bossConeMarkerDamage, _targetMask);
        Destroy(cone);
    }

    private IEnumerator MultyConeCorutine(Vector3 pos, float timeToDel)
    {
        pos.y -= _yPosCorrection;
        float r = Random.Range(0, k_Angle);
        GameObject[] cones = new GameObject[_multyConesCount];
        MarkerDamageScript[] conesScript = new MarkerDamageScript[_multyConesCount];
        Material[] conesMaterial = new Material[_multyConesCount];
        for (int i = 0; i < cones.Length; i++)
        {
            cones[i] = Instantiate(_coneMarkerPrefab, pos, Quaternion.Euler(0, r, 180));
            conesScript[i] = cones[i].GetComponent<MarkerDamageScript>();
            conesScript[i].ConeResize(_markersConfigs.bossMultyConeMarkerWidth, _markersConfigs.bossMultyConeMarkerLength);
            conesMaterial[i] = cones[i].GetComponent<Renderer>().material;
            MaterialSetAlfa(conesMaterial[i], Color.green);
            r += k_Angle;
        }
        Vector3 currRotation = cones[0].transform.eulerAngles;
        _conesTransform.Rotate(0, currRotation.y,0);
        float delay = timeToDel / 3;
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < cones.Length; i++)
        {
            MaterialSetAlfa(conesMaterial[i], Color.yellow);
        }
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < cones.Length; i++)
        {
            MaterialSetAlfa(conesMaterial[i], Color.red);
        }
        yield return new WaitForSeconds(delay);
        foreach (VisualEffect effect in _yagaMultyConeEffects)
        {
            effect.Play();
        }
        for (int i = 0; i < cones.Length; i++)
        {
            conesScript[i].TryToHit(_markersConfigs.bossConeMarkerDamage, _targetMask);
            Destroy(cones[i]);
        }
        yield return new WaitForSeconds(1f);
        _conesTransform.rotation = Quaternion.Euler(0,0,0);
    }

    private IEnumerator ClowdSpellCorutine(ThirdPersonController controller, Vector3 pos)
    {
        pos.y += 1f;
        GameObject clowd = Instantiate(_ClowdEffect, pos, Quaternion.identity);
        var clowdMarkerScript = clowd.GetComponent<MarkerDamageScript>();
        clowdMarkerScript.PondResize(_markersConfigs.bossClowdMarkerSize);
        var clowdFollow = clowd.GetComponent<ClowdScript>();
        clowdFollow.Position = pos;
        clowdFollow.Controller = controller;
        clowdFollow.Speed = _markersConfigs.bossClowdMarkerSpeed;
        clowdMarkerScript.Slowing(_markersConfigs.bossClowdMarkerSlowing, _markersConfigs.bossClowdEffectTimer);
        Destroy(clowd, _markersConfigs.bossClowdMarkerLifetime);
        yield break;
    }



    private IEnumerator BossPondCorutine(Vector3 pos, float timeToDel)
    {
        GameObject pond = Instantiate(_pondMarkerPrefab, pos, Quaternion.identity);
        float delay = timeToDel / 3;
        var pondScript = pond.GetComponent<MarkerDamageScript>();
        var pondMaterial = pond.GetComponent<Renderer>().material;
        pondScript.PondResize(_markersConfigs.bossPondMarkerSize);
        MaterialSetAlfa(pondMaterial, Color.green);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(pondMaterial, Color.yellow);
        yield return new WaitForSeconds(delay);
        MaterialSetAlfa(pondMaterial, Color.red);
        yield return new WaitForSeconds(delay);
        _BigAOEEffect.transform.position = pos;
        _BigAOEEffect.Play();
        pondScript.TryToHit(_markersConfigs.bossPondMarkerDamage, _targetMask);
        Destroy(pond);
    }


    private void OnDestroy()
    {
        LikhoChargingState.CreateMarker -= CreateEnemyPondMarker;
        GiantChargeState.CreateMarker -= CreateEnemyRayMarker;
    }
}
