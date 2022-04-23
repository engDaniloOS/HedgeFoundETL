# Hedge Fund Services
Sistemas para extração e disponibilização de registros diários de fundos Hedge.
 
# Atividades
- [x] Subir MVP do ETL
- [x] Subir MVP da API
- [X] Revisão do código do ETL
- [X] Revisão do código da API
- [ ] Revisão da documentação
- [ ] Testes unitários para o ETL
- [ ] Testes unitários para API
- [ ] Revisão dos testes unitários do ETL
- [ ] Revisão dos testes unitários da API

# Bancos de dados
Antes de executar quaisquer aplicações do projeto, ajustar as configurações do ETL e API para apontarem para o DB desejado. Para alterar a connection string no projeto do ETL, ir até o Program.cs. Já na API, a string de conexão encontra-se no arquivo padrão: appsettings.json.
Todavia, por padrão as aplicações estão apontando para um DB SQL em arquivo salvo no repositório.

# Endpoints
Para facilitar a execução e testes, a API esta documentadas com Swagger.

* [GET] /api/HedgeFunds
  ** Endpoint responsável por retornar todos registros diários para determinado fundo Hedge. 
  ** Como parâmetro obrigatório é necessário informar o CNPJ do fundo (ex: 00.000.000/0000-00). 
  ** Como parâmetros não obrigatórios também podemos informar as datas de início e fim que desejamos resgatar os registros, no formato "YYYY-MM-DD". Ainda como parâmetros opcionais, podemos informar a quantidade de itens por páginas que desejamos que a consulta retorne, assim como o número da página desejada.
  **Validações: As datas informadas precisam ser superiores a 31/12/2016, e iguais ou inferiores ao dia da consulta na API. A quantidade de itens por páginas é limitada a 100.
  Como.
  ** Retorno 200: Teremos um objeto com informações referentes a paginação, e uma lista com registros do fundo, contendo cada um deles o cnpj, data, valor do portfólio, valor da cota, valor patrimônial, valor investido no dia, valor resgatado no dia, e número de cotistas.
  ** Retorno 400: Caso a consulta encontre erros de processamento, ou os dados informados na requisição sejam inválidos;
  ** Retorno 404: Caso a consulta não retorne resultados;

# Frameworks utilizados
Para o projeto foram utilizadas os seguintes frameworks e bibliotecas:
- Todos os projetos foram desenvolvidos com .NET Core 5.0;
- Para os testes foram utilizados Moq e NUnit;
- Para a API utilizamos o ASP.NET Core;
- Para mapeamento objeto-relacional foi utilizado o EF Core;
- Para a paginação foi utilizado o X.PagedList;

# Decisões técnicas
## Desenho de solução
Para o desenho da solução do ETL foi utilizada uma simplificação da Clean Architecture, de modo que:
1 - Os contratos com a definição de negócio e seus modelos estão no centro da aplicação, sem dependência de outras camadas;
2 - Acima da camada de negócios encontramos a camada de serviço, que abstrai toda a infraestrutura necessária do projeto;
3 - Camada de infraestrutura;

Já para a API, que é enxuta e com poucas regras de negócio, foi utilizada o DDD com o modelo mais clássico - Controllers - Services - Repository;

# Execução do projeto
## Execução com o docker
É possível executar a API utilizando o api.dockerfile. Para isso:
1 - Execute o comando para criar a imagem: "docker build -f api.dockerfile -t tag .";
2 - Execute o comando para criar o container: "docker run -it ImageName";

## Execução sem o docker:
### Projeto de testes:
Abrir o cmd na pasta do projeto de testes e executar o comando: dotnet test BrazilianHedgeFund.API.Tests.csproj

### Projeto ETL e/ou API:
- Baixar as dependências necessárias: dotnet restore
- Compilar o projeto: dotnet build
- Executar o projeto com acesso ao teminal para entrada de informações na aplicação: dotnet run --interactive



 
