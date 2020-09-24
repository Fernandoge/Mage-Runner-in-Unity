using System.Collections;
using UnityEngine;

public class ManaController : MonoBehaviour
{
    [SerializeField] private Transform _manaBarHolder;
    [SerializeField] private Transform _manaBarBackground;
    private float _currentMana;
    private int _totalMana;

    public float currentMana => _currentMana;
    
    public void Initialize(int playerMana)
    {
        _totalMana = playerMana;
        _currentMana = _totalMana;
    }

    public void UpdateMana(int value)
    {
        _currentMana = _currentMana + value > 100 ? 100 : _currentMana + value;
        float barValue = _currentMana / _totalMana;
        _manaBarHolder.localScale = new Vector3(barValue, _manaBarHolder.localScale.y, _manaBarHolder.localScale.z);
    }

    public void NoManaFeedback() => StartCoroutine(BlinkManaBar());

    private IEnumerator BlinkManaBar()
    {
        var seconds = new WaitForSeconds(0.1f);
        for (int i = 0; i <= 5; i++)
        {
            _manaBarBackground.gameObject.SetActive(i % 2 == 0);
            yield return seconds;
        }

        _manaBarBackground.gameObject.SetActive(true);
    }
}
