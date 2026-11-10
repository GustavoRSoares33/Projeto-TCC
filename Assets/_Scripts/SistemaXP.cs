
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SistemaXP : MonoBehaviour
{
    public static SistemaXP Instancia;

    [Header("Progresso do jogador")]
    [Min(0f)] public float xpAtual = 0f;
    [Min(1)] public int nivelAtual = 1;
    [Min(1)] public int xpNecessarioParaProximoNivel = 100;

    [Header("Configuração de progressão")]
    [Tooltip("Quanto aumenta a exigência de XP a cada nível.")]
    [Min(0)] public int aumentoXPPorNivel = 25;

    [Header("Interface do Desktop")]
    public TMP_Text textoNivel;
    public Slider barraXP;
    public TMP_Text textoProgressoXP;

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        AtualizarInterface();
    }

    public void AdicionarXP(float quantidade)
    {
        if (quantidade <= 0f)
            return;

        xpAtual += quantidade;

        while (xpAtual >= xpNecessarioParaProximoNivel)
        {
            xpAtual -= xpNecessarioParaProximoNivel;
            nivelAtual++;

            xpNecessarioParaProximoNivel += aumentoXPPorNivel;

            Debug.Log($"Parabéns! Você alcançou o nível {nivelAtual}.");
        }

        AtualizarInterface();
        SistemaSave.Salvar();
    }

    public void AtualizarInterface()
    {
        if (textoNivel != null)
            textoNivel.text = $"Nível {nivelAtual}";

        if (barraXP != null)
        {
            barraXP.minValue = 0f;
            barraXP.maxValue = xpNecessarioParaProximoNivel;
            barraXP.value = xpAtual;
        }

        if (textoProgressoXP != null)
        {
            textoProgressoXP.text =
                $"{Mathf.FloorToInt(xpAtual)} / {xpNecessarioParaProximoNivel} XP";
        }
    }
}
