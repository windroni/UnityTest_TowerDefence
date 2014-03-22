using UnityEngine;
using System.Collections;

// 유니티는 GameObject는 new로 생성할수 있지만 콤포넌트는 안됨.
// gameObject.AddComponent<gameObject>();로 생성한다.
// 
public class EnemyMove : MonoBehaviour {

	public Transform[] path = null;
	public float moveSpeed = 1.0f;
	public float rotationSpeed = 10.0f;

	int currentPathIndex = 0;
	Transform nextDestination = null;

	public Animation anim;
	public float walkAnimationSpeed = 1.0f;

	// Use this for initialization
	void Start () 
	{
		path = MapManager.instance.GetPathArray();
		FindNextDestination();
		if(anim)
		{
			anim["Walk"].speed = walkAnimationSpeed;
			anim.Play("Walk");
		}
	}

	void FindNextDestination()
	{
		if(currentPathIndex < path.Length)
		{
			nextDestination = path[currentPathIndex];
			if(currentPathIndex == 0)
			{
				transform.position = nextDestination.position;
			}

			++currentPathIndex;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	// Update is called once per frame
	void Update () 
	{
		if(nextDestination == null)
			return;

		Vector3 targetPos = nextDestination.position;
		Vector3 myPos = transform.position;
		myPos.y = targetPos.y = 0.0f;

		float distance = (targetPos - myPos).magnitude;
		if(distance < 0.1f)
		{
			FindNextDestination();
			//회전을 여기서 구현하면 부드러운 회전이 안된다.
			// 이쪽 루틴은 목적지 도착해서 한번만 들어오기 때문에..
		}
		else
		{
			Debug.Log ("Update Path : " + distance);
			Vector3 dir   = nextDestination.position - transform.position;
			dir.y = 0.0f; //y값을 없애버린다.
			dir.Normalize(); //길이가 1이 된다.(방향이 중요하다. 길이는 필요없을때)
			transform.position += (dir * moveSpeed) * Time.deltaTime;


			Quaternion from = transform.rotation; //벡터에 방향이 있다.
			Quaternion to = Quaternion.LookRotation(dir); // 방향을 주면 해당 방향으로 회전한다. 유니티의 로테이션은 기본적으로 Quaternion(복소수) 타입이다.
			transform.rotation = Quaternion.Lerp(from, to, rotationSpeed * Time.deltaTime); //방향 회전을 부드럽게 보간해 주는 함수(Quaternion.Lerp)

			//	srep : 선형 보간 (구현해 보기) 구면 보간 (방향을 알때 쿼터니언구하기
			// Quaternion.Euler : 축에 대한 각도를 넣으면 새로운  회전 값을 준다. (오일러값을 알때 쿼터니언 구하기)

			// 뒤 돈 상태에서 시야에 보인다 안보인다 체크 계산은 벡터로 계산.(내적(각도를 구할수 있따), 외적(코사인세타값등))

			// 내적 : 두개의 벡터에서 나온값이 0보다 작으면 자신의 시선 방향이 뒤.
			// transform.foward : 시선벡터(z방향) 
			// transform.up : 머리 위 벡터 각은 항상 90도
			// transform.right : 
		}
	}
}
























