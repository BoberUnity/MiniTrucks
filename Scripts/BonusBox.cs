using UnityEngine;

public class BonusBox : MonoBehaviour
{
  void Start ()
	{
    gameObject.layer = 1 << 1;
	}
}
