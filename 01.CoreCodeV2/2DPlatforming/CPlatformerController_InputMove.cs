using UnityEngine;
using System.Collections;

public class CPlatformerController_InputMove : CObjectBase {

    [GetComponent]
    protected CPlatformerController _pPlayer = null;

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        MoveCharacter();
        JumpCharacter();
    }

    protected void StopMoveCharacter()
    {
        _pPlayer.DoInputVelocity(Vector2.zero, false);
    }

    protected void MoveCharacter()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _pPlayer.DoInputVelocity(directionalInput, Input.GetKey(KeyCode.LeftShift));
    }

    protected void JumpCharacter()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _pPlayer.DoJumpInputDown();

        if (Input.GetKeyUp(KeyCode.Space))
            _pPlayer.DoJumpInputUp();
    }
}
