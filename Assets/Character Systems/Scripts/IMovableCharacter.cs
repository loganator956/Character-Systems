using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterSystems.Movement
{
    public interface IMovableCharacter
    {
        /// <summary>
        /// Send inputs to the movement component
        /// </summary>
        /// <param name="inputs">X and Y inputs for movement</param>
        public void Move(Vector2 inputs);
        public void SetSprint(bool isSprint);
    } 
}
