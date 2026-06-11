using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LinhaDeCodigo : MonoBehaviour
{
    [Tooltip("Ex: 'linha_while', 'linha_incremento'")]
    public string idDaLinha;

    private Button meuBotao;

    void Start()
    {
        meuBotao = GetComponent<Button>();
        // Conecta o clique do botão diretamente ao nosso Gerenciador Lógico
        meuBotao.onClick.AddListener(AoClicarNaLinha);
    }

    private void AoClicarNaLinha()
    {
        // Encontra o gerente na cena e manda a resposta do jogador para ele
        GerenciadorLogica gerente = FindFirstObjectByType<GerenciadorLogica>();

        if (gerente != null)
        {
            gerente.ValidarBug(idDaLinha, gameObject);
        }
    }
}