using UnityEngine;

// Isso permite criar os contratos clicando com o botão direito na Unity!
[CreateAssetMenu(fileName = "Novo Contrato", menuName = "Tycoon/Contrato de Lógica")]
public class Contrato : ScriptableObject
{
    public enum TipoDeMissao { PreencherLacuna, EncontrarBug }
    public enum Dificuldade { Estagiario, Junior, Pleno, Senior }

    [Header("Informações Básicas")]
    public string tituloDaMissao;
    [TextArea] public string descricao;
    public Dificuldade nivelDificuldade;
    public TipoDeMissao tipoDaMissao;

    [Header("Recompensas")]
    public float pagamento;
    public int xpGanho; // Opcional, se tiver sistema de nível

    [System.Serializable] // Isso faz a Unity exibir essa caixinha no Inspector
    public struct LinhaDeContrato
    {
        [TextArea(1, 3)]
        public string[] fragmentos; // Os pedaços de texto DESTA linha
        public string[] respostasIDs; // Os IDs dos buracos DESTA linha
    }

    [Header("Código Fonte do Desafio")]
    public LinhaDeContrato[] linhasDoCodigo; // O código completo é uma lista de linhas!
}