using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterJump : MonoBehaviour 
{
    [SerializeField] private AnimationCurve _jumpCurve;    
    [SerializeField] private float _startY = 0.05f;
    [SerializeField] private float _jumpHeight = 10f;
    [SerializeField] private float _speedJump = 1f;
    private float _iteration = 0f;
    private GameObject _temp; // пустой gameobject для передачи transform 
    private int _countOfJumps = 0;
    private Rigidbody _rig;
    private Transform _obstaclePosStepBehind;

    private void Reset() 
    {
        _rig = GetComponent<Rigidbody>();
        _rig.freezeRotation = true;
        _rig.useGravity = false;
    }

    private void Awake() 
    {
        _temp = new GameObject();
        _obstaclePosStepBehind = _temp.transform;
    }

    private void Update() 
    {
        var pos = transform.localPosition;
        pos.y = _startY + _jumpCurve.Evaluate(_iteration) * _jumpHeight;

        transform.localPosition = pos;
        _iteration += Time.deltaTime * _speedJump;

        if(_iteration < 1f) return;
        _iteration = 0f;

        //препятствия будут генерироваться в точке приземления через 3 прыжка 
        _countOfJumps+=1;

        if(_countOfJumps % 3 != 0)
        {   
            _obstaclePosStepBehind.position = transform.position;         
            if (Physics.Raycast(transform.position, -transform.up, out var hit, 5f))
            { 
                var _normalizedToGround = hit.normal;
                Quaternion toRotation = Quaternion.FromToRotation(transform.up, _normalizedToGround) * transform.rotation;        
                _obstaclePosStepBehind.rotation = toRotation;
            }
        }
        else
        {
            MountainsGenerator.OnGenerateMountine?.Invoke(_obstaclePosStepBehind);
        }
    }
    
}