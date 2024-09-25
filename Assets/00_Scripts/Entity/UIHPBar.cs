// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using Unity.Netcode;

// public class UIHPBar : NetworkSingleton<UIHPBar>
// {
//     [SerializeField] private Slider fillHPBar;
//     [SerializeField] private TextMeshProUGUI hpValueText;

//     [ServerRpc(RequireOwnership = false)]
//     public void SetHP_ServerRpc(ulong clientId)
//     {
//         GameObject playerGo = PlayerManager.Instance.PlayerGameObjects[clientId];
//         PlayerHealth playerHealth = playerGo.GetComponent<PlayerHealth>();
//         int currentHp = (int)playerHealth.CurrentHealth;
//         SetHP_ClientRpc(currentHp, playerHealth.GetComponent<PlayerController>().PlayerCharacterData.GetMaxHp(), clientId);
//     }

//     [ClientRpc]
//     public void SetHP_ClientRpc(float currentHealth, float maxHealth, ulong clientId)
//     {
//         if (NetworkManager.LocalClientId != clientId) return;
//         fillHPBar.maxValue = maxHealth;
//         fillHPBar.value = currentHealth;
//         hpValueText.SetText(currentHealth + " / " + (int)maxHealth);
//     }

//     public override void OnNetworkSpawn()
//     {
//         if (IsServer)
//         {
//             PlayerManager.Instance.OnAfterClientConnect += SetHP_ServerRpc;
//         }
//     }
//     public override void OnNetworkDespawn()
//     {
//         if (IsServer)
//         {
//             PlayerManager.Instance.OnAfterClientConnect -= SetHP_ServerRpc;
//         }
//     }


//     protected override void InitAfterAwake()
//     {
//     }
//     // private void OnEnable() {
//     //     GameController.OnBeforeStart += SetHPDefault;
//     //     GameController.WhileInGame += ShowHPValue;
//     // }
//     // private void OnDisable() {
//     //     GameController.OnBeforeStart -= SetHPDefault;
//     //     GameController.WhileInGame -= ShowHPValue;
//     // }
// }
