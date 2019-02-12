using System.Collections.Generic;
using UnityEngine;

//ScriptableObjectを継承したクラス
[CreateAssetMenu(menuName = "MyBoid/ParamList")]
public class ParamList : ScriptableObject
{
    public List<BoidParam> BoidParamList = new List<BoidParam>();
}

//System.Serializableを設定しないと、データを保持できない(シリアライズできない)ので注意
[System.Serializable]
public class BoidParam
{
    public string Name = "なまえ";
    public float initSpeed = 2f;
    public float minSpeed = 2f;
    public float maxSpeed = 5f;
    public float neighborDistance = 1f;
    public float neighborFov = 90f;
    public float separationWeight = 5f;
    public float wallScale = 5f;
    public float wallDistance = 3f;
    public float wallWeight = 1f;
    public float alignmentWeight = 2f;
    public float cohesionWeight = 3f;

}