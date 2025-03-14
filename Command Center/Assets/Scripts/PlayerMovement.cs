using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform _playerParent;
    [SerializeField] private Transform _playerCharacter;
    [SerializeField] private Rigidbody _playerRb;
    
    private float speed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        _playerRb.MovePosition(_playerParent.position + movement * speed * Time.deltaTime);

        // Looking
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 dir = hit.point - _playerCharacter.position;
            dir.y = 0;

            if (dir.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                _playerCharacter.rotation = Quaternion.RotateTowards(_playerCharacter.rotation, targetRotation, 500 * Time.deltaTime);
            }
        }

        // Sprinting
        if (Input.GetButton("Sprint"))
        {
            speed = _runSpeed;
        }
        else
        {
            speed = _walkSpeed;
        }
    }
}
