using System.Collections;
using UnityEngine;

public class CtrlHackDeleteExternalCharacterController : MonoBehaviour
{
    [SerializeField] private CutsceneHumanExternalMovement leadCharacter;
    [SerializeField] private Transform leadTarget;

    [System.Serializable]
    public class Follower
    {
        public CutsceneHumanExternalMovement character;
        public Transform target;
    }

    [SerializeField] private Follower[] followers;

    [SerializeField] private CutsceneCameraController cutsceneCameraController; // 🎥 Kamera kontrolcüsü referansı

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        while (!leadCharacter.IsReady()) // IsReady fonksiyonu eklemelisin
        yield return null;
        
        bool leadArrived = false;
        Debug.Log("Enumerator");
        Debug.Log("leadTarget.position:" + leadTarget.position);
        // 1️⃣ Lead karakter yürüsün
        leadCharacter.MoveTo(leadTarget.position, isRunning: false, isSneaking: false, onArrived: () =>
        {
            leadArrived = true;
        });

        yield return new WaitUntil(() => leadArrived);
        Debug.Log("✅ Lead karakter yerine ulaştı");

        // 🎥 Kamera: Lead karakter yaklaşınca cmApproach
        cutsceneCameraController.OnLeadCharacterArrived();

        // 2️⃣ Diğer karakterleri sırayla gönder
        foreach (var f in followers)
        {
            bool followerArrived = false;

            f.character.MoveTo(f.target.position, isRunning: false, isSneaking: false, onArrived: () =>
            {
                followerArrived = true;
            });

            yield return new WaitUntil(() => followerArrived);
        }

        Debug.Log("✅ Tüm karakterler yerine ulaştı");

        // 🎥 Kamera: Herkes hazır, cmButton → 2sn sonra cmWide
        cutsceneCameraController.OnAllCharactersReady();

        // 3️⃣ Lead karakter tuşa bassın
        leadCharacter.GetComponent<Animator>().SetTrigger("triggerInteract");

        // ✨ Tuş animasyonu bitince yazılar başlasın
        FindObjectOfType<FinalSequenceController>().StartHackSequence();
    }
}
