// using UnityEngine;
// using UnityEngine.Serialization;
// #if ENABLE_INPUT_SYSTEM 
// using UnityEngine.InputSystem;
// #endif

// namespace StarterAssets
// {
//     [RequireComponent(typeof(CharacterController))]
// #if ENABLE_INPUT_SYSTEM 
//     [RequireComponent(typeof(PlayerInput))]
// #endif
//     public class ThirdPersonController : MonoBehaviour
//     {
//         // Existing variables remain unchanged
//         [Header("Player")]
//         [Tooltip("Move speed of the character in m/s")]
//         public float MoveSpeed = 2.0f;

//         [Tooltip("Sprint speed of the character in m/s")]
//         public float SprintSpeed = 5.335f;

//         [Tooltip("How fast the character turns to face movement direction")]
//         [Range(0.0f, 0.3f)]
//         public float RotationSmoothTime = 0.12f;

//         [Tooltip("Acceleration and deceleration")]
//         public float SpeedChangeRate = 10.0f;

//         public AudioClip LandingAudioClip;
//         public AudioClip[] FootstepAudioClips;
//         [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

//         [Space(10)]
//         [Tooltip("The height the player can jump")]
//         public float JumpHeight = 1.2f;

//         [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
//         public float Gravity = -15.0f;

//         [Space(10)]
//         [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
//         public float JumpTimeout = 0.50f;

//         [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
//         public float FallTimeout = 0.15f;

//         [Header("Player Grounded")]
//         [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
//         public bool Grounded = true;

//         [Tooltip("Useful for rough ground")]
//         public float GroundedOffset = -0.14f;

//         [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
//         public float GroundedRadius = 0.28f;

//         [Tooltip("What layers the character uses as ground")]
//         public LayerMask GroundLayers;

//         [Header("Cinemachine")]
//         [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
//         public GameObject CinemachineCameraTarget;

//         [Tooltip("How far in degrees can you move the camera up")]
//         public float TopClamp = 70.0f;

//         [Tooltip("How far in degrees can you move the camera down")]
//         public float BottomClamp = -30.0f;

//         [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
//         public float CameraAngleOverride = 0.0f;

//         [Tooltip("For locking the camera position on all axis")]
//         public bool LockCameraPosition = false;

//         // cinemachine
//         private float _cinemachineTargetYaw;
//         private float _cinemachineTargetPitch;

//         // player
//         private float _speed;
//         private float _animationBlend;
//         private float _targetRotation = 0.0f;
//         private float _rotationVelocity;
//         private float _verticalVelocity;
//         private float _terminalVelocity = 53.0f;

//         // timeout deltatime
//         private float _jumpTimeoutDelta;
//         private float _fallTimeoutDelta;

//         // animation IDs
//         private int _animIDSpeed;
//         private int _animIDGrounded;
//         private int _animIDJump;
//         private int _animIDFreeFall;
//         private int _animIDMotionSpeed;
//         private int _aiming;
//         private int _shooting;
//         private int _attacking;

//         public GameObject FakeArrow;
//         public GameObject Arrow;
//         public Transform Arrowpoint;

//         public GameObject maincamera;
//         public GameObject Aimcamera;

//         public GameObject Bow;
//         public GameObject BackBow;
        
//         public GameObject Sowrd;
//         public GameObject BackSowrd;
        
//         bool isBow = false;
//         bool isSword = true;
        

// #if ENABLE_INPUT_SYSTEM 
//         private PlayerInput _playerInput;
// #endif
//         private Animator _animator;
//         private CharacterController _controller;
//         private StarterAssetsInputs _input;
//         private GameObject _mainCamera;

//         private const float _threshold = 0.01f;

//         private bool _hasAnimator;

//         private bool IsCurrentDeviceMouse
//         {
//             get
//             {
// #if ENABLE_INPUT_SYSTEM
//                 return _playerInput.currentControlScheme == "KeyboardMouse";
// #else
//                 return false;
// #endif
//             }
//         }

//         private void Awake()
//         {
//             // get a reference to our main camera
//             if (_mainCamera == null)
//             {
//                 _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
//             }
//         }

//         private void Start()
//         {
//             _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
//             _hasAnimator = TryGetComponent(out _animator);
//             _controller = GetComponent<CharacterController>();
//             _input = GetComponent<StarterAssetsInputs>();
// #if ENABLE_INPUT_SYSTEM 
//             _playerInput = GetComponent<PlayerInput>();
// #else
//             Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
// #endif

//             AssignAnimationIDs();

           
//             _jumpTimeoutDelta = JumpTimeout;
//             _fallTimeoutDelta = FallTimeout;
//         }

//         private void Update()
//         {
//             _hasAnimator = TryGetComponent(out _animator);

//             JumpAndGravity();
//             GroundedCheck();
//             Move();
//             SwitchControl();
//             BowAiming();
//             SwordAttacking();
//         }

//         void SwitchControl()
//         {
//             if (Input.GetKeyDown(KeyCode.F))
//             {
//                 isBow = !isBow; 
//                 isSword = !isBow;

//                 Bow.SetActive(isBow);
//                 BackBow.SetActive(!isBow);
//                 Sowrd.SetActive(isSword);
//                 BackSowrd.SetActive(!isSword);
//             }
//         }

//         public void SwordAttacking()    
//         {
//             if (isSword)
//             {
//                 if (Grounded && !_input.sprint)
//                 {
//                     if (_input.attacking)
//                     {
//                         _animator.SetBool(_attacking, true);
//                     }
//                     else
//                     {
//                         _animator.SetBool(_attacking, false); 
//                     }
//                 }
//             }
//         }
        
//         public void BowAiming()
//         {
//             if (isBow)
//             {
//                 if (Grounded && !_input.sprint) 
//                 {
//                     if (_input.aiming)
//                     {
//                         maincamera.SetActive(false);
//                         Aimcamera.SetActive(true);
//                         _animator.SetBool(_aiming, true);
//                         FakeArrow.SetActive(true);

                 
//                         float targetYaw = _cinemachineTargetYaw; 
//                         float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref _rotationVelocity, RotationSmoothTime);
//                         transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

//                         if (_input.shooting)
//                         {
//                             _animator.SetBool(_shooting, true);
//                             FakeArrow.SetActive(false);
//                         }
//                         else
//                         {   
//                             _animator.SetBool(_shooting, false);
//                         }
//                     }
//                     else
//                     {
//                         maincamera.SetActive(true);
//                         Aimcamera.SetActive(false);
//                         FakeArrow.SetActive(false);
//                         _animator.SetBool(_aiming, false);
//                         _animator.SetBool(_shooting, false);
//                     }
//                 }
//                 else
//                 {
//                     FakeArrow.SetActive(false);
//                     _animator.SetBool(_aiming, false);
//                     _animator.SetBool(_shooting, false);
//                 }
//             }
            
//         }

//         public void ArrowMove()
//         {
//             FakeArrow.SetActive(false);
          
//             GameObject arrow = Instantiate(Arrow, Arrowpoint.position, Quaternion.identity);
           
//             Vector3 shootDirection = Aimcamera.transform.forward;
//             arrow.GetComponent<Rigidbody>().AddForce(shootDirection * 25f, ForceMode.Impulse);
//         }

//         private void LateUpdate()
//         {
//             CameraRotation();
//         }

//         private void AssignAnimationIDs()
//         {
//             _animIDSpeed = Animator.StringToHash("Speed");
//             _animIDGrounded = Animator.StringToHash("Grounded");
//             _animIDJump = Animator.StringToHash("Jump");
//             _animIDFreeFall = Animator.StringToHash("FreeFall");
//             _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
//             // Sowrd
//              _attacking = Animator.StringToHash("Attacking");
          
//             // Archery     
//              _shooting = Animator.StringToHash("Shooting");
//               _aiming = Animator.StringToHash("Aiming");
            
//         }

//         private void GroundedCheck()
//         {
//             Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
//             Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

//             if (_hasAnimator)
//             {
//                 _animator.SetBool(_animIDGrounded, Grounded);
//             }
//         }

//         private void CameraRotation()
//         {
//             if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
//             {
//                 float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

//                 _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
//                 _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
//             }

//             _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
//             _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

//             CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
//         }

//         private void Move()
//         {
//             float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

//             if (_input.move == Vector2.zero) targetSpeed = 0.0f;

//             float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

//             float speedOffset = 0.1f;
//             float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

//             if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
//             {
//                 _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
//                 _speed = Mathf.Round(_speed * 1000f) / 1000f;
//             }
//             else
//             {
//                 _speed = targetSpeed;
//             }

//             _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
//             if (_animationBlend < 0.01f) _animationBlend = 0f;

//             Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

//             // Only rotate the character based on movement input if not aiming
//             if (_input.move != Vector2.zero && !_input.aiming)
//             {
//                 _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
//                 float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
//                 transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
//             }

//             Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

//             _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

//             if (_hasAnimator)
//             {
//                 _animator.SetFloat(_animIDSpeed, _animationBlend);
//                 _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
//             }
//         }

//         private void JumpAndGravity()
//         {
//             if (Grounded)
//             {
//                 _fallTimeoutDelta = FallTimeout;

//                 if (_hasAnimator)
//                 {
//                     _animator.SetBool(_animIDJump, false);
//                     _animator.SetBool(_animIDFreeFall, false);
//                 }

//                 if (_verticalVelocity < 0.0f)
//                 {
//                     _verticalVelocity = -2f;
//                 }

//                 if (_input.jump && _jumpTimeoutDelta <= 0.0f)
//                 {
//                     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

//                     if (_hasAnimator)
//                     {
//                         _animator.SetBool(_animIDJump, true);
//                     }
//                 }

//                 if (_jumpTimeoutDelta >= 0.0f)
//                 {
//                     _jumpTimeoutDelta -= Time.deltaTime;
//                 }
//             }
//             else
//             {
//                 _jumpTimeoutDelta = JumpTimeout;

//                 if (_fallTimeoutDelta >= 0.0f)
//                 {
//                     _fallTimeoutDelta -= Time.deltaTime;
//                 }
//                 else
//                 {
//                     if (_hasAnimator)
//                     {
//                         _animator.SetBool(_animIDFreeFall, true);
//                     }
//                 }

//                 _input.jump = false;
//             }

//             if (_verticalVelocity < _terminalVelocity)
//             {
//                 _verticalVelocity += Gravity * Time.deltaTime;
//             }
//         }

//         private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
//         {
//             if (lfAngle < -360f) lfAngle += 360f;
//             if (lfAngle > 360f) lfAngle -= 360f;
//             return Mathf.Clamp(lfAngle, lfMin, lfMax);
//         }

//         private void OnDrawGizmosSelected()
//         {
//             Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
//             Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

//             if (Grounded) Gizmos.color = transparentGreen;
//             else Gizmos.color = transparentRed;

//             Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
//         }

//         private void OnFootstep(AnimationEvent animationEvent)
//         {
//             if (animationEvent.animatorClipInfo.weight > 0.5f)
//             {
//                 if (FootstepAudioClips.Length > 0)
//                 {
//                     var index = Random.Range(0, FootstepAudioClips.Length);
//                     AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
//                 }
//             }
//         }

//         private void OnLand(AnimationEvent animationEvent)
//         {
//             if (animationEvent.animatorClipInfo.weight > 0.5f)
//             {
//                 AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
//             }
//         }
//     }
// }

using UnityEngine;
using UnityEngine.Serialization;
using TMPro;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        // Existing variables remain unchanged
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 8.0f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        // Speed boost variables
        private bool _isSpeedBoosted = false;
        private float _speedBoostMultiplier = 1.25f;
        private float _speedBoostDuration = 60f;
        private float _speedBoostTimer;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        public TextMeshProUGUI countCoinsText;
        public TextMeshProUGUI healthText;
        public TextMeshProUGUI speedBoostText;

        private int countCoins;
        private int health;
        private bool speedBoostCheck;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _aiming;
        private int _shooting;
        private int _attacking;

        public GameObject FakeArrow;
        public GameObject Arrow;
        public Transform Arrowpoint;

        public GameObject maincamera;
        public GameObject Aimcamera;

        public GameObject Bow;
        public GameObject BackBow;
        
        public GameObject Sowrd;
        public GameObject BackSowrd;

        // Added for PlayerShooting functionality
        public UIManager uiManager;
        private LineRenderer lineRenderer;

        bool isBow = false;
        bool isSword = true;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            countCoins = 0; 
            SetCountText();
            health = 50;
            SetHealthText();
            speedBoostCheck = false;
            SetSpeedText();
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            // Initialize LineRenderer for shooting visualization
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Standard"));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.enabled = false;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            SwitchControl();
            BowAiming();
            SwordAttacking();

            if (_isSpeedBoosted)
            {
                _speedBoostTimer -= Time.deltaTime;
                SetSpeedText();
                if (_speedBoostTimer <= 0)
                {
                    _isSpeedBoosted = false;
                    MoveSpeed = 4.0f;
                    SprintSpeed = 8.0f;
                    speedBoostCheck = false;
                    SetSpeedText();
                }
            }

            // Update LineRenderer position
            if (lineRenderer.enabled)
            {
                lineRenderer.SetPosition(0, Arrowpoint.position);
            }
        }

        void SwitchControl()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isBow = !isBow; 
                isSword = !isBow;

                Bow.SetActive(isBow);
                BackBow.SetActive(!isBow);
                Sowrd.SetActive(isSword);
                BackSowrd.SetActive(!isSword);
            }
        }

        public void SwordAttacking()    
        {
            if (isSword)
            {
                if (Grounded && !_input.sprint)
                {
                    if (_input.attacking)
                    {
                        _animator.SetBool(_attacking, true);
                    }
                    else
                    {
                        _animator.SetBool(_attacking, false); 
                    }
                }
            }
        }
        
        public void BowAiming()
        {
            if (!isBow) return;

            if (Grounded && !_input.sprint)
            {
                if (_input.aiming)
                {
                    maincamera.SetActive(false);
                    Aimcamera.SetActive(true);
                    _animator.SetBool(_aiming, true);
                    FakeArrow.SetActive(true);

                    // Continuously align character to Aimcamera Yaw
                    float targetYaw = Aimcamera.transform.rotation.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYaw, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

                    // Dynamic movement relative to Aimcamera
                    Vector3 forward = Aimcamera.transform.forward;
                    Vector3 right = Aimcamera.transform.right;
                    forward.y = 0f;
                    right.y = 0f;
                    forward.Normalize();
                    right.Normalize();

                    Vector3 move = forward * _input.move.y + right * _input.move.x;
                    _controller.Move(move * MoveSpeed * Time.deltaTime);

                    if (_input.shooting)
                    {
                        _animator.SetBool(_shooting, true);
                        FakeArrow.SetActive(false);
                    }
                    else
                    {
                        _animator.SetBool(_shooting, false);
                    }
                }
                else
                {
                    maincamera.SetActive(true);
                    Aimcamera.SetActive(false);
                    FakeArrow.SetActive(false);
                    _animator.SetBool(_aiming, false);
                    _animator.SetBool(_shooting, false);
                }
            }
            else
            {
                FakeArrow.SetActive(false);
                _animator.SetBool(_aiming, false);
                _animator.SetBool(_shooting, false);
            }
        }

        public void ArrowMove()
        {
            FakeArrow.SetActive(false);
          
            GameObject arrow = Instantiate(Arrow, Arrowpoint.position, Quaternion.identity);
            Vector3 shootDirection = Aimcamera.transform.forward;
            arrow.GetComponent<Rigidbody>().AddForce(shootDirection * 25f, ForceMode.Impulse);

            // Raycasting for hit detection
            Ray ray = new Ray(Arrowpoint.position, shootDirection);
            RaycastHit hit;

            // Visualize the shot with a red line
            lineRenderer.SetPosition(0, Arrowpoint.position);
            lineRenderer.SetPosition(1, Arrowpoint.position + shootDirection * 100f);
            lineRenderer.enabled = true;
            Invoke("HideLine", 0.1f);

            // Check for hits
            if (Physics.Raycast(ray, out hit))
            {
                EnemyController enemy = hit.collider.GetComponent<EnemyController>();
                if (enemy != null && !enemy.IsHit())
                {
                    uiManager.UpdateMessage($"Arrow Hit: {hit.collider.gameObject.name}", Color.green);
                    enemy.Hit();
                    uiManager.UpdateScore();
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    if (gameManager != null)
                    {
                        gameManager.EnemyHit();
                    }
                }
                else
                {
                    uiManager.UpdateMessage("Arrow Miss", Color.blue);
                }
            }
            else
            {
                uiManager.UpdateMessage("Arrow Miss", Color.blue);
            }
        }

        private void HideLine()
        {
            lineRenderer.enabled = false;
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("Speed")) 
            {
                StartCoroutine(SpeedBoost());
                Destroy(other.gameObject);
                speedBoostCheck = true;
                SetSpeedText();
            }
            if(other.gameObject.CompareTag("Coin"))
            {
                Destroy(other.gameObject);
                countCoins = countCoins + 1;
                SetCountText();
            }
            if(other.gameObject.CompareTag("Health"))
            {
                if(health <= 50)
                {
                    health = health + 50;
                    SetHealthText();
                }
                if(health > 50)
                {
                    health = health + (100-health);
                    SetHealthText();
                }
                Destroy(other.gameObject);
            }
        }

        private IEnumerator SpeedBoost()
        {
            _isSpeedBoosted = true;
            speedBoostCheck = true;
            if(MoveSpeed == 4.0f && SprintSpeed == 8.0f)
            {
                MoveSpeed *= _speedBoostMultiplier;
                SprintSpeed *= _speedBoostMultiplier;
            }
        
            _speedBoostTimer = _speedBoostDuration;
            yield return new WaitForSeconds(_speedBoostDuration);
        }

        void SetCountText() 
        {
            countCoinsText.text = countCoins.ToString();
        }

        void SetHealthText() 
        {
            if(health >= 75)
            {
                healthText.color = Color.green;
                healthText.text = health.ToString() + "%";
            }
            if(health < 50)
            {
                healthText.color = Color.red;
                healthText.text = health.ToString() + "%";
            }
            if(health >= 50 && health < 75)
            {
                healthText.color = Color.yellow;
                healthText.text = health.ToString() + "%";
            }
        }

        void SetSpeedText() 
        {
            if(speedBoostCheck)
            {
                speedBoostText.text = "Active (" + _speedBoostTimer.ToString("0") + ")";
                speedBoostText.color = Color.green;
            }
            else
            {
                speedBoostText.text = "Inactive";
                speedBoostText.color = Color.red;
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _attacking = Animator.StringToHash("Attacking");
            _shooting = Animator.StringToHash("Shooting");
            _aiming = Animator.StringToHash("Aiming");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (_input.aiming && isBow)
                return;
            
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            if (_input.move != Vector2.zero && !_input.aiming)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}