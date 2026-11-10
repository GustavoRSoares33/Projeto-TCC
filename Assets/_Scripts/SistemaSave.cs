
using UnityEngine;

public class SistemaSave : MonoBehaviour
{
    public static SistemaSave Instancia;

    private const string ChaveSaveExiste = "DebugOS_SaveExiste";

    private const string ChaveDinheiro = "DebugOS_Dinheiro";
    private const string ChaveXP = "DebugOS_XP";
    private const string ChaveNivel = "DebugOS_Nivel";
    private const string ChaveXPNecessario = "DebugOS_XPNecessario";
    private const string ChaveAumentoXP = "DebugOS_AumentoXP";

    private const string ChaveMissoes = "DebugOS_Missoes";

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Carregar();
    }

    private void OnApplicationPause(bool pausado)
    {
        if (pausado)
            Salvar();
    }

    private void OnApplicationQuit()
    {
        Salvar();
    }

    public static bool TemSave()
    {
        return PlayerPrefs.GetInt(ChaveSaveExiste, 0) == 1;
    }

    public static void Salvar()
    {
        if (EconomiaManager.Instancia == null ||
            SistemaXP.Instancia == null)
        {
            Debug.LogWarning(
                "Não foi possível salvar: EconomiaManager ou SistemaXP não encontrado."
            );
            return;
        }

        PlayerPrefs.SetFloat(
            ChaveDinheiro,
            EconomiaManager.Instancia.saldoAtual
        );

        PlayerPrefs.SetFloat(
            ChaveXP,
            SistemaXP.Instancia.xpAtual
        );

        PlayerPrefs.SetInt(
            ChaveNivel,
            SistemaXP.Instancia.nivelAtual
        );

        PlayerPrefs.SetInt(
            ChaveXPNecessario,
            SistemaXP.Instancia.xpNecessarioParaProximoNivel
        );

        PlayerPrefs.SetInt(
            ChaveAumentoXP,
            SistemaXP.Instancia.aumentoXPPorNivel
        );

        // Salva também o estado do DebugMail.
        if (SistemaMissoes.Instancia != null)
        {
            string estadoMissoes =
                SistemaMissoes.Instancia.ExportarEstado();

            PlayerPrefs.SetString(ChaveMissoes, estadoMissoes);
        }

        PlayerPrefs.SetInt(ChaveSaveExiste, 1);
        PlayerPrefs.Save();

        Debug.Log("Progresso do DebugOS salvo com sucesso!");
    }

    public void Carregar()
    {
        if (!TemSave())
        {
            Debug.Log("Nenhum progresso salvo. Iniciando novo jogo.");
            return;
        }

        if (EconomiaManager.Instancia == null ||
            SistemaXP.Instancia == null)
        {
            Debug.LogError(
                "Não foi possível carregar: EconomiaManager ou SistemaXP não encontrado."
            );
            return;
        }

        EconomiaManager.Instancia.saldoAtual =
            PlayerPrefs.GetFloat(ChaveDinheiro, 0f);

        SistemaXP.Instancia.xpAtual =
            PlayerPrefs.GetFloat(ChaveXP, 0f);

        SistemaXP.Instancia.nivelAtual =
            PlayerPrefs.GetInt(ChaveNivel, 1);

        SistemaXP.Instancia.xpNecessarioParaProximoNivel =
            PlayerPrefs.GetInt(ChaveXPNecessario, 100);

        SistemaXP.Instancia.aumentoXPPorNivel =
            PlayerPrefs.GetInt(ChaveAumentoXP, 25);

        // Restaura o estado das missões.
        if (SistemaMissoes.Instancia != null)
        {
            string estadoMissoes =
                PlayerPrefs.GetString(ChaveMissoes, "");

            if (!string.IsNullOrEmpty(estadoMissoes))
            {
                SistemaMissoes.Instancia.ImportarEstado(estadoMissoes);
            }
        }
        else
        {
            Debug.LogWarning(
                "SistemaMissoes não encontrado. O estado das missões não foi carregado."
            );
        }

        EconomiaManager.Instancia.AtualizarInterface();
        SistemaXP.Instancia.AtualizarInterface();

        // Atualiza a lista visual depois de restaurar as missões.
        QuadroDeContratos quadro =
            FindFirstObjectByType<QuadroDeContratos>();

        if (quadro != null)
            quadro.AtualizarQuadro();

        Debug.Log("Progresso do DebugOS carregado com sucesso!");
    }

    public static void ApagarSave()
    {
        PlayerPrefs.DeleteKey(ChaveDinheiro);
        PlayerPrefs.DeleteKey(ChaveXP);
        PlayerPrefs.DeleteKey(ChaveNivel);
        PlayerPrefs.DeleteKey(ChaveXPNecessario);
        PlayerPrefs.DeleteKey(ChaveAumentoXP);
        PlayerPrefs.DeleteKey(ChaveMissoes);
        PlayerPrefs.DeleteKey(ChaveSaveExiste);

        PlayerPrefs.Save();

        Debug.Log("Progresso anterior apagado. Novo jogo iniciado.");
    }
}
