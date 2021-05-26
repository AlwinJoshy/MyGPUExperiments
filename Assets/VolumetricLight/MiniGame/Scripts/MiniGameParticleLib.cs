using System.Collections.Generic;
using UnityEngine;

    public enum ParticleType
    {
        CannonWaterRipple,
        BoatBlastSmoke,
        GroundHit,
        BoatHit,
        HeartWorldPickUp,
        CoinWorldPickup,
        boatExplossion,
        voiceParticle
    }

public class MiniGameParticleLib : MonoBehaviour
{
    public static MiniGameParticleLib instance;

    public ParticleCollection[] particleCollection;

    Queue<ParticleSpawnMemo> momoCollection = new Queue<ParticleSpawnMemo>();

    private void Start() {
        if(instance != null) Destroy(this);
        else instance = this;
    }

    public void PlayEffect(ParticleType particleID, Vector3 pos, Vector3 dir, bool noRot){
        for (int i = 0; i < particleCollection.Length; i++)
        {
            if(particleCollection[i].typeName != particleID)continue;
            else{

                if(momoCollection.Count < 1){
                    momoCollection.Enqueue(new ParticleSpawnMemo(pos, dir, noRot));
                }

                    ParticleSpawnMemo selectedMemo = momoCollection.Peek();
                    selectedMemo.SetMemo(pos, dir, noRot);
                    momoCollection.Dequeue();


                particleCollection[i].requests.Enqueue(selectedMemo);
                particleCollection[i].inQueue = true;
                return;
            } 
        }
    }

    private void Update() {
        for (int i = 0; i < particleCollection.Length; i++)
        {
            if(particleCollection[i].inQueue){
                particleCollection[i].particleSystem.transform.position = particleCollection[i].requests.Peek().position;
                if(!particleCollection[i].requests.Peek().noRot)particleCollection[i].particleSystem.transform.forward = particleCollection[i].requests.Peek().direction;
                particleCollection[i].particleSystem.Play();
                momoCollection.Enqueue(particleCollection[i].requests.Peek());
                particleCollection[i].requests.Dequeue();
                if(particleCollection[i].requests.Count < 1)particleCollection[i].inQueue = false;
            }
        }
    }

    public class ParticleSpawnMemo{
        public bool noRot;
        public Vector3 position;
        public Vector3 direction;

        public ParticleSpawnMemo(Vector3 memoPos, Vector3 memoDir, bool noRot){
            position = memoPos;
            direction = memoDir;
            this.noRot = noRot;
        }

        public void SetMemo(Vector3 memoPos, Vector3 memoDir, bool noRot){
            position = memoPos;
            direction = memoDir;
            this.noRot = noRot;
        }

    }

[System.Serializable]
    public class ParticleCollection{
        public bool inQueue;
        public ParticleType typeName;
        public ParticleSystem particleSystem;
        public Queue<ParticleSpawnMemo> requests = new Queue<ParticleSpawnMemo>();
    }


}

