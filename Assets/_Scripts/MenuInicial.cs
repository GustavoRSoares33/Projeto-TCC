
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuInicial : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private string cenaJogo = "SampleScene";

    [Header("Botões")]
    [SerializeField] private Button botaoContinuar;

    private void Start()
    {
        AtualizarBotaoContinuar();
    }

    private void AtualizarBotaoContinuar()
    {
        if (botaoContinuar != null)
        {
            botaoContinuar.interactable = SistemaSave.TemSave();
        }
    }

    public void NovoJogo()
    {
        SistemaSave.ApagarSave();

        SceneManager.LoadScene(cenaJogo);
    }

    public void Continuar()
    {
        if (SistemaSave.TemSave())
        {
            SceneManager.LoadScene(cenaJogo);
        }
        else
        {
            Debug.LogWarning("Não existe nenhum progresso salvo.");
        }
    }

    public void Sair()
    {
        Debug.Log("Saindo do DebugOS...");

        Application.Quit();
    }
}
