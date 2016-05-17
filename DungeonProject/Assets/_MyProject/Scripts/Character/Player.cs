using UnityEngine;
using System.Collections;

public class Player : Character {

	void Update()
	{
		//Debug
		SetState(enum_State.STAY);

		if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.X)){
			SetDirection(enum_Direction.UNDER);
		}else if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)){
			SetDirection(enum_Direction.UP);
		}else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)){
			SetDirection(enum_Direction.RIGHT);
		}else if(Input.GetKeyDown(KeyCode.LeftArrow)||Input.GetKeyDown(KeyCode.A)){
			SetDirection(enum_Direction.LEFT);
		}else if(Input.GetKeyDown(KeyCode.Q)){
			SetDirection(enum_Direction.UPPERLEFT);
		}else if(Input.GetKeyDown(KeyCode.E)){
			SetDirection(enum_Direction.UPPERRIGHT);
		}else if(Input.GetKeyDown(KeyCode.Z)){
			SetDirection(enum_Direction.UNDERLEFT);
		}else if(Input.GetKeyDown(KeyCode.C)){
			SetDirection(enum_Direction.UNDERRIGHT);
		}else{
			//Nothing.
		}

		if(Input.GetKeyDown(KeyCode.Space)){
			SetState(enum_State.MOVE);
		}

		Action();
		//Debug
	}
}
