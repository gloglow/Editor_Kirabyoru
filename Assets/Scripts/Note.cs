using UnityEngine;
using UnityEngine.Pool;

public class Note : MonoBehaviour
{
    public IObjectPool<GameObject> notePool { get; set; }　//　ノーツのオブジェクトプール

    public Player player;

    // ノーツの移動に関与する変数
    public Vector3 dirVec;　//　方向ベクトル
    private Vector3 initialPos; // ノーツが生成された位置
    public float initialTime; // ノーツが生成された時間を記憶
    private Vector3 destination; // ノーツが到達するポイント（判定線の上）

    public float speed = 2; //　ノーツのスピード。

    public int status;　//　ノーツの作動を制御。0 : idle, 1 : 到達ポイントを把握, 2 : 移動
    
    public int index;　//　何番目のノーツなのか

    private void Update()
    {
        switch (status)
        {
            case 0:
                break;
            case 1:　//　移動に必要な変数を初期化、到達ポイントを把握
                initialTime = (float)AudioSettings.dspTime;　//　生成された時の時間を初期時間として記録
                initialPos = transform.position; // 生成された時の位置を初期位置として記録
                GetDestination();　//　到達ポイントを把握
                status = 2;
                break;

            case 2: // 移動
                Vector3 route = destination - initialPos;　//　移動する経路
                float timePassed = ((float)AudioSettings.dspTime - initialTime) / (player.secondPerBeat * speed);　//　どれぐらい時間が経ったか（生成された時は０、到達ポイントに届く時は１）

                transform.position = initialPos + route * timePassed;
                break;
        }
    }

    private void GetDestination()　//　方向ベクトルの方向にrayを描き、到達ポイントを把握
    {
        RaycastHit rayHit;
        int layerMask = (1 << 7); // 判定線のpartのレイヤー

        if (Physics.Raycast(transform.position, dirVec, out rayHit, Mathf.Infinity, layerMask))
        {
            destination = rayHit.point; // rayに打たれたポイントを到達ポイントとして記録
        }
    }

    public void Exit()
    {
        //　ノーツを非活性化
        status = 0;
        notePool.Release(gameObject);
    }
}