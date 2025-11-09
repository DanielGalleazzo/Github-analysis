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
    sb += `Autor: ${c.Autor}\n`;
    sb += `Data: ${dataFormatada}\n`;
    sb += `Mensagem: ${c.Mensagem}\n`;
    sb += "-----------------------------\n";
  }
  return sb;
}

async function main() {
  const usuario = "Galleazzo"; // mais uma vez usando o meu irmao como exemplo xD valeu, @Galleazzo
  const repositorio = "Casamento-Julia-Paulo-BackEnd";
  const api_key = "";
  const data1 = "2025-09-13T00:00:00Z";
  const data2 = "2025-09-27T23:59:59Z";

  console.log(`procurando commits de ${usuario} em ${repositorio}`);

  const response = await fetch(`https://api.github.com/repos/${usuario}/${repositorio}/commits?author=${usuario}&since=${data1}&until=${data2}`, {
    headers: {
      "User-Agent": "AppName/1.0",
      "Authorization": `token ${api_key}`
    }
  });

  const content = await response.text();
  const doc = JSON.parse(content);

  if (!Array.isArray(doc) || doc.length === 0) {
    console.log("nao encontrei nada.");
    return;
  }

  const commitsLista = doc.map(commit => ({
    Autor: commit.commit.author.name,
    Data: commit.commit.author.date,
    Mensagem: commit.commit.message
  }));

  const commitsTexto = gerarTextoCommits(commitsLista);

  console.log("histórico de commits:\n");
  console.log(commitsTexto);

  console.log("analisando commits com GPT...\n");
  const resumo = await analisarCommitsComGPT(commitsTexto);
  console.log("resumo da análise:\n");
  console.log(resumo);
}

main();
