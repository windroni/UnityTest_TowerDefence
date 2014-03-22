using UnityEngine;
using System.Collections;

public class RoateLookTarget : MonoBehaviour {

	public float rotationSpeed = 3.0f;

	public void RotateToTarget(Transform target)
	{
		if(target == null)
			return;

		Vector3 dir = target.position - transform.position;
		dir.y = 0.0f;
		dir.Normalize();

		Quaternion from = transform.rotation; //벡터에 방향이 있다.
		Quaternion to = Quaternion.LookRotation(dir); // 방향을 주면 해당 방향으로 회전한다. 유니티의 로테이션은 기본적으로 Quaternion(복소수) 타입이다.
		transform.rotation = Quaternion.Lerp(from, to, rotationSpeed * Time.deltaTime); //방향 회전을 부드럽게 보간해 주는 함수(Quaternion.Lerp)
	}
}
