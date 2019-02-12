using UnityEngine;
using System.Collections.Generic;//List用
using System.Collections.ObjectModel;//ReadOnlyCollection用

public class Simulation : MonoBehaviour
{
    [SerializeField] int boidCount = 100;

    [SerializeField] GameObject boidPrefab;

    [SerializeField] Param param;

    List<Boid> boids_ = new List<Boid>();
    public ReadOnlyCollection<Boid> boids
    {
        get { return boids_.AsReadOnly(); }
    }

    void AddBoid()
    {
        GameObject go = Instantiate(boidPrefab, Random.insideUnitSphere, Random.rotation) as GameObject;//クローン生成
        go.transform.SetParent(transform);//親子関係の設定
        Boid boid = go.GetComponent<Boid>();
        boid.simulation = this;
        boid.param = param;
        boids_.Add(boid);
    }

    void RemoveBoid()
    {
        if (boids_.Count == 0) return;//リスト空のときはスルー
        var lastIndex = boids_.Count - 1;
        var boid = boids_[lastIndex];
        Destroy(boid.gameObject);//listは削除されてもインスタンスは残り続けるためdelete
        boids_.RemoveAt(lastIndex);

    }

    void Update()
    {
        while (boids_.Count < boidCount)
        {
            AddBoid();//インスペクターから数を増やしたとき
        }
        while (boids_.Count > boidCount)
        {
            RemoveBoid();//インスペクターから数を減らしたとき
        }
    }

    //Gizmosを描画(周囲の四角形)
    //関数というよりも決まった構文
    void OnDrawGizmos()
    {
        if (!param) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one * param.wallScale);
    }
}

