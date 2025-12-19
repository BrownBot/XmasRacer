using UnityEngine;
using UnityEngine.InputSystem;

public class KartController : MonoBehaviour
{
    [SerializeField] private float _acelerationForce = 10f;
    [SerializeField] private float _turnForce = 5f;
    [SerializeField] private MeshRenderer _thrust;
    [SerializeField] private MeshRenderer _left;
    [SerializeField] private MeshRenderer _right;
    
    private PlayerInput _playerInput;
    private Rigidbody _rigidbody;
    
    private Vector2 _moveInput;
    private bool _isAccelerating;
    private InputAction _moveAction;
    private InputAction _accelerateAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _accelerateAction = _playerInput.actions["Accelerate"];
        _rigidbody = GetComponent<Rigidbody>();
        foreach (var wheelCollider in GetComponentsInChildren<WheelCollider>())
        {
            wheelCollider.motorTorque = 0.00001f; // Prevents wheels from freezing when idle
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReadInputs();
    }
    
    void LateUpdate()
    {
        ApplyMovement();
    }
    
   
    private void ReadInputs()
    {
        // Read Move axis (Vector2)
        _moveInput = _moveAction.ReadValue<Vector2>();
        if (_moveInput.x == 0)
        {
            _left.material.color = Color.white;
            _right.material.color = Color.white;
        }
        else if (_moveInput.x < 0)
        {
            _left.material.color = Color.yellow;
            _right.material.color = Color.white;
        }
        else
        {
            _left.material.color = Color.white;
            _right.material.color = Color.yellow;
        }
        
        // Read Accelerate button
        _isAccelerating = _accelerateAction.IsPressed();
        _thrust.material.color = _isAccelerating ? Color.red : Color.white;
    }
    
    private void ApplyMovement()
    {
        // Apply forward acceleration force when accelerate button is pressed
        if (_isAccelerating)
        {
            _rigidbody.AddForce(transform.forward * _acelerationForce, ForceMode.Impulse);
        }
        
        // Apply torque force on local Y-axis for rotation based on horizontal input
        if (Mathf.Abs(_moveInput.x) > 0.01f)
        {
            float turnTorque = _moveInput.x * _turnForce;
            _rigidbody.AddRelativeTorque(0f, turnTorque, 0f, ForceMode.Impulse);
        }
    }
}
