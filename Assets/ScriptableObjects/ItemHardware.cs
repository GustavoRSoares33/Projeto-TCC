using UnityEngine;

// Essa linha mágica cria um atalho no botão direito da Unity!
[CreateAssetMenu(fileName = "NovoHardware", menuName = "Tycoon/Item de Hardware")]
public class ItemHardware : ScriptableObject
{
    [Header("Informações Básicas")]
    public string nomeDoItem;
    public Sprite icone; // A imagem que vai aparecer na loja

    [TextArea]
    public string descricao;

    [Header("Economia e Status")]
    public float preco;
    public int ganhoDePerformance; // O quanto essa peça melhora a máquina

    // Um Enum para categorizar as peças na loja
    public TipoHardware tipoDePeca;

    public enum TipoHardware
    {
        Processador,
        PlacaDeVideo,
        MemoriaRAM,
        Cooler,
        Monitor
    }
}