using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable
{
    void Hover();
    void UnHover();
    void Construct(int Building_ID);
    void Deconstruct();
}