using System;
using System.Collections.Generic;
using System.Text;

namespace AnáliseCommitsV1
{
    public class TextoCommit
    {
        public static string GerarTextoCommits(List<(DateTime Data, string Autor, string Mensagem)> commitsLista)
        {
            var sb = new StringBuilder();
            foreach (var c in commitsLista)
            {
                sb.AppendLine($"Autor: {c.Autor}");
                sb.AppendLine($"Data: {c.Data:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"Mensagem: {c.Mensagem}");
                sb.AppendLine("-----------------------------");
            }
            return sb.ToString();
        }
    }
}
