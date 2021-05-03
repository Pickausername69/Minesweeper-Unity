using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A guessor choose
/// </summary>
public interface IGuessor
{
    Vector2Int Guess();
}
public enum GuessorType 
{
    Random,
    Corner,
}
