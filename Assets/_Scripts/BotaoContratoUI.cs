using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BotaoContratoUI : MonoBehaviour
{
    [Header("Componentes Visuais do Botão")]
    public TMP_Text textoTitulo;
    public TMP_Text textoDificuldade;
    public TMP_Text textoRecompensa;
    public TMP_Text textoXp;
    public Button botaoAceitar;

    private Contrato contratoDesteBotao;
    private GerenciadorLogica gerenciador;

    

    // Essa função é chamada pelo Quadro quando o botão é gerado na tela
    public void ConfigurarBotao(Contrato contratoSorteado, GerenciadorLogica refGerenciador)
    {
        contratoDesteBotao = contratoSorteado;
        gerenciador = refGerenciador;

        // Preenche o visual
        textoTitulo.text = contratoDesteBotao.tituloDaMissao;
        textoDificuldade.text = $"Nível: {contratoDesteBotao.nivelDificuldade}";
        textoRecompensa.text = $"R$ {contratoDesteBotao.pagamento}";
        textoXp.text = $"XP: {contratoDesteBotao.xpGanho}";

        // Configura o clique via código (não precisa mais arrastar no Inspector!)
        botaoAceitar.onClick.RemoveAllListeners();
        botaoAceitar.onClick.AddListener(AoClicarNoContrato);
    }

    private void AoClicarNoContrato()
    {
        // Envia o contrato selecionado e o próprio objeto do botão para o gerente
        gerenciador.AceitarContrato(contratoDesteBotao, this.gameObject);
    }
}