using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RouteSystem
{
	public class Route : MonoBehaviour
	{
		public List<Segment> segments = new List<Segment>();
		public GameObject frontTire, rearTire;

		bool isMoving = false;
		public float startSpeed = 0, speed = 0, endSpeed = 0;
		public float estimateTime = 0;
		public float remainTime = 0;
		public float acceleration = 0;
		public float dis = 0;
		public float currentDis = 0;
		public float lerp = 0;
		public int index = 0;
		public Vector3 start, mid, end, startPos;
		public Quaternion startQ, endQ;

		bool hasNextNode
		{
			get
			{
				return (segments.Count > index + 1);
			}
		}

		bool hasNextTwoNode
		{
			get
			{
				return (segments.Count > index + 2);
			}
		}

		bool isEmptyList { get { return segments.Count == 0; } }

		bool endOfList { get { return segments.Count <= index;  } }

		Segment current
		{
			get
			{
				return (isEmptyList) ? null : segments[index];
			}
		}

		Segment next
		{
			get
			{
				return (isEmptyList || !hasNextNode) ? null : segments[index + 1];
			}
		}

		Vector3 currentPos
		{
			get
			{
				return current.pos;
			}
		}
		// Start is called before the first frame update
		void Start()
		{
			StartRouting();
		}

		// Update is called once per frame
		void Update()
		{
			if (isMoving)
			{
				//remainTime -= 0.85f;
				//float diff = estimateTime - remainTime;
				//float ratio = diff / estimateTime;
				//if (ratio < 0.25f)
				//{
				//	float accel = (current.topSpeed - startSpeed) / (estimateTime * 0.25f);
				//	acceleration = accel;
				//	speed += accel;
				//	transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(currentPos - transform.position, transform.up), diff / (estimateTime * 0.25f));
				//} else if (ratio > 0.75f)
				//{
				//	float accel = (endSpeed - current.topSpeed) / (estimateTime * 0.25f);
				//	acceleration = accel;
				//	speed += accel;
				//	Vector3 nextPos = hasNextNode ? next.pos : current.pos;
				//	Vector3 lookat = nextPos - transform.position;
				//	transform.rotation = Quaternion.Lerp(Quaternion.LookRotation(transform.forward, transform.up), Quaternion.LookRotation(lookat, transform.up), (remainTime - estimateTime * 0.75f) / (estimateTime * 0.25f));
				//}
				//transform.position += speed * transform.forward;
				//if (remainTime <= 0)
				//	LoadNextNode();
				speed = current.topSpeed;
				currentDis += speed * Time.deltaTime;
				float lerpValue = currentDis / dis;
				lerp = lerpValue;
				frontTire.transform.Rotate(5 * speed, 0, 0, Space.Self);
				//rearTire.transform.Rotate(Vector3.right * 5 * speed, Space.Self);
				rearTire.transform.Rotate(5 * speed, 0, 0, Space.Self);
				List<Renderer> list = new List<Renderer>(frontTire.GetComponentsInChildren<Renderer>());
				list.ForEach(r => r.material.color = Color.blue);
				if (lerpValue <= 0.334f)
				{
					if (lerpValue < 0.21f)
					{
						// transform.rotation = Quaternion.Lerp(startQ, Quaternion.LookRotation(mid - start), lerpValue / 0.21f);
						transform.rotation = Quaternion.Lerp(startQ, Quaternion.LookRotation(start - transform.position), lerpValue / 0.21f);
						transform.position += transform.forward * speed * Time.deltaTime;
						startPos = transform.position;
					}
					else
					{
						transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mid - start), (lerpValue - 0.21f) / 0.124f);
						transform.position = Vector3.Lerp(startPos, start, (lerpValue - 0.21f) / 0.089f);
					}
				}
				else if (lerpValue <= 0.667f)
				{
					transform.position = Vector3.Lerp(start, mid, (lerpValue - 0.334f) * 3);
					startQ = Quaternion.LookRotation(mid - start);
				}
				else if (lerpValue <= 1)
				{
					if (lerpValue < 0.88f)
					{
						transform.rotation = Quaternion.Lerp(startQ, Quaternion.LookRotation(end - mid), (lerpValue - 0.66f) / 0.22f);
						transform.position += transform.forward * speed * Time.deltaTime;
						startPos = transform.position;
					}
					else
					{
						transform.position = Vector3.Lerp(startPos, end, (lerpValue - .88f) / .119f);
						startQ = Quaternion.LookRotation(transform.forward);
					}
				} else
				{
					LoadNextNode();
				}
			}
		}

		public void StartRouting()
		{
			if (segments.Count > 0)
			{
				startSpeed = 0;
				endSpeed = GetEndSpeed();
				dis = Vector3.Distance(currentPos, transform.position);
				estimateTime = remainTime = dis / ((speed + current.topSpeed) * 2) + dis / current.topSpeed / 2 + dis / endSpeed / 4;
				start = transform.position + (currentPos - transform.position) / 3;
				mid = start + (currentPos - transform.position) / 3;
				if (hasNextNode)
				{
					Vector3 nextStart = (next.pos - currentPos) / 3 + currentPos;
					end = mid + Quaternion.Lerp(Quaternion.LookRotation(currentPos - transform.position), Quaternion.LookRotation(nextStart - mid), 0.5f) * Vector3.forward * Vector3.Distance(nextStart, mid) / 1.5f;
				}
				else
					end = currentPos;
				GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				a.transform.position = start;
				GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				b.transform.position = mid;
				GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				c.transform.position = end;

				startQ = Quaternion.LookRotation(transform.forward);
				isMoving = true;
			}
			else
			{
				isMoving = false;
			}
		}

		float GetEndSpeed()
		{
			float resultSpeed = 0;
			if(hasNextNode)
			{
				if (Vector3.Angle(transform.forward, currentPos - transform.position) > 20)
					resultSpeed = current.topSpeed / 2;
				else
					resultSpeed = (speed + next.topSpeed) / 2;
			}
			return resultSpeed;
		}

		void LoadNextNode ()
		{
			++index;
			currentDis = 0;
			if (!endOfList)
			{
				dis = Vector3.Distance(currentPos, transform.position);
				if (hasNextNode)
				{
					endSpeed = GetEndSpeed();
					estimateTime = remainTime = dis / ((speed + current.topSpeed) * 2) + dis / current.topSpeed / 2 + dis / endSpeed / 4;
					start = segments[index - 1].pos + (currentPos - segments[index - 1].pos) / 3;
					mid = start + (currentPos - transform.position) / 3;
					Vector3 nextStart = (next.pos - currentPos) / 3 + currentPos;
					end = mid + Quaternion.Lerp(Quaternion.LookRotation(currentPos - transform.position), Quaternion.LookRotation(nextStart - mid), 0.5f) * Vector3.forward * Vector3.Distance(nextStart, mid) / 1.5f;
				}
				else
				{
					endSpeed = 0;
					estimateTime = remainTime = dis / ((speed + current.topSpeed) * 2) + dis / current.topSpeed / 2 + dis / current.topSpeed / 4;
					start = segments[index - 1].pos + (currentPos - segments[index - 1].pos) / 3;
					mid = start + (currentPos - transform.position) / 3;
					end = currentPos;
				}
				startSpeed = speed;
			}
			else
				isMoving = false;
			GameObject a = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			a.transform.position = start;
			GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			b.transform.position = mid;
			GameObject c = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			c.transform.position = end;
		}
	}
}
