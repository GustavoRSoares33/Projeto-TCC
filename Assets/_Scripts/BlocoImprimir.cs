using System.Collections;
using UnityEngine;

// Note que ele herda de BlocoLogicoBase e não de MonoBehaviour
public class BlocoImprimir : BlocoLogicoBase
{
    [TextArea]
    public string mensagemParaImprimir = "Olá, Mundo! Meu primeiro bloco funcionou.";

    public override IEnumerator ExecutarBloco()
    {
        // Aqui dentro fica a lógica real do bloco
        Debug.Log("<color=cyan>[SISTEMA LOGICO]</color> " + mensagemParaImprimir);

        // Pede para a Unity esperar 1 segundo antes de ir para o próximo bloco.
        // Isso é fundamental para o aluno conseguir "ver" o código rodando no simulador.
        yield return new WaitForSeconds(1f);
    }
}