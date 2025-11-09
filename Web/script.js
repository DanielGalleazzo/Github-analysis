async function analisarCommitsComGPT(commitsTexto) {
  const apiKey = ""; 
  const requestBody = {
    model: "gpt-4o-mini",
    messages: [
      { role: "system", content: "Você é um assistente que analisa commits de código." },
      { role: "user", content: `Analise os commits abaixo e me dê um resumo: ${commitsTexto}` }
    ]
  };

  const response = await fetch("https://api.openai.com/v1/chat/completions", {
    method: "POST",
    headers: {
      "Authorization": `Bearer ${apiKey}`,
      "Content-Type": "application/json"
    },
    body: JSON.stringify(requestBody)
  });

  const data = await response.json();
  return data.choices[0].message.content;
}

function gerarTextoCommits(commitsLista) {
  let sb = "";
  for (const c of commitsLista) {
    const dataFormatada = new Date(c.Data).toISOString().replace("T", " ").substring(0, 19);
    sb += `Autor: ${c.Autor}`;
    sb += `Data: ${dataFormatada}`;
    sb += `Mensagem: ${c.Mensagem};`;
    sb += "-----------------------------";
  }
  return sb;
}

async function main() {
  const usuario = document.getElementById("Username").value;
  const repositorio = document.getElementById("RepositoryText").value;
  const api_key = ""; 
  const data1 = document.getElementById("firstDateText").value;
  const data2 = document.getElementById("secondDateText").value;

  const saida = document.getElementById("saida");
  saida.textContent = `Procurando commits de ${usuario} em ${repositorio}...`;

  try {
    const response = await fetch(
      `https://api.github.com/repos/${usuario}/${repositorio}/commits?author=${usuario}&since=${data1}&until=${data2}`,
      {
        headers: {
          "User-Agent": "AppName/1.0",
          "Authorization": `token ${api_key}`
        }
      }
    );

    const content = await response.text();
    const doc = JSON.parse(content);

    if (!Array.isArray(doc) || doc.length === 0) {
      saida.textContent += "Nenhum commit encontrado.";
      return;
    }

    const commitsLista = doc.map(commit => ({
      Autor: commit.commit.author.name,
      Data: commit.commit.author.date,
      Mensagem: commit.commit.message
    }));

    const commitsTexto = gerarTextoCommits(commitsLista);

    saida.textContent += "Histórico de commits:" + commitsTexto + "Analisando com GPT...";

    const resumo = await analisarCommitsComGPT(commitsTexto);
    saida.textContent += "Resumo da análise:" + resumo;
  } catch (error) {
    console.error(error);
    saida.textContent += "Erro";
  }
}


document.getElementById("goButton").addEventListener("click", main);
