using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour {
    [SerializeField] private Rigidbody rb;
    [SerializeField] private MeshRenderer meshRenderer;

    private Vector2 _movementVector;
    private float _movementX;
    private float _movementY;
    public float speed;

    [SerializeField] private UIDocument uiDocument;
    private Label _labelWin;
    private Label _labelCount;

    [SerializeField] private GameObject pickUpParent;
    private int _count;
    private int _maxCount;

    [SerializeField] private GameObject enemy;
    private Vector3 _enemyStartPos;

    private bool _gameOver;

    private void Start() {
        var root = uiDocument.rootVisualElement;
        _labelCount = root.Q<Label>("labelCount");
        _labelWin = root.Q<Label>("labelWin");
        
        _maxCount = pickUpParent.transform.childCount;
        _count = 0;

        _enemyStartPos = enemy.transform.position;
    }

    private void FixedUpdate() {
        if(_gameOver && meshRenderer.enabled == false) return;

        var movement = new Vector3(_movementX, 0.0f, _movementY);
        rb.AddForce(movement * speed);

        CheckOutOfBounds();
    }

    [UsedImplicitly]
    private void OnMove(InputValue movementValue) {
        _movementVector = movementValue.Get<Vector2>();
        _movementX = _movementVector.x;
        _movementY = _movementVector.y;
    }

    [UsedImplicitly]
    private void OnJump(InputValue value) {
        if(!_gameOver) return;

        rb.isKinematic = false;
        ResetPlayerPos();
        meshRenderer.enabled = true;

        _labelWin.style.visibility = Visibility.Hidden;
        _count = 0;
        SetCountText();

        for(var i = 0; i < pickUpParent.transform.childCount; i++) {
            var pickUp = pickUpParent.transform.GetChild(i).gameObject;
            pickUp.SetActive(true);
        }

        enemy.transform.position = _enemyStartPos;
        enemy.SetActive(true);

        _gameOver = false;
    }

    private void CheckOutOfBounds() {
        if(!(transform.position.y < -5)) return;

        ResetPlayerPos();
    }

    private void SetCountText() {
        _labelCount.text = $" Count: {_count}";
    }

    private void ResetPlayerPos() {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = new Vector3(0f, 0.5f, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            meshRenderer.enabled = false;
            rb.isKinematic = true;
            _labelWin.text = "You Lose!";
            _labelWin.style.visibility = Visibility.Visible;
            _gameOver = true;
            return;
        }

        if(!other.gameObject.CompareTag("PickUp")) return;

        other.gameObject.SetActive(false);
        _count++;
        SetCountText();

        if(_count != _maxCount) return;
        _labelWin.style.visibility = Visibility.Visible;
        enemy.SetActive(false);
        _gameOver = true;
    }
}